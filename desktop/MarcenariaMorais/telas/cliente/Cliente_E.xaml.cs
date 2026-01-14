using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Cliente_E.xam
    /// </summary>
    public partial class Cliente_E : Page
    {
        private Cliente cli;
        private int id;

        public Cliente_E()
        {
            InitializeComponent();
        }

        public void CarregarCampos(Cliente cli)
        {
            id = cli.Id;

            inp_cpf .FillText = cli.Cpf;
            inp_tel1.FillText = cli.Telefone1;
            inp_tel2.FillText = cli.Telefone2;
            inp_cep .FillText = cli.Cep;

            inp_nome  .SetText(cli.Nome);
            inp_bairro.SetText(cli.Bairro);
            inp_rua   .SetText(cli.Rua);
            inp_com   .SetText(cli.Complemento);

            inp_num   .SetNumber(cli.Num_casa);
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação de alteração de dados", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (string.IsNullOrEmpty(inp_cpf.getUnmaskedText()))
            {
                MessageBox.Show("Preencha o campo do CPF", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_nome.Text))
            {
                MessageBox.Show("Preencha o campo do nome", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_tel1.getUnmaskedText()))
            {
                MessageBox.Show("Preencha o campo do telefone 1", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_cep.getUnmaskedText()))
            {
                MessageBox.Show("Preencha o campo do CEP", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_bairro.Text))
            {
                MessageBox.Show("Preencha o campo do bairro", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_rua.Text))
            {
                MessageBox.Show("Preencha o campo da rua", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_num.Text))
            {
                MessageBox.Show("Preencha o campo do número", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string cpf    = inp_cpf.getUnmaskedText().Trim();
            string nome   = inp_nome.Text.Trim();
            string tel1   = inp_tel1.getUnmaskedText().Trim();
            string tel2   = string.IsNullOrWhiteSpace(inp_tel2.getUnmaskedText()) ? null : inp_tel2.getUnmaskedText().Trim();
            string cep    = inp_cep.getUnmaskedText().Trim();
            string bairro = string.IsNullOrWhiteSpace(inp_bairro.Text) ? "" : inp_bairro.Text;
            string rua    = string.IsNullOrWhiteSpace(inp_rua.Text) ? "" : inp_rua.Text;
            int    nCasa  = int.TryParse(inp_num.Text, out int num) ? num : 0;
            string compl  = string.IsNullOrWhiteSpace(inp_com.Text) ? "" : inp_com.Text;

            cli = new Cliente
            {
                Id          = id,
                Cpf         = cpf,
                Nome        = nome,
                Telefone1   = tel1,
                Telefone2   = tel2,
                Cep         = cep,
                Bairro      = bairro,
                Rua         = rua,
                Num_casa    = nCasa,
                Complemento = compl
            };

            bool? resultado = cli.Alterar();

            if (resultado == true)
            {
                NavigationHandler.SetAndRefresh("ClienteL");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("ClienteL");
        }
    }
}
