using System;
using System.Data;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Text;
using System.Text.RegularExpressions;

namespace MarcenariaMorais
{
    public class Catalogo
    {
        public int Id { get; set; }
        public string Img1 { get; set; }
        public string Img2 { get; set; }
        public string Img3 { get; set; }
        public string Img4 { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Tamanho { get; set; }
        public string Descricao { get; set; }

        // FTP config
        private string ftpHost = "ftp.quaiati.com.br";
        private string ftpUser = "marcenaria_morais@quaiati.com.br";
        private string ftpPass = "21314151**mm";
        private string ftpFolder = "/View/Catalog/img/catalog/";

        public static DataTable ListarTamanhoParaCombo()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "SELECT DISTINCT Cat_tamanho FROM Catalogo";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR PARA COMBO ***
                foreach (DataRow row in dt.Rows)
                {
                    //row["Cat_id"] = row["Cat_id"].ToString();
                    row["Cat_tamanho"] = row["Cat_tamanho"].ToString();
                    //row["Cat_cate"] = row["Cat_cate"].ToString();
                }

                conn.Close();
                return dt;
            }
            return null;
        }

        public static DataTable ListarCategoriaParaCombo()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = "SELECT DISTINCT Cat_cate FROM Catalogo";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // *** DESCRIPTOGRAFAR PARA COMBO ***
                foreach (DataRow row in dt.Rows)
                {
                    //row["Cat_id"] = row["Cat_id"].ToString();
                    row["Cat_cate"] = row["Cat_cate"].ToString();
                    //row["Cat_cate"] = row["Cat_cate"].ToString();
                }

                conn.Close();
                return dt;
            }
            return null;
        }

        public static DataTable ListarTodos()
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"SELECT Cat_id, Cat_img1, Cat_img2, Cat_img3, Cat_img4, 
                               Cat_nome, Cat_cate, Cat_tamanho, Cat_desc
                               FROM Catalogo";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                conn.Close();
                return dt;
            }
            return null;
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

        private bool UploadImagemFTP(string localPath, string nomeArquivo)
        {
            string uri = $"ftp://{ftpHost}{ftpFolder}{nomeArquivo}";
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(ftpUser, ftpPass);
                req.UsePassive = true;
                req.UseBinary = true;
                req.KeepAlive = false;

                byte[] fileBytes = File.ReadAllBytes(localPath);
                req.ContentLength = fileBytes.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(fileBytes, 0, fileBytes.Length);
                }
                using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
                {
                    // Upload OK
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao subir imagem: {nomeArquivo}\n{ex.Message}");
                return false;
            }
        }

        private bool DeletarImagemFTP(string nomeArquivo)
        {
            if (string.IsNullOrEmpty(nomeArquivo)) return true;

            string uri = $"ftp://{ftpHost}{ftpFolder}{nomeArquivo}";
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
                req.Method = WebRequestMethods.Ftp.DeleteFile;
                req.Credentials = new NetworkCredential(ftpUser, ftpPass);
                req.UsePassive = true;
                req.KeepAlive = false;

                using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
                {
                    // Arquivo deletado
                }
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public int? Cadastrar(string imgLocal1, string imgLocal2 = null, string imgLocal3 = null, string imgLocal4 = null)
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sql = @"INSERT INTO Catalogo 
                    (Cat_img1, Cat_img2, Cat_img3, Cat_img4, Cat_nome, Cat_cate, Cat_tamanho, Cat_desc)
                    VALUES (@img1, @img2, @img3, @img4, @nome, @cate, @tamanho, @desc)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@img1", "");
                cmd.Parameters.AddWithValue("@img2", DBNull.Value);
                cmd.Parameters.AddWithValue("@img3", DBNull.Value);
                cmd.Parameters.AddWithValue("@img4", DBNull.Value);
                cmd.Parameters.AddWithValue("@nome", Nome);
                cmd.Parameters.AddWithValue("@cate", Categoria);
                cmd.Parameters.AddWithValue("@tamanho", Tamanho);
                cmd.Parameters.AddWithValue("@desc", Descricao);

                try
                {
                    cmd.ExecuteNonQuery();
                    int idGerado = (int)cmd.LastInsertedId;

                    // Tenta limpar o nome - se der erro, exibe mensagem
                    string nomeBase;
                    try
                    {
                        nomeBase = LimparNomeArquivo(Nome);
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        MessageBox.Show(ex.Message, "Erro no Nome do Produto", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Deleta o registro que foi criado
                        var connTemp = banco.Conectar();
                        if (connTemp != null)
                        {
                            MySqlCommand cmdDel = new MySqlCommand("DELETE FROM Catalogo WHERE Cat_id = @id", connTemp);
                            cmdDel.Parameters.AddWithValue("@id", idGerado);
                            cmdDel.ExecuteNonQuery();
                            connTemp.Close();
                        }
                        return null;
                    }

                    // Todos os arquivos serão .png
                    string nomeImg1 = $"{nomeBase}_{idGerado}_1.png";
                    string nomeImg2 = string.IsNullOrEmpty(imgLocal2) ? null : $"{nomeBase}_{idGerado}_2.png";
                    string nomeImg3 = string.IsNullOrEmpty(imgLocal3) ? null : $"{nomeBase}_{idGerado}_3.png";
                    string nomeImg4 = string.IsNullOrEmpty(imgLocal4) ? null : $"{nomeBase}_{idGerado}_4.png";

                    // Upload para FTP
                    if (!UploadImagemFTP(imgLocal1, nomeImg1))
                    {
                        conn.Close();
                        return null;
                    }
                    if (!string.IsNullOrEmpty(imgLocal2)) UploadImagemFTP(imgLocal2, nomeImg2);
                    if (!string.IsNullOrEmpty(imgLocal3)) UploadImagemFTP(imgLocal3, nomeImg3);
                    if (!string.IsNullOrEmpty(imgLocal4)) UploadImagemFTP(imgLocal4, nomeImg4);

                    // Copia local
                    string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");
                    if (!Directory.Exists(pastaLocal))
                        Directory.CreateDirectory(pastaLocal);

                    try
                    {
                        File.Copy(imgLocal1, Path.Combine(pastaLocal, nomeImg1), true);
                        if (!string.IsNullOrEmpty(imgLocal2))
                            File.Copy(imgLocal2, Path.Combine(pastaLocal, nomeImg2), true);
                        if (!string.IsNullOrEmpty(imgLocal3))
                            File.Copy(imgLocal3, Path.Combine(pastaLocal, nomeImg3), true);
                        if (!string.IsNullOrEmpty(imgLocal4))
                            File.Copy(imgLocal4, Path.Combine(pastaLocal, nomeImg4), true);
                    }
                    catch { /* Ignora erro de cópia local */ }

                    // Atualiza o banco com os nomes das imagens
                    string sqlUpdate = @"UPDATE Catalogo 
                        SET Cat_img1 = @img1, Cat_img2 = @img2, Cat_img3 = @img3, Cat_img4 = @img4
                        WHERE Cat_id = @id";

                    MySqlCommand cmdUpdate = new MySqlCommand(sqlUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@img1", nomeImg1);
                    cmdUpdate.Parameters.AddWithValue("@img2", nomeImg2 ?? (object)DBNull.Value);
                    cmdUpdate.Parameters.AddWithValue("@img3", nomeImg3 ?? (object)DBNull.Value);
                    cmdUpdate.Parameters.AddWithValue("@img4", nomeImg4 ?? (object)DBNull.Value);
                    cmdUpdate.Parameters.AddWithValue("@id", idGerado);
                    cmdUpdate.ExecuteNonQuery();

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

        public bool? Alterar(string imgLocal1 = null, string imgLocal2 = null, string imgLocal3 = null, string imgLocal4 = null)
        {
            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn == null) return false;

            // Busca o nome antigo e imagens antigas do banco
            string sqlSelect = "SELECT Cat_nome, Cat_img1, Cat_img2, Cat_img3, Cat_img4 FROM Catalogo WHERE Cat_id = @id";
            MySqlCommand cmdSelect = new MySqlCommand(sqlSelect, conn);
            cmdSelect.Parameters.AddWithValue("@id", Id);

            string nomeAntigoLimpo = null;
            string img1Antiga = null;
            string img2Antiga = null;
            string img3Antiga = null;
            string img4Antiga = null;

            using (var reader = cmdSelect.ExecuteReader())
            {
                if (reader.Read())
                {
                    string nomeAntigo = reader["Cat_nome"].ToString();
                    try { nomeAntigoLimpo = LimparNomeArquivo(nomeAntigo); } catch { }

                    img1Antiga = reader["Cat_img1"] != DBNull.Value ? reader["Cat_img1"].ToString() : null;
                    img2Antiga = reader["Cat_img2"] != DBNull.Value ? reader["Cat_img2"].ToString() : null;
                    img3Antiga = reader["Cat_img3"] != DBNull.Value ? reader["Cat_img3"].ToString() : null;
                    img4Antiga = reader["Cat_img4"] != DBNull.Value ? reader["Cat_img4"].ToString() : null;
                }
            }

            // Tenta limpar o nome - se der erro, exibe mensagem
            string nomeBase;
            try
            {
                nomeBase = LimparNomeArquivo(Nome);
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
            string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");
            if (!Directory.Exists(pastaLocal))
                Directory.CreateDirectory(pastaLocal);

            // ========== PRIMEIRO: Processa NOVAS imagens ==========

            // Processa imagem 1
            if (!string.IsNullOrEmpty(imgLocal1))
            {
                string nomeImg1 = $"{nomeBase}_{Id}_1.png";
                string caminhoLocalCompleto = Path.Combine(pastaLocal, nomeImg1);

                try
                {
                    // Se o nome mudou E existe imagem antiga, deleta a antiga ANTES
                    if (nomeMudou && !string.IsNullOrEmpty(img1Antiga))
                    {
                        string caminhoAntigo = Path.Combine(pastaLocal, img1Antiga);
                        if (File.Exists(caminhoAntigo))
                            File.Delete(caminhoAntigo);
                        DeletarImagemFTP(img1Antiga);
                    }

                    // Deleta a imagem atual (mesmo nome) se existir
                    if (File.Exists(caminhoLocalCompleto))
                    {
                        File.Delete(caminhoLocalCompleto);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    // Converte para PNG localmente
                    ConverterParaPNG(imgLocal1, caminhoLocalCompleto);

                    // Faz upload do arquivo PNG
                    if (UploadImagemFTP(caminhoLocalCompleto, nomeImg1))
                    {
                        Img1 = nomeImg1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar imagem 1: {ex.Message}");
                }
            }

            // Processa imagem 2
            if (!string.IsNullOrEmpty(imgLocal2))
            {
                string nomeImg2 = $"{nomeBase}_{Id}_2.png";
                string caminhoLocalCompleto = Path.Combine(pastaLocal, nomeImg2);

                try
                {
                    // Se o nome mudou E existe imagem antiga, deleta a antiga ANTES
                    if (nomeMudou && !string.IsNullOrEmpty(img2Antiga))
                    {
                        string caminhoAntigo = Path.Combine(pastaLocal, img2Antiga);
                        if (File.Exists(caminhoAntigo))
                            File.Delete(caminhoAntigo);
                        DeletarImagemFTP(img2Antiga);
                    }

                    if (File.Exists(caminhoLocalCompleto))
                    {
                        File.Delete(caminhoLocalCompleto);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    ConverterParaPNG(imgLocal2, caminhoLocalCompleto);

                    if (UploadImagemFTP(caminhoLocalCompleto, nomeImg2))
                    {
                        Img2 = nomeImg2;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar imagem 2: {ex.Message}");
                }
            }

            // Processa imagem 3
            if (!string.IsNullOrEmpty(imgLocal3))
            {
                string nomeImg3 = $"{nomeBase}_{Id}_3.png";
                string caminhoLocalCompleto = Path.Combine(pastaLocal, nomeImg3);

                try
                {
                    // Se o nome mudou E existe imagem antiga, deleta a antiga ANTES
                    if (nomeMudou && !string.IsNullOrEmpty(img3Antiga))
                    {
                        string caminhoAntigo = Path.Combine(pastaLocal, img3Antiga);
                        if (File.Exists(caminhoAntigo))
                            File.Delete(caminhoAntigo);
                        DeletarImagemFTP(img3Antiga);
                    }

                    if (File.Exists(caminhoLocalCompleto))
                    {
                        File.Delete(caminhoLocalCompleto);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    ConverterParaPNG(imgLocal3, caminhoLocalCompleto);

                    if (UploadImagemFTP(caminhoLocalCompleto, nomeImg3))
                    {
                        Img3 = nomeImg3;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar imagem 3: {ex.Message}");
                }
            }

            // Processa imagem 4
            if (!string.IsNullOrEmpty(imgLocal4))
            {
                string nomeImg4 = $"{nomeBase}_{Id}_4.png";
                string caminhoLocalCompleto = Path.Combine(pastaLocal, nomeImg4);

                try
                {
                    // Se o nome mudou E existe imagem antiga, deleta a antiga ANTES
                    if (nomeMudou && !string.IsNullOrEmpty(img4Antiga))
                    {
                        string caminhoAntigo = Path.Combine(pastaLocal, img4Antiga);
                        if (File.Exists(caminhoAntigo))
                            File.Delete(caminhoAntigo);
                        DeletarImagemFTP(img4Antiga);
                    }

                    if (File.Exists(caminhoLocalCompleto))
                    {
                        File.Delete(caminhoLocalCompleto);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    ConverterParaPNG(imgLocal4, caminhoLocalCompleto);

                    if (UploadImagemFTP(caminhoLocalCompleto, nomeImg4))
                    {
                        Img4 = nomeImg4;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar imagem 4: {ex.Message}");
                }
            }

            // ========== DEPOIS: Se nome mudou, renomeia as imagens que NÃO foram substituídas ==========

            if (nomeMudou)
            {
                // Renomeia img1 SE não forneceu nova
                if (string.IsNullOrEmpty(imgLocal1) && !string.IsNullOrEmpty(img1Antiga))
                {
                    string caminhoAntigo = Path.Combine(pastaLocal, img1Antiga);
                    string nomeNovo = $"{nomeBase}_{Id}_1.png";
                    string caminhoNovo = Path.Combine(pastaLocal, nomeNovo);

                    if (File.Exists(caminhoAntigo))
                    {
                        try
                        {
                            File.Copy(caminhoAntigo, caminhoNovo, true);
                            UploadImagemFTP(caminhoNovo, nomeNovo);
                            File.Delete(caminhoAntigo);
                            DeletarImagemFTP(img1Antiga);
                            Img1 = nomeNovo;
                        }
                        catch { }
                    }
                }

                // Renomeia img2 SE não forneceu nova
                if (string.IsNullOrEmpty(imgLocal2) && !string.IsNullOrEmpty(img2Antiga))
                {
                    string caminhoAntigo = Path.Combine(pastaLocal, img2Antiga);
                    string nomeNovo = $"{nomeBase}_{Id}_2.png";
                    string caminhoNovo = Path.Combine(pastaLocal, nomeNovo);

                    if (File.Exists(caminhoAntigo))
                    {
                        try
                        {
                            File.Copy(caminhoAntigo, caminhoNovo, true);
                            UploadImagemFTP(caminhoNovo, nomeNovo);
                            File.Delete(caminhoAntigo);
                            DeletarImagemFTP(img2Antiga);
                            Img2 = nomeNovo;
                        }
                        catch { }
                    }
                }

                // Renomeia img3 SE não forneceu nova
                if (string.IsNullOrEmpty(imgLocal3) && !string.IsNullOrEmpty(img3Antiga))
                {
                    string caminhoAntigo = Path.Combine(pastaLocal, img3Antiga);
                    string nomeNovo = $"{nomeBase}_{Id}_3.png";
                    string caminhoNovo = Path.Combine(pastaLocal, nomeNovo);

                    if (File.Exists(caminhoAntigo))
                    {
                        try
                        {
                            File.Copy(caminhoAntigo, caminhoNovo, true);
                            UploadImagemFTP(caminhoNovo, nomeNovo);
                            File.Delete(caminhoAntigo);
                            DeletarImagemFTP(img3Antiga);
                            Img3 = nomeNovo;
                        }
                        catch { }
                    }
                }

                // Renomeia img4 SE não forneceu nova
                if (string.IsNullOrEmpty(imgLocal4) && !string.IsNullOrEmpty(img4Antiga))
                {
                    string caminhoAntigo = Path.Combine(pastaLocal, img4Antiga);
                    string nomeNovo = $"{nomeBase}_{Id}_4.png";
                    string caminhoNovo = Path.Combine(pastaLocal, nomeNovo);

                    if (File.Exists(caminhoAntigo))
                    {
                        try
                        {
                            File.Copy(caminhoAntigo, caminhoNovo, true);
                            UploadImagemFTP(caminhoNovo, nomeNovo);
                            File.Delete(caminhoAntigo);
                            DeletarImagemFTP(img4Antiga);
                            Img4 = nomeNovo;
                        }
                        catch { }
                    }
                }
            }

            // Define os nomes finais para o banco
            if (string.IsNullOrEmpty(Img1)) Img1 = img1Antiga;
            if (string.IsNullOrEmpty(Img2)) Img2 = img2Antiga;
            if (string.IsNullOrEmpty(Img3)) Img3 = img3Antiga;
            if (string.IsNullOrEmpty(Img4)) Img4 = img4Antiga;

            string sql = @"UPDATE Catalogo 
        SET Cat_nome = @nome, Cat_cate = @cate, Cat_tamanho = @tamanho, Cat_desc = @desc,
            Cat_img1 = @img1, Cat_img2 = @img2, Cat_img3 = @img3, Cat_img4 = @img4
        WHERE Cat_id = @id";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nome", Nome);
            cmd.Parameters.AddWithValue("@cate", Categoria);
            cmd.Parameters.AddWithValue("@tamanho", Tamanho);
            cmd.Parameters.AddWithValue("@desc", Descricao);
            cmd.Parameters.AddWithValue("@img1", Img1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@img2", Img2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@img3", Img3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@img4", Img4 ?? (object)DBNull.Value);
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

        // Método auxiliar para converter imagem para PNG
        private void ConverterParaPNG(string origemPath, string destinoPath)
        {
            try
            {
                System.Windows.Media.Imaging.BitmapImage bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(origemPath);
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));

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
        public bool? Excluir()
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                $"Tem certeza que deseja excluir '{Nome}'?\nAs imagens também serão removidas do servidor.",
                "Confirmar Exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmacao != MessageBoxResult.Yes) return null;

            Banco banco = new Banco();
            var conn = banco.Conectar();

            if (conn != null)
            {
                string sqlSelect = "SELECT Cat_img1, Cat_img2, Cat_img3, Cat_img4 FROM Catalogo WHERE Cat_id = @id";
                MySqlCommand cmdSelect = new MySqlCommand(sqlSelect, conn);
                cmdSelect.Parameters.AddWithValue("@id", Id);

                try
                {
                    using (var reader = cmdSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Img1 = reader["Cat_img1"] != DBNull.Value ? reader["Cat_img1"].ToString() : null;
                            Img2 = reader["Cat_img2"] != DBNull.Value ? reader["Cat_img2"].ToString() : null;
                            Img3 = reader["Cat_img3"] != DBNull.Value ? reader["Cat_img3"].ToString() : null;
                            Img4 = reader["Cat_img4"] != DBNull.Value ? reader["Cat_img4"].ToString() : null;
                        }
                    }

                    string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");

                    // Deleta do FTP e LOCAL na hora
                    if (!string.IsNullOrEmpty(Img1))
                    {
                        DeletarImagemFTP(Img1);
                        try
                        {
                            string caminhoLocal = Path.Combine(pastaLocal, Img1);
                            if (File.Exists(caminhoLocal))
                                File.Delete(caminhoLocal);
                        }
                        catch { }
                    }

                    if (!string.IsNullOrEmpty(Img2))
                    {
                        DeletarImagemFTP(Img2);
                        try
                        {
                            string caminhoLocal = Path.Combine(pastaLocal, Img2);
                            if (File.Exists(caminhoLocal))
                                File.Delete(caminhoLocal);
                        }
                        catch { }
                    }

                    if (!string.IsNullOrEmpty(Img3))
                    {
                        DeletarImagemFTP(Img3);
                        try
                        {
                            string caminhoLocal = Path.Combine(pastaLocal, Img3);
                            if (File.Exists(caminhoLocal))
                                File.Delete(caminhoLocal);
                        }
                        catch { }
                    }

                    if (!string.IsNullOrEmpty(Img4))
                    {
                        DeletarImagemFTP(Img4);
                        try
                        {
                            string caminhoLocal = Path.Combine(pastaLocal, Img4);
                            if (File.Exists(caminhoLocal))
                                File.Delete(caminhoLocal);
                        }
                        catch { }
                    }

                    // Deleta do banco
                    string sql = "DELETE FROM Catalogo WHERE Cat_id = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", Id);

                    int linhas = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (linhas > 0)
                    {
                        MessageBox.Show("Produto e imagens excluídos com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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