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
    public class OperationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
            {
                var v = (TypeOperation)value;
                if (v == TypeOperation.Entree)
                    return Properties.Resources.Entree;
                else
                    return Properties.Resources.Sortie;
                
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
