using System.Globalization;

namespace StarterApp.Converters;

/// <summary>
/// Converts a string value to a boolean indicating whether the string has content.
/// Returns true if the string is not null, empty, or whitespace; false otherwise.
/// Used in XAML to show or hide elements based on whether a string property has a value,
/// such as showing an error message label only when ErrorMessage is non-empty.
/// </summary>
/// <example>
/// Show a label only when ErrorMessage has content:
/// <code>IsVisible="{Binding ErrorMessage, Converter={StaticResource StringToBoolConverter}}"</code>
/// </example>
public class StringToBoolConverter : IValueConverter
{
    /// <summary>
    /// Returns true if the bound string is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string value to evaluate.</param>
    /// <param name="targetType">The target type of the binding (unused).</param>
    /// <param name="parameter">Unused.</param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>True if the string has content, false otherwise.</returns>
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
            return !string.IsNullOrWhiteSpace(stringValue);

        return false;
    }

    /// <summary>
    /// Converts a boolean back to a string representation.
    /// Returns "true" for true and an empty string for false.
    /// </summary>
    /// <param name="value">The boolean value to convert back.</param>
    /// <param name="targetType">The target type of the binding (unused).</param>
    /// <param name="parameter">Unused.</param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>"true" if the value is true, otherwise an empty string.</returns>
    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? "true" : string.Empty;

        return string.Empty;
    }
}