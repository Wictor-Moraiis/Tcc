using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para HomeAjuda.xam
    /// </summary>
    public partial class HomeAjuda : Page
    {
        public HomeAjuda()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
