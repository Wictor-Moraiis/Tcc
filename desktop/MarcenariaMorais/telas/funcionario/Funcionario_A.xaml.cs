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
    /// Interação lógica para Funcionario_A.xam
    /// </summary>
    public partial class Funcionario_A : Page
    {
        public Funcionario_A()
        {
            InitializeComponent();
        }

        public void LimparCampos()
        {
            inp_cat.SetPaths("Catg_nome", "Catg_id");

            inp_cpf .Reset();
            inp_nome.Reset();
            inp_tel1.Reset();
            inp_tel2.Reset();
            inp_desc.Reset();
            inp_cat .Reset();

            var categorias = Cat_Funcionario.ListarParaCombo();
            inp_cat.LoadItems(categorias?.DefaultView);
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja SALVAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (inp_cat.GetSelectedItem() == null)
            {
                MessageBox.Show("Escolha uma categoria.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inp_nome.Text))
            {
                MessageBox.Show("Preencha o campo do nome.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nome  = inp_nome.Text;
            string desc  = inp_desc.Text;

            string cpf   = inp_cpf .getUnmaskedText();
            string tel1  = inp_tel1.getUnmaskedText();
            string t2    = inp_tel2.getUnmaskedText();

            string tel2  = string.IsNullOrWhiteSpace(t2) ? null : t2;

            int    catId = (int)inp_cat.GetSelectedValue();

            Funcionario f = new Funcionario
            {
                Cpf         = cpf,
                Nome        = nome,
                Telefone1   = tel1,
                Telefone2   = tel2,
                Descricao   = desc,
                CategoriaId = catId
            };

            bool? resultado = f.Cadastrar();

            if (resultado == true)
            {
                LimparCampos();
                NavigationHandler.SetAndRefresh("FuncionarioL");
            }
            else if (resultado == false)
                MessageBox.Show("Erro ao cadastrar funcionário.", "Cadastro Não Realizado", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("FuncionarioL");
        }
    }
}
