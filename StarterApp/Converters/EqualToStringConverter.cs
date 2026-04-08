using System.Globalization;

namespace StarterApp.Converters;

/// <summary>
/// Converts a string value to a boolean by comparing it to a given parameter.
/// Returns true if the bound string equals the ConverterParameter, false otherwise.
/// Used in XAML to conditionally show or hide elements based on a string property value.
/// </summary>
/// <example>
/// Show a button only when Status equals "Requested":
/// <code>IsVisible="{Binding Status, Converter={StaticResource EqualToStringConverter}, ConverterParameter='Requested'}"</code>
/// </example>
public class EqualToStringConverter : IValueConverter
{
    /// <summary>
    /// Returns true if the bound string value equals the ConverterParameter string.
    /// </summary>
    /// <param name="value">The bound string value to compare.</param>
    /// <param name="targetType">The target type of the binding (unused).</param>
    /// <param name="parameter">The string to compare against.</param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>True if value equals parameter, false otherwise.</returns>
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && parameter is string target)
            return stringValue == target;

        return false;
    }

    /// <summary>
    /// Not implemented. This converter does not support two-way binding.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}