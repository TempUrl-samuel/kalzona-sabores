using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

public static class Importar {
    public static int Gravar(List<PedidoJson> lista) {
        Regras.Exigir(lista!=null && lista.Count>0,"Arquivo sem pedidos.");
        int novos=0;
        using(var db=new Dados()) {
            db.Iniciar();
            foreach(var documento in lista) {
                var p = documento?.Converter();
                Validar(p);
                if(db.Valor("SELECT pedido_id FROM importacao WHERE pedido_id=?",p.id_pedido)!=null) continue;
                var envio=db.Linhas("SELECT cliente_id,documento FROM envio_site WHERE id=?",p.id_pedido);
                int cliente;
                if(envio.Count>0) {
                    // Quando o pedido veio deste site, o registro original confirma itens, endereço e preços.
                    var original=Dados.Json.Deserialize<Pedido>((string)envio[0]["documento"]);
                    bool mesmosItens = original.itens.Count == p.itens.Count && original.itens.Zip(p.itens,
                        (a,b) => a.produto.id_produto == b.produto.id_produto &&
                            a.preco_unitario == b.preco_unitario && a.quantidade == b.quantidade).All(igual => igual);
                    Regras.Exigir(mesmosItens && original.valor_total == p.valor_total &&
                        original.taxa_entrega == p.taxa_entrega && original.data_pedido == p.data_pedido &&
                        original.forma_pagamento == p.forma_pagamento && original.observacao == p.observacao &&
                        original.cliente.email == p.cliente.email &&
                        Dados.Json.Serialize(original.endereco_entrega) == Dados.Json.Serialize(p.endereco_entrega),
                        "O JSON diverge do pedido registrado pelo site.");
                    cliente=Convert.ToInt32(envio[0]["cliente_id"]);
                } else {
                    var cadastro=db.Valor("SELECT id_cliente FROM cliente WHERE email=?",p.cliente.email);
                    if(cadastro==null) {
                        db.Executar("INSERT INTO cliente(nome,email,telefone,senha_hash) VALUES(?,?,?,?)",p.cliente.nome,p.cliente.email,p.cliente.telefone,Dados.Senha(Guid.NewGuid().ToString("N")));
                        cliente=Convert.ToInt32(db.Valor("SELECT LAST_INSERT_ID()"));Regras.SalvarEndereco(db,cliente,p.endereco_entrega);
                    } else cliente=Convert.ToInt32(cadastro);
                }
                RegistroPedido.Salvar(db, cliente, p);
                novos++;
            }
            db.Confirmar();
        }
        return novos;
    }
    public static void Validar(Pedido p) {
        Regras.Exigir(p!=null && p.id_pedido>0,"Código de pedido inválido.");
        Regras.ClienteValido(p.cliente);Regras.Destino(p.endereco_entrega);
        DateTime data; Regras.Exigir(DateTime.TryParse(p.data_pedido,CultureInfo.InvariantCulture,DateTimeStyles.None,out data),"Data inválida.");
        Regras.Texto(p.forma_pagamento,20);Regras.Opcional(p.observacao,300);
        Regras.Exigir(p.itens!=null && p.itens.Count>0 && p.itens.Count<=100 && p.taxa_entrega>=0,"Confira os itens e a taxa.");
        decimal total=p.taxa_entrega;
        var ids=new HashSet<int>();
        foreach(var item in p.itens) {
            Regras.Exigir(item!=null && item.produto!=null && item.produto.id_produto>0 && ids.Add(item.produto.id_produto)
                && item.quantidade>0 && item.quantidade<=99 && item.preco_unitario>=0 && Decimal.Round(item.preco_unitario,2)==item.preco_unitario,"Item inválido.");
            total+=item.quantidade*item.preco_unitario;
        }
        Regras.Exigir(total==p.valor_total,"O total diverge dos itens e da taxa de entrega.");
    }
}
