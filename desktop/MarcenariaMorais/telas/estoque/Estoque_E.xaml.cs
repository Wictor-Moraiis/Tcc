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
    /// Interação lógica para Estoque_E.xam
    /// </summary>
    public partial class Estoque_E : Page
    {
        private Estoque est;
        private int id;
        private string imagemPreSelecionada = null;
        private string imagemSelecionada = null;

        public Estoque_E()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            inp_img.Destino = IO.Path.Combine(IO.Path.GetTempPath(), "MarcenariaMoraisDados/Img_Estoque");
        }

        public void CarregarCampos(Estoque est)
        {
            id = est.Id;

            inp_produto.SetText(est.Produto);

            inp_quant.SetNumber(est.Quantidade);

            inp_tel.FillText     = est.Telefone;
            imagemPreSelecionada = est.Imagem;
            if (!string.IsNullOrEmpty(imagemPreSelecionada))
                inp_img.fileToLoad   = imagemPreSelecionada;
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação de alteração de dados", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (!int.TryParse(inp_quant.Text, out int quant))
            {
                MessageBox.Show("Quantidade inválida!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            imagemSelecionada = inp_img.GetFile();

            // Verifica se foi REALMENTE selecionada uma nova imagem
            // Se o caminho for igual ao pré-selecionado, significa que NÃO mudou
            string novaImagem = null;
            if (!string.IsNullOrEmpty(imagemSelecionada) && imagemSelecionada != imagemPreSelecionada)
            {
                // Verifica se é um caminho de arquivo VÁLIDO (não é caminho do banco)
                if (IO.File.Exists(imagemSelecionada))
                {
                    novaImagem = imagemSelecionada;
                }
            }

            est = new Estoque
            {
                Id = id,
                Produto = inp_produto.Text.Trim(),
                Quantidade = quant,
                Telefone = inp_tel.getUnmaskedText().Trim()
            };

            // Passa NULL se não selecionou nova imagem, ou o caminho da nova se selecionou
            bool? resultado = est.Alterar(novaImagem);

            if (resultado == true)
            {
                
                NavigationHandler.SetAndRefresh("EstoqueL");
            }
            else if (resultado == false) 
            {
                MessageBox.Show("Erro ao alterar os dados do produto.", "Alteração Não Realizado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("EstoqueL");
        }
    }
}
