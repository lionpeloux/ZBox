using System;
using System.Collections.Generic;
using System.Linq;

namespace ZymeToolbox.Core.API.PVGIS.Queries
{
    public abstract class Query
    {
        private Dictionary<string, string> Params;
        private List<string> Index;

        protected Query(List<string> paramNames)
        {
            Params = new Dictionary<string, string>();
            Index = new List<string>();

            foreach (var paramName in paramNames)
            {
                Params.Add(paramName, "");
                Index.Add(paramName);
            }
        }

        protected void SetValue<T>(string paramName, T value)
        {
            if (value == null) return;
            if (!Params.ContainsKey(paramName)) throw new ArgumentException($"{paramName} is not a valid query parameter");
            
            switch (value)
            {
                case bool :
                    Params[paramName] = Convert.ToInt32(value).ToString();
                    break;
                default:
                    Params[paramName] = Convert.ToString(value);
                    break;
            }
        }

        protected string Build(string APIService)
        {
            var kvp = new List<string>();
            foreach (var key in Index)
            {
                string val;
                if (Params.TryGetValue(key, out val))
                    if (val != "")
                        kvp.Add(string.Format("{0}={1}", key, val));
            }

            var query = string.Join("&", kvp);
            var urlQuery = string.Format("{0}{1}?{2}", Services.APIEntryPoint, APIService, query);
            return urlQuery;
        }

        public abstract string Build();

        protected void SetHorizon(bool? useHorizon, List<double> userHorizon)
        {
            if (useHorizon != null)
                SetValue("usehorizon", useHorizon);

            if (userHorizon != null)
                SetValue("userhorizon", string.Join(",", userHorizon.Select(h => Math.Round(h, 2))));
            
        }

        protected void SetRadiationDatabase(string radDatabaseName)
        {
            SetValue("raddatabase", radDatabaseName);
        }

        protected void SetYears(int? startYear, int? endYear)
        {
            SetValue("startyear", startYear);
            SetValue("endyear", endYear);
        }

        protected void SetMonth(int? month)
        {
            SetValue("month", month);
        }

        protected void SetPVOrientation(double? inclination = null, double? azimuth = null)
        {
            SetValue("angle", inclination);
            SetValue("aspect", azimuth);
        }

        protected void SetPVMouting(PVTrackingType? tracking = PVTrackingType.Fixed,
            double? inclination = null,
            double? azimuth = null,
            bool? optimizeInclination = null,
            bool? optimizeInclinationAndAzimuth = null)
        {
            SetValue("trackingtype", (int)tracking);
            SetValue("angle", inclination);
            SetValue("aspect", azimuth);
            SetValue("optimalinclination", optimizeInclination);
            SetValue("optimalangles", optimizeInclinationAndAzimuth);

        }

        protected void SetPVSystem(double peakpower, double loss,
            PVMountingType mountingplace = PVMountingType.Free,
            PVTechnologyType pvtechchoice = PVTechnologyType.crystSi)
        {
            SetValue("peakpower", peakpower);
            SetValue("loss", loss);
            SetValue("pvtechchoice", Convert.ToString(pvtechchoice));
            SetValue("mountingplace", Convert.ToString(mountingplace).ToLower());
        }
    }
}
