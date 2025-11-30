using System.Globalization;

namespace LaurelLibrary.Views.Converters;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return Application.Current?.Resources["Tertiary"] ?? Colors.Gray;

        var statusColor = value as string;

        return statusColor switch
        {
            "Danger" => Application.Current?.Resources["Danger"] ?? Colors.Red,
            "Warning" => Application.Current?.Resources["Warning"] ?? Colors.Orange,
            "Success" => Application.Current?.Resources["Success"] ?? Colors.Green,
            _ => Application.Current?.Resources["Tertiary"] ?? Colors.Gray
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
