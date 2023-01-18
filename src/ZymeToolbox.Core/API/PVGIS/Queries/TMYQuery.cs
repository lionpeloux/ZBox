using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class TMYQuery : Query
    {
        private TMYQuery(List<string> paramNames) : base(paramNames) { }

        public static TMYQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "usehorizon",
                "userhorizon",
                "startyear",
                "endyear",
                "outputformat"
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv" && outputformat != "epw")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic', 'csv' or 'epw'.");

            var query = new TMYQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("tmy");

        public TMYQuery WithHorizon(bool? useHorizon, List<double>? userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public TMYQuery WithYears(int? startYear, int? endYear)
        {
            SetYears(startYear, endYear);
            return this;
        }
    }
}