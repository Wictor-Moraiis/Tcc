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

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Financeiro_A.xam
    /// </summary>
    public partial class Financeiro_A : Page
    {
        public Financeiro_A()
        {
            InitializeComponent();
        }

        public void LimparCampos()
        {
            List<string> tipos = new List<string> { "Entrada", "Saída" };

            inp_tipo.AddItemsByText(tipos);

            inp_desc .Reset();
            inp_valor.Reset();

            inp_tipo.SelectIndex(0);

            inp_data.SelectedDate = null;
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja SALVAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (string.IsNullOrWhiteSpace(inp_desc.Text) || string.IsNullOrWhiteSpace(inp_valor.Text) || inp_data.SelectedDate == null)
            {
                MessageBox.Show("Preencha todos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(inp_valor.Text.Replace('.', ','), out double valor))
            {
                MessageBox.Show("Valor inválido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int lbi = inp_tipo.lb.SelectedIndex;

            if (lbi == 1)
                valor *= -1;


            string   desc = inp_desc.Text.Trim();
            DateTime data = inp_data.SelectedDate.Value;

            Financeiro fin = new Financeiro
            {
                Desc  = desc,
                Valor = valor,
                Data  = data
            };

            int? id = fin.Cadastrar();
            if (id != null)
            {
                MessageBox.Show("Lançamento cadastrado com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationHandler.SetAndRefresh("FinanceiroL");
            }
            else
            {
                MessageBox.Show("Erro ao cadastrar lançamento.");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("FinanceiroL");
        }
    }
}
