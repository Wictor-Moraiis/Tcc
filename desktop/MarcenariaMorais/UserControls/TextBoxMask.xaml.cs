using Org.BouncyCastle.Math.EC.Rfc8032;
using System;
using System.Collections.Generic;
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
using static System.Net.Mime.MediaTypeNames;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para TextBoxMask.xam
    /// </summary>
    public partial class TextBoxMask : System.Windows.Controls.UserControl
    {
        private string noMaskText = "";
        private int maxLenght;
        private bool isFiling = false;
        public string FillText;

        public TextBoxMask()
        {
            InitializeComponent();
            DataContext = this;
        }
        
        public string Mask
        {
            get { return (string)GetValue(TextBoxMaskProperty); }
            set { SetValue(TextBoxMaskProperty, value); }
        }

        public static readonly DependencyProperty TextBoxMaskProperty =
            DependencyProperty.Register(
                    nameof(Mask),
                    typeof(string),
                    typeof(TextBoxPlaceholder),
                    new PropertyMetadata("")
            );

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbx_texto.Text = Mask;
            maxLenght = Mask.Count(c => c == '_');
            if (FillText != null) 
            { 
                Fill(FillText);
            }
        }

        /// <summary>
        /// Reinicia as suas propriedades para seu estado default
        /// </summary>
        public void Reset()
        {
            tbx_texto.Text = Mask;
            noMaskText = "";
            tbx_texto.Select(0, 0);
        }

        /// <summary>
        /// Preenche o TextBox, adequando uma string para a sua máscara
        /// </summary>
        private void Fill(string text)
        {
            isFiling = true;

            string tbxT = Mask;
            char[] t    = tbxT.ToCharArray(); // Texto em máscara
            char[] set  = text.ToCharArray(); // Texto que vai ser colocado sobre a máscara
            int    s    = Mask.Length;

            for (int i = 0; i < set.Length; i++) 
            {
                tbxT = String.Join("", t);
                int pos = GetNextInputPos(tbxT);
                //Debug.WriteLine(pos.ToString() + " " + i.ToString());
                t[pos] = set[i];
            }

            Debug.WriteLine(string.Join("", t));

            tbx_texto.Text = String.Join("", t);
            tbx_texto.Select(s, 0);
            noMaskText = text;
            isFiling = false;
            Debug.WriteLine(tbx_texto.Text);
        }

        /// <summary>
        /// Pega o texto sem a filtragem da máscara
        /// </summary>
        public string getUnmaskedText()
        {
            return noMaskText;
        }

        /// <summary>
        /// Pega o tamanho máximo do texto permitido pela máscara
        /// </summary>
        /// <returns></returns>
        public int getMaxLenght()
        {
            return maxLenght;
        }

        /// <summary>
        /// Pega a próxima posição para inserir o input; Ignorando outros campos que não sejam '_'
        /// </summary>
        private int GetNextInputPos(string text)
        {
            int pos = -1;

            if (text == null || text.Length < 1)
                return pos;

            char[] t = text.ToCharArray();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].ToString() == "_")
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }

        /// <summary>
        /// Pega a última posição digitada para remover o input
        /// </summary>
        private int GetLastInputPos(string text)
        {
            int pos = -1;

            if (text == null || text.Length < 1)
                return pos;

            char[] m = Mask.ToCharArray();
            char[] t = text.ToCharArray();
            for (int i = t.Length - 1; i >= 0; i--)
            {
                if (t[i].ToString() != "_" && m[i].ToString() == "_")
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }

        // Função repsonsável por apagar ou deletar o caracteres do texto da máscara
        private void tbx_texto_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;

                if (noMaskText.Length > 0)
                    noMaskText = noMaskText.Remove(noMaskText.Length - 1);

                string t = tbx_texto.Text;
                int pos = GetLastInputPos(t);

                if (pos < 0)
                    return;
                
                char[] current = t.ToCharArray();
                char[] m = Mask.ToCharArray();

                current[pos] = m[pos]; // Substitui o caractere anterior pelo caractere da mascará na mesma posição

                tbx_texto.Text = String.Join("", current);
                tbx_texto.Select(pos, 0);
            }
        }

        // Recebe e modifica o texto digitado pelo usuário para adequálo à máscara
        private void tbx_texto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (isFiling)
                return;

            e.Handled = true;

            int val;
            bool eInt = int.TryParse(e.Text,out val); // É int?

            if (!eInt || noMaskText.Length >= maxLenght)
                return;

            noMaskText += e.Text;
            Debug.WriteLine(noMaskText);

            string tbxT = tbx_texto.Text;
            char[] t = tbxT.ToCharArray();
            char key = Convert.ToChar(e.Text); // Tecla precionada
            int pos = GetNextInputPos(tbxT);
            int s = pos + 1; // Seleção

            if (pos < 0)
                return;

            t[pos] = key;
            tbx_texto.Text = String.Join("", t);
            tbx_texto.Select(s, 0);

        }

        // Impede com que os comandos de 'copiar' e 'colar' sejam executados nele para evitar bugs
        private void tbx_texto_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }
    }

}
