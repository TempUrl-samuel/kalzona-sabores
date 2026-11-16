partial class FrmLogin
{
    private System.Windows.Forms.FlowLayoutPanel campos;
    private System.Windows.Forms.Label lblemail;
    private System.Windows.Forms.TextBox email;
    private System.Windows.Forms.Label lblsenha;
    private System.Windows.Forms.TextBox senha;
    private System.Windows.Forms.Button btnConfirmar;

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.campos = new System.Windows.Forms.FlowLayoutPanel();
        this.campos.Name = "campos";
        this.campos.Dock = System.Windows.Forms.DockStyle.Fill;
        this.campos.Padding = new System.Windows.Forms.Padding(20);
        this.campos.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.campos.WrapContents = false;
        this.lblemail = new System.Windows.Forms.Label();
        this.lblemail.Name = "lblemail";
        this.lblemail.Text = "E-mail do gerente";
        this.lblemail.AutoSize = true;
        this.email = new System.Windows.Forms.TextBox();
        this.email.Name = "email";
        this.email.Width = 280;
        this.email.MaxLength = 100;
        this.lblsenha = new System.Windows.Forms.Label();
        this.lblsenha.Name = "lblsenha";
        this.lblsenha.Text = "Senha";
        this.lblsenha.AutoSize = true;
        this.senha = new System.Windows.Forms.TextBox();
        this.senha.Name = "senha";
        this.senha.Width = 280;
        this.senha.MaxLength = 64;
        this.senha.UseSystemPasswordChar = true;
        this.btnConfirmar = new System.Windows.Forms.Button();
        this.btnConfirmar.Name = "btnConfirmar";
        this.btnConfirmar.Text = "Entrar";
        this.btnConfirmar.AutoSize = true;
        this.Controls.Add(this.campos);
        this.campos.Controls.Add(this.lblemail);
        this.campos.Controls.Add(this.email);
        this.campos.Controls.Add(this.lblsenha);
        this.campos.Controls.Add(this.senha);
        this.campos.Controls.Add(this.btnConfirmar);
        this.Name = "FrmLogin";
        this.Size = new System.Drawing.Size(360, 255);
        this.Text = "Kalzona | Entrar";
        this.AcceptButton = this.btnConfirmar;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.ResumeLayout(false);
    }
}
