using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

public class Dados : IDisposable {
    public MySqlConnection Conexao;
    public MySqlTransaction Transacao;
    public static readonly JavaScriptSerializer Json = new JavaScriptSerializer();

    public Dados() {
        Conexao = new Conexao().conectarBD();
    }
    public void Iniciar() { Transacao = Conexao.BeginTransaction(); }
    public void Confirmar() { Transacao.Commit(); Transacao.Dispose(); Transacao = null; }
    public void Dispose() {
        if (Transacao != null) Transacao.Dispose();
        Conexao.Dispose();
    }
    MySqlCommand Comando(string sql, object[] valores) {
        int indice = 0;
        string parametrizado = Regex.Replace(sql, @"\?", m => "@p" + indice++);
        var cmd = new MySqlCommand(parametrizado, Conexao, Transacao);
        foreach (object valor in valores) cmd.Parameters.AddWithValue("@p" + cmd.Parameters.Count, valor ?? DBNull.Value);
        return cmd;
    }
    public int Executar(string sql, params object[] valores) {
        using (var cmd = Comando(sql, valores)) return cmd.ExecuteNonQuery();
    }
    public object Valor(string sql, params object[] valores) {
        using (var cmd = Comando(sql, valores)) return cmd.ExecuteScalar();
    }
    public DataTable Tabela(string sql, params object[] valores) {
        using (var cmd = Comando(sql, valores))
        using (var reader = cmd.ExecuteReader()) {
            var table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
    public List<Dictionary<string,object>> Linhas(string sql, params object[] valores) {
        var table = Tabela(sql,valores);
        var lista = new List<Dictionary<string,object>>();
        foreach(DataRow row in table.Rows) {
            var item = new Dictionary<string,object>();
            foreach(DataColumn col in table.Columns) item[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            lista.Add(item);
        }
        return lista;
    }
    public static string Senha(string senha) {
        using(var hash = new Rfc2898DeriveBytes(senha,16,210000,HashAlgorithmName.SHA256))
            return Convert.ToBase64String(hash.Salt) + "." + Convert.ToBase64String(hash.GetBytes(32));
    }
    public static bool Confere(string senha, object guardada) {
        if(guardada == null || guardada == DBNull.Value) return false;
        if (!Regex.IsMatch(guardada.ToString(), @"^[A-Za-z0-9+/]{22}==\.[A-Za-z0-9+/]{43}=$")) return false;
        string[] partes = guardada.ToString().Split('.');
        using(var hash = new Rfc2898DeriveBytes(senha,Convert.FromBase64String(partes[0]),210000,HashAlgorithmName.SHA256)) {
            byte[] a = hash.GetBytes(32), b = Convert.FromBase64String(partes[1]);
            int erro = a.Length ^ b.Length;
            for(int i=0;i<Math.Min(a.Length,b.Length);i++) erro |= a[i]^b[i];
            return erro == 0;
        }
    }
}
public class Cliente { public string nome,email,telefone,cpf,senha; }
public class Endereco { public string cep,rua,numero,complemento,cidade,estado; }
public class Produto { public int id_produto; public string nome; public decimal preco; }
public class Item { public Produto produto; public int quantidade; public decimal preco_unitario; }
public class Pedido {
    public int id_pedido; public string data_pedido,status,forma_pagamento,observacao;
    public decimal taxa_entrega,valor_total;
    public Cliente cliente; public Endereco endereco_entrega; public List<Item> itens;
}
public static class Regras {
    public static void Exigir(bool ok,string mensagem) { if(!ok) throw new ArgumentException(mensagem); }
    public static string Texto(string s,int max) {
        s=(s??"").Trim(); Exigir(s.Length>0 && s.Length<=max,"Confira os campos obrigatórios."); return s;
    }
    public static string Opcional(string s,int max) {
        s=(s??"").Trim(); Exigir(s.Length<=max,"Texto muito longo."); return s;
    }
    public static void ClienteValido(Cliente c) {
        Exigir(c!=null,"Cliente ausente.");
        c.nome=Texto(c.nome,80); c.email=Texto(c.email,120).ToLowerInvariant();
        Exigir(Regex.IsMatch(c.email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$"),"E-mail inválido.");
        c.telefone=Regex.Replace(c.telefone??"",@"\D","");
        Exigir(c.telefone.Length==10 || c.telefone.Length==11,"Confira o telefone com DDD.");
    }
    public static void Destino(Endereco e) {
        Exigir(e!=null,"Informe o endereço.");
        e.cep=Regex.Replace(e.cep??"",@"\D",""); Exigir(e.cep.Length==8,"Confira o CEP.");
        e.rua=Texto(e.rua,120); e.numero=Texto(e.numero,10); e.cidade=Texto(e.cidade,80);
        e.complemento=Opcional(e.complemento,100); e.estado=Texto(e.estado,2).ToUpperInvariant();
        Exigir((" AC AL AP AM BA CE DF ES GO MA MT MS MG PA PB PR PE PI RJ RN RS RO RR SC SP SE TO ").Contains(" "+e.estado+" "),"Estado inválido.");
    }
    public static int GravarEndereco(Dados db,int id,Endereco e) {
        db.Executar("INSERT INTO endereco(id_cliente,cep,rua,numero,complemento,cidade,estado) VALUES(?,?,?,?,?,?,?)",id,e.cep,e.rua,e.numero,e.complemento,e.cidade,e.estado);
        return Convert.ToInt32(db.Valor("SELECT LAST_INSERT_ID()"));
    }
    public static void SalvarEndereco(Dados db,int id,Endereco e) {
        int endereco = GravarEndereco(db,id,e);
        db.Executar("INSERT INTO perfil_cliente(id_cliente,id_endereco) VALUES(?,?) ON DUPLICATE KEY UPDATE id_endereco=VALUES(id_endereco)",id,endereco);
    }
}
