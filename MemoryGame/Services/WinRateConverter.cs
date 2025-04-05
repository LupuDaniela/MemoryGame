using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryGame.Services
{
    public class WinRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MemoryGame.Model.PlayerStatistic playerStat && playerStat.GamesPlayed > 0)
            {
                double winRate = (double)playerStat.GamesWon / playerStat.GamesPlayed * 100;
                return winRate.ToString("0.0");
            }
            return "0.0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
