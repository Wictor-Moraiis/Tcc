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
    /// Interaction logic for Pedido_E.xaml
    /// </summary>
    public partial class Pedido_E : Page
    {
        private int id;

        public Pedido_E()
        {
            InitializeComponent();
        }

        private string DoubleToString(double val)
        {
            return val.ToString("C2").Replace("R$ ", "").Replace(".", "").Replace("-", "");
        }

        public void CarregarCampos(Pedido ped)
        {
            DataTable    clientes = Cliente.ListarParaCombo();
            DataTable    estoque  = Estoque.ListarParaCombo();
            DataRow      cliId    = clientes.Select($"Cli_id = '{ped.Cli_id}'")[0];
            List<string> ops      = new List<string> { "Sim", "Não" };

            id = ped.Id;

            inp_cli.SetPaths("Cli_nome", "Cli_id");

            inp_cli.LoadItems(clientes?.DefaultView);

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

            inp_executado.AddItemsByText(ops);

            // Carregar os dados
            inp_cli.SelectOption(cliId);

            inp_desc.SetText(ped.Descricao);

            Debug.WriteLine($"Valor do pedido: {ped.Valor}");
            Debug.WriteLine($"Valor do pedido em string: {DoubleToString(ped.Valor)}");

            inp_valor.SetText(DoubleToString(ped.Valor));

            inp_data_realizado.SelectedDate = ped.DataRealizado;
            inp_data_entrega  .SelectedDate = ped.DataEntrega;

            DataRow estq1 = ped.Estq_id1 != null ? estoque.Select($"Estq_id = '{ped.Estq_id1}'")[0] : null;
            DataRow estq2 = ped.Estq_id2 != null ? estoque.Select($"Estq_id = '{ped.Estq_id2}'")[0] : null;
            DataRow estq3 = ped.Estq_id3 != null ? estoque.Select($"Estq_id = '{ped.Estq_id3}'")[0] : null;
            DataRow estq4 = ped.Estq_id4 != null ? estoque.Select($"Estq_id = '{ped.Estq_id4}'")[0] : null;
            DataRow estq5 = ped.Estq_id5 != null ? estoque.Select($"Estq_id = '{ped.Estq_id5}'")[0] : null;

            inp_estq1.SelectOption(estq1);
            inp_estq2.SelectOption(estq2);
            inp_estq3.SelectOption(estq3);
            inp_estq4.SelectOption(estq4);
            inp_estq5.SelectOption(estq5);

            if (ped.Executado)
                inp_executado.SelectIndex(0);
            else
                inp_executado.SelectIndex(1);
        }

        private void btn_salvar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja ALTERAR os dados?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
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

            int         Cliid     = Convert.ToInt32(inp_cli.GetSelectedValue().ToString());
            string      desc      = inp_desc.Text.Trim();
            DateTime    realizado = inp_data_realizado.SelectedDate.Value;
            DateTime    entrega   = inp_data_entrega.SelectedDate.Value;
            int?        estq1     = inp_estq1.GetSelectedValue() != null ? (int?)inp_estq1.GetSelectedValue() : null;
            int?        estq2     = inp_estq2.GetSelectedValue() != null ? (int?)inp_estq2.GetSelectedValue() : null;
            int?        estq3     = inp_estq3.GetSelectedValue() != null ? (int?)inp_estq3.GetSelectedValue() : null;
            int?        estq4     = inp_estq4.GetSelectedValue() != null ? (int?)inp_estq4.GetSelectedValue() : null;
            int?        estq5     = inp_estq5.GetSelectedValue() != null ? (int?)inp_estq5.GetSelectedValue() : null;
            ListBoxItem exv       = (ListBoxItem)inp_executado.GetSelectedValue();
            bool        executado = (string)exv.Content == "Sim";

            Pedido ped = new Pedido
            {
                Id            = id,
                Cli_id        = Cliid,
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

            bool? resultado = ped.Alterar();
            if (resultado == true)
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
