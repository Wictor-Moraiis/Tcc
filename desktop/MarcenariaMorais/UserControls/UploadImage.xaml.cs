using Microsoft.Win32;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para UploadImage.xam
    /// </summary>
    public partial class UploadImage : UserControl
    {
        private string arquivo = null;   // Arquivo selecionado no momento
        public  string fileToLoad; // Variável usada para garantir que um arquivo seja carregado
        public bool keepName = false;
        public bool RandomFile = true;
        public string Destino // Pasta-destino do arquivo
        {
            get { return (string)GetValue(DestinoProperty); }
            set { SetValue(DestinoProperty, value); }
        }

        public static readonly DependencyProperty DestinoProperty =
        DependencyProperty.Register(
            nameof(Destino),
            typeof(string),
            typeof(UploadImage),
            new PropertyMetadata(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ImagensTemp"))
        );

        public UploadImage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reinicia as suas propriedades para seu estado default
        /// </summary>
        public void Reset()
        {
            tbk_texto.Text = "Nenhuma imagem selecionada";
            tbk_texto.Opacity = 0.5;
            arquivo = "";
        }

        /// <summary>
        /// Carrega um único arquivo a partir de seu caminho
        /// </summary>
        private void LoadFile(string file)
        {
            arquivo           = file;
            string nome       = System.IO.Path.GetFileName(file);
            tbk_texto.Text    = nome;
            tbk_texto.Opacity = 1;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Reset();
            if (fileToLoad != null) 
                LoadFile(fileToLoad);
        }

        /// <summary>
        /// Pega o arquivo que está selecionado
        /// </summary>
        public string GetFile()
        {
            if (!string.IsNullOrEmpty(arquivo))
                return arquivo;
            return null;
        }

        private void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo de Imagem|*.png;*.bmp;*.jpg;*.jpeg;*.webp";

            if (openFileDialog.ShowDialog() == true)
            {
                if (RandomFile) {
                    try
                    {
                        string arq = openFileDialog.FileName;

                        //Define onde vai a imagem copiada independente de onde o programa for instalado
                        string pastaDestino = Destino;

                        //verifica se a pasta  existe
                        if (!Directory.Exists(pastaDestino)) { Directory.CreateDirectory(pastaDestino); }

                        //Pega só o tipo de extençao do arquivo
                        string extension = System.IO.Path.GetExtension(arq);

                        //Cria um nome random para o arquivo e junta com a extenção e coloca um limite de 5 caracteres
                        string newName = Guid.NewGuid().ToString().Substring(0, 5) + extension;

                        // Caso KeepName seja true, ele manterá o nome original do arquivo
                        if (keepName)
                            newName = System.IO.Path.GetFileName(arq);

                        //Junta as duas string e cria o caminho do arquivo
                        string destino = System.IO.Path.Combine(pastaDestino, newName);

                        //copia o arquivo
                        File.Copy(arq, destino, true);

                        string nome = System.IO.Path.GetFileName(arq);

                        tbk_texto.Text = nome;
                        tbk_texto.Opacity = 1;

                        MessageBox.Show("Imagem copiada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                        arquivo = arq;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Não foi possível fazer o upload da imagem: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try { 
                        string arq  = openFileDialog.FileName;
                        string nome = System.IO.Path.GetFileName(arq);

                        tbk_texto.Text    = nome;
                        tbk_texto.Opacity = 1;

                        MessageBox.Show("Imagem selecionada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                        arquivo = arq;
                    }
                    catch (Exception ex)
                    {
                    MessageBox.Show("Não foi possível carregar a imagem: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
