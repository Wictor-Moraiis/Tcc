using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;

namespace MarcenariaMorais
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataRealizado { get; set; }
        public DateTime DataEntrega { get; set; }
        public bool Executado { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public int Cli_id { get; set; }  // ← MUDOU DE Cli_cpf PARA Cli_id
        public int? Estq_id1 { get; set; }
        public int? Estq_id2 { get; set; }
        public int? Estq_id3 { get; set; }
        public int? Estq_id4 { get; set; }
        public int? Estq_id5 { get; set; }

        public static DataTable ListarTodos()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"
                    SELECT 
                        p.Ped_id,
                        p.Ped_realizado,
                        p.Ped_entrega,
                        p.Ped_executado,
                        p.Ped_desc,
                        p.Ped_valor,
                        p.Cli_id,
                        c.Cli_nome,
                        p.Estq_id1,
                        p.Estq_id2,
                        p.Estq_id3,
                        p.Estq_id4,
                        p.Estq_id5,
                        e1.Estq_produto AS Estq_produto1,
                        e2.Estq_produto AS Estq_produto2,
                        e3.Estq_produto AS Estq_produto3,
                        e4.Estq_produto AS Estq_produto4,
                        e5.Estq_produto AS Estq_produto5
                    FROM Pedido p
                    INNER JOIN Cliente c ON p.Cli_id = c.Cli_id
                    LEFT JOIN Estoque e1 ON p.Estq_id1 = e1.Estq_id
                    LEFT JOIN Estoque e2 ON p.Estq_id2 = e2.Estq_id
                    LEFT JOIN Estoque e3 ON p.Estq_id3 = e3.Estq_id
                    LEFT JOIN Estoque e4 ON p.Estq_id4 = e4.Estq_id
                    LEFT JOIN Estoque e5 ON p.Estq_id5 = e5.Estq_id
                    ORDER BY p.Ped_realizado DESC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR NOME DO CLIENTE ***
                foreach (DataRow row in dt.Rows)
                {
                    row["Cli_nome"] = Criptografia.Descriptografar(row["Cli_nome"].ToString());
                }

                conn.Close();
                return dt;
            }
            return null;
        }

        public static DataTable ListarEmExecucao()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"
                    SELECT p.Ped_id, p.Ped_realizado, p.Ped_entrega, 
                           p.Ped_desc, p.Ped_valor, c.Cli_nome
                    FROM Pedido p
                    INNER JOIN Cliente c ON p.Cli_id = c.Cli_id
                    WHERE p.Ped_executado = 0
                    ORDER BY p.Ped_entrega ASC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR NOME DO CLIENTE ***
                foreach (DataRow row in dt.Rows)
                {
                    row["Cli_nome"] = Criptografia.Descriptografar(row["Cli_nome"].ToString());
                }

                conn.Close();
                return dt;
            }
            return null;
        }

        public int? Cadastrar()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"INSERT INTO Pedido 
                    (Ped_realizado, Ped_entrega, Ped_executado, Ped_desc, Ped_valor, Cli_id, 
                     Estq_id1, Estq_id2, Estq_id3, Estq_id4, Estq_id5)
                    VALUES (@realizado, @entrega, @executado, @desc, @valor, @cliId, 
                            @est1, @est2, @est3, @est4, @est5)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@realizado", DataRealizado.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@entrega", DataEntrega.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@executado", Executado);
                cmd.Parameters.AddWithValue("@desc", Descricao);
                cmd.Parameters.AddWithValue("@valor", Valor);
                cmd.Parameters.AddWithValue("@cliId", Cli_id);  // ← MUDOU
                cmd.Parameters.AddWithValue("@est1", Estq_id1.HasValue ? (object)Estq_id1.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est2", Estq_id2.HasValue ? (object)Estq_id2.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est3", Estq_id3.HasValue ? (object)Estq_id3.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est4", Estq_id4.HasValue ? (object)Estq_id4.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est5", Estq_id5.HasValue ? (object)Estq_id5.Value : DBNull.Value);

                try
                {
                    cmd.ExecuteNonQuery();
                    int idGerado = (int)cmd.LastInsertedId;
                    MessageBox.Show("Pedido cadastrado com sucesso!", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                    conn.Close();
                    return idGerado;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                    return null;
                }
            }
            return null;
        }

        public bool? Alterar()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"UPDATE Pedido SET 
                    Ped_realizado=@realizado, Ped_entrega=@entrega, Ped_executado=@executado,
                    Ped_desc=@desc, Ped_valor=@valor, Cli_id=@cliId,
                    Estq_id1=@est1, Estq_id2=@est2, Estq_id3=@est3, Estq_id4=@est4, Estq_id5=@est5
                    WHERE Ped_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@realizado", DataRealizado.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@entrega", DataEntrega.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@executado", Executado);
                cmd.Parameters.AddWithValue("@desc", Descricao);
                cmd.Parameters.AddWithValue("@valor", Valor);
                cmd.Parameters.AddWithValue("@cliId", Cli_id);  // ← MUDOU
                cmd.Parameters.AddWithValue("@est1", Estq_id1.HasValue ? (object)Estq_id1.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est2", Estq_id2.HasValue ? (object)Estq_id2.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est3", Estq_id3.HasValue ? (object)Estq_id3.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est4", Estq_id4.HasValue ? (object)Estq_id4.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@est5", Estq_id5.HasValue ? (object)Estq_id5.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Pedido alterado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Pedido não encontrado.");
                        return false;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                    return false;
                }
            }
            return false;
        }

        public bool? MarcarComoExecutado()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "UPDATE Pedido SET Ped_executado=1, Ped_entrega=@entrega WHERE Ped_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@entrega", DataEntrega.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Pedido marcado como executado!");
                        return true;
                    }
                    return false;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                    return false;
                }
            }
            return false;
        }

        public bool? Excluir()
        {
            MessageBoxResult confirm = MessageBox.Show(
                $"Deseja excluir o pedido #{Id}?",
                "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return null;

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "DELETE FROM Pedido WHERE Ped_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Pedido excluído com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Pedido não encontrado.");
                        return false;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                    return false;
                }
            }
            return false;
        }
    }
}