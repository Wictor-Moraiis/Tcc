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
    /// Interação lógica para Cat_Funcionario_E.xam
    /// </summary>
    public partial class Cat_Funcionario_E : Page
    {
        private int id;

        public Cat_Funcionario_E()
        {
            InitializeComponent();
        }

        public void CarregarCampos(Cat_Funcionario catf)
        {
            id = catf.Catg_id;

            inp_nome   .SetText(catf.Catg_nome);
            inp_salario.SetText(DoubleToString(catf.Catg_sal));
        }

        private string DoubleToString(double val)
        {
            return val.ToString("C2").Replace("R$ ", "").Replace(".", "").Replace("-", "");
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (string.IsNullOrWhiteSpace(inp_nome.Text) || string.IsNullOrWhiteSpace(inp_salario.Text))
            {
                MessageBox.Show("Preencha todos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(inp_salario.Text.Replace(".", ","), out double salario))
            {
                MessageBox.Show("Salário inválido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nome = inp_nome.Text.Trim();

            Cat_Funcionario cat = new Cat_Funcionario
            {
                Catg_id   = id,
                Catg_nome = nome,
                Catg_sal  = salario
            };

            bool? ok = cat.Alterar();
            if (ok == true)
            {
                MessageBox.Show("Categoria alterada com sucesso", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationHandler.SetAndRefresh("CatFuncionarioL");
            }
            else if (ok == false) 
            {
                MessageBox.Show("Erro ao alterar os dados da categoria.", "Alteração Não Realizado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("CatFuncionarioL");
        }
    }
}
