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
    /// Interação lógica para Cat_Funcionario_A.xam
    /// </summary>
    public partial class Cat_Funcionario_A : Page
    {
        public Cat_Funcionario_A()
        {
            InitializeComponent();
        }

        public void LimparCampos()
        {
            inp_nome   .Reset();
            inp_salario.Reset();
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja SALVAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                Catg_nome = nome,
                Catg_sal  = salario
            };

            int? id = cat.Cadastrar();
            if (id != null)
            {
                MessageBox.Show("Categoria cadastrada com sucesso.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                LimparCampos();
                NavigationHandler.SetAndRefresh("CatFuncionarioL");
            }
            else
            {
                MessageBox.Show("Erro ao cadastrar categoria.", "Cadastro Não Realizado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("CatFuncionarioL");
        }
    }
}
