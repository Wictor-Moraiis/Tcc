using MySqlX.XDevAPI;
using Org.BouncyCastle.Cms;
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
    /// Interação lógica para Pedidos_A.xam
    /// </summary>
    public partial class Pedido_A : Page
    {
        public Pedido_A()
        {
            InitializeComponent();
        }

        public void LimparCampos()
        {
            var clientes = Cliente.ListarParaCombo();
            var estoque  = Estoque.ListarParaCombo();

            inp_cli.SetPaths("Cli_nome", "Cli_id");

            inp_cli.LoadItems(clientes?.DefaultView);

            inp_desc .Reset();
            inp_valor.Reset();

            inp_data_realizado.SelectedDate = null;
            inp_data_entrega  .SelectedDate = null;

            inp_estq1.ClearItems();
            inp_estq2.ClearItems();
            inp_estq3.ClearItems();
            inp_estq4.ClearItems();
            inp_estq5.ClearItems();

            inp_estq1.SetPaths("Estq_produto", "Estq_id");
            inp_estq2.SetPaths("Estq_produto", "Estq_id");
            inp_estq3.SetPaths("Estq_produto", "Estq_id");
            inp_estq4.SetPaths("Estq_produto", "Estq_id");
            inp_estq5.SetPaths("Estq_produto", "Estq_id");

            inp_estq1.LoadItems(estoque?.DefaultView);
            inp_estq2.LoadItems(estoque?.DefaultView);
            inp_estq3.LoadItems(estoque?.DefaultView);
            inp_estq4.LoadItems(estoque?.DefaultView);
            inp_estq5.LoadItems(estoque?.DefaultView);

            //DataTable dt = new DataTable();
            //dt.Columns.Add("Estq_produto", typeof(string));
            //dt.Columns.Add("Estq_id", typeof(int));

            //DataRow itemVazio = dt.NewRow();
            //itemVazio["Estq_produto"] = "-";
            //itemVazio["Estq_id"] = -1;

            //inp_estq1.lb.Items.Add(null);

            List<string> ops = new List<string> { "Sim", "Não" };
            inp_executado.AddItemsByText(ops);
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja SALVAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            if (inp_cli.GetSelectedItem() == null || string.IsNullOrWhiteSpace(inp_desc.Text) ||
                string.IsNullOrWhiteSpace(inp_valor.Text) || inp_data_realizado.SelectedDate == null ||
                inp_data_entrega.SelectedDate == null)
            {
                MessageBox.Show("Preencha todos os campos obrigatórios.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!double.TryParse(inp_valor.Text.Replace(".", ","), out double valor))
            {
                MessageBox.Show("Valor inválido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int         id        = Convert.ToInt32(inp_cli.GetSelectedValue().ToString());
            string      desc      = inp_desc.Text.Trim();
            DateTime    realizado = inp_data_realizado.SelectedDate.Value;
            DateTime    entrega   = inp_data_entrega  .SelectedDate.Value;
            int?        estq1     = inp_estq1.GetSelectedValue() != null ? (int?)inp_estq1.GetSelectedValue() : null;
            int?        estq2     = inp_estq2.GetSelectedValue() != null ? (int?)inp_estq2.GetSelectedValue() : null;
            int?        estq3     = inp_estq3.GetSelectedValue() != null ? (int?)inp_estq3.GetSelectedValue() : null;
            int?        estq4     = inp_estq4.GetSelectedValue() != null ? (int?)inp_estq4.GetSelectedValue() : null;
            int?        estq5     = inp_estq5.GetSelectedValue() != null ? (int?)inp_estq5.GetSelectedValue() : null;
            ListBoxItem exv       = (ListBoxItem)inp_executado.GetSelectedValue();
            bool        executado = (string)exv.Content == "Sim";

            Pedido ped = new Pedido
            {
                Cli_id        = id,
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

            int? idp = ped.Cadastrar();
            if (idp != null)
            {
                NavigationHandler.SetAndRefresh("PedidoL");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("PedidoL");
        }
    }
}
