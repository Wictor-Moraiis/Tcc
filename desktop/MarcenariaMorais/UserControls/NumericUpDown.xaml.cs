using System;
using System.Collections.Generic;
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
    /// Interação lógica para NumericUpDown.xam
    /// </summary>

    // TODO: Transformar tudo em duble
    public partial class NumericUpDown : UserControl
    {
        public string Text;

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public int DefaultValue
        {
            get { return (int)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                nameof(MinValue),
                typeof(int),
                typeof(NumericUpDown),
                new PropertyMetadata(0)
            );

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(int),
                typeof(NumericUpDown),
                new PropertyMetadata(255)
            );

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(
                nameof(DefaultValue),
                typeof(int),
                typeof(NumericUpDown),
                new PropertyMetadata(0)
            );

        public NumericUpDown()
        {
            InitializeComponent();
            tb_num.Text = DefaultValue.ToString();
        }

        /// <summary>
        /// Reinicia as suas propriedades para seu estado default
        /// </summary>
        public void Reset()
        {
            tb_num.Text= DefaultValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Valor</param>
        public void SetNumber(int n)
        {
            tb_num.Text = n.ToString();
        }

        private void btn_cima_Click(object sender, RoutedEventArgs e)
        {
            int n = Convert.ToInt32(tb_num.Text);
            if (n < MaxValue)
                tb_num.Text = Convert.ToString(n + 1);
        }

        private void btn_baixo_Click(object sender, RoutedEventArgs e)
        {
            int n = Convert.ToInt32(tb_num.Text);
            if (n > MinValue)
                tb_num.Text = Convert.ToString(n - 1);
        }

        private void tb_num_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Ao segurar tanto o botão para cima, quanto para baixo, ele aumenta e diminui respectivamente

            if (e.Key == Key.Up)
            {
                btn_cima.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btn_cima, new object[] { true });
            }

            if (e.Key == Key.Down)
            {
                btn_baixo.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btn_baixo, new object[] { true });
            }
        }

        private void tb_num_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btn_cima, new object[] { false });

            if (e.Key == Key.Down)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(btn_baixo, new object[] { false });
        }

        private void tb_num_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int number = Convert.ToInt32(tb_num.Text);

                if (number > MaxValue) tb_num.Text = MaxValue.ToString();
                if (number < MinValue) tb_num.Text = MinValue.ToString();

                tb_num.SelectionStart = tb_num.Text.Length;
                Text = tb_num.Text;
            } 
            catch { }
        }
    }
}
