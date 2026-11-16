using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

class Gerencia {
    [STAThread] static void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using(var login=new FrmLogin()) if(login.ShowDialog()==DialogResult.OK) Application.Run(new FrmPrincipal());
    }
}
