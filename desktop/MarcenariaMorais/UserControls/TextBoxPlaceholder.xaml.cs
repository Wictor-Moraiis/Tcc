using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interação lógica para TextBoxPlaceholder.xam
    /// </summary>
    public partial class TextBoxPlaceholder : UserControl
    {
        private Boolean focus = false;
        public string Text;
        public event TextChangedEventHandler TextChanged;

        public string TextPlaceholder
        {
            get { return (string)GetValue(TextBoxPlaceholderProperty); }
            set { SetValue(TextBoxPlaceholderProperty, value); }
        }

        public static readonly DependencyProperty TextBoxPlaceholderProperty =
            DependencyProperty.Register(
                    nameof(TextPlaceholder),
                    typeof(string),
                    typeof(TextBoxPlaceholder),
                    new PropertyMetadata("Placeholder")
            );

        public TextBoxPlaceholder()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Reseta o texto inscrito
        /// </summary>
        public void Reset()
        {
            tbx_texto.Text = "";
            Text = "";
        }

        /// <summary>
        /// Define o texto
        /// </summary>
        public void SetText(string t)
        {
            Text = t;
            tbx_texto.Text = t;
        }

        /// <summary>
        /// Checa se o TextBox não está vazio
        /// </summary>
        private void TextBoxNotEmpty()
        {
            if ( string.IsNullOrEmpty(tbx_texto.Text) )
                tb_placeholder.Visibility = Visibility.Visible;
            else
                tb_placeholder.Visibility = Visibility.Hidden;
        }

        // Parte lógica do placeholder

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxNotEmpty();
            Text = tbx_texto.Text;

            TextChanged?.Invoke(sender, e);
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //tb_placeholder.Visibility = Visibility.Hidden;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxNotEmpty();
        }
    }
}
