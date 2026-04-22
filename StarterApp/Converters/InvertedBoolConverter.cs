using System.Globalization;

namespace StarterApp.Converters;

/// <summary>
/// Converts a boolean value to its logical inverse.
/// Used in XAML to disable controls while an operation is in progress,
/// where IsBusy=true should map to IsEnabled=false.
/// </summary>
/// <example>
/// Disable a button while loading:
/// <code>IsEnabled="{Binding IsBusy, Converter={StaticResource InvertedBoolConverter}}"</code>
/// </example>
public class InvertedBoolConverter : IValueConverter
{
    /// <summary>
    /// Returns the logical inverse of the bound boolean value.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="targetType">The target type of the binding (unused).</param>
    /// <param name="parameter">Unused.</param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>The inverted boolean, or false if the value is not a boolean.</returns>
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return false;
    }

    /// <summary>
    /// Returns the logical inverse of the bound boolean value.
    /// Supports two-way binding scenarios.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="targetType">The target type of the binding (unused).</param>
    /// <param name="parameter">Unused.</param>
    /// <param name="culture">The culture to use (unused).</param>
    /// <returns>The inverted boolean, or false if the value is not a boolean.</returns>
    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return false;
    }
}