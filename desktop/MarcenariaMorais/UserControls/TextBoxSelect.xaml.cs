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
    /// Interação lógica para TextBoxSelect.xam
    /// </summary>
    public partial class TextBoxSelect : UserControl
    {
        private Boolean focus = false;

        public ListBoxItem itemSelecionado;
        public string TextBoxSelectPlaceholder
        {
            get { return (string)GetValue(TextBoxSelectPlaceholderProperty); }
            set { SetValue(TextBoxSelectPlaceholderProperty, value); }
        }

        public static readonly DependencyProperty TextBoxSelectPlaceholderProperty =
            DependencyProperty.Register(
                nameof(TextBoxSelectPlaceholder),
                typeof(string),
                typeof(ListBoxSelect),
                new PropertyMetadata("Placeholder")
            );

        public TextBoxSelect()
        {
            InitializeComponent();
        }

        // Lógica da ListBox

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
                mw.SizeChanged     += (s, args) => RepositionPopup(); RepositionPopup(); // Reposicionar o popup quando o TAMANHO da tela mudar
                mw.LocationChanged += (s, args) => RepositionPopup(); RepositionPopup(); // Reposicionar o popup quando a POSIÇÃO da tela mudar
            }

            //ListBoxItem item = (ListBoxItem)lb.Items[0];
            //lb.Height = item.Height * 3;
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
            tbx_texto.Text = "";

            tbk_placeholder.Opacity = 0.5;
            po.IsOpen = false;
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
        /// Retorna o texto da TextBox
        /// </summary>
        public string GetText()
        {
            return tbx_texto.Text;
        }

        /// <summary>
        /// Define o texto da TextBox
        /// </summary>
        public void SetText(string text)
        {
            tbx_texto.Text = text;
        }        

        /// <summary>
        /// Seleciona uma de suas opções a partir de uma DataRow
        /// </summary>
        public void SelectOption(DataRow option)
        {
            string display = lb.DisplayMemberPath;
            string s       = Convert.ToString(option[display]);

            tbk_placeholder.Text    = s;
            lb.SelectedItem         = option;
            tbk_placeholder.Opacity = 1;
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

            tbx_texto.Text = s;

            tbk_placeholder.Opacity = 1;
            po.IsOpen = false;
        }

        // Lógica do Placeholder

        private void TextBoxNotEmpty(object sender)
        {
            var textBox = sender as TextBox;

            if (textBox.Text.Length <= 0 && !focus)
            {
                tbk_placeholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbk_placeholder.Visibility = Visibility.Hidden;
            }
        }

        private void tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxNotEmpty(sender);
        }
        private void tbx_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = true;
            //tbk_placeholder.Visibility = Visibility.Hidden;
        }

        private void tbx_LostFocus(object sender, RoutedEventArgs e)
        {
            focus = false;
            TextBoxNotEmpty(sender);
        }
    }
}
