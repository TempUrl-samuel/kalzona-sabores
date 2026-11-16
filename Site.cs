using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using MySql.Data.MySqlClient;

class Entrada {
    public Cliente cliente; public Endereco endereco; public List<Linha> itens;
    public string email,senha,nome,assunto,mensagem,pagamento,observacao,chave;
}
class Linha { public int id,quantidade; }
class Sessao { public int cliente; public DateTime expira; }
class Site {
    static Dictionary<string,Sessao> sessoes=new Dictionary<string,Sessao>();
    static void Main() {
        var server=new HttpListener();
        server.Prefixes.Add("http://localhost:8080/");
        server.Start(); Console.WriteLine("Kalzona em http://localhost:8080/ — Ctrl+C para sair.");
        while(true) {
            var ctx=server.GetContext();
            try { Atender(ctx); }
            catch(ArgumentException ex) { Resposta(ctx,400,new {erro=ex.Message}); }
            catch(UnauthorizedAccessException) { Resposta(ctx,401,new {erro="Entre na sua conta."}); }
            catch(MySqlException ex) {
                bool duplicado=ex.Number==1062;
                Console.Error.WriteLine("MySQL: "+ex.Number);
                Resposta(ctx,duplicado?409:503,new {erro=duplicado?"Registro já existente.":"Confira a conexão com o MySQL."});
            }
            catch(Exception ex) { Console.Error.WriteLine(ex.GetType().Name); Resposta(ctx,500,new {erro="Não foi possível concluir a operação."}); }
            finally { ctx.Response.Close(); }
        }
    }
    static void Resposta(HttpListenerContext ctx,int status,object dados) {
        ctx.Response.StatusCode=status;
        ctx.Response.ContentType="application/json; charset=utf-8";
        ctx.Response.Headers["Cache-Control"]="no-store";
        byte[] bytes=Encoding.UTF8.GetBytes(Dados.Json.Serialize(dados));
        ctx.Response.OutputStream.Write(bytes,0,bytes.Length);
    }
    static int Usuario(HttpListenerContext ctx) {
        var cookie=ctx.Request.Cookies["kalzona"];
        Sessao sessao;
        if(cookie==null || !sessoes.TryGetValue(cookie.Value,out sessao) || sessao.expira<DateTime.UtcNow)
            throw new UnauthorizedAccessException();
        return sessao.cliente;
    }
    static void Entrar(HttpListenerContext ctx,int id) {
        foreach(var antigo in sessoes.Where(p=>p.Value.expira<DateTime.UtcNow).Select(p=>p.Key).ToArray()) sessoes.Remove(antigo);
        string token=Guid.NewGuid().ToString("N")+Guid.NewGuid().ToString("N");
        sessoes[token]=new Sessao {cliente=id,expira=DateTime.UtcNow.AddHours(8)};
        ctx.Response.AppendHeader("Set-Cookie","kalzona="+token+"; HttpOnly; SameSite=Strict; Path=/");
    }
    static void Atender(HttpListenerContext ctx) {
        var req=ctx.Request;
        string path=req.Url.AbsolutePath;
        if(!path.StartsWith("/api/")) {
            if(req.HttpMethod!="GET") {ctx.Response.StatusCode=405;return;}
            string root=Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"wwwroot"))+Path.DirectorySeparatorChar;
            string file=Path.GetFullPath(Path.Combine(root,Uri.UnescapeDataString(path=="/"?"index.html":path.Substring(1))));
            if(!file.StartsWith(root,StringComparison.OrdinalIgnoreCase)||!File.Exists(file)) {ctx.Response.StatusCode=404;return;}
            var mime=new Dictionary<string,string>{{".html","text/html; charset=utf-8"},{".css","text/css; charset=utf-8"},{".js","text/javascript; charset=utf-8"},{".svg","image/svg+xml"},{".png","image/png"}};
            string tipo;if(!mime.TryGetValue(Path.GetExtension(file),out tipo)){ctx.Response.StatusCode=404;return;}
            ctx.Response.ContentType=tipo; using(var f=File.OpenRead(file))f.CopyTo(ctx.Response.OutputStream);return;
        }
        Entrada d=new Entrada();
        if(req.HttpMethod=="POST") {
            Regras.Exigir(req.Headers["X-Kalzona"]=="1","Solicitação inválida.");
            Regras.Exigir(req.ContentLength64>=0 && req.ContentLength64<=64000,"Solicitação grande demais.");
            using(var reader=new StreamReader(req.InputStream,Encoding.UTF8)) d=Dados.Json.Deserialize<Entrada>(reader.ReadToEnd());
            Regras.Exigir(d!=null,"Conteúdo inválido.");
        }
        string rota=req.HttpMethod+" "+path;
        using(var db=new Dados()) {
            object resultado;
            switch(rota) {
                case "GET /api/produtos":
                    resultado=db.Linhas("SELECT p.id_produto AS id,c.posicao,p.nome,p.preco FROM produto p JOIN cardapio_site c ON c.id_produto=p.id_produto WHERE p.disponivel=1 ORDER BY c.posicao");break;
                case "POST /api/cadastro":
                    Regras.ClienteValido(d.cliente); Regras.Destino(d.endereco);
                    Regras.Exigir(d.cliente.senha!=null && d.cliente.senha.Length>=8 && d.cliente.senha.Length<=64,"A senha deve ter de 8 a 64 caracteres.");
                    string cpf=System.Text.RegularExpressions.Regex.Replace(d.cliente.cpf??"",@"\D","");
                    Regras.Exigir(cpf.Length==11,"Informe os 11 dígitos do CPF.");
                    db.Iniciar();
                    db.Executar("INSERT INTO cliente(nome,email,telefone,senha_hash) VALUES(?,?,?,?)",d.cliente.nome,d.cliente.email,d.cliente.telefone,Dados.Senha(d.cliente.senha));
                    int novo=Convert.ToInt32(db.Valor("SELECT LAST_INSERT_ID()"));
                    Regras.SalvarEndereco(db,novo,d.endereco);
                    db.Executar("UPDATE perfil_cliente SET cpf=? WHERE id_cliente=?",cpf,novo);db.Confirmar();Entrar(ctx,novo);resultado=new {id=novo};break;
                case "POST /api/login":
                    var rows=db.Linhas("SELECT id_cliente AS id,senha_hash AS senha FROM cliente WHERE email=?",Regras.Texto(d.email,120));
                    Regras.Exigir(rows.Count==1 && Dados.Confere(d.senha??"",rows[0]["senha"]),"E-mail ou senha incorretos.");
                    int id=Convert.ToInt32(rows[0]["id"]);Entrar(ctx,id);resultado=new {id=id};break;
                case "POST /api/sair":
                    if(req.Cookies["kalzona"]!=null)sessoes.Remove(req.Cookies["kalzona"].Value);
                    ctx.Response.AppendHeader("Set-Cookie","kalzona=; Max-Age=0; Path=/; HttpOnly; SameSite=Strict");resultado=new {ok=true};break;
                case "GET /api/perfil":
                    resultado=db.Linhas("SELECT c.id_cliente AS id,c.nome,c.email,c.telefone,e.cep,e.rua,e.numero,e.complemento,e.cidade,e.estado FROM cliente c LEFT JOIN perfil_cliente f ON f.id_cliente=c.id_cliente LEFT JOIN endereco e ON e.id_endereco=COALESCE(f.id_endereco,(SELECT MIN(a.id_endereco) FROM endereco a WHERE a.id_cliente=c.id_cliente)) WHERE c.id_cliente=?",Usuario(ctx)).Single();break;
                case "POST /api/perfil":
                    int user=Usuario(ctx);Regras.ClienteValido(d.cliente);Regras.Destino(d.endereco);
                    db.Iniciar();db.Executar("UPDATE cliente SET nome=?,email=?,telefone=? WHERE id_cliente=?",d.cliente.nome,d.cliente.email,d.cliente.telefone,user);
                    Regras.SalvarEndereco(db,user,d.endereco);db.Confirmar();resultado=new {ok=true};break;
                case "POST /api/contato":
                    db.Executar("INSERT INTO contato(nome,email,assunto,mensagem) VALUES(?,?,?,?)",Regras.Texto(d.nome,80),Regras.Texto(d.email,120),Regras.Texto(d.assunto,80),Regras.Texto(d.mensagem,1000));resultado=new {ok=true};break;
                case "POST /api/pedidos": resultado=Finalizar(db,Usuario(ctx),d);break;
                case "GET /api/pedidos":
                    resultado=db.Linhas("SELECT s.id, COALESCE(p.status,IF(i.cancelado=1,'Cancelado','Aguardando importação')) AS status, s.documento FROM envio_site s LEFT JOIN importacao i ON i.pedido_id=s.id LEFT JOIN pedido p ON p.id_pedido=i.id_interno WHERE s.cliente_id=? ORDER BY s.id DESC",Usuario(ctx));break;
                default: Resposta(ctx,404,new {erro="Página inexistente."});return;
            }
            Resposta(ctx,200,resultado);
        }
    }
    static object Finalizar(Dados db,int user,Entrada d) {
        Guid chave; Regras.Exigir(Guid.TryParse(d.chave,out chave),"Identificador de compra inválido.");
        Regras.Destino(d.endereco);
        Regras.Exigir(new[]{"pix","cartao","dinheiro"}.Contains(d.pagamento),"Escolha a forma de pagamento.");
        Regras.Exigir(d.itens!=null && d.itens.Count>0 && d.itens.Count<=100,"Confira o carrinho.");
        db.Iniciar();
        db.Executar("INSERT INTO envio_site(cliente_id,chave) VALUES(?,?) ON DUPLICATE KEY UPDATE id=LAST_INSERT_ID(id)",user,chave.ToString());
        int id=Convert.ToInt32(db.Valor("SELECT LAST_INSERT_ID()"));
        object pronto=db.Valor("SELECT documento FROM envio_site WHERE id=?",id);
        if(pronto!=null && pronto!=DBNull.Value){db.Confirmar();return new {id=id};}
        var cliente=db.Linhas("SELECT nome,email,telefone FROM cliente WHERE id_cliente=?",user).Single();
        var p=new Pedido {id_pedido=id,data_pedido=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),status="Pendente",forma_pagamento=d.pagamento,
            taxa_entrega=7.50m,observacao=Regras.Opcional(d.observacao,300),endereco_entrega=d.endereco,
            cliente=new Cliente {nome=(string)cliente["nome"],email=(string)cliente["email"],telefone=(string)cliente["telefone"]},itens=new List<Item>()};
        var usados=new HashSet<int>();
        foreach(var linha in d.itens) {
            Regras.Exigir(linha!=null && usados.Add(linha.id) && linha.quantidade>=1 && linha.quantidade<=99,"Quantidade inválida.");
            var row=db.Linhas("SELECT nome,preco FROM produto WHERE id_produto=? AND disponivel=1 FOR UPDATE",linha.id);
            Regras.Exigir(row.Count==1,"Um produto ficou indisponível. Revise o carrinho.");
            decimal preco=Convert.ToDecimal(row[0]["preco"]);
            p.itens.Add(new Item {quantidade=linha.quantidade,preco_unitario=preco,produto=new Produto {id_produto=linha.id,nome=(string)row[0]["nome"],preco=preco}});
            p.valor_total+=linha.quantidade*preco;
        }
        p.valor_total+=p.taxa_entrega;
        RegistroPedido.Salvar(db, user, p);
        db.Executar("UPDATE envio_site SET documento=? WHERE id=?",Dados.Json.Serialize(p),id);
        db.Confirmar();return new {id=id};
    }
}
