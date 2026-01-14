using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;

namespace MarcenariaMorais
{
    public class Cat_Funcionario
    {
        public int Catg_id { get; set; }
        public string Catg_nome { get; set; }
        public double Catg_sal { get; set; }

        public static List<Cat_Funcionario> Listar()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();
            List<Cat_Funcionario> categorias = new List<Cat_Funcionario>();

            if (conn != null)
            {
                string sql = "SELECT Catg_id, Catg_nome, Catg_sal FROM Categoria_Funcionario ORDER BY Catg_nome";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categorias.Add(new Cat_Funcionario
                        {
                            Catg_id = reader.GetInt32("Catg_id"),
                            Catg_nome = reader.GetString("Catg_nome"),
                            Catg_sal = reader.GetDouble("Catg_sal")
                        });
                    }
                }
                conn.Close();
            }
            return categorias;
        }

        public static DataTable ListarParaCombo()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "SELECT Catg_id, Catg_nome FROM Categoria_Funcionario ORDER BY Catg_nome";
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
                string sql = "INSERT INTO Categoria_Funcionario (Catg_nome, Catg_sal) VALUES (@nome, @sal)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", Catg_nome);
                cmd.Parameters.AddWithValue("@sal", Catg_sal);

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
                string sql = "UPDATE Categoria_Funcionario SET Catg_nome=@nome, Catg_sal=@sal WHERE Catg_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", Catg_nome);
                cmd.Parameters.AddWithValue("@sal", Catg_sal);
                cmd.Parameters.AddWithValue("@id", Catg_id);

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
                $"Deseja excluir a categoria '{Catg_nome}'?",
                "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return null;

            Banco banco = new Banco();
            var conn = banco.Conectar();
            if (conn != null)
            {
                string sql = "DELETE FROM Categoria_Funcionario WHERE Catg_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Catg_id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (linhas > 0)
                    {
                        MessageBox.Show("Categoria excluída com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Não encontrada.");
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
