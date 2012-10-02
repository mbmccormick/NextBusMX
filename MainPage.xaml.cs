using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NextBusMX.Data;
using NextBusMX.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NextBusMX
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        #region Data Binding

        CityBusDataSource DataSource;
        DispatcherTimer RefreshTimer;

        public static readonly DependencyProperty StopsProperty = DependencyProperty.Register("Stops", typeof(ObservableCollection<CityBusStop>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<CityBusStop>()));

        public ObservableCollection<CityBusStop> Stops
        {
            get { return (ObservableCollection<CityBusStop>)GetValue(StopsProperty); }
            set { SetValue(StopsProperty, value); }
        }

        public static readonly DependencyProperty StopTimesProperty = DependencyProperty.Register("StopTimes", typeof(ObservableCollection<CityBusStopTime>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<CityBusStopTime>()));

        public ObservableCollection<CityBusStopTime> StopTimes
        {
            get { return (ObservableCollection<CityBusStopTime>)GetValue(StopTimesProperty); }
            set { SetValue(StopTimesProperty, value); }
        }

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataSource = new CityBusDataSource();
            Stops = await DataSource.Stops();

            this.lstStops.ItemContainerGenerator.ItemsChanged += (sender, f) =>
            {
                if (this.lstStops.SelectedIndex == -1)
                {
                    this.lstStops.SelectedIndex = 0;
                    this.lstStops.UpdateLayout();
                }
            };
            
            RefreshTimer = new DispatcherTimer();
            RefreshTimer.Interval = new TimeSpan(0, 0, 20);
            RefreshTimer.Tick += RefreshTimer_Tick;

            RefreshTimer.Start();
        }

        private async void RefreshTimer_Tick(object sender, object e)
        {
            if (this.lstStops.SelectedItem == null)
            {
                StopTimes = await DataSource.StopTimes(Stops.First() as CityBusStop);

                if (StopTimes.Count == 0)
                {
                    StopTimes.Add(new CityBusStopTime() { RouteNumber = "N/A", RouteName = "No departures in next 2 hours", ArrivalTime = DateTime.MinValue });
                }
            }
            else
            {
                StopTimes = await DataSource.StopTimes(this.lstStops.SelectedItem as CityBusStop);

                if (StopTimes.Count == 0)
                {
                    StopTimes.Add(new CityBusStopTime() { RouteNumber = "N/A", RouteName = "No departures in next 2 hours", ArrivalTime = DateTime.MinValue });
                }
            }
        }

        private void lstStops_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTimer_Tick(null, null);
        }
    }
}
