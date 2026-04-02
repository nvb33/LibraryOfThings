using System.Globalization;

namespace StarterApp.Converters;

public class BoolToColorConverter : IValueConverter
{
    // Active colour — used when bool is true
    public Color TrueColor { get; set; } = Color.FromArgb("#512BD4");

    // Inactive colour — used when bool is false
    public Color FalseColor { get; set; } = Color.FromArgb("#9E9E9E");

    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? TrueColor : FalseColor;

        return FalseColor;
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}