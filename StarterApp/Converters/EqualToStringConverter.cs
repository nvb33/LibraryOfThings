using System.Globalization;

namespace StarterApp.Converters;

public class EqualToStringConverter : IValueConverter
{
    // Returns true if the bound value equals the ConverterParameter
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && parameter is string target)
            return stringValue == target;

        return false;
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}