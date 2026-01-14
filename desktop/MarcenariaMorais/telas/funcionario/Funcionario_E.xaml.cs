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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Funcionario_E.xam
    /// </summary>
    public partial class Funcionario_E : Page
    {
        private int id;

        public Funcionario_E()
        {
            InitializeComponent();
        }

        public void CarregarCampos(Funcionario fun)
        {
            var categorias = Cat_Funcionario.ListarParaCombo();
            DataRow cat    = Cat_Funcionario.ListarParaCombo().Select($"Catg_id = '{fun.CategoriaId}'")[0];

            inp_cat.SetPaths("Catg_nome", "Catg_id");

            inp_cat.LoadItems(categorias?.DefaultView);

            id = fun.Id;

            inp_cpf .FillText = fun.Cpf;
            inp_tel1.FillText = fun.Telefone1;

            inp_nome.SetText(fun.Nome);
            inp_desc.SetText(fun.Descricao);

            inp_cat.SelectOption(cat);

            if (fun.Telefone2 != null)
                inp_tel2.FillText = fun.Telefone2;
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação de alteração de dados", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (inp_cat.GetSelectedItem() == null)
            {
                MessageBox.Show("Escolha a categoria.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(inp_nome.Text))
            {
                MessageBox.Show("Digite o nome do funcionário.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nome  = inp_nome.Text;
            string desc  = inp_desc.Text;

            string cpf   = inp_cpf .getUnmaskedText();
            string tel1  = inp_tel1.getUnmaskedText();
            string t2    = inp_tel2.getUnmaskedText();

            string tel2  = string.IsNullOrWhiteSpace(t2) ? null : t2;

            int    catId = (int)inp_cat.GetSelectedValue();

            Debug.WriteLine(inp_cat.GetSelectedValue().ToString());

            Funcionario fun = new Funcionario
            {
                Id          = id,
                Cpf         = cpf,
                Nome        = nome,
                Telefone1   = tel1,
                Telefone2   = tel2,
                Descricao   = desc,
                CategoriaId = catId
            };

            bool? resultado = fun.Alterar();

            if (resultado == true)
            {
                NavigationHandler.SetAndRefresh("FuncionarioL");
            }
            else if (resultado == false)
            {
                MessageBox.Show("Erro ao alterar os dados do funcionário.", "Alteração Não Realizado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("FuncionarioL");
        }
    }
}
