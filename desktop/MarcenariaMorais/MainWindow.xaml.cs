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
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        Login Login { get; set; }
        Menu Menu { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Login = new Login(this);
            Menu = new Menu();

            f_main.Content = Login;
            
        }

        public void GoToMenu()
        {
            f_main.Content = Menu;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Coloca e tira o aplicativo da tela cheia
            if (e.Key == Key.F11 && this.WindowStyle == WindowStyle.None)
            {
                // Sai de tela cheia
                e.Handled        = true;
                this.ResizeMode  = ResizeMode.CanResize;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
            else if (e.Key == Key.F11 && this.WindowStyle != WindowStyle.None)
            {
                // Entra em tela cheia
                e.Handled        = true;
                this.ResizeMode  = ResizeMode.NoResize;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
        }
    }
}
