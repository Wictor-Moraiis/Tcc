using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
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
    /// Interação lógica para Catalogo_A.xam
    /// </summary>
    public partial class Catalogo_A : Page
    {

        public Catalogo_A()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string destino   = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");
            inp_img1.Destino = destino;
            inp_img2.Destino = destino;
            inp_img3.Destino = destino;
            inp_img4.Destino = destino;

            inp_img1.RandomFile = false;
            inp_img2.RandomFile = false;
            inp_img3.RandomFile = false;
            inp_img4.RandomFile = false;
        }

        public void LimparCampos()
        {
            inp_tamanho  .Reset();
            inp_categoria.Reset();

            var tamanhos = Catalogo.ListarTamanhoParaCombo();
            var categorias = Catalogo.ListarCategoriaParaCombo();

            inp_tamanho.SetPaths("Cat_tamanho", "");
            inp_categoria.SetPaths("Cat_cate", "");

            inp_tamanho  .LoadItems(tamanhos?.DefaultView);
            inp_categoria.LoadItems(categorias?.DefaultView);

            inp_nome     .Reset();
            inp_categoria.Reset();
            inp_tamanho  .Reset();
            inp_descricao.Reset();
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja SALVAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (string.IsNullOrWhiteSpace(inp_nome.Text))
            {
                MessageBox.Show("Preencha o nome do produto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (inp_tamanho.GetText() == null)
            {
                MessageBox.Show("Selecione o tamanho.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_img1.GetFile()))
            {
                MessageBox.Show("Selecione pelo menos a primeira imagem.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nome               = inp_nome.Text.Trim();
            string categoria          = inp_categoria.GetText();
            string descricao          = inp_descricao.Text.Trim();
            string tamanhoSelecionado = inp_tamanho.GetText();
            string img1               = inp_img1.GetFile();
            string img2               = inp_img2.GetFile();
            string img3               = inp_img3.GetFile();
            string img4               = inp_img4.GetFile();

            Catalogo cat = new Catalogo
            {
                Nome      = nome,
                Categoria = categoria,
                Tamanho   = tamanhoSelecionado,
                Descricao = descricao
            };

            int? idGerado = cat.Cadastrar(img1, img2, img3, img4);

            if (idGerado != null)
            {
                MessageBox.Show("Produto cadastrado e imagens enviadas ao servidor com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LimparCampos();
                NavigationHandler.SetAndRefresh("CatalogoL");
            }
            else
            {
                MessageBox.Show("Erro ao cadastrar produto.");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("CatalogoL");
        }
    }
}
