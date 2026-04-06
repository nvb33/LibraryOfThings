using System.Globalization;

namespace StarterApp.Converters;

public class BoolToColorConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Color.FromArgb("#512BD4");
    public Color FalseColor { get; set; } = Color.FromArgb("#9E9E9E");

    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        bool b = value is bool boolVal && boolVal;

        // Support inline parameter format "TrueColor|FalseColor"
        if (parameter is string paramStr && paramStr.Contains('|'))
        {
            var parts = paramStr.Split('|');
            if (parts.Length == 2)
            {
                var trueColor = Color.FromArgb(parts[0]);
                var falseColor = Color.FromArgb(parts[1]);
                return b ? trueColor : falseColor;
            }
        }

        return b ? TrueColor : FalseColor;
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}