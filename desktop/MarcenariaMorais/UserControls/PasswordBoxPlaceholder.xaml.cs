using System;
using System.Collections.Generic;
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
    /// Interação lógica para PasswordBoxPlaceholder.xam
    /// </summary>
    public partial class PasswordBoxPlaceholder : UserControl
    {

        private Boolean focus = false;
        public string Password { get; set; }
        public string PasswordPlaceholder
        {
            get { return (string)GetValue(PasswordBoxPlaceholderProperty); }
            set { SetValue(PasswordBoxPlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PasswordBoxPlaceholderProperty =
            DependencyProperty.Register(
                    nameof(PasswordPlaceholder),
                    typeof(string),
                    typeof(TextBoxPlaceholder),
                    new PropertyMetadata("Placeholder")
            );

        public PasswordBoxPlaceholder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pega a senha digitada
        /// </summary>
        public string GetPassword()
        {
            return pb.Password;
        }

        /// <summary>
        /// Checa se o TextBox não está vazio
        /// </summary>
        private void TextBoxNotEmpty(object sender)
        {
            var pb = sender as PasswordBox;

            if (pb.Password.Length <= 0 && !focus)
            {
                tb_placeholder.Visibility = Visibility.Visible;
            }
            else
            {
                tb_placeholder.Visibility = Visibility.Hidden;
            }
        }

        // Lógica do placeholder

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBoxNotEmpty(sender);
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = true;
            //tb_placeholder.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            focus = false;
            TextBoxNotEmpty(sender);
        }
    }
}
