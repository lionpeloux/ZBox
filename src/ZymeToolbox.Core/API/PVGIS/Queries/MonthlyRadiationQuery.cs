using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class MonthlyRadiationQuery : Query
    {
        private MonthlyRadiationQuery(List<string> paramNames) : base(paramNames) { }

        public static MonthlyRadiationQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "usehorizon",
                "userhorizon",
                "raddatabase",
                "startyear",
                "endyear",
                "horirrad",
                "optrad",
                "selectrad",
                "angle",
                "mr_dni",
                "d2g",
                "avtemp",
                "outputformat"
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new MonthlyRadiationQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("MRcalc");

        public MonthlyRadiationQuery WithHorizon(bool useHorizon, List<double> userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public MonthlyRadiationQuery WithRadiationDatabase(string raddatabase)
        {
            SetRadiationDatabase(raddatabase);
            return this;
        }

        public MonthlyRadiationQuery WithYears(int? startYear = null, int? endYear = null)
        {
            SetYears(startYear, endYear);
            return this;
        }

        public MonthlyRadiationQuery WithIrradiationOutputs(bool? globalHorizontalRad = null, bool? directNormalRad = null, bool ? globalRadAtOptimalAngle = null, double? inclination = 0)
        {
            SetValue("horirrad", globalHorizontalRad);
            SetValue("mr_dni", directNormalRad);
            SetValue("optrad", globalRadAtOptimalAngle);

            if (inclination != null)
            {
                SetValue("selectrad", true);
                SetValue("angle", inclination);
            }
            return this;
        }

        public MonthlyRadiationQuery WithDiffuseToGlobalRatio()
        {
            SetValue("d2g", true);
            return this;
        }

        public MonthlyRadiationQuery WithAverageTemperature()
        {
            SetValue("avtemp", true);
            return this;
        }

    }
}