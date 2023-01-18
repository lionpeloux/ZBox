using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class DailyRadiationQuery : Query
    {
        private DailyRadiationQuery(List<string> paramNames) : base(paramNames) { }

        public static DailyRadiationQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "usehorizon",
                "userhorizon",
                "raddatabase",
                "month",
                "angle",
                "aspect",
                "global",
                "glob_2axis",
                "clearsky",
                "clearsky_2axis",
                "showtemperatures",
                "localtime",
                "outputformat"
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new DailyRadiationQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("DRcalc");

        public DailyRadiationQuery WithHorizon(bool useHorizon, List<double> userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public DailyRadiationQuery WithRadiationDatabase(string raddatabase)
        {
            SetRadiationDatabase(raddatabase);
            return this;
        }

        public DailyRadiationQuery WithMonth(int month)
        {
            SetMonth(month);
            return this;
        }
        public DailyRadiationQuery WithAllMonths()
        {
            SetMonth(0);
            return this;
        }

        public DailyRadiationQuery WithOrientation(double? inclination = null, double? azimuth = null)
        {
            SetPVOrientation(inclination, azimuth);
            return this;
        }

        public DailyRadiationQuery WithOutputs(bool? global = null, bool? global_2axis = null, bool? clearsky = null, bool? clearsky_2axis = null)
        {
            SetValue("global", global);
            SetValue("glob_2axis", global_2axis);
            SetValue("clearsky", clearsky);
            SetValue("clearsky_2axis", clearsky_2axis);
            return this;
        }

        public DailyRadiationQuery WithTemperature()
        {
            SetValue("showtemperatures", true);
            return this;
        }

        public DailyRadiationQuery WithLocalTime()
        {
            SetValue("localtime", true);
            return this;
        }

    }
}