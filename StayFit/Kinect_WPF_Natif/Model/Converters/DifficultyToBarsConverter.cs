using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Kinect_WPF_Natif.Model.Converters
{
    public class DifficultyToBarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int difficulty = (int)value;
            var brushes = new List<SolidColorBrush>();

            SolidColorBrush activeBrush;
            switch (difficulty)
            {
                case 1:
                    activeBrush = Brushes.Green;
                    break;
                case 2:
                    activeBrush = Brushes.LimeGreen;
                    break;
                case 3:
                    activeBrush = Brushes.Yellow;
                    break;
                case 4:
                    activeBrush = Brushes.Orange;
                    break;
                case 5:
                    activeBrush = Brushes.DarkRed;
                    break;
                default:
                    return Brushes.Gray;
            }

            for (int i = 1; i <= 5; i++)
            {
                brushes.Add(i <= difficulty ? activeBrush : new SolidColorBrush(Color.FromRgb(60, 60, 60)));
            }
            return brushes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
