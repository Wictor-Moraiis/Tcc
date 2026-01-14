using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace MarcenariaMorais
{
    public class Estoque
    {
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public string Telefone { get; set; }
        public string Imagem { get; set; }
        public int Id { get; set; }

        public static DataTable ListarTodos()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"
                    SELECT Estq_id, Estq_produto, Estq_quant, Estq_tel_forne, Estq_img
                    FROM Estoque";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR TELEFONE ***
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Estq_tel_forne"] != DBNull.Value && !string.IsNullOrEmpty(row["Estq_tel_forne"].ToString()))
                    {
                        row["Estq_tel_forne"] = Criptografia.Descriptografar(row["Estq_tel_forne"].ToString());
                    }
                }

                conn.Close();
                return dt;
            }

            return null;
        }

        public static DataTable ListarParaCombo()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "SELECT Estq_id, Estq_produto FROM Estoque ORDER BY Estq_produto";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();

                return dt;
            }
            return null;
        }

        // Método para converter qualquer imagem para PNG
        private void ConverterParaPNG(string origemPath, string destinoPath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(origemPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (FileStream stream = new FileStream(destinoPath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao converter imagem para PNG: {ex.Message}");
            }
        }

        // Método para limpar e encurtar o nome do arquivo
        private string LimparNomeArquivo(string nome)
        {
            if (string.IsNullOrEmpty(nome))
                throw new Exception("O nome do produto não pode estar vazio!");

            // Remove acentos
            string normalizado = RemoverAcentos(nome);

            // Remove caracteres especiais, mantém apenas letras, números e underline
            normalizado = Regex.Replace(normalizado, @"[^a-zA-Z0-9_]", "");

            // Pega apenas as primeiras 8 letras
            if (normalizado.Length > 8)
                normalizado = normalizado.Substring(0, 8);

            // Se ficou vazio após limpeza, retorna erro
            if (string.IsNullOrEmpty(normalizado))
                throw new Exception("O nome do produto contém apenas caracteres especiais.\nPor favor, renomeie o produto usando letras e números!");

            return normalizado.ToLower();
        }

        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;

            StringBuilder sb = new StringBuilder();
            foreach (char c in texto.Normalize(NormalizationForm.FormD))
            {
                if (char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private bool IsDigitsOnly(string str)
        {
            return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
        }

        public int? Cadastrar(string imgLocal = null)
        {
            if (!string.IsNullOrWhiteSpace(Telefone))
            {
                if (!IsDigitsOnly(Telefone) || Telefone.Length != 11)
                {
                    MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                try
                {
                    // *** CRIPTOGRAFAR TELEFONE ***
                    string telCripto = string.IsNullOrWhiteSpace(Telefone) ? null : Criptografia.Criptografar(Telefone);

                    string sql = @"INSERT INTO Estoque (Estq_produto, Estq_quant, Estq_tel_forne, Estq_img)
                                   VALUES (@Produto, @Quant, @Tel_forne, @Img)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Produto", Produto);
                    cmd.Parameters.AddWithValue("@Quant", Quantidade);
                    cmd.Parameters.AddWithValue("@Tel_forne", (object)telCripto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Img", "");

                    cmd.ExecuteNonQuery();

                    int idGerado = (int)cmd.LastInsertedId;

                    // Atualiza o Id da instância
                    Id = idGerado;

                    // Tenta limpar o nome - se der erro, exibe mensagem
                    string nomeBase;
                    try
                    {
                        nomeBase = LimparNomeArquivo(Produto);
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        MessageBox.Show(ex.Message, "Erro no Nome do Produto", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Deleta o registro que foi criado
                        var connTemp = banco.Conectar();
                        if (connTemp != null)
                        {
                            MySqlCommand cmdDel = new MySqlCommand("DELETE FROM Estoque WHERE Estq_id = @id", connTemp);
                            cmdDel.Parameters.AddWithValue("@id", idGerado);
                            cmdDel.ExecuteNonQuery();
                            connTemp.Close();
                        }
                        return null;
                    }

                    // Nome da imagem: primeiras8letras_id.png
                    string nomeImagem = $"{nomeBase}_{idGerado}.png";

                    // Caminho para salvar no banco (com prefixo)
                    string caminhoNoBanco = $"MarcenariaMoraisDados/Img_Estoque/{nomeImagem}";

                    // Pasta local de imagens
                    string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Estoque");
                    if (!Directory.Exists(pastaLocal))
                        Directory.CreateDirectory(pastaLocal);

                    // Salva a imagem localmente convertida para PNG
                    if (!string.IsNullOrEmpty(imgLocal))
                    {
                        try
                        {
                            string destinoCompleto = Path.Combine(pastaLocal, nomeImagem);
                            ConverterParaPNG(imgLocal, destinoCompleto);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao salvar imagem: {ex.Message}");
                        }
                    }

                    // Atualiza o banco com o caminho completo da imagem
                    string sqlUpdate = @"UPDATE Estoque 
                        SET Estq_img = @img
                        WHERE Estq_id = @id";

                    MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@img", caminhoNoBanco);
                    cmdUpdate.Parameters.AddWithValue("@id", idGerado);

                    if (!string.IsNullOrEmpty(imgLocal)) // Editado -> Adicionado para não executar caso a image não exista
                        cmdUpdate.ExecuteNonQuery();

                    MessageBox.Show("Produto cadastrado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                    conn.Close();
                    return idGerado;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062) 
                    {
                        MessageBox.Show("Não pode ter dois produtos com o mesmo nome.", "Produto Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return null;
                    }
                    MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                    conn.Close();
                    return null;
                }
            }

            return null;
        }

        public bool? Alterar(string imgLocal = null)
        {
            if (!string.IsNullOrWhiteSpace(Telefone))
            {
                if (!IsDigitsOnly(Telefone) || Telefone.Length != 11)
                {
                    MessageBox.Show("O telefone deve conter exatamente 11 números.", "Telefone Inválido!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn == null) return false;

            // Busca o nome antigo e imagem antiga do banco
            string sqlSelect = "SELECT Estq_produto, Estq_img FROM Estoque WHERE Estq_id = @id";
            MySqlCommand cmdSelect = new MySqlCommand(sqlSelect, conn);
            cmdSelect.Parameters.AddWithValue("@id", Id);

            string nomeAntigoLimpo = null;
            string imgAntiga = null;

            using (var reader = cmdSelect.ExecuteReader())
            {
                if (reader.Read())
                {
                    string nomeAntigo = reader["Estq_produto"].ToString();
                    try { nomeAntigoLimpo = LimparNomeArquivo(nomeAntigo); } catch { }

                    imgAntiga = reader["Estq_img"] != DBNull.Value ? reader["Estq_img"].ToString() : null;
                }
            }

            // Tenta limpar o nome - se der erro, exibe mensagem
            string nomeBase;
            try
            {
                nomeBase = LimparNomeArquivo(Produto);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Erro no Nome do Produto", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            // Verifica se o nome mudou
            bool nomeMudou = (nomeAntigoLimpo != null && nomeAntigoLimpo != nomeBase);

            // Pasta local de imagens
            string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Estoque");
            if (!Directory.Exists(pastaLocal))
                Directory.CreateDirectory(pastaLocal);

            // Nome da nova imagem (SEMPRE usa o padrão)
            string nomeImagem = $"{nomeBase}_{Id}.png";
            string caminhoNoBanco = $"MarcenariaMoraisDados/Img_Estoque/{nomeImagem}";
            string destinoCompleto = Path.Combine(pastaLocal, nomeImagem);

            // Se forneceu nova imagem
            if (!string.IsNullOrEmpty(imgLocal))
            {
                try
                {
                    // DELETA a imagem antiga (mesmo nome) ANTES de salvar
                    if (File.Exists(destinoCompleto))
                    {
                        try
                        {
                            File.Delete(destinoCompleto);
                            // Força liberação de memória
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao deletar imagem antiga: {ex.Message}");
                        }
                    }

                    // Converte a NOVA imagem para PNG com o nome padrão
                    ConverterParaPNG(imgLocal, destinoCompleto);
                    Imagem = caminhoNoBanco;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar imagem: {ex.Message}");
                }
            }

            // Se o nome mudou mas NÃO forneceu nova imagem
            if (nomeMudou && string.IsNullOrEmpty(imgLocal) && !string.IsNullOrEmpty(imgAntiga))
            {
                try
                {
                    string nomeArquivoAntigo = imgAntiga.Replace("MarcenariaMoraisDados/Img_Estoque/", "");
                    string caminhoAntigo = Path.Combine(pastaLocal, nomeArquivoAntigo);

                    if (File.Exists(caminhoAntigo))
                    {
                        // COPIA (não converte) a imagem antiga com o novo nome
                        // Já está em PNG, só precisa copiar
                        File.Copy(caminhoAntigo, destinoCompleto, true);

                        // DELETA a imagem antiga NA HORA
                        try
                        {
                            File.Delete(caminhoAntigo);
                        }
                        catch { }

                        Imagem = caminhoNoBanco;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao renomear imagem: {ex.Message}");
                }
            }

            // Se o nome mudou E forneceu nova imagem, deleta a antiga com nome diferente
            if (nomeMudou && !string.IsNullOrEmpty(imgLocal) && !string.IsNullOrEmpty(imgAntiga))
            {
                try
                {
                    string nomeArquivoAntigo = imgAntiga.Replace("MarcenariaMoraisDados/Img_Estoque/", "");
                    string caminhoAntigo = Path.Combine(pastaLocal, nomeArquivoAntigo);

                    if (File.Exists(caminhoAntigo) && caminhoAntigo != destinoCompleto)
                    {
                        File.Delete(caminhoAntigo);
                    }
                }
                catch { }
            }

            try
            {
                // *** CRIPTOGRAFAR TELEFONE ***
                string telCripto = string.IsNullOrWhiteSpace(Telefone) ? null : Criptografia.Criptografar(Telefone);

                string sql = @"UPDATE Estoque 
                  SET Estq_produto = @Produto, 
                  Estq_quant = @Quant, 
                  Estq_tel_forne = @Tel_forne";

                if (!string.IsNullOrEmpty(Imagem))
                    sql += ", Estq_img = @Img";

                sql += " WHERE Estq_id = @Id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Produto", Produto);
                cmd.Parameters.AddWithValue("@Quant", Quantidade);
                cmd.Parameters.AddWithValue("@Tel_forne", (object)telCripto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", Id);

                if (!string.IsNullOrEmpty(Imagem))
                    cmd.Parameters.AddWithValue("@Img", Imagem);

                int linhas = cmd.ExecuteNonQuery();
                conn.Close();

                if (linhas > 0)
                {
                    MessageBox.Show("Produto alterado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Produto não encontrado.");
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    MessageBox.Show("Não pode ter dois produtos com o mesmo nome.", "Produto Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                MessageBox.Show($"Erro MySQL {ex.Number}: {ex.Message}");
                conn.Close();
                return false;
            }
        }

        public bool? Excluir()
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                $"Tem certeza que deseja excluir o produto '{Produto}'?\nA imagem também será removida.",
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
                string sqlSelect = "SELECT Estq_img FROM Estoque WHERE Estq_id = @id";
                MySqlCommand cmdSelect = new MySqlCommand(sqlSelect, conn);
                cmdSelect.Parameters.AddWithValue("@id", Id);

                try
                {
                    using (var reader = cmdSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Imagem = reader["Estq_img"] != DBNull.Value ? reader["Estq_img"].ToString() : null;
                        }
                    }

                    string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Estoque");

                    // DELETA a imagem NA HORA
                    if (!string.IsNullOrEmpty(Imagem))
                    {
                        string nomeArquivo = Imagem.Replace("MarcenariaMoraisDados/Img_Estoque/", "");
                        string caminhoCompleto = Path.Combine(pastaLocal, nomeArquivo);

                        try
                        {
                            if (File.Exists(caminhoCompleto))
                                File.Delete(caminhoCompleto);
                        }
                        catch { }
                    }

                    // Deleta do banco
                    string sql = "DELETE FROM Estoque WHERE Estq_id = @Id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Id", Id);

                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Produto excluído com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Produto não encontrado.");
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