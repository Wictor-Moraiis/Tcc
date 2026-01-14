using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using IO = System.IO;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Catalogo_E.xam
    /// </summary>
    public partial class Catalogo_E : Page
    {
        private int id;
        private string destino = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");

        // Armazena as imagens pré-selecionadas
        private string imagemPreSelecionada1 = null;
        private string imagemPreSelecionada2 = null;
        private string imagemPreSelecionada3 = null;
        private string imagemPreSelecionada4 = null;

        public Catalogo_E()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            inp_img1.Destino = destino;
            inp_img2.Destino = destino;
            inp_img3.Destino = destino;
            inp_img4.Destino = destino;

            inp_img1.RandomFile = false;
            inp_img2.RandomFile = false;
            inp_img3.RandomFile = false;
            inp_img4.RandomFile = false;
        }

        public void CarregarCampos(Catalogo cat)
        {
            var tamanhos = Catalogo.ListarTamanhoParaCombo();
            var categorias = Catalogo.ListarCategoriaParaCombo();

            inp_tamanho.SetPaths("Cat_tamanho", "");
            inp_categoria.SetPaths("Cat_cate", "");

            inp_tamanho.LoadItems(tamanhos?.DefaultView);
            inp_categoria.LoadItems(categorias?.DefaultView);

            inp_tamanho.SetText(cat.Tamanho);
            inp_categoria.SetText(cat.Categoria);

            inp_nome.SetText(cat.Nome);
            inp_descricao.SetText(cat.Descricao);

            inp_categoria.SetText(cat.Categoria);

            // Armazena as imagens originais (somente o nome do arquivo, não o caminho completo)
            imagemPreSelecionada1 = cat.Img1;
            imagemPreSelecionada2 = cat.Img2;
            imagemPreSelecionada3 = cat.Img3;
            imagemPreSelecionada4 = cat.Img4;

            // Carrega as imagens nos controles (caminho completo)
            inp_img1.fileToLoad = IO.Path.Combine(destino, cat.Img1);
            inp_img2.fileToLoad = string.IsNullOrEmpty(cat.Img2) ? null : IO.Path.Combine(destino, cat.Img2);
            inp_img3.fileToLoad = string.IsNullOrEmpty(cat.Img3) ? null : IO.Path.Combine(destino, cat.Img3);
            inp_img4.fileToLoad = string.IsNullOrEmpty(cat.Img4) ? null : IO.Path.Combine(destino, cat.Img4);

            id = cat.Id;
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)

                return;
            if (string.IsNullOrWhiteSpace(inp_nome.Text))
            {
                MessageBox.Show("Preencha o nome do produto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_tamanho.GetText()))
            {
                MessageBox.Show("Selecione o tamanho.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_img1.GetFile()))
            {
                MessageBox.Show("Selecione pelo menos a primeira imagem.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nome = inp_nome.Text.Trim();
            string categoria = inp_categoria.GetText();
            string descricao = inp_descricao.Text.Trim();
            string tamanho = inp_tamanho.GetText();

            string img1 = inp_img1.GetFile();
            string img2 = inp_img2.GetFile();
            string img3 = inp_img3.GetFile();
            string img4 = inp_img4.GetFile();

            // Verifica se cada imagem REALMENTE mudou
            // Compara com o caminho completo da imagem pré-selecionada
            string caminhoCompleto1 = string.IsNullOrEmpty(imagemPreSelecionada1) ? null : IO.Path.Combine(destino, imagemPreSelecionada1);
            string caminhoCompleto2 = string.IsNullOrEmpty(imagemPreSelecionada2) ? null : IO.Path.Combine(destino, imagemPreSelecionada2);
            string caminhoCompleto3 = string.IsNullOrEmpty(imagemPreSelecionada3) ? null : IO.Path.Combine(destino, imagemPreSelecionada3);
            string caminhoCompleto4 = string.IsNullOrEmpty(imagemPreSelecionada4) ? null : IO.Path.Combine(destino, imagemPreSelecionada4);

            string novaImg1 = (!string.IsNullOrEmpty(img1) && img1 != caminhoCompleto1 && IO.File.Exists(img1)) ? img1 : null;
            string novaImg2 = (!string.IsNullOrEmpty(img2) && img2 != caminhoCompleto2 && IO.File.Exists(img2)) ? img2 : null;
            string novaImg3 = (!string.IsNullOrEmpty(img3) && img3 != caminhoCompleto3 && IO.File.Exists(img3)) ? img3 : null;
            string novaImg4 = (!string.IsNullOrEmpty(img4) && img4 != caminhoCompleto4 && IO.File.Exists(img4)) ? img4 : null;

            Catalogo cat = new Catalogo
            {
                Id = id,
                Nome = nome,
                Categoria = categoria,
                Tamanho = tamanho,
                Descricao = descricao
            };

            // Passa NULL se não mudou, ou o caminho se mudou
            bool? resultado = cat.Alterar(novaImg1, novaImg2, novaImg3, novaImg4);

            if (resultado == true)
            {
                MessageBox.Show("Produto alterado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationHandler.SetAndRefresh("CatalogoL");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("CatalogoL");
        }
    }
}
