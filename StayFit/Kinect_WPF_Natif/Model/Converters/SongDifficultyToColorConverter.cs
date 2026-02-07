using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace Kinect_WPF_Natif.Model.Converters
{
    public class SongDifficultyToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int difficulty = (int)value;
            switch (difficulty)
            {
                case 1:
                    return Brushes.Green;
                case 2:
                    return Brushes.LimeGreen;
                case 3:
                    return Brushes.Yellow;
                case 4:
                    return Brushes.Orange;
                case 5:
                    return Brushes.DarkRed;
                default:
                    return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
