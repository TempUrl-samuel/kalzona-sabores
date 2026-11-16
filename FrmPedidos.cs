using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

partial class FrmPedidos : UserControl {
    Action recarregar;
    public FrmPedidos() { InitializeComponent(); ConfigurarPedidos(); }
    public void Atualizar() { try { recarregar(); } catch(Exception ex) { MessageBox.Show(ex.Message); } }
    void Ligar(Button b, Action acao) { b.Click += delegate { try { acao(); } catch(Exception ex) { MessageBox.Show(this, ex.Message, "Kalzona"); } }; }
    void ConfigurarPedidos() {
        var grid = gradePedidos;
        Action atualizar = delegate { using (var db = new Dados()) grid.DataSource = db.Tabela("SELECT p.id_pedido AS id,i.pedido_id AS codigo_site,c.nome,p.status,p.valor_total,p.forma_pagamento,p.data_pedido,e.rua,e.numero,e.complemento,e.cidade,e.estado,e.cep,p.observacao FROM pedido p JOIN cliente c ON c.id_cliente=p.id_cliente JOIN endereco e ON e.id_endereco=p.id_endereco LEFT JOIN importacao i ON i.id_interno=p.id_pedido ORDER BY p.id_pedido DESC"); };
        Ligar(atualizarPedidos, atualizar);
        recarregar = atualizar;
        Func<int> id=delegate {Regras.Exigir(grid.SelectedRows.Count>0,"Selecione uma linha.");return Convert.ToInt32(grid.SelectedRows[0].Cells["id"].Value);};
Ligar(btnImportar, delegate { ImportarArquivo(atualizar); });
        status.Items.AddRange(new object[] {"Pendente","Recebido","Em preparo","Saiu para entrega","Entregue"}); status.SelectedIndex=0;
Ligar(salvarStatus,delegate {int pedido=id(); using(var db=new Dados()) {db.Iniciar();db.Executar("UPDATE pedido SET status=? WHERE id_pedido=?",status.Text,pedido);db.Executar("UPDATE pedidos SET status=? WHERE id_pedido=?",status.Text,pedido);db.Confirmar();}atualizar();});
            Ligar(verItens,delegate {
                using(var db=new Dados()) {
                    var itens=db.Tabela("SELECT p.nome,i.quantidade,i.preco_unitario FROM item_pedido i JOIN produto p ON p.id_produto=i.id_produto WHERE i.id_pedido=?",id());
                    string resumo="";foreach(DataRow item in itens.Rows)resumo+=item["quantidade"]+" × "+item["nome"]+" — R$ "+item["preco_unitario"]+Environment.NewLine;
                    MessageBox.Show(resumo,"Itens do pedido");
                }
            });
            Ligar(cancelar,delegate {
                int pedido=id();if(!Confirmar())return;
                using(var db=new Dados()) {db.Iniciar();db.Executar("UPDATE importacao SET cancelado=1 WHERE id_interno=?",pedido);db.Executar("DELETE FROM item_pedido WHERE id_pedido=?",pedido);db.Executar("DELETE FROM pedido WHERE id_pedido=?",pedido);db.Confirmar();}atualizar();
            });
    }
    bool Confirmar() { return MessageBox.Show(this,"Confirma a exclusão do registro selecionado?","Kalzona",MessageBoxButtons.YesNo)==DialogResult.Yes; }
    public static void ImportarArquivo(Action atualizar) {
        using (var arquivo = new OpenFileDialog { Filter = "Pedidos (*.json)|*.json" }) {
            if (arquivo.ShowDialog() != DialogResult.OK) return;
            Regras.Exigir(new FileInfo(arquivo.FileName).Length <= 4000000, "Arquivo maior que 4 MB.");
            string textoJson = File.ReadAllText(arquivo.FileName);
            var lista = textoJson.TrimStart().StartsWith("[")
                ? JsonSerializer.Deserialize<List<PedidoJson>>(textoJson)
                : new List<PedidoJson> { JsonSerializer.Deserialize<PedidoJson>(textoJson) };
            int novos = Importar.Gravar(lista);
            atualizar();
            MessageBox.Show(novos == 0 ? "Os pedidos deste arquivo já foram importados." :
                lista.Count == 1 ? "Pedido " + lista[0].IdPedido + " importado com sucesso!" :
                "Pedidos importados: " + novos);
        }
    }
}
