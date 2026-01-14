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
    /// Interação lógica para Estoque_A.xam
    /// </summary>
    public partial class Estoque_A : Page
    {
        private string imagemSelecionada = null;

        public Estoque_A()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            inp_img.Destino = IO.Path.Combine(IO.Path.GetTempPath(), "MarcenariaMoraisDados/Img_Estoque");
        }

        public void LimparCampos()
        {
            inp_produto.Reset();
            inp_quant  .Reset();
            inp_tel    .Reset();

            imagemSelecionada = null;
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja SALVAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            imagemSelecionada = inp_img.GetFile();

            if (!int.TryParse(inp_quant.Text, out int quant))
            {
                MessageBox.Show("Quantidade inválida.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (inp_produto.Text.Length == 0)
            {
                MessageBox.Show("Por favor preencha o nome do produto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string prod = inp_produto.Text.Trim();
            string tel = inp_tel.getUnmaskedText().Trim();

            Estoque est = new Estoque
            {
                Produto = prod,
                Quantidade = quant,
                Telefone = tel
            };

            // Passa o caminho da imagem original para o método Cadastrar

            int? idGerado = est.Cadastrar(imagemSelecionada);

            if (idGerado == null)
            {
                MessageBox.Show("Não foi possível cadastrar produto.", "Cadastro Não Realizado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            NavigationHandler.SetAndRefresh("EstoqueL");
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("EstoqueL");
        }
    }
}
