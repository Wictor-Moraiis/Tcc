using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    /// É a primeira página aberta pelo software, utilizada para transição para o menu principal através de uma senha
    /// </summary>
    public partial class Login : Page
    {
        private MainWindow mw;
        public Login(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
        }

        /// <summary>
        /// Checa se a senha está certa, caso esteja, muda o conteúdo do frame principal para a página HomeAjuda
        /// </summary>
        private Boolean CheckPassword(string pass = "")
        {
            if (pass != "SUA_SENHA_AQUI") return false;

            return true;
        }

        private void LoginCheck()
        {
            var MainWindow = this.Parent as MainWindow; // Ele pega a aplicação da tela principal para pode executar o GoToHome
            string s = pb_senha.GetPassword();

            if (mw != null && CheckPassword(s))
                mw.GoToMenu();
        }

        private void btn_entrar_Click(object sender, RoutedEventArgs e)
        {
            LoginCheck();
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginCheck();
        }
    }
}
