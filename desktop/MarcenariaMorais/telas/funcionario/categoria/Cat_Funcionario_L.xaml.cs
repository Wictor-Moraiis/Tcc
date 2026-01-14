using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Cat_Funcionario_L.xam
    /// </summary>
    public partial class Cat_Funcionario_L : Page
    {
        private List<Cat_Funcionario> daO; // Dados Originais
        private ICollectionView categoriasView;

        public Cat_Funcionario_L()
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
            daO = Cat_Funcionario.Listar();
            categoriasView = CollectionViewSource.GetDefaultView(daO);
            dt_tabela.ItemsSource = categoriasView;
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("CatFuncionarioA");
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma categoria para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var catSel = (Cat_Funcionario)dt_tabela.SelectedItem;

            int    id   = catSel.Catg_id;
            string nome = catSel.Catg_nome.Trim();
            double sal  = catSel.Catg_sal;

            Cat_Funcionario cat = new Cat_Funcionario
            {
                Catg_id   = id,
                Catg_nome = nome,
                Catg_sal  = sal
            };

            NavigationHandler.SetAndRefresh("CatFuncionarioE", new object[] { cat });
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma categoria para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var catSel = (Cat_Funcionario)dt_tabela.SelectedItem;

            Cat_Funcionario cat = new Cat_Funcionario
            {
                Catg_id   = catSel.Catg_id,
                Catg_nome = catSel.Catg_nome,
                Catg_sal  = catSel.Catg_sal
            };

            bool? ok = cat.Excluir();
            if (ok == true)
            {
                CarregarTabela();
            }
        }

        private void tb_tabelas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ti_funcionarios.IsSelected)
            {
                ti_funcionarios.IsSelected = false;
                ti_categorias  .IsSelected = true;

                NavigationHandler.SetAndRefresh("FuncionarioL");
            }
        }

        private void tc_tabelas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ti_funcionarios.IsSelected == true)
            {
                NavigationHandler.SetAndRefresh("FuncionarioL");
                ti_categorias.IsSelected = true;
            }
        }

        private void AplicarFiltro(object sender, TextChangedEventArgs e)
        {
            if (daO == null) return;

            string textoPesquisa = tb_pesquisa.Text.Trim();

            if (string.IsNullOrWhiteSpace(textoPesquisa))
            {
                // Se não há texto, mostra tudo
                categoriasView.Filter = null;
            }
            else
            {
                categoriasView.Filter = obj =>
                {
                    var cat = obj as Cat_Funcionario;
                    if (cat == null) return false;

                    // Sua lógica de filtro aqui. Exemplo:
                    return cat.Catg_nome.IndexOf(textoPesquisa, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            }
        }
    }
}
