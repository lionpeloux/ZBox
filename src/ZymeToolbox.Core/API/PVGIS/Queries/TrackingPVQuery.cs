using System;
using System.Collections.Generic;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public class TrackingPVQuery : Query
    {
        private TrackingPVQuery(List<string> paramNames) : base(paramNames) { }

        public static TrackingPVQuery Create(double latitude, double longitude, string outputformat = "json")
        {
            var paramNames = new List<string>()
            {
                "lat",
                "lon",
                "usehorizon",
                "userhorizon",
                "raddatabase",
                "pvtechchoice",
                "peakpower",
                "loss",
                "vertical_axis",
                "verticalaxisangle",
                "vertical_optimum",
                "inclined_axis",
                "inclinedaxisangle",
                "inclined_optimum",
                "twoaxis",
                "outputformat",
            };

            if (outputformat != "json" && outputformat != "basic" && outputformat != "csv")
                throw new ArgumentException($"Output format is {outputformat}. Must be 'json', 'basic' or 'csv'.");

            var query = new TrackingPVQuery(paramNames);
            query.SetValue("lat", latitude);
            query.SetValue("lon", longitude);
            query.SetValue("outputformat", outputformat);

            return query;
        }

        public override string Build() => Build("PVcalc");

        public TrackingPVQuery WithHorizon(bool? useHorizon, List<double>? userHorizon)
        {
            SetHorizon(useHorizon, userHorizon);
            return this;
        }

        public TrackingPVQuery WithRadiationDatabase(string? raddatabase)
        {
            SetRadiationDatabase(raddatabase);
            return this;
        }

        public TrackingPVQuery WithPVSystem(PVTechnologyType? pvtechchoice, double? peakpower, double? loss)
        {
            SetValue("pvtechchoice", pvtechchoice);
            SetValue("peakpower", peakpower);
            SetValue("loss", loss);
            return this;
        }

        public TrackingPVQuery WithPVMouting_VerticalAxis(double axisAngle, bool optimize = false)
        {
            SetValue("vertical_axis", 1);
            SetValue("vertical_optimum", optimize);
            SetValue("verticalaxisangle", axisAngle);
            return this;
        }
        public TrackingPVQuery WithPVMouting_InclinedAxis(double axisAngle, bool optimize = false)
        {
            SetValue("inclined_axis", 1);
            SetValue("inclined_optimum", optimize);
            SetValue("inclinedaxisangle", axisAngle);
            return this;
        }

        public TrackingPVQuery WithPVMouting_TwoAxis(bool enable = true)
        {
            SetValue("twoaxis", enable);
            return this;
        }
    }
}