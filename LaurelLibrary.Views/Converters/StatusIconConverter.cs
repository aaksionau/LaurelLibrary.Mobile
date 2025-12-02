using System.Globalization;

namespace LaurelLibrary.Views.Converters;

public class StatusIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return "\uf02d";

        var statusText = value?.ToString() ?? string.Empty;

        if (statusText.Contains("Overdue"))
            return "\uf06a";
        
        if (statusText.Contains("Due in"))
            return "\uf017";
        
        return "\uf02d";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
