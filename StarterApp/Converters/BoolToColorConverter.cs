using System.Globalization;

namespace StarterApp.Converters;

/// <summary>
/// Converts a boolean value to a <see cref="Color"/> for use in XAML bindings.
/// Supports both property-based configuration and an inline pipe-delimited parameter format.
/// </summary>
/// <example>
/// Basic usage with default colours:
/// <code>BackgroundColor="{Binding IsActive, Converter={StaticResource BoolToColorConverter}}"</code>
/// Inline parameter usage with custom colours:
/// <code>TextColor="{Binding IsSelected, Converter={StaticResource BoolToColorConverter}, ConverterParameter='#FFD700|#CCCCCC'}"</code>
/// </example>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>Gets or sets the colour returned when the bound value is true. Defaults to purple (#512BD4).</summary>
    public Color TrueColor { get; set; } = Color.FromArgb("#512BD4");

    /// <summary>Gets or sets the colour returned when the bound value is false. Defaults to grey (#9E9E9E).</summary>
    public Color FalseColor { get; set; } = Color.FromArgb("#9E9E9E");

    /// <summary>
    /// Converts a boolean value to a colour.
    /// If a pipe-delimited parameter is provided (e.g. "#FFD700|#CCCCCC"), those colours
    /// are used instead of the configured <see cref="TrueColor"/> and <see cref="FalseColor"/>.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type of the binding (unused).</param>
    /// <param name="parameter">Optional pipe-delimited hex colour string in the format "TrueColour|FalseColour".</param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>A <see cref="Color"/> corresponding to the boolean value.</returns>
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        bool b = value is bool boolVal && boolVal;

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

    /// <summary>
    /// Not implemented. This converter does not support two-way binding.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}