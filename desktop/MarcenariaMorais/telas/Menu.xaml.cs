using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Menu.xam
    /// </summary>
    public partial class Menu : Page
    {
        // Telas
        private HomeAjuda         home;

        // Listagens
        private Estoque_L         estoqueL;
        private Funcionario_L     funcionarioL;
        private Cat_Funcionario_L catFuncionarioL;
        private Cliente_L         clienteL;
        private Pedido_L          pedidoL;
        private Financeiro_L      financeiroL;
        private Catalogo_L        catalogoL;

        // Adicionar
        private Estoque_A         estoqueA;
        private Funcionario_A     funcionarioA;
        private Cat_Funcionario_A catFuncionarioA;
        private Cliente_A         clienteA;
        private Pedido_A          pedidoA;
        private Financeiro_A      financeiroA;
        private Catalogo_A        catalogoA;

        // Editar
        private Estoque_E         estoqueE;
        private Funcionario_E     funcionarioE;
        private Cat_Funcionario_E catFuncionarioE;
        private Cliente_E         clienteE;
        private Pedido_E          pedidoE;
        private Financeiro_E      financeiroE;
        private Catalogo_E        catalogoE;

        // Variáveis
        private List<Button> botoes;
        private MainWindow   mainWindow;

        // Styles
        private Style botaoMenu  = (Style)Application.Current.Resources["BotaoMenu"];
        private Style botaoSelec = (Style)Application.Current.Resources["BotaoMenuSelec"];

        public Menu()
        {
            InitializeComponent();

            // Inicializandoa as telas

            home            = new HomeAjuda();

            // Listagem
            estoqueL        = new Estoque_L ();
            funcionarioL    = new Funcionario_L();
            catFuncionarioL = new Cat_Funcionario_L();
            clienteL        = new Cliente_L();
            pedidoL         = new Pedido_L();
            financeiroL     = new Financeiro_L();
            catalogoL       = new Catalogo_L();

            // Adicionar
            estoqueA        = new Estoque_A();
            funcionarioA    = new Funcionario_A();
            catFuncionarioA = new Cat_Funcionario_A();
            clienteA        = new Cliente_A();
            pedidoA         = new Pedido_A();
            financeiroA     = new Financeiro_A();
            catalogoA       = new Catalogo_A();

            // Editar
            estoqueE        = new Estoque_E();
            funcionarioE    = new Funcionario_E();
            catFuncionarioE = new Cat_Funcionario_E();
            clienteE        = new Cliente_E();
            pedidoE         = new Pedido_E();
            financeiroE     = new Financeiro_E();
            catalogoE       = new Catalogo_E();

            // Inicialização das variáveis
            botoes                  = sp_botoes.Children.Cast<Button>().ToList();
            mainWindow              = Application.Current.MainWindow as MainWindow;
            NavigationHandler.Frame = f_telas;

            // Tela inicial
            f_telas.Content = home;

            // Adição das telas no NavigationHandler

            // Estoque
            NavigationHandler.AddPage("EstoqueL", estoqueL);
            NavigationHandler.AddPage("EstoqueA", estoqueA);
            NavigationHandler.AddPage("EstoqueE", estoqueE);

            // Clientes
            NavigationHandler.AddPage("ClienteL", clienteL);
            NavigationHandler.AddPage("ClienteA", clienteA);
            NavigationHandler.AddPage("ClienteE", clienteE);

            // Funcionario
            NavigationHandler.AddPage("FuncionarioL", funcionarioL);
            NavigationHandler.AddPage("FuncionarioA", funcionarioA);
            NavigationHandler.AddPage("FuncionarioE", funcionarioE);

            // Categoria Funcionario
            NavigationHandler.AddPage("CatFuncionarioL", catFuncionarioL);
            NavigationHandler.AddPage("CatFuncionarioA", catFuncionarioA);
            NavigationHandler.AddPage("CatFuncionarioE", catFuncionarioE);

            // Pedidos
            NavigationHandler.AddPage("PedidoL", pedidoL);
            NavigationHandler.AddPage("PedidoA", pedidoA);
            NavigationHandler.AddPage("PedidoE", pedidoE);

            // Financeiro
            NavigationHandler.AddPage("FinanceiroL", financeiroL);
            NavigationHandler.AddPage("FinanceiroA", financeiroA);
            NavigationHandler.AddPage("FinanceiroE", financeiroE);

            // Catálogo
            NavigationHandler.AddPage("CatalogoL", catalogoL);
            NavigationHandler.AddPage("CatalogoA", catalogoA);
            NavigationHandler.AddPage("CatalogoE", catalogoE);
        }

        /// <summary>
        /// Torna a primeira letra de uma string maiúscula
        /// </summary>
        private string FirstLetterUp(string str)
        {
            if (str != null)
                return char.ToUpper(str[0]) + str.Substring(1);
            else
                return str;
        }

        /// <summary>
        /// Retorna um icone a partir de seu tipo (preto ou branco) e seu nome 
        /// </summary>
        /// <param name="type">Pode ser: preto (IP) ou branco (IB)</param>
        private ImageSource GetIcon(int type, string name)
        {
            ImageSource icon = null;
            string n = FirstLetterUp(name);

            if (type == 0) // Branco
            {
                icon = (ImageSource)FindResource("IB" + n);
            } 
            else // Preto
            {
                icon = (ImageSource)FindResource("IP" + n);
            }

            return icon;
        }

        /// <summary>
        /// Gerencia a navegação do menu lateral e altera o estilo do botão selecionado;
        /// </summary>
        /// <param name="sender"></param>
        private void MenuNavigation(object sender)
        {
            Button b = sender as Button; // botão
            string n = b.Name;           // nome
            Image bi = b.FindName(n.Replace("btn", "img")) as Image; // botão imagem

            for (int i = 0; i < botoes.Count; i++)
            {
                Button btn     = (Button)botoes[i];
                string btnNome = btn.Name;
                Image img      = botoes[i].FindName(btnNome.Replace("btn", "img")) as Image;

                botoes[i].ClearValue(Button.StyleProperty);

                if (img != null)
                {
                    img.Source = GetIcon(0, btnNome.Replace("btn_", ""));
                }
            }

            b.Style = botaoSelec;
            bi.Source = GetIcon(1, n.Replace("btn_", ""));
        }
        
        // Botões de navegação do menu lateral

        private void btn_home_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = home;
            MenuNavigation(sender);
        }
        private void btn_estoque_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = estoqueL;
            estoqueL.CarregarTabela();
            MenuNavigation(sender);
        }
        private void btn_funcionarios_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = funcionarioL;
            funcionarioL.CarregarTabela();
            MenuNavigation(sender);
        }

        private void btn_clientes_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = clienteL;
            clienteL.CarregarTabela();
            MenuNavigation(sender);
        }

        private void btn_pedidos_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = pedidoL;
            pedidoL.CarregarTabela();
            MenuNavigation(sender);
        }

        private void btn_financeiro_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = financeiroL;
            financeiroL.CarregarTabela();
            MenuNavigation(sender);
        }

        private void btn_catalogo_Click(object sender, RoutedEventArgs e)
        {
            f_telas.Content = catalogoL;
            catalogoL.CarregarTabela();
            MenuNavigation(sender);
        }

        private void btn_ajuda_Click(object sender, RoutedEventArgs e)
        {
            string pdf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Arquivos/manual.pdf");

            if (File.Exists(pdf))
            {
                try
                {
                    var psi = new ProcessStartInfo(pdf)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    Console.WriteLine($"PDF aberto: {pdf}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao abrir o PDF: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"ERRO: pdf não foi encontrado {pdf}");
            }
        }
    }
}
