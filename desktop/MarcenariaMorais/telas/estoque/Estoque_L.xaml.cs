using MySql.Data.MySqlClient;
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
using ZstdSharp.Unsafe;
using IO = System.IO;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Estoque_L.xam
    /// </summary>
    public partial class Estoque_L : Page
    {
        private DataTable daO; // Dados Originais

        private string imagemSelecionada = null;
        public Estoque_L()
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
            // Limpa completamente
            dt_tabela.ItemsSource = null;
            dt_tabela.Items.Refresh();

            // Força garbage collector
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Pequeno delay para garantir limpeza
            System.Threading.Thread.Sleep(100);

            // Carrega os dados
            daO = Estoque.ListarTodos();
            dt_tabela.ItemsSource = daO?.DefaultView;

            // Força refresh visual
            dt_tabela.Items.Refresh();
            dt_tabela.UpdateLayout();

            imagemSelecionada = null;
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("EstoqueA");
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Estq_id"]);

            Estoque est = new Estoque { Id = id, Produto = row["Estq_produto"].ToString() };

            bool? resultado = est.Excluir();

            if (resultado == true)
            {
                CarregarTabela();
            }
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um item para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Estq_id"]);

            imagemSelecionada = Convert.ToString(row["Estq_img"]);

            // nome do novo arquivo selecionado (se houver)
            string nomeArquivo = !string.IsNullOrEmpty(imagemSelecionada)
                ? IO.Path.GetFileName(imagemSelecionada)
                : row["Estq_img"].ToString().Replace("MarcenariaMoraisDados/Img_Estoque/", "");

            string caminhoBanco = !string.IsNullOrEmpty(nomeArquivo)
                ? $"MarcenariaMoraisDados/Img_Estoque/{nomeArquivo}"
                : "";

            string prod = Convert.ToString(row["Estq_produto"]);
            int qtd = int.TryParse(Convert.ToString(row["Estq_quant"]), out int quant) ? quant : 0;
            string tel = Convert.ToString(row["Estq_tel_forne"]).Trim();

            Estoque est = new Estoque
            {
                Id = id,
                Produto = prod,
                Quantidade = qtd,
                Telefone = tel,
                Imagem = caminhoBanco
            };

            NavigationHandler.SetAndRefresh("EstoqueE", new object[] { est });
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
                string filtro = $@"CONVERT(Estq_tel_forne, 'System.String') LIKE '%{textoPesquisa}%' OR Estq_produto LIKE '%{textoPesquisa}%'";

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