using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class OffGridPVQuery : Query
    {
        private OffGridPVQuery(List<string> paramNames) : base(paramNames) { }

        public static OffGridPVQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "usehorizon",
                "userhorizon",
                "raddatabase",
                "peakpower",
                "batterysize",
                "cutoff",
                "consumptionday",
                "hourconsumption",
                "angle",
                "aspect",
                "outputformat",
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new OffGridPVQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("SHScalc");

        public OffGridPVQuery WithHorizon(bool? useHorizon, List<double>? userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public OffGridPVQuery WithRadiationDatabase(string raddatabase)
        {
            SetRadiationDatabase(raddatabase);
            return this;
        }

        public OffGridPVQuery WithPVSystem(double peakpower, double batterysize, double cutoff)
        {
            SetValue("peakpower", peakpower);
            SetValue("batterysize", batterysize);
            SetValue("cutoff", cutoff);
            return this;
        }

        public OffGridPVQuery WithPVMouting(double inclination, double azimuth)
        {
            SetValue("angle", inclination);
            SetValue("aspect", azimuth);
            return this;
        }

        public OffGridPVQuery WithConsumption(double dailyConsumption)
        {
            SetValue("consumptionday", dailyConsumption);
            return this;
        }

        public OffGridPVQuery WithConsumption(List<double> hourlyConsumption)
        {
            var count = hourlyConsumption.Count;
            switch (count)
            {
                case 1:
                    SetValue("consumptionday", hourlyConsumption[0]);
                    break;
                case 24:
                    SetValue("hourconsumption", String.Join(",", hourlyConsumption));
                    break;
                default:
                    throw new ArgumentException($"hourlyConsumption must have exactly 24 values. Contains {hourlyConsumption.Count}.");
            }
            return this;
        }
    }
}