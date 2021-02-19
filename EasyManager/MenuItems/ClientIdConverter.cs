using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasyManager.MenuItems
{
    public class ClientIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int id = (int)value;
                if (id == 0)
                    return "-";
                var data = EasyManagerDb.DbManager.GetById<EasyManagerLibrary.Client>(id);
                return $"{data.Prenom} {data.Nom}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
