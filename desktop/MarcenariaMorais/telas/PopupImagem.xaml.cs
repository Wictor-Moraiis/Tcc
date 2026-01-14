using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MarcenariaMorais
{
    /// <summary>
    /// Lógica interna para PopupImagem.xaml
    /// </summary>
    public partial class PopupImagem : Window
    {
        public ObservableCollection<string> CaminhosDasImagens { get; set; }

        private string _nomeArquivoSelecionado;
        public string NomeArquivoSelecionado
        {
            get { return _nomeArquivoSelecionado; }
            set
            {
                _nomeArquivoSelecionado = value;
                OnPropertyChanged(nameof(NomeArquivoSelecionado));
            }
        }

        public string Pasta;

        // Para notificar o XAML
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PopupImagem()
        {
            InitializeComponent();
            DataContext = this;
        }


        //pega as imagens salvar dentro da pasta
        public void Carregamento()
        {
            string pastaImagens = Path.GetFullPath(Pasta);

            if (Directory.GetFiles(pastaImagens).Length > 0)
            {
                CaminhosDasImagens = new ObservableCollection<string>(
                Directory.GetFiles(pastaImagens, "*.*").Where(file => file.EndsWith(".jpg") || file.EndsWith(".png")
                || file.EndsWith(".jpeg") || file.EndsWith(".bmp")));
                tbk_nome.Text = Path.GetFileName(CaminhosDasImagens[0]);
            }
            return;
        }

        //Seletor de imagens
        private void lstImagens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string caminhoSelecionado = lstImagens.SelectedItem as string;

            if (!string.IsNullOrEmpty(caminhoSelecionado))
            {
                imgDestaque.Source = new BitmapImage(new System.Uri(caminhoSelecionado)); //Imagem exibida
                tbk_nome.Text = Path.GetFileName(caminhoSelecionado);
            }
        }

        //Possibilita que usemos o Scroll do MOuse na horizontal paa mover a lista
        private ScrollViewer FindScrollViewer(DependencyObject d)
        {

            if (d is ScrollViewer)
            {
                return d as ScrollViewer;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var SW = FindScrollViewer(VisualTreeHelper.GetChild(d, i));
                if (SW != null)
                {
                    return SW;
                }
            }

            return null;
        }

        private void ListViewer_PreviewMoseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = FindScrollViewer(lstImagens);
            var h = scrollViewer.HorizontalOffset;
            var d = e.Delta;
            var nv = 0d;

            if (d < 0)
            {
                nv = d + h < 0 ? 0 : d + h;
            }
            else
            {
                nv = d + h > scrollViewer.ScrollableWidth ? scrollViewer.ScrollableWidth : d + h;
            }

            scrollViewer.ScrollToVerticalOffset(0);
            scrollViewer.ScrollToHorizontalOffset(nv);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_avancar_Click(object sender, RoutedEventArgs e)
        {
            int total = CaminhosDasImagens?.Count ?? 0;
            if (total == 0) { return; }

            int currentIndex = lstImagens.SelectedIndex;

            // Se nada estiver selecionado, começa do índice 0
            if (currentIndex == -1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex = (currentIndex + 1) % total;  // Vai para o próximo, volta ao início no fim
                lstImagens.SelectedIndex = currentIndex;
                lstImagens.ScrollIntoView(lstImagens.SelectedItem); // Garante que a miniatura selecionada fique visível
            }
        }

        private void btn_voltar_Click(object sender, RoutedEventArgs e)
        {
            int total = CaminhosDasImagens?.Count ?? 0;
            if (total == 0)
                return;

            int currentIndex = lstImagens.SelectedIndex;

            // Se nada estiver selecionado, começa do índice 0
            if (currentIndex == -1)
            {
                currentIndex = total - 1;
            }
            else
            {
                currentIndex = (currentIndex - 1 + total) % total;  // Vai para o próximo, volta ao início no fim

                lstImagens.SelectedIndex = currentIndex;
                lstImagens.ScrollIntoView(lstImagens.SelectedItem);
            }
        }
    }
}
