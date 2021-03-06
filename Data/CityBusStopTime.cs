﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace NextBusMX.Data
{
    public class CityBusStopTime
    {
        public string RouteNumber
        {
            get;
            set;
        }
        
        public string RouteName
        {
            get;
            set;
        }

        public DateTime ArrivalTime
        {
            get;
            set;
        }

        public string FriendlyArrivalTime
        {
            get
            {
                int minutes = Convert.ToInt32((ArrivalTime - DateTime.Now).TotalMinutes);

                if (minutes < 0)
                    return "";
                else if (minutes == 0)
                    return "Boarding";
                else if (minutes == 1)
                    return "1 min";
                else if (minutes > 15)
                    return ArrivalTime.ToString("h:mm tt");
                else
                    return Convert.ToInt32((ArrivalTime - DateTime.Now).TotalMinutes) + " mins";
            }
        }

        public SolidColorBrush ArrivalTimeForeground
        {
            get
            {
                int minutes = Convert.ToInt32((ArrivalTime - DateTime.Now).TotalMinutes);

                if (minutes < 0)
                {
                    return (SolidColorBrush)App.Current.Resources["ApplicationForegroundThemeBrush"]; // default
                }
                else if (minutes <= 5)
                {
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 219, 39, 39)); // red
                }
                else if (minutes <= 15)
                {
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 245, 223, 56)); // yellow
                }
                else
                {
                    return (SolidColorBrush)App.Current.Resources["ApplicationForegroundThemeBrush"]; // default
                }
            }
        }
    }
}
