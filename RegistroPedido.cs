using System;
using System.Globalization;

public static class RegistroPedido {
    public static void Salvar(Dados db, int cliente, Pedido p) {
        db.Executar("INSERT INTO importacao(pedido_id) VALUES(?)",p.id_pedido);
        db.Executar("INSERT INTO pedido(id_cliente,id_endereco,data_pedido,status,forma_pagamento,taxa_entrega,valor_total,observacao) VALUES(?,?,?,'Pendente',?,?,?,?)",
            cliente,Regras.GravarEndereco(db,cliente,p.endereco_entrega),DateTime.Parse(p.data_pedido,CultureInfo.InvariantCulture),p.forma_pagamento,p.taxa_entrega,p.valor_total,p.observacao??"");
        int interno=Convert.ToInt32(db.Valor("SELECT LAST_INSERT_ID()"));
        db.Executar("UPDATE importacao SET id_interno=? WHERE pedido_id=?",interno,p.id_pedido);
        db.Executar("INSERT INTO pedidos(codigo_pedido,nome_cliente,total,status,id_pedido) VALUES(?,?,?,'Pendente',?)",p.id_pedido.ToString(CultureInfo.InvariantCulture),p.cliente.nome,p.valor_total,interno);
        foreach(var item in p.itens) {
            Regras.Exigir(db.Valor("SELECT id_produto FROM produto WHERE id_produto=?",item.produto.id_produto)!=null,"Produto ausente do catálogo SQL.");
            db.Executar("INSERT INTO item_pedido(id_pedido,id_produto,quantidade,preco_unitario) VALUES(?,?,?,?)",interno,item.produto.id_produto,item.quantidade,item.preco_unitario);
        }
    }
}
