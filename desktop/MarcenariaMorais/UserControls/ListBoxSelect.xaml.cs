using Org.BouncyCastle.Crypto.Paddings;
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
    /// Interação lógica para ListBoxSelect.xam
    /// </summary>

    /*
    Funcionalidades:
    - Abrir e fechar ao clicar no botão de expandir
    - Expandir o listbox e mostrá-lo sobre os outros elementos, sem ser alinhado automáticamente
    - Desselecionar ao pressionar ESC
    */

    public partial class ListBoxSelect : UserControl
    {
        private string originalPlaceholder;
        public string ItemPlaceholder
        {
            get { return (string)GetValue(ItemPlaceholderProperty); }
            set { SetValue(ItemPlaceholderProperty, value); }
        }

        public static readonly DependencyProperty ItemPlaceholderProperty =
            DependencyProperty.Register(
                nameof(ItemPlaceholder),
                typeof(string),
                typeof(ListBoxSelect),
                new PropertyMetadata("Placeholder")
            );

        public ListBoxSelect()
        {
            InitializeComponent();

            // Permite que o UserControl receba foco e eventos de teclado
            this.Focusable = true;
        }

        /// <summary>
        /// Reposiciona a interface
        /// </summary>
        private void RepositionPopup()
        {
            if (po.IsOpen)
            {
                po.IsOpen = false;
                po.IsOpen = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var mw = Application.Current.MainWindow;
            if (mw != null)
            {
                mw.SizeChanged += (s, args) => RepositionPopup(); // Reposicionar o popup quando o TAMANHO da tela mudar
                mw.LocationChanged += (s, args) => RepositionPopup(); // Reposicionar o popup quando a POSIÇÃO da tela mudar
            }
            originalPlaceholder = ItemPlaceholder;

            // Adiciona o evento KeyDown
            this.KeyDown += ListBoxSelect_KeyDown;
            lb.KeyDown += ListBoxSelect_KeyDown;
        }

        /// <summary>
        /// Manipula o evento KeyDown para desselecionar quando ESC é pressionado
        /// </summary>
        private void ListBoxSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Desseleciona o item
                lb.SelectedItem = null;
                tb.Text = ItemPlaceholder;
                tb.Opacity = 0.5;

                // Fecha o popup se estiver aberto
                po.IsOpen = false;

                // Marca o evento como tratado
                e.Handled = true;
            }
        }

        /// <summary>
        /// Limpa todos os itens da ListBox
        /// </summary>
        public void ClearItems()
        {
            lb.Items.Clear();
        }

        /// <summary>
        /// Reinicia as suas propriedades para seu estado default
        /// </summary>
        public void Reset()
        {
            lb.SelectedItem = null;
            tb.Text = this.ItemPlaceholder;

            tb.Opacity = 0.5;
            po.IsOpen = false;
        }

        /// <summary>
        /// Muda o valor selecionado da ListBox
        /// </summary>
        public void SetSelectedValue(object selectedValue)
        {
            if (selectedValue.GetType() == typeof(DataRowView))
            {
                DataRowView d = (DataRowView)lb.SelectedItem;
            }
            else if (selectedValue.GetType() == typeof(ListBoxItem))
            {
                ListBoxItem i = (ListBoxItem)selectedValue;
                lb.SelectedItem = i;
            }
        }

        /// <summary>
        /// Pega o item selecionado da ListBox
        /// </summary>
        public object GetSelectedItem()
        {
            return lb.SelectedItem;
        }

        /// <summary>
        /// Pega o valor selecionado da ListBox
        /// </summary>
        public object GetSelectedValue()
        {
            return lb.SelectedValue;
        }

        /// <summary>
        /// Muda o index selecionado
        /// </summary>
        public void SelectIndex(int index)
        {
            lb.SelectedIndex = index;
        }

        /// <summary>
        /// Carrega um grupo de items na ListBox
        /// </summary>
        public void LoadItems(DataView dv)
        {
            lb.ItemsSource = null;
            lb.Items.Clear();
            lb.ItemsSource = dv;
        }

        /// <summary>
        /// Adiciona items à ListBox a partir de uma lista
        /// </summary>
        public void AddItemsByText(List<string> items)
        {
            lb.SelectedItem = null;
            lb.Items.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = items[i];
                lb.Items.Add(lbi);
            }
        }

        /// <summary>
        /// Define o que ficará a mostra e o que servirá de valor para todos os itens
        /// </summary>
        /// <param name="DisplayMemberPath">Nome do caminho que ficara de display</param>
        /// <param name="SelectedValuePath">Nome do valor registrado nos items</param>
        public void SetPaths(string DisplayMemberPath, string SelectedValuePath)
        {
            lb.DisplayMemberPath = DisplayMemberPath;
            lb.SelectedValuePath = SelectedValuePath;
        }

        /// <summary>
        /// Seleciona um item da ListBox a partir de uma DataRow
        /// </summary>
        public void SelectOption(DataRow option)
        {
            if (option == null)
                return;

            string display = lb.DisplayMemberPath;
            string selecValue = lb.SelectedValuePath;
            string s = Convert.ToString(option[display]);

            tb.Text = s;
            lb.SelectedItem = option;
            lb.SelectedValue = option[selecValue];
            tb.Opacity = 1;
        }

        private void btn_lista_Click(object sender, RoutedEventArgs e)
        {
            po.IsOpen = !po.IsOpen;
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb.SelectedItem == null)
                return;

            string s = "";

            if (lb.SelectedItem.GetType() == typeof(DataRowView))
            {
                DataRowView d = (DataRowView)lb.SelectedItem;
                string display = lb.DisplayMemberPath;
                s = Convert.ToString(d[display]);
            }
            else if (lb.SelectedItem.GetType() == typeof(ListBoxItem))
            {
                ListBoxItem i = (ListBoxItem)lb.SelectedItem;
                s = i.Content.ToString();
            }

            tb.Text = s;
            tb.Opacity = 1;
            po.IsOpen = false;
        }
    }
}
