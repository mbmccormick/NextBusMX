using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;

namespace NextBus_MX.Data
{
    public class CityBusDataSource
    {
        public async Task<ObservableCollection<CityBusStop>> Stops()
        {
            ObservableCollection<CityBusStop> stops = new ObservableCollection<CityBusStop>();

            string line;
            StringReader reader = new StringReader(GTFS.Stops);

            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = line.Split("\",\"".ToCharArray());

                CityBusStop stop = new CityBusStop();
                stop.ID = fields[1];
                stop.Name = fields[3];

                if (stop.ID.ToUpper() == "BUS271" ||
                    stop.ID.ToUpper() == "BUS300" ||
                    stop.ID.ToUpper() == "BUS547")
                    stops.Add(stop);
            }

            return stops;
        }

        public async Task<ObservableCollection<CityBusStopTime>> StopTimes(CityBusStop stop)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://myride.gocitybus.com/widget/Default1.aspx?pt=30&code=" + stop.ID);
            
            ObservableCollection<CityBusStopTime> stopTimes = new ObservableCollection<CityBusStopTime>();

            string line;
            StringReader reader = new StringReader(await response.Content.ReadAsStringAsync());

            for (int i = 0; i < 5; i++) // skip ahead 5 lines
                reader.ReadLine();

            while ((line = reader.ReadLine()) != null)
            {
                string routeName = line.Replace("<RouteName>", "").Replace("</RouteName>", "").Trim();

                line = reader.ReadLine();
                string timeTillArrival = line.Replace("<TimeTillArrival>", "").Replace("</TimeTillArrival>", "").Trim();

                CityBusStopTime stopTime = new CityBusStopTime();
                stopTime.RouteNumber = routeName.Split(' ')[0].Trim();
                stopTime.RouteName = routeName.Replace(stopTime.RouteNumber, "").Trim();

                if (stopTime.RouteNumber.Length > 3)
                    stopTime.RouteNumber = stopTime.RouteNumber.Substring(0, 3);

                stopTime.RouteNumber = stopTime.RouteNumber.ToUpper();

                if (timeTillArrival == "DUE")
                {
                    stopTime.ArrivalTime = DateTime.Now;
                }
                else
                {
                    stopTime.ArrivalTime = DateTime.Now.AddMinutes(Convert.ToInt32(timeTillArrival.Replace("min", "")));
                }

                stopTimes.Add(stopTime);

                for (int i = 0; i < 2; i++) // skip ahead 2 lines
                    reader.ReadLine();
            }

            return stopTimes;
        }
    }
}
