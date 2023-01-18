using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class HorizonProfileQuery : Query
    {
        private HorizonProfileQuery(List<string> paramNames) : base(paramNames) { }

        public static HorizonProfileQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "userhorizon",
                "outputformat"
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new HorizonProfileQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("printhorizon");

        public HorizonProfileQuery WithUserHorizon(List<double> userHorizon)
        {
            SetHorizon(true, userHorizon);
            return this;
        }
    }
}
