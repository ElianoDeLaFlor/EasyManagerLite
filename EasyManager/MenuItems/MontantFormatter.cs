using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasyManager.MenuItems
{
    public class MontantFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                if (decimal.TryParse(value.ToString(), out decimal d))
                {
                    //var v = (decimal)value;
                    return InfoChecker.CurrencyFormat(d);
                }
                return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
