using Mysqlx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Catalogo_L.xam
    /// </summary>
    public partial class Catalogo_L : Page
    {
        private string tempFolder;
        private DataTable daO; // Dados Originais

        public Catalogo_L()
        {
            InitializeComponent();
            CarregarTabela();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tb_pesquisa.TextChanged += AplicarFiltro;
        }

        public void CarregarTabela()
        {
            // Limpa o DataGrid primeiro
            dt_tabela.ItemsSource = null;

            // Força garbage collector para liberar imagens da memória
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Carrega os dados
            daO = Catalogo.ListarTodos();
            dt_tabela.ItemsSource = daO?.DefaultView;
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("CatalogoA");
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Cat_id"]);

            string nome = row["Cat_nome"].ToString().Trim();
            string categoria = row["Cat_cate"].ToString().Trim();
            string tamanho = row["Cat_tamanho"].ToString();
            string descricao = row["Cat_desc"].ToString().Trim();

            string img1 = row["Cat_img1"].ToString();
            string img2 = row["Cat_img2"].ToString();
            string img3 = row["Cat_img3"].ToString();
            string img4 = row["Cat_img4"].ToString();

            Catalogo cat = new Catalogo
            {
                Id = id,
                Img1 = img1,
                Img2 = img2,
                Img3 = img3,
                Img4 = img4,
                Nome = nome,
                Categoria = categoria,
                Tamanho = tamanho,
                Descricao = descricao
            };

            NavigationHandler.SetAndRefresh("CatalogoE", new object[] { cat });
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para excluir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Cat_id"]);

            Catalogo cat = new Catalogo
            {
                Id = id,
                Nome = row["Cat_nome"].ToString()
            };

            bool? resultado = cat.Excluir();

            if (resultado == true)
            {
                CarregarTabela();
            }
        }

        private void AplicarFiltro(object sender, TextChangedEventArgs e)
        {
            if (daO == null) return;

            string textoPesquisa = tb_pesquisa.Text.Trim();

            if (string.IsNullOrWhiteSpace(textoPesquisa))
            {
                daO.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                textoPesquisa = textoPesquisa.Replace("'", "''");
                string filtro = $@"Cat_nome LIKE '%{textoPesquisa}%'";

                try
                {
                    daO.DefaultView.RowFilter = filtro;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao filtrar: {ex.Message}");
                    daO.DefaultView.RowFilter = string.Empty;
                }
            }
        }

        private void PopupImagem(object sender, RoutedEventArgs e)
        {
            // Pega os dados da linha DIRETAMENTE do DataGrid (não das imagens do botão)
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item!");
                return;
            }

            DataRowView row = (DataRowView)dt_tabela.SelectedItem;

            string nomeImg1 = row["Cat_img1"] != DBNull.Value ? row["Cat_img1"].ToString() : null;
            string nomeImg2 = row["Cat_img2"] != DBNull.Value ? row["Cat_img2"].ToString() : null;
            string nomeImg3 = row["Cat_img3"] != DBNull.Value ? row["Cat_img3"].ToString() : null;
            string nomeImg4 = row["Cat_img4"] != DBNull.Value ? row["Cat_img4"].ToString() : null;

            Debug.WriteLine($"Img1: {nomeImg1}");
            Debug.WriteLine($"Img2: {nomeImg2}");
            Debug.WriteLine($"Img3: {nomeImg3}");
            Debug.WriteLine($"Img4: {nomeImg4}");

            // Pasta local das imagens
            string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");

            // Cria pasta temporária
            tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Substring(0, 5));
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            bool hasImages = false;

            // Copia as imagens que existem
            if (!string.IsNullOrEmpty(nomeImg1))
            {
                string caminhoOrigem = Path.Combine(pastaLocal, nomeImg1);
                Debug.WriteLine($"Verificando: {caminhoOrigem}");
                Debug.WriteLine($"Existe? {File.Exists(caminhoOrigem)}");

                if (File.Exists(caminhoOrigem))
                {
                    string caminhoDestino = Path.Combine(tempFolder, nomeImg1);
                    File.Copy(caminhoOrigem, caminhoDestino, true);
                    hasImages = true;
                }
            }

            if (!string.IsNullOrEmpty(nomeImg2))
            {
                string caminhoOrigem = Path.Combine(pastaLocal, nomeImg2);
                if (File.Exists(caminhoOrigem))
                {
                    string caminhoDestino = Path.Combine(tempFolder, nomeImg2);
                    File.Copy(caminhoOrigem, caminhoDestino, true);
                    hasImages = true;
                }
            }

            if (!string.IsNullOrEmpty(nomeImg3))
            {
                string caminhoOrigem = Path.Combine(pastaLocal, nomeImg3);
                if (File.Exists(caminhoOrigem))
                {
                    string caminhoDestino = Path.Combine(tempFolder, nomeImg3);
                    File.Copy(caminhoOrigem, caminhoDestino, true);
                    hasImages = true;
                }
            }

            if (!string.IsNullOrEmpty(nomeImg4))
            {
                string caminhoOrigem = Path.Combine(pastaLocal, nomeImg4);
                if (File.Exists(caminhoOrigem))
                {
                    string caminhoDestino = Path.Combine(tempFolder, nomeImg4);
                    File.Copy(caminhoOrigem, caminhoDestino, true);
                    hasImages = true;
                }
            }

            if (hasImages)
            {
                PopupImagem popup = new PopupImagem();
                popup.Pasta = tempFolder;
                popup.Carregamento();
                popup.Show();
            }
            else
            {
                MessageBox.Show("Esse item não possui imagens!", "Não foi possível abrir o visualizador", MessageBoxButton.OK);
            }
        }
    }
}