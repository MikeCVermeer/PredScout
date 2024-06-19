using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PredScout
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

        public class TruncateTextConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string text && text.Length > 24)
                {
                    return text.Substring(0, 21) + "...";
                }
                return value;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    public class RankAndRoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "Rank")
            {
                if (value is string rank)
                {
                    switch (rank)
                    {
                        case "Bronze I":
                        case "Bronze II":
                        case "Bronze III":
                            return Brushes.Brown;
                        case "Silver I":
                        case "Silver II":
                        case "Silver III":
                            return Brushes.Silver;
                        case "Gold I":
                        case "Gold II":
                        case "Gold III":
                            return Brushes.Gold;
                        case "Platinum I":
                        case "Platinum II":
                        case "Platinum III":
                            return Brushes.LightBlue;
                        case "Diamond I":
                        case "Diamond II":
                        case "Diamond III":
                            return Brushes.LightSkyBlue;
                        case "Master I":
                        case "Master II":
                        case "Master III":
                            return Brushes.Purple;
                        case "Grandmaster":
                            return Brushes.Red;
                        default:
                            return Brushes.Black;
                    }
                }
            }
            else if (parameter.ToString() == "Role" && value is string role)
            {
                switch (role)
                {
                    case "Offlane":
                        return Brushes.Orange;
                    case "Jungle":
                        return Brushes.LightGreen;
                    case "Midlane":
                        return Brushes.Magenta;
                    case "Carry":
                        return Brushes.Red;
                    case "Support":
                        return Brushes.DeepSkyBlue;
                    default:
                        return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
