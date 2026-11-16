using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class ClienteJson {
    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("telefone")]
    public string Telefone { get; set; }

    public Cliente Converter() {
        return new Cliente {
            nome = Nome,
            email = Email,
            telefone = Telefone
        };
    }
}

public class EnderecoJson {
    [JsonPropertyName("cep")]
    public string Cep { get; set; }

    [JsonPropertyName("estado")]
    public string Estado { get; set; }

    [JsonPropertyName("cidade")]
    public string Cidade { get; set; }

    [JsonPropertyName("rua")]
    public string Rua { get; set; }

    [JsonPropertyName("numero")]
    public string Numero { get; set; }

    [JsonPropertyName("complemento")]
    public string Complemento { get; set; }

    public Endereco Converter() {
        return new Endereco {
            cep = Cep,
            estado = Estado,
            cidade = Cidade,
            rua = Rua,
            numero = Numero,
            complemento = Complemento
        };
    }
}

public class ProdutoJson {
    [JsonPropertyName("id_produto")]
    public int IdProduto { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("preco")]
    public decimal Preco { get; set; }

    public Produto Converter() {
        return new Produto {
            id_produto = IdProduto,
            nome = Nome,
            preco = Preco
        };
    }
}

public class ItemJson {
    [JsonPropertyName("produto")]
    public ProdutoJson Produto { get; set; }

    [JsonPropertyName("quantidade")]
    public int Quantidade { get; set; }

    [JsonPropertyName("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    public Item Converter() {
        return new Item {
            produto = Produto?.Converter(),
            quantidade = Quantidade,
            preco_unitario = PrecoUnitario
        };
    }
}

public class PedidoJson {
    [JsonPropertyName("id_pedido")]
    public int IdPedido { get; set; }

    [JsonPropertyName("data_pedido")]
    public string DataPedido { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("forma_pagamento")]
    public string FormaPagamento { get; set; }

    [JsonPropertyName("taxa_entrega")]
    public decimal TaxaEntrega { get; set; }

    [JsonPropertyName("valor_total")]
    public decimal ValorTotal { get; set; }

    [JsonPropertyName("observacao")]
    public string Observacao { get; set; }

    [JsonPropertyName("cliente")]
    public ClienteJson Cliente { get; set; }

    [JsonPropertyName("endereco_entrega")]
    public EnderecoJson EnderecoEntrega { get; set; }

    [JsonPropertyName("itens")]
    public List<ItemJson> Itens { get; set; }

    public Pedido Converter() {
        return new Pedido {
            id_pedido = IdPedido,
            data_pedido = DataPedido,
            status = Status,
            forma_pagamento = FormaPagamento,
            taxa_entrega = TaxaEntrega,
            valor_total = ValorTotal,
            observacao = Observacao,
            cliente = Cliente?.Converter(),
            endereco_entrega = EnderecoEntrega?.Converter(),
            itens = Itens?.Select(i => i?.Converter()).ToList()
        };
    }
}
