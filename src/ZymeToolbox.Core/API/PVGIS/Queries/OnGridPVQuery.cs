using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class OnGridPVQuery : Query
    {
        private OnGridPVQuery(List<string> paramNames) : base(paramNames) { }

        public static OnGridPVQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "usehorizon",
                "userhorizon",
                "raddatabase",
                "peakpower",
                "pvtechchoice",
                "mountingplace",
                "loss",
                "fixed",
                "angle",
                "aspect",
                "optimalinclination",
                "optimalangles",
                "inclined_axis",
                "inclined_optimum",
                "inclinedaxisangle",
                "vertical_axis",
                "vertical_optimum",
                "verticalaxisangle",
                "twoaxis",
                "pvprice",
                "systemcost",
                "interest",
                "lifetime",
                "outputformat"
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new OnGridPVQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("PVcalc");

        public OnGridPVQuery WithHorizon(bool useHorizon, List<double> userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public OnGridPVQuery WithRadiationDatabase(string raddatabase)
        {
            SetRadiationDatabase(raddatabase);
            return this;
        }

        public OnGridPVQuery WithPVMouting(double? inclination = null, double? azimuth = null,
           bool? optimizeInclination = null,
           bool? optimizeInclinationAndAzimuth = null)
        {
            SetValue("angle", inclination);
            SetValue("aspect", azimuth);
            SetValue("optimalinclination", optimizeInclination);
            SetValue("optimalangles", optimizeInclinationAndAzimuth);
            return this;
        }

        public OnGridPVQuery WithPVSystem(double peakpower = 1.0, double loss = 14.0,
            PVMountingType mountingplace = PVMountingType.Free,
            PVTechnologyType pvtechchoice = PVTechnologyType.crystSi)
        {
            SetPVSystem(peakpower, loss, mountingplace, pvtechchoice);
            return this;
        }

        public OnGridPVQuery WithPVCostModel(double systemcost, double interest = 14.0, int lifetime = 25)
        {
            SetValue("systemcost", systemcost);
            SetValue("interest", interest);
            SetValue("lifetime", lifetime);
            return this;
        }
    }
}