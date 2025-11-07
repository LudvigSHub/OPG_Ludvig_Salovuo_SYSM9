using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CookMaster.Converters
{
    // Används För att binda en bool till en visibility egenskap i XAML
    // Just i detta fallet är det för view eller edit läge i recipedetails
    // det är så man kan byta mellan att visa eller redigera ett recept
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => (value is bool b && b) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type t, object p, CultureInfo c)
            => (value is Visibility v && v != Visibility.Visible);
    }
}
