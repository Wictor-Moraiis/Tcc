using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MarcenariaMorais
{
    public class ImgPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string relativePath = value as string;
            if (string.IsNullOrEmpty(relativePath))
                return null;

            Debug.WriteLine("\n");

            string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), relativePath.Replace('/', Path.DirectorySeparatorChar));
            Debug.WriteLine($"Realtive path = {relativePath}");
            Debug.WriteLine($"Path Combined = {fullPath}");

            if ((string)parameter == "Catalogo")
            {
                Debug.WriteLine("Tentando carregar uma imagem a partir do catalogo");
                string pastaLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MarcenariaMoraisDados/Img_Catalogo");
                string path = Path.Combine(pastaLocal, relativePath);
                fullPath = path;
            }

            Debug.WriteLine("Loading relativePath: " + relativePath);
            Debug.WriteLine("Loading fullPath: " + fullPath);

            if (!File.Exists(fullPath))
                return null;

            try
            {
                BitmapImage bitmap = new BitmapImage();

                // Carrega direto do FileStream - SEMPRE recarrega do disco
                using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Carrega na memória
                    bitmap.StreamSource = stream; // USA STREAM - não cacheia por URI
                    bitmap.EndInit();
                }

                bitmap.Freeze(); // Libera recursos
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar imagem: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}