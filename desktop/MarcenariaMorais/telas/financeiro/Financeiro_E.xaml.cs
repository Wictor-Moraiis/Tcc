using Mysqlx.Crud;
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

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Financeiro_E.xam
    /// </summary>
    public partial class Financeiro_E : Page
    {
        private Financeiro fin;
        private int id;

        public Financeiro_E()
        {
            InitializeComponent();
        }

        public void CarregarCampos(Financeiro fin)
        {
            List<string> tipos = new List<string> { "Entrada", "Saída" };

            id = fin.Id;

            inp_tipo.AddItemsByText(tipos);

            if (fin.Valor.ToString().Contains("-"))
                inp_tipo.SelectIndex(1);
            else 
                inp_tipo.SelectIndex(0);

            inp_desc .SetText(fin.Desc);
            inp_valor.SetText(DoubleToString(fin.Valor));

            inp_data.SelectedDate = fin.Data;
        }

        private string DoubleToString(double val)
        {
            return val.ToString("C2").Replace("R$ ", "").Replace(".", "").Replace("-", "");
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação de alteração de dados", MessageBoxButton.YesNo, MessageBoxImage.Question);
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

            fin = new Financeiro()
            {
                Id = id,
                Desc = desc,
                Valor = valor,
                Data = data
            };

            bool? ok = fin.Alterar();
            if (ok == true)
            {
                MessageBox.Show("Lançamento alterado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationHandler.SetAndRefresh("FinanceiroL");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("FinanceiroL");
        }
    }
}
