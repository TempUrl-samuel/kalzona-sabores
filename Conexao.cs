using System;
using MySql.Data.MySqlClient;

public class Conexao {
    public MySqlConnection conectarBD() {
        string dadosConexao = Environment.GetEnvironmentVariable("KALZONA_DB");
        if (String.IsNullOrWhiteSpace(dadosConexao))
            throw new InvalidOperationException("Configure KALZONA_DB para acessar delivery_calzone_pizza.");
        var conn = new MySqlConnection(dadosConexao);
        try {
            conn.Open();
            return conn;
        } catch (MySqlException ex) {
            conn.Dispose();
            throw new InvalidOperationException("Conexão com o MySQL não estabelecida.", ex);
        }
    }
}
