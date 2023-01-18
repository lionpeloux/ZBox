using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class HourlyRadiationQuery : Query
    {
        private HourlyRadiationQuery(List<string> paramNames) : base(paramNames) { }

        public static HourlyRadiationQuery Create(double latitude, double longitude, bool components = true, string outputformat = "json")
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
                "pvcalculation",
                "peakpower",
                "pvtechchoice",
                "mountingplace",
                "loss",
                "trackingtype",
                "angle",
                "aspect",
                "optimalinclination",
                "optimalangles",
                "components",
                "outputformat"
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new HourlyRadiationQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("components", Convert.ToInt32(components));
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("seriescalc");

        public HourlyRadiationQuery WithHorizon(bool? useHorizon, List<double>? userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public HourlyRadiationQuery WithRadiationDatabase(string raddatabase)
        {
            SetRadiationDatabase(raddatabase);
            return this;
        }

        public HourlyRadiationQuery WithYears(int? startYear, int? endYear)
        {
            SetYears(startYear, endYear);
            return this;
        }

        public HourlyRadiationQuery WithPVMouting(
            PVTrackingType tracking,
            double? inclination,
            double? azimuth,
            bool? optimizeInclination,
            bool? optimizeInclinationAndAzimuth)
        {
            SetPVMouting(tracking, inclination, azimuth, optimizeInclination, optimizeInclinationAndAzimuth);
            return this;
        }

        public HourlyRadiationQuery WithPVSystem(
            double peakpower, 
            double loss,
            PVMountingType mountingplace = PVMountingType.Free,
            PVTechnologyType pvtechchoice = PVTechnologyType.crystSi)
        {
            SetValue("pvcalculation", 1);
            SetPVSystem(peakpower, loss, mountingplace, pvtechchoice);
            return this;
        }
    }
}