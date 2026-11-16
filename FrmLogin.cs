using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

partial class FrmLogin : Form {
    public FrmLogin() { InitializeComponent(); Ligar(btnConfirmar, Entrar); }
    void Ligar(Button b, Action acao) { b.Click += delegate { try { acao(); } catch(Exception ex) { MessageBox.Show(this, ex.Message, "Kalzona"); } }; }
    void Entrar() {
            using (var conn = new Conexao().conectarBD())
            using (var cmd = new MySqlCommand("SELECT senha FROM usuarios WHERE email = @email",conn)) {
                cmd.Parameters.AddWithValue("@email",email.Text.Trim());
                using (MySqlDataReader leitor = cmd.ExecuteReader())
                    Regras.Exigir(leitor.Read() && Dados.Confere(senha.Text,leitor["senha"]),"E-mail ou senha incorretos.");
            }
            DialogResult=DialogResult.OK;
    }
}
