using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
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
    /// Interação lógica para Cliente_L.xam
    /// </summary>
    public partial class Cliente_L : Page
    {
        private DataTable daO; // Dados Originais

        public Cliente_L()
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
            daO = Cliente.ListarTodos();
            dt_tabela.ItemsSource = daO?.DefaultView;
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("ClienteA");
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione o cliente na lista para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Cli_id"]);

            Cliente cliente = new Cliente { Id = id };

            bool? resultado = cliente.Excluir();

            if (resultado == true)
            {
                CarregarTabela();
            }
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione o cliente na lista para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var row = (DataRowView)dt_tabela.SelectedItem;

            int id = Convert.ToInt32(row["Cli_id"]);

            Debug.WriteLine($"THIS IS THE FUCKING ID: {id}");

            string cpf    = Convert.ToString(row["Cli_cpf"]).Trim();
            string nome   = string.IsNullOrWhiteSpace(row["Cli_nome"].ToString()) ? null : row["Cli_nome"].ToString().Trim();
            string tel1   = Convert.ToString(row["Cli_tel1"]);
            string tel2   = Convert.ToString(row["Cli_tel2"]);
            string cep    = Convert.ToString(row["Cli_cep"]).Trim();
            string bairro = row["Cli_bairro"].ToString();
            string rua    = row["Cli_rua"].ToString();
            string com    = row["Cli_complemento"].ToString();
            int    num    = int.TryParse(row["Cli_num_casa"].ToString(), out int numc) ? numc : 0;

            Cliente cli = new Cliente
            {
                Id          = id,
                Cpf         = cpf,
                Nome        = nome,
                Telefone1   = tel1,
                Telefone2   = tel2,
                Cep         = cep,
                Bairro      = bairro,
                Rua         = rua,
                Num_casa    = num,
                Complemento = com
            };

            NavigationHandler.SetAndRefresh("ClienteE", new object[] { cli });
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
                string filtro = $@"CONVERT(Cli_cpf, 'System.String') LIKE '%{textoPesquisa}%' OR Cli_nome LIKE '%{textoPesquisa}%'";

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
