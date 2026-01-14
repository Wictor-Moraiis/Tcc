using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MarcenariaMorais
{
    public class CpfFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            string cpf = value.ToString();

            string digits = new string(cpf.Where(char.IsDigit).ToArray());

            if (digits.Length < 11)
            {
                // Garante que o cpf esteja com os zeros no inicío caso ele seja menor que 11 números
                digits = digits.PadLeft(11, '0');
            }
            else if (digits.Length > 11)
            {
                // Garante que o cpf esteja apenas com 11 números
                digits = digits.Substring(0, 11);
            }

            return $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9, 2)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            // Remove a formatação
            string digits = new string(value.ToString().Where(char.IsDigit).ToArray());

            if (digits.Length < 11)
            {
                digits = digits.PadLeft(11, '0');
            }
            else if (digits.Length > 11)
            {
                digits = digits.Substring(0, 11);
            }

            return digits;
        }
    }
}
