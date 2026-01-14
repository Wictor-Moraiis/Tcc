using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace MarcenariaMorais
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Telefone1 { get; set; }
        public string Telefone2 { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Rua { get; set; }
        public int Num_casa { get; set; }
        public string Complemento { get; set; }

        public static DataTable ListarTodos()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"
                    SELECT Cli_id, Cli_cpf, Cli_nome, Cli_tel1, Cli_tel2, Cli_cep,
                           Cli_bairro, Cli_rua, Cli_num_casa, Cli_complemento
                    FROM Cliente";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR DADOS ***
                foreach (DataRow row in dt.Rows)
                {
                    row["Cli_cpf"] = Criptografia.Descriptografar(row["Cli_cpf"].ToString());
                    row["Cli_nome"] = Criptografia.Descriptografar(row["Cli_nome"].ToString());
                    row["Cli_tel1"] = Criptografia.Descriptografar(row["Cli_tel1"].ToString());

                    if (row["Cli_tel2"] != DBNull.Value && !string.IsNullOrEmpty(row["Cli_tel2"].ToString()))
                        row["Cli_tel2"] = Criptografia.Descriptografar(row["Cli_tel2"].ToString());

                    row["Cli_bairro"] = Criptografia.Descriptografar(row["Cli_bairro"].ToString());
                    row["Cli_rua"] = Criptografia.Descriptografar(row["Cli_rua"].ToString());
                    row["Cli_num_casa"] = Criptografia.Descriptografar(row["Cli_num_casa"].ToString());

                    if (row["Cli_complemento"] != DBNull.Value && !string.IsNullOrEmpty(row["Cli_complemento"].ToString()))
                        row["Cli_complemento"] = Criptografia.Descriptografar(row["Cli_complemento"].ToString());
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
                MessageBox.Show("O CPF deve conter exatamente 11 números.", "CPF inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Validação de Telefone 1
            if (!IsDigitsOnly(Telefone1) || Telefone1.Length != 11)
            {
                MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 1 inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Validação de Telefone 2 (se informado)
            if (!string.IsNullOrWhiteSpace(Telefone2))
            {
                if (!IsDigitsOnly(Telefone2) || Telefone2.Length != 11)
                {
                    MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 2 inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                if (Telefone1 == Telefone2)
                {
                    MessageBox.Show("Os dois telefones não podem ser iguais.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            // *** VALIDAR DUPLICATAS ANTES DE INSERIR ***
            if (CpfJaExiste(Cpf))
            {
                MessageBox.Show("CPF já cadastrado no sistema!", "CPF Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (TelefoneJaExiste(Telefone1))
            {
                MessageBox.Show("Telefone 1 já cadastrado no sistema!", "Telefone 1 Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                string bairroCripto = Criptografia.Criptografar(Bairro);
                string ruaCripto = Criptografia.Criptografar(Rua);
                string numCripto = Criptografia.Criptografar(Num_casa.ToString());
                string complCripto = string.IsNullOrWhiteSpace(Complemento) ? null : Criptografia.Criptografar(Complemento);

                string sql = @"INSERT INTO Cliente
                      (Cli_cpf, Cli_cpf_hash, Cli_nome, Cli_tel1, Cli_tel1_hash, Cli_tel2, 
                       Cli_cep, Cli_bairro, Cli_rua, Cli_num_casa, Cli_complemento) 
                      VALUES (@cpf, @cpfHash, @nome, @tel1, @tel1Hash, @tel2, 
                              @cep, @bairro, @rua, @numCasa, @complemento)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@cpf", cpfCripto);
                cmd.Parameters.AddWithValue("@cpfHash", cpfHash);
                cmd.Parameters.AddWithValue("@nome", nomeCripto);
                cmd.Parameters.AddWithValue("@tel1", tel1Cripto);
                cmd.Parameters.AddWithValue("@tel1Hash", tel1Hash);
                cmd.Parameters.AddWithValue("@tel2", (object)tel2Cripto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cep", Cep);
                cmd.Parameters.AddWithValue("@bairro", bairroCripto);
                cmd.Parameters.AddWithValue("@rua", ruaCripto);
                cmd.Parameters.AddWithValue("@numCasa", numCripto);
                cmd.Parameters.AddWithValue("@complemento", (object)complCripto ?? DBNull.Value);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Cliente cadastrado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public bool? Alterar()
        {
            // Validações
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
                    MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone 2 inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                if (Telefone1 == Telefone2)
                {
                    MessageBox.Show("Os dois telefones não podem ser iguais.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                string bairroCripto = Criptografia.Criptografar(Bairro);
                string ruaCripto = Criptografia.Criptografar(Rua);
                string numCripto = Criptografia.Criptografar(Num_casa.ToString());
                string complCripto = string.IsNullOrWhiteSpace(Complemento) ? null : Criptografia.Criptografar(Complemento);

                string sql = @"UPDATE Cliente
                       SET Cli_cpf = @cpf,
                           Cli_cpf_hash = @cpfHash,
                           Cli_nome = @nome,
                           Cli_tel1 = @tel1,
                           Cli_tel1_hash = @tel1Hash,
                           Cli_tel2 = @tel2,
                           Cli_cep = @cep,
                           Cli_bairro = @bairro,
                           Cli_rua = @rua,
                           Cli_num_casa = @numCasa,
                           Cli_complemento = @complemento
                       WHERE Cli_id = @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@cpf", cpfCripto);
                cmd.Parameters.AddWithValue("@cpfHash", cpfHash);
                cmd.Parameters.AddWithValue("@nome", nomeCripto);
                cmd.Parameters.AddWithValue("@tel1", tel1Cripto);
                cmd.Parameters.AddWithValue("@tel1Hash", tel1Hash);
                cmd.Parameters.AddWithValue("@tel2", (object)tel2Cripto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cep", Cep);
                cmd.Parameters.AddWithValue("@bairro", bairroCripto);
                cmd.Parameters.AddWithValue("@rua", ruaCripto);
                cmd.Parameters.AddWithValue("@numCasa", numCripto);
                cmd.Parameters.AddWithValue("@complemento", (object)complCripto ?? DBNull.Value);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (linhas > 0)
                    {
                        MessageBox.Show("Cliente alterado com sucesso", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Cliente não encontrado ou nenhuma alteração realizada.");
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
                $"Tem certeza que deseja excluir o cliente selecionado?",
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
                string sql = "DELETE FROM Cliente WHERE Cli_id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);

                try
                {
                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Cliente excluído com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Cliente não encontrado.");
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

        // *** VALIDAÇÃO DE DUPLICATAS POR HASH ***
        private bool CpfJaExiste(string cpf)
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string hash = Criptografia.GerarHash(cpf);
                string sql = "SELECT COUNT(*) FROM Cliente WHERE Cli_cpf_hash = @hash AND Cli_id != @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@id", Id); // Para permitir alterar o próprio registro

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
                string sql = "SELECT COUNT(*) FROM Cliente WHERE Cli_tel1_hash = @hash AND Cli_id != @id";

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

        public static DataTable ListarParaCombo()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "SELECT Cli_id, Cli_cpf, Cli_nome FROM Cliente ORDER BY Cli_nome";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR PARA COMBO ***
                foreach (DataRow row in dt.Rows)
                {
                    row["Cli_id"] = Criptografia.Descriptografar(row["Cli_id"].ToString());
                    row["Cli_nome"] = Criptografia.Descriptografar(row["Cli_nome"].ToString());
                }

                conn.Close();
                return dt;
            }
            return null;
        }
    }
}