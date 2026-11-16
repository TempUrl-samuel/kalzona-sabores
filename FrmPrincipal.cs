using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

partial class FrmPrincipal : Form {
    public FrmPrincipal() {
        InitializeComponent();
        ConfigurarProdutos();
        ConfigurarCategorias();
        ConfigurarEntregadores();
        ConfigurarContato();
        Shown += delegate { painelPedidos.Atualizar(); };
        abaPedidos.Enter += delegate { painelPedidos.Atualizar(); };
    }
    void Ligar(Button b, Action acao) { b.Click += delegate { try { acao(); } catch(Exception ex) { MessageBox.Show(this, ex.Message, "Kalzona"); } }; }
    void ConfigurarProdutos() {
        var grid = gradeProdutos;
        Action atualizar = delegate { using (var db = new Dados()) grid.DataSource = db.Tabela("SELECT id_produto AS id,nome,tipo AS categoria,preco FROM produto WHERE disponivel=1 ORDER BY id_produto"); };
        Ligar(atualizarProdutos, atualizar);
        abaProdutos.Enter += delegate { try { atualizar(); } catch(Exception ex) { MessageBox.Show(ex.Message); } };
        Func<int> id=delegate {Regras.Exigir(grid.SelectedRows.Count>0,"Selecione uma linha.");return Convert.ToInt32(grid.SelectedRows[0].Cells["id"].Value);};
grid.SelectionChanged+=delegate {if(grid.SelectedRows.Count>0 && grid.Columns.Contains("preco"))preco.Value=Math.Max(preco.Minimum,Convert.ToDecimal(grid.SelectedRows[0].Cells["preco"].Value));};
            Ligar(salvarPreco,delegate {
                var produto = new Gestao.Produto {
                    Id = id(), Preco = preco.Value,
                    Nome = Convert.ToString(grid.SelectedRows[0].Cells["nome"].Value),
                    Categoria = Convert.ToString(grid.SelectedRows[0].Cells["categoria"].Value)
                };
                using(var db=new Dados()) db.Executar("UPDATE produto SET preco=? WHERE id_produto=?",produto.Preco,produto.Id);
                atualizar();
            });
            Ligar(excluirProduto,delegate {int produto=id();if(!Confirmar())return;using(var db=new Dados())db.Executar("UPDATE produto SET disponivel=0 WHERE id_produto=?",produto);atualizar();});
    }
    void ConfigurarCategorias() {
        var grid = gradeCategorias;
        Action atualizar = delegate { using (var db = new Dados()) grid.DataSource = db.Tabela("SELECT id,nome,descricao FROM categoria ORDER BY id"); };
        Ligar(atualizarCategorias, atualizar);
        abaCategorias.Enter += delegate { try { atualizar(); } catch(Exception ex) { MessageBox.Show(ex.Message); } };
        Func<int> id=delegate {Regras.Exigir(grid.SelectedRows.Count>0,"Selecione uma linha.");return Convert.ToInt32(grid.SelectedRows[0].Cells["id"].Value);};
string tabela="categoria", coluna="descricao"; var nome=nomeCategorias; var valor=valorCategorias;
        grid.SelectionChanged+=delegate {if(grid.SelectedRows.Count>0 && grid.Columns.Contains(coluna)) {nome.Text=Convert.ToString(grid.SelectedRows[0].Cells["nome"].Value);valor.Text=Convert.ToString(grid.SelectedRows[0].Cells[coluna].Value);}};
            Ligar(InserirCategorias,delegate {using(var db=new Dados())db.Executar("INSERT INTO "+tabela+"(nome,"+coluna+") VALUES(?,?)",Regras.Texto(nome.Text,80),Regras.Texto(valor.Text,valor.MaxLength));atualizar();});
            Ligar(AlterarCategorias,delegate {using(var db=new Dados())db.Executar("UPDATE "+tabela+" SET nome=?,"+coluna+"=? WHERE id=?",Regras.Texto(nome.Text,80),Regras.Texto(valor.Text,valor.MaxLength),id());atualizar();});
            Ligar(ExcluirCategorias,delegate {int registro=id();if(!Confirmar())return;using(var db=new Dados())db.Executar("DELETE FROM "+tabela+" WHERE id=?",registro);atualizar();});
    }
    void ConfigurarEntregadores() {
        var grid = gradeEntregadores;
        Action atualizar = delegate { using (var db = new Dados()) grid.DataSource = db.Tabela("SELECT id,nome,telefone FROM entregador ORDER BY id"); };
        Ligar(atualizarEntregadores, atualizar);
        abaEntregadores.Enter += delegate { try { atualizar(); } catch(Exception ex) { MessageBox.Show(ex.Message); } };
        Func<int> id=delegate {Regras.Exigir(grid.SelectedRows.Count>0,"Selecione uma linha.");return Convert.ToInt32(grid.SelectedRows[0].Cells["id"].Value);};
string tabela="entregador", coluna="telefone"; var nome=nomeEntregadores; var valor=valorEntregadores;
        grid.SelectionChanged+=delegate {if(grid.SelectedRows.Count>0 && grid.Columns.Contains(coluna)) {nome.Text=Convert.ToString(grid.SelectedRows[0].Cells["nome"].Value);valor.Text=Convert.ToString(grid.SelectedRows[0].Cells[coluna].Value);}};
            Ligar(InserirEntregadores,delegate {using(var db=new Dados())db.Executar("INSERT INTO "+tabela+"(nome,"+coluna+") VALUES(?,?)",Regras.Texto(nome.Text,80),Regras.Texto(valor.Text,valor.MaxLength));atualizar();});
            Ligar(AlterarEntregadores,delegate {using(var db=new Dados())db.Executar("UPDATE "+tabela+" SET nome=?,"+coluna+"=? WHERE id=?",Regras.Texto(nome.Text,80),Regras.Texto(valor.Text,valor.MaxLength),id());atualizar();});
            Ligar(ExcluirEntregadores,delegate {int registro=id();if(!Confirmar())return;using(var db=new Dados())db.Executar("DELETE FROM "+tabela+" WHERE id=?",registro);atualizar();});
    }
    void ConfigurarContato() {
        var grid = gradeContato;
        Action atualizar = delegate { using (var db = new Dados()) grid.DataSource = db.Tabela("SELECT id,nome,email,assunto,mensagem FROM contato ORDER BY id DESC"); };
        Ligar(atualizarContato, atualizar);
        abaContato.Enter += delegate { try { atualizar(); } catch(Exception ex) { MessageBox.Show(ex.Message); } };
    }
    bool Confirmar() { return MessageBox.Show(this,"Confirma a exclusão do registro selecionado?","Kalzona",MessageBoxButtons.YesNo)==DialogResult.Yes; }
}
