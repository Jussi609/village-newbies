using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace VillageNewbies.Helpers
{
    // Konvertteri, joka muuntaa null-arvon falseksi ja ei-null-arvon trueksi
    public class NotNullToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null!;
        }
    }

    // Konvertteri, joka muuntaa tyhjän merkkijonon falseksi ja ei-tyhjän trueksi
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null!;
        }
    }

    // Konvertteri, joka muuntaa arvon sen vastakohdaksi (true -> false, false -> true)
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }
    }
} 