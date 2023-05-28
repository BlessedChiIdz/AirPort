using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AirportTimeTable.Views.Converters {
    internal class StatusToColor: IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value is not string @str) throw new ArgumentException("Не тот тип: " + value);
            return @str switch {
                "Отменён" => Brushes.Red,
                "Задерживается" => Brushes.Yellow,
                _ => Brushes.White,
            };
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
