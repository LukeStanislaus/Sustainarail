using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Sustainarail
{
    enum TrainTypes
    {
        Hitatchi9Car,
        InterCity1259Car,
        Hitatchi5Car,
        InterCity1255Car,
        Class357
    }
    enum ControlType
    {
        UpdateToExisting, Modular, ERTMS
    }
    enum Electrification
    {
        Electrified, NonElectrified
    }

    class Train
    {
        public TrainTypes Name { get; set; }
        public int Speed { get; set; }
        public int Capacity { get; set; }
        public int CountOnTrain { get; set; }
        public List<Station> Route { get; set; }
        public int TotalUsage { get; set; }
        public TimeSpan Time { get; set; }

        public Train(TrainTypes name, int speed, int capacity, List<Station> route, int totalUsage)
        {
            Name = name;
            Speed = speed;
            Capacity = capacity;
            Route = route;
            CountOnTrain = 0;
            TotalUsage = totalUsage;
            Time = new TimeSpan();
        }

        public List<Station> CalculateService()
        {
            Console.WriteLine("{0}:", Name.ToString());

            while (Time.Hours == 0)
            {

                for (int j = 0; j < Route.Count; j++)
                {
                    var station = Route[j];
                    var initialCount = CountOnTrain;
                    Console.WriteLine("     Station: {0}", station.Name);
                    if (Time.Hours != 0) break;
                    var countLeavingTrain = CountOnTrain * (station.Leaving + station.Entering) / TotalUsage;
                    Console.WriteLine("         People leaving train:{0}", countLeavingTrain);
                    station.Entering -= countLeavingTrain;
                    CountOnTrain = (CountOnTrain - countLeavingTrain);
                    station.Leaving -= (Capacity - CountOnTrain);

                    CountOnTrain = Capacity;
                    if (station.Entering < 0) station.Entering = 0;
                    if (station.Leaving < 0)
                    {
                        CountOnTrain += station.Leaving;
                        station.Leaving = 0;
                    }
                    Console.WriteLine("         People entering train:{0}", CountOnTrain-(initialCount - countLeavingTrain));
                    Console.WriteLine("         People on train:{0}", CountOnTrain);
                    var distanceTravelled = station.DistFromLondon - Route[j == 0 ? 0 : j - 1].DistFromLondon;
                    var minutesTaken = (60 * (distanceTravelled < 0 ? -distanceTravelled : distanceTravelled) / Speed);
                    Time = Time.Add(new TimeSpan(0, (int)Math.Ceiling(minutesTaken), 0));
                    Console.WriteLine("         People which still need to leave station:{0}", station.Leaving);
                    Console.WriteLine("         People which still need to enter station:{0}", station.Entering);
                }
                Route.Reverse();
            }
            if (Route[0].DistFromLondon != 0) Route.Reverse();

            return Route;
        }
    }
    class Station
    {
        public string Name { get; set; }
        public int Leaving { get; set; }
        public int Entering { get; set; }
        public float DistFromLondon { get; set; }
        public Electrification Electrification { get; set; }
        public ControlType ControlType { get; set; }

        public Station(string name, int leaving, int entering, float distFromLondon, Electrification electrification, ControlType controlType)
        {
            Name = name;
            Leaving = leaving;
            Entering = entering;
            DistFromLondon = distFromLondon;
            Electrification = electrification;
            ControlType = ControlType;
        }

    }
    class Program
    {
        public static void PrintRailway(List<Station> Railway)
        {
            for (int i = 0; i < Railway.Count; i++)
            {
                Console.WriteLine("Station:{0} People Leaving:{1}, People Entering:{2}", 
                  Railway[i].Name, Railway[i].Leaving, Railway[i].Entering);

            }

        }
        static void Main(string[] args)
        {
            var BristolTempleMeads = new Station("BristolTempleMeads", 2145, 2145, 118.5f, Electrification.Electrified, ControlType.ERTMS);
            var BathSpa = new Station("BathSpa", 1105, 1105, 107, Electrification.Electrified, ControlType.ERTMS);
            var Chippenham = new Station("Chippenham", 338, 338, 94, Electrification.NonElectrified, ControlType.ERTMS);
            var Swindon = new Station("Swindon", 624, 624, 77.25f, Electrification.Electrified, ControlType.Modular);
            var Didcot = new Station("Didcot", 702, 702, 53.25f, Electrification.Electrified, ControlType.ERTMS);
            var Reading = new Station("Reading", 2912, 2912, 36, Electrification.Electrified, ControlType.ERTMS);
            var LondonPaddington = new Station("LondonPaddington", 6370, 6370, 0, Electrification.Electrified, ControlType.ERTMS);
            var Railway = new List<Station>()
                {BristolTempleMeads, BathSpa, Chippenham, Swindon, Didcot, Reading, LondonPaddington};
            Railway.Reverse();
            var result = Railway;
            for (int i = 0; i < 5; i++)
            {
                var Hitatchi = new Train(TrainTypes.Hitatchi9Car, 140, 627, result, 28393);
                result = Hitatchi.CalculateService();
            }            
            for (int i = 0; i < 5; i++)
            {
                var InterCity = new Train(TrainTypes.InterCity1259Car, 125, 531, result, 28393);
                result = InterCity.CalculateService();
            }
            for (int i = 0; i < 5; i++)
            {
                var Hitatchi = new Train(TrainTypes.Hitatchi5Car, 140, 315, result, 28393);
                result = Hitatchi.CalculateService();
            }
            for (int i = 0; i < 5; i++)
            {
                var InterCity = new Train(TrainTypes.InterCity1255Car, 125, 274, result, 28393);
                result = InterCity.CalculateService();
            }
            if (result[0].DistFromLondon != 0)
            {
                result.Reverse();
            }          
            for (int i = 0; i < 5; i++)
            {
                var Class357 = new Train(TrainTypes.Class357, 100, 282, result.Take(4).ToList(), 28393);
                var r = Class357.CalculateService();
                result[0] = r[0];
                result[1] = r[1];
                result[2] = r[2];
            }
            if (result[0].DistFromLondon == 0)
            {
                result.Reverse();
            }          
            for (int i = 0; i < 5; i++)
            {
                var Class357 = new Train(TrainTypes.Class357, 100, 282, result.Take(2).ToList(), 28393);
                var r= Class357.CalculateService();
                result[0] = r[0];
                result[1] = r[1];
            }
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Leaving != 0)
                {
                    PrintRailway(result);
                    Console.WriteLine("Doesn't work");
                    break; 
                }
                if (result.Count-1 == i)
                {
                    PrintRailway(result);
                    Console.WriteLine("Works");
                }
            }

        }
    }
}