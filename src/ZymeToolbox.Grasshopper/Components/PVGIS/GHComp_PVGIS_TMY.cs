using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_TMY : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{96F57C1E-0CCA-473D-9060-874BFB1981E7}");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public GHComp_PVGIS_TMY()
          : base("PVGIS Typical Meteorological Year", "PVGIS TMY", "Get the Typical Meteorological Year (TMY) from PVGIS API.",
            "ZBox", "2 | Climate API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);

            // YEAR
            pManager.AddIntegerParameter("Start Year", "startYear", "Start year of the serie. If not provided will default to first available year.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("End Year", "endYear", "End year of the serie. If not provided will default to last available year.", GH_ParamAccess.item);

            // Horizon
            pManager.AddBooleanParameter("Use Horizon", "useHorizon", "Calculation will take into account shadows from high horizon if set to true.", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("User Horizon", "userHorizon", "User specified horizon. Specify the height of the horizon at equidistant directions around the point of interest, in degrees", GH_ParamAccess.list);

            // RUN / SAVE
            pManager.AddTextParameter("File Path", "path", "Path to either a folder or a .epw file (to specify the name) to persiste the TMY data.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "run", "Sends the HTTP request when set to true.", GH_ParamAccess.item, false);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Url Query", "urlQuery", "HTTP resquest url send to the REST API.", GH_ParamAccess.item);
            pManager.AddTextParameter("Meta", "json_meta", "'meta' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddTextParameter("Inputs", "json_inputs", "'inputs' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elevation", "elevation", "Elevation (meter).", GH_ParamAccess.item);

            pManager.AddTextParameter("Time UTC", "time", "Hourly time stamps in UTC.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Global Irradiance", "radGlobal", "Global solar irradiance on the horizontal plane in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Direct Irradiance", "radDirect", "Direct (beam) solar irradiance (normal to sun) in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Diffuse Irradiance", "radDiffuse", "Diffuse solar irradiance on the horizontal plane in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Infrared Irradiance", "radInfrared", "Surface infrared (thermal) irradiance on the horizontal plane in W/m2.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Temperature at 2m", "T2m", "Temperature of air at 2 meters in °C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Relative Humidity", "RH", "Relative humidity of air in %.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Air Pressure", "P", "Surface air pressure in Pa.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Wind Speed at 10m", "WS10m", "Wind speed at 10 meters in m/s.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Wind Direction at 10m", "WD10m", "Wind direction at 10 meters in degree (0° = North, 90° = Est, 180° = South, 270° = West).", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            if (!DA.GetData("Run", ref run)) return;
            if (!run) return;

            double latitude = 0;
            double longitude = 0;
            int? startYear = null;
            int? endYear = null;
            bool useHorizon = true;
            var userHorizon = new List<double>();
            string path = "";

            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = TMYQuery.Create(latitude, longitude, "json");

            if (!DA.GetData(2, ref startYear)) startYear = null;
            if (!DA.GetData(3, ref endYear)) endYear = null;
            query.WithYears(startYear, endYear);

            // HORIZON
            DA.GetData(4, ref useHorizon);
            DA.GetDataList(5, userHorizon);
            //if (!DA.GetDataList(5, userHorizon)) query.WithHorizon(useHorizon, null);
            query.WithHorizon(useHorizon, userHorizon);

            // RETRIEVE TMY
            var response = Services.GetTMY(query);

            // WRITE EPW
            if (DA.GetData(6, ref path))
            {
                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(path);
                string filName = "";

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    //this is a directory
                    var md = response.GetMeteoData();
                    var filename = string.Format("tmy_{0:F3}_{1:F3}_{2}_{3}.epw", latitude, longitude, md.YearMin, md.YearMax);
                    path = Path.Combine(path, filename);
                    Services.WriteEPW(path, latitude, longitude, startYear, endYear, useHorizon, null);
                }
                else
                {
                    // this is a file
                    if (Path.GetExtension(path) != "epw")
                        throw new ArgumentException("File must have '*.epw' extension.");
                    Services.WriteEPW(path, latitude, longitude, startYear, endYear, useHorizon, null);
                }


            }




            DA.SetData(0, response.UrlQuery);
            DA.SetData(1, response.Meta.ToString(true));
            DA.SetData(2, response.Inputs.ToString(true));

            DA.SetData(3, response.Location.Latitude);
            DA.SetData(4, response.Location.Longitude);
            DA.SetData(5, response.Location.Elevation);
            DA.SetDataList(6, response.Time);
            DA.SetDataList(7, response.GlobalIrradiance);
            DA.SetDataList(8, response.DirectIrradiance);
            DA.SetDataList(9, response.DiffuseIrradiance);
            DA.SetDataList(10, response.InfraredIrradiance);
            DA.SetDataList(11, response.Temperature_2m);
            DA.SetDataList(12, response.RelativeHumidity);
            DA.SetDataList(13, response.AirPressure);
            DA.SetDataList(14, response.WindSpeed_10m);
            DA.SetDataList(15, response.WindDirection_10m);
        }


    }
}