using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;

namespace MarcenariaMorais
{
    public class Financeiro
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public double Valor { get; set; }
        public DateTime Data { get; set; }

        public static DataTable ListarTodos()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "SELECT Fin_id, Fin_mov_desc, Fin_mov_val, Fin_data FROM Financeiro ORDER BY Fin_data DESC";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
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
                string sql = "INSERT INTO Financeiro (Fin_mov_desc, Fin_mov_val, Fin_data) VALUES (@desc, @valor, @data)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@desc", Desc);
                cmd.Parameters.AddWithValue("@valor", Valor);
                cmd.Parameters.AddWithValue("@data", Data.ToString("yyyy-MM-dd"));

                try
                {
                    cmd.ExecuteNonQuery();
                    int idGerado = (int)cmd.LastInsertedId;
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
                string sql = "UPDATE Financeiro SET Fin_mov_desc=@desc, Fin_mov_val=@valor, Fin_data=@data WHERE Fin_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@desc", Desc);
                cmd.Parameters.AddWithValue("@valor", Valor);
                cmd.Parameters.AddWithValue("@data", Data.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();
                    return linhas > 0;
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
                $"Deseja realmente excluir o lançamento '{Desc}'?",
                "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return null;

            Banco banco = new Banco();
            var conn = banco.Conectar();
            if (conn != null)
            {
                string sql = "DELETE FROM Financeiro WHERE Fin_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (linhas > 0)
                    {
                        MessageBox.Show("Lançamento excluído com sucesso", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Não encontrado.");
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