partial class FrmPedidos
{
    private System.Windows.Forms.DataGridView gradePedidos;
    private System.Windows.Forms.FlowLayoutPanel barraPedidos;
    private System.Windows.Forms.Button atualizarPedidos;
    private System.Windows.Forms.Button btnImportar;
    private System.Windows.Forms.ComboBox status;
    private System.Windows.Forms.Button salvarStatus;
    private System.Windows.Forms.Button verItens;
    private System.Windows.Forms.Button cancelar;

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.gradePedidos = new System.Windows.Forms.DataGridView();
        this.gradePedidos.Name = "gradePedidos";
        this.gradePedidos.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gradePedidos.ReadOnly = true;
        this.gradePedidos.AllowUserToAddRows = false;
        this.gradePedidos.AllowUserToDeleteRows = false;
        this.gradePedidos.MultiSelect = false;
        this.gradePedidos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.gradePedidos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
        this.barraPedidos = new System.Windows.Forms.FlowLayoutPanel();
        this.barraPedidos.Name = "barraPedidos";
        this.barraPedidos.Dock = System.Windows.Forms.DockStyle.Top;
        this.barraPedidos.Height = 85;
        this.barraPedidos.AutoScroll = true;
        this.barraPedidos.BackColor = System.Drawing.Color.FromArgb(255,153,85);
        this.barraPedidos.Padding = new System.Windows.Forms.Padding(5);
        this.atualizarPedidos = new System.Windows.Forms.Button();
        this.atualizarPedidos.Name = "atualizarPedidos";
        this.atualizarPedidos.Text = "Atualizar";
        this.atualizarPedidos.AutoSize = true;
        this.btnImportar = new System.Windows.Forms.Button();
        this.btnImportar.Name = "btnImportar";
        this.btnImportar.Text = "Importar JSON";
        this.btnImportar.AutoSize = true;
        this.status = new System.Windows.Forms.ComboBox();
        this.status.Name = "status";
        this.status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.salvarStatus = new System.Windows.Forms.Button();
        this.salvarStatus.Name = "salvarStatus";
        this.salvarStatus.Text = "Salvar status";
        this.salvarStatus.AutoSize = true;
        this.verItens = new System.Windows.Forms.Button();
        this.verItens.Name = "verItens";
        this.verItens.Text = "Ver itens";
        this.verItens.AutoSize = true;
        this.cancelar = new System.Windows.Forms.Button();
        this.cancelar.Name = "cancelar";
        this.cancelar.Text = "Cancelar e excluir";
        this.cancelar.AutoSize = true;
        this.Controls.Add(this.gradePedidos);
        this.Controls.Add(this.barraPedidos);
        this.barraPedidos.Controls.Add(this.atualizarPedidos);
        this.barraPedidos.Controls.Add(this.btnImportar);
        this.barraPedidos.Controls.Add(this.status);
        this.barraPedidos.Controls.Add(this.salvarStatus);
        this.barraPedidos.Controls.Add(this.verItens);
        this.barraPedidos.Controls.Add(this.cancelar);
        this.Name = "FrmPedidos";
        this.Size = new System.Drawing.Size(1030, 575);
        this.Text = "Pedidos";
        this.ResumeLayout(false);
    }
}
