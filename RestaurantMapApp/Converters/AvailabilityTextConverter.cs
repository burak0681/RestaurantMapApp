using System.Globalization;

namespace RestaurantMapApp.Converters;

public class AvailabilityTextConverter : IValueConverter
{
    public static AvailabilityTextConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isAvailable)
        {
            return isAvailable ? "MEVCUT" : "TÜKENDİ";
        }
        return "BİLİNMİYOR";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 