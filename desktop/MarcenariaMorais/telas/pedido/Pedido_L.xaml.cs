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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Pedido_L.xam
    /// </summary>
    public partial class Pedido_L : Page
    {
        private DataTable daO; // Dados Originais

        public Pedido_L()
        {
            InitializeComponent();
            CarregarTabela();
        }

        public void CarregarTabela()
        {
            daO = Pedido.ListarTodos();
            dt_tabela.ItemsSource = daO?.DefaultView;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tb_pesquisa.TextChanged += AplicarFiltro;
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("PedidoA");
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um pedido para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;

            int    id   = Convert.ToInt32(row["Ped_id"]);
            int    cliId   = Convert.ToInt32(row["Cli_id"]);
            string cpf  = row["Cli_id"]  .ToString();
            string desc = row["Ped_desc"].ToString().Trim();

            Debug.WriteLine($"FOR SOME GOD DAMN REASON THIS IS THE FUCKING ID THAT IS BEING SELECTED: {cliId}");

            Debug.WriteLine($"\nTry parse id as int: {Convert.ToBoolean(int.Parse(row["Ped_id"].ToString()))}");

            double valor = Convert.ToDouble(row["Ped_valor"]);

            DateTime realizado = Convert.ToDateTime(row["Ped_realizado"].ToString());
            DateTime entrega   = Convert.ToDateTime(row["Ped_entrega"].ToString());

            Debug.WriteLine($"WTF: {string.IsNullOrEmpty(row["Estq_id2"].ToString())}");

            int? estq1 = !string.IsNullOrEmpty(row["Estq_id1"].ToString()) ? (int?)Convert.ToInt32(row["Estq_id1"].ToString()) : null;
            int? estq2 = !string.IsNullOrEmpty(row["Estq_id2"].ToString()) ? (int?)Convert.ToInt32(row["Estq_id2"].ToString()) : null;
            int? estq3 = !string.IsNullOrEmpty(row["Estq_id3"].ToString()) ? (int?)Convert.ToInt32(row["Estq_id3"].ToString()) : null;
            int? estq4 = !string.IsNullOrEmpty(row["Estq_id4"].ToString()) ? (int?)Convert.ToInt32(row["Estq_id4"].ToString()) : null;
            int? estq5 = !string.IsNullOrEmpty(row["Estq_id5"].ToString()) ? (int?)Convert.ToInt32(row["Estq_id5"].ToString()) : null;

            bool executado = Convert.ToBoolean(row["Ped_executado"]);

            Pedido ped = new Pedido
            {
                Id            = id,
                Cli_id        = cliId,
                Descricao     = desc,
                Valor         = valor,
                DataRealizado = realizado,
                DataEntrega   = entrega,
                Executado     = executado,
                Estq_id1      = estq1,
                Estq_id2      = estq2,
                Estq_id3      = estq3,
                Estq_id4      = estq4,
                Estq_id5      = estq5
            };

            NavigationHandler.SetAndRefresh("PedidoE", new object[] { ped });
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um pedido para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;
            int id  = Convert.ToInt32(row["Ped_id"]);

            Pedido ped = new Pedido { Id = id };

            bool? ok = ped.Excluir();
            if (ok == true)
            {
                CarregarTabela();
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
                string filtro = $@"CONVERT(Ped_id, 'System.String') LIKE '%{textoPesquisa}%' OR Cli_nome LIKE '%{textoPesquisa}%'";

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
