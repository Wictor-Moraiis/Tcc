using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MarcenariaMorais
{
    /// <summary>
    /// Responsável pela navegação entre as telas do menu
    /// </summary>
    public static class NavigationHandler
    {
        private static Frame _frame;
        public static Frame Frame
        {
            get => _frame;
            set { if (_frame != value && value is System.Windows.Controls.Frame) _frame = value; }
        }

        public static Dictionary<string, Page> PageList = new Dictionary<string, Page>();

        /// <summary>
        /// Pega o último caractére de uma string
        /// </summary>
        private static char GetlastCharacter(string s)
        {
            char[] c = s.ToCharArray();
            char last = c[c.Length - 1];
            return last;
        }

        /// <summary>
        /// Muda a página atual
        /// </summary>
        public static void SetPage(string name)
        {
            if (!PageList.ContainsKey(name) && Frame != null) 
                return;

            var p = PageList[name]; // Page

            if (Frame.Content != p && p is System.Windows.Controls.Page) 
                Frame.Content = p;
        }

        /// <summary>
        /// Muda a página e executa uma função dela dependendo de seu tipo:
        /// L - Listar;
        /// A - Adicionar;
        /// E - Editar
        /// </summary>
        public static void SetAndRefresh(string name, object[] args = null)
        {
            if (!PageList.ContainsKey(name) && Frame != null)
                return;

            var p = PageList[name]; // Page

            if (Frame.Content == p && !(p is System.Windows.Controls.Page))
                return;

            char type = GetlastCharacter(name);
            switch (type) 
            {
                case 'L': // Listagem
                    try
                    {
                        var method = p.GetType().GetMethod("CarregarTabela");
                        if (method != null) 
                            method.Invoke(p, new object[0]);
                    }
                    catch (Exception e) 
                    {
                        MessageBox.Show($"Erro, não foi possível atualizar a tabela: {e.Message}");
                    }
                    break;
                case 'A': // Adicionar
                    try
                    {
                        var method = p.GetType().GetMethod("LimparCampos");
                        if (method != null)
                            method.Invoke(p, new object[0]);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Erro, não foi possível limpar os campos: {e.Message}");
                    }
                    break;
                case 'E': // Editar
                    try
                    {
                        var method = p.GetType().GetMethod("CarregarCampos");
                        if (method != null)
                            method.Invoke(p, args);
                    }
                    catch (Exception e) 
                    { 
                        MessageBox.Show($"Erro, não foi possível carregar os campos: {e.Message}");
                    }
                    break;
            }

            Frame.Content = p;
        }

        /// <summary>
        /// Adiciona uma página na sua lista de páginas navegáveis
        /// </summary>
        public static void AddPage(string name, Page page)
        {
            try
            {
                PageList.Add(name, page);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error ao adicionar página {name}: {e.Message}");
            }
        }
    }
}
