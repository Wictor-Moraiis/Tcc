using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace MarcenariaMorais
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Telefone1 { get; set; }
        public string Telefone2 { get; set; }
        public string Descricao { get; set; }
        public int CategoriaId { get; set; }

        public static DataTable ListarTodos()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"
                    SELECT f.Func_id, f.Func_cpf, f.Func_nome, f.Func_tel1, f.Func_tel2, f.Func_desc,
                           c.Catg_nome, c.Catg_sal
                    FROM Funcionario f
                    INNER JOIN Categoria_Funcionario c ON f.Catg_id = c.Catg_id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR DADOS ***
                foreach (DataRow row in dt.Rows)
                {
                    row["Func_cpf"] = Criptografia.Descriptografar(row["Func_cpf"].ToString());
                    row["Func_nome"] = Criptografia.Descriptografar(row["Func_nome"].ToString());
                    row["Func_tel1"] = Criptografia.Descriptografar(row["Func_tel1"].ToString());

                    if (row["Func_tel2"] != DBNull.Value && !string.IsNullOrEmpty(row["Func_tel2"].ToString()))
                        row["Func_tel2"] = Criptografia.Descriptografar(row["Func_tel2"].ToString());

                    // Descrição não precisa descriptografar (não é sensível)
                }

                conn.Close();
                return dt;
            }

            return null;
        }

        public bool? Cadastrar()
        {
            // Validação de CPF
            if (!IsDigitsOnly(Cpf) || Cpf.Length != 11)
            {
                MessageBox.Show("O CPF deve conter exatamente 11 números.", "CPF Inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Validação de Telefone 1
            if (!IsDigitsOnly(Telefone1) || Telefone1.Length != 11)
            {
                MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 1 Inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Validação de Telefone 2 (se informado)
            if (!string.IsNullOrWhiteSpace(Telefone2))
            {
                if (!IsDigitsOnly(Telefone2) || Telefone2.Length != 11)
                {
                    MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 2 Inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                if (Telefone1 == Telefone2)
                {
                    MessageBox.Show("Os dois telefones não podem ser iguais.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            // *** VALIDAR DUPLICATAS ANTES DE INSERIR ***
            if (CpfJaExiste(Cpf))
            {
                MessageBox.Show("CPF já está cadastrado no sistema.", "CPF Duplicado!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (TelefoneJaExiste(Telefone1))
            {
                MessageBox.Show("Telefone 1 já cadastrado no sistema!", "Telefone 1 Duplicado!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                // *** CRIPTOGRAFAR DADOS ***
                string cpfCripto = Criptografia.Criptografar(Cpf);
                string cpfHash = Criptografia.GerarHash(Cpf);
                string nomeCripto = Criptografia.Criptografar(Nome);
                string tel1Cripto = Criptografia.Criptografar(Telefone1);
                string tel1Hash = Criptografia.GerarHash(Telefone1);
                string tel2Cripto = string.IsNullOrWhiteSpace(Telefone2) ? null : Criptografia.Criptografar(Telefone2);

                string sql = @"INSERT INTO Funcionario 
                      (Func_cpf, Func_cpf_hash, Func_nome, Func_tel1, Func_tel1_hash, Func_tel2, Func_desc, Catg_id) 
                      VALUES (@cpf, @cpfHash, @nome, @tel1, @tel1Hash, @tel2, @desc, @catg)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@cpf", cpfCripto);
                cmd.Parameters.AddWithValue("@cpfHash", cpfHash);
                cmd.Parameters.AddWithValue("@nome", nomeCripto);
                cmd.Parameters.AddWithValue("@tel1", tel1Cripto);
                cmd.Parameters.AddWithValue("@tel1Hash", tel1Hash);
                cmd.Parameters.AddWithValue("@tel2", (object)tel2Cripto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@desc", Descricao);
                cmd.Parameters.AddWithValue("@catg", CategoriaId);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Funcionário cadastrado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    return false;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        MessageBox.Show("Não pode ter dois funcionários com o mesmo CPF.", "CPF Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                    return false;
                }
            }

            return false;
        }

        public bool? Alterar()
        {
            // Validação de CPF
            if (!IsDigitsOnly(Cpf) || Cpf.Length != 11)
            {
                MessageBox.Show("O CPF deve conter exatamente 11 números.", "CPF inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (!IsDigitsOnly(Telefone1) || Telefone1.Length != 11)
            {
                MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 1 inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(Telefone2))
            {
                if (!IsDigitsOnly(Telefone2) || Telefone2.Length != 11)
                {
                    MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 2 inválido! ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                if (Telefone1 == Telefone2)
                {
                    MessageBox.Show("Os dois telefones não podem ser iguais.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                // *** CRIPTOGRAFAR DADOS ***
                string cpfCripto = Criptografia.Criptografar(Cpf);
                string cpfHash = Criptografia.GerarHash(Cpf);
                string nomeCripto = Criptografia.Criptografar(Nome);
                string tel1Cripto = Criptografia.Criptografar(Telefone1);
                string tel1Hash = Criptografia.GerarHash(Telefone1);
                string tel2Cripto = string.IsNullOrWhiteSpace(Telefone2) ? null : Criptografia.Criptografar(Telefone2);

                string sql = @"UPDATE Funcionario 
                       SET Func_cpf = @cpf,
                           Func_cpf_hash = @cpfHash,
                           Func_nome = @nome, 
                           Func_tel1 = @tel1,
                           Func_tel1_hash = @tel1Hash,
                           Func_tel2 = @tel2, 
                           Func_desc = @desc, 
                           Catg_id = @catg
                       WHERE Func_id = @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@cpf", cpfCripto);
                cmd.Parameters.AddWithValue("@cpfHash", cpfHash);
                cmd.Parameters.AddWithValue("@nome", nomeCripto);
                cmd.Parameters.AddWithValue("@tel1", tel1Cripto);
                cmd.Parameters.AddWithValue("@tel1Hash", tel1Hash);
                cmd.Parameters.AddWithValue("@tel2", (object)tel2Cripto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@desc", Descricao);
                cmd.Parameters.AddWithValue("@catg", CategoriaId);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Funcionário alterado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Funcionário não encontrado ou nenhuma alteração foi realizada.", "Alteração Não Realizada", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        public bool? Excluir()
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                $"Tem certeza que deseja excluir o funcionário selecionado?",
                "Confirmar Exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmacao != MessageBoxResult.Yes)
            {
                return null;
            }

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "DELETE FROM Funcionario WHERE Func_id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Funcionário excluído com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Funcionário não encontrado.");
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

        // *** BUSCAR ID PELO HASH DO CPF ***
        public static int? BuscarIdPorCpf(string cpf)
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string cpfHash = Criptografia.GerarHash(cpf);
                string sql = "SELECT Func_id FROM Funcionario WHERE Func_cpf_hash = @hash";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@hash", cpfHash);

                try
                {
                    object resultado = cmd.ExecuteScalar();
                    conn.Close();

                    if (resultado != null && int.TryParse(resultado.ToString(), out int id))
                    {
                        return id;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                }
            }
            return null;
        }

        // *** VALIDAÇÃO DE DUPLICATAS POR HASH ***
        private bool CpfJaExiste(string cpf)
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string hash = Criptografia.GerarHash(cpf);
                string sql = "SELECT COUNT(*) FROM Funcionario WHERE Func_cpf_hash = @hash AND Func_id != @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@id", Id);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();

                return count > 0;
            }
            return false;
        }

        private bool TelefoneJaExiste(string telefone)
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string hash = Criptografia.GerarHash(telefone);
                string sql = "SELECT COUNT(*) FROM Funcionario WHERE Func_tel1_hash = @hash AND Func_id != @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@id", Id);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();

                return count > 0;
            }
            return false;
        }

        private bool IsDigitsOnly(string str)
        {
            return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
        }
    }
}



    