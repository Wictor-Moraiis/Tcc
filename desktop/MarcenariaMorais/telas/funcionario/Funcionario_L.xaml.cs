using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interação lógica para Funcionario_L.xam
    /// </summary>
    public partial class Funcionario_L : Page
    {
        private DataTable daO; // Dados Originais

        public Funcionario_L()
        {
            InitializeComponent();
            CarregarTabela();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tb_pesquisa.TextChanged += AplicarFiltro;
        }

        public void CarregarTabela()
        {
            dt_tabela.ItemsSource = null;
            daO = Funcionario.ListarTodos();
            dt_tabela.ItemsSource = daO?.DefaultView;
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("FuncionarioA");
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione o funcionário na lista para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;

            var id = Funcionario.BuscarIdPorCpf(row["Func_cpf"].ToString());
            if (id == null)
            {
                MessageBox.Show("Funcionário não encontrado pelo CPF.");
                return;
            }
            
            DataRow cat = Cat_Funcionario.ListarParaCombo().Select($"Catg_nome = '{row["Catg_nome"]}'")[0];

            string cpf   = row["Func_cpf"] .ToString();
            string nome  = row["Func_nome"].ToString();
            string tel1  = row["Func_tel1"].ToString();
            string t2    = row["Func_tel2"].ToString();

            string tel2 = string.IsNullOrWhiteSpace(t2) ? null : t2;

            string desc  = row["Func_desc"].ToString();
            int    catId = Convert.ToInt32(cat["Catg_id"]);

            Funcionario fun = new Funcionario
            {
                Id          = (int)id,
                Cpf         = cpf,
                Nome        = nome,
                Telefone1   = tel1,
                Telefone2   = tel2,
                Descricao   = desc,
                CategoriaId = catId
            };

            NavigationHandler.SetAndRefresh("FuncionarioE", new object[] { fun });
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione o funcionário na lista para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;

            var id = Funcionario.BuscarIdPorCpf(row["Func_cpf"].ToString());
            if (id == null)
            {
                MessageBox.Show("Funcionário não encontrado pelo CPF.");
                return;
            }

            Funcionario f = new Funcionario { Id = id.Value };

            bool? resultado = f.Excluir();

            if (resultado == true)
            {
                CarregarTabela();
            }
        }

        private void tc_tabelas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ti_categorias?.IsSelected == true)
            {
                NavigationHandler.SetAndRefresh("CatFuncionarioL");
                ti_funcionarios.IsSelected = true;
            }
        }

        private void AplicarFiltro(object sender, TextChangedEventArgs e)
        {
            if (daO == null) return;

            string textoPesquisa = tb_pesquisa.Text.Trim();

            if (string.IsNullOrWhiteSpace(textoPesquisa))
            {
                // Se não há texto, mostra tudo
                daO.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                // Escapa aspas simples para evitar erros de sintaxe
                textoPesquisa = textoPesquisa.Replace("'", "''");

                // Filtro que busca em múltiplas colunas
                // Para colunas numéricas, usa CONVERT. Para texto, usa LIKE direto
                string filtro = $@"CONVERT(Func_cpf, 'System.String') LIKE '%{textoPesquisa}%' OR Func_nome LIKE '%{textoPesquisa}%'";

                try
                {
                    daO.DefaultView.RowFilter = filtro;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao filtrar: {ex.Message}");
                    daO.DefaultView.RowFilter = string.Empty;
                }
            }
        }
    }
}
