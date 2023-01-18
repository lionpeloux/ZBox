using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Climat.Grasshopper.Components
{
    public class GHComp_PVGIS_DailyRadiation : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{61EF9F25-07D2-4FFB-811D-24FEC71A4E18}");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public GHComp_PVGIS_DailyRadiation()
          : base("PVGIS Daily Radiation", "PVGIS Daily", "Get the daily radiation results from PVGIS API.",
            "ZBox", "2 | Climat API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            
            // MONTH
            pManager.AddIntegerParameter("Month", "month", "Number of the month. If 0 all 12 months will be returned.", GH_ParamAccess.item, 0);

            // Horizon
            pManager.AddBooleanParameter("Use Horizon", "useHorizon", "Calculation will take into account shadows from high horizon if set to true.", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("User Horizon", "userHorizon", "User specified horizon. Specify the height of the horizon at equidistant directions around the point of interest, in degrees", GH_ParamAccess.list);

            // POA
            pManager.AddNumberParameter("POA Inclination", "poaInclination", "Inclination angle of the POA from the horizontal plane in degree (0° = hozirontal | 90° = vertical). Not relevant for 2-axis tracking POA", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("POA Azimuth", "poaAzimuth", "Azimuth angle of the fixed POA in degree (0° = South, 90° = West, -90° = East). Not relevant for POA with tracking.", GH_ParamAccess.item, 0);
            
            // OUTPUTS
            pManager.AddBooleanParameter("Time Unit", "localTime", "If true, output the time in the local time zone (not daylight saving time) instead of UTC.", GH_ParamAccess.item, false);

            pManager.AddBooleanParameter("Run", "run", "Sends the HTTP request when set to true.", GH_ParamAccess.item);

            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Url Query", "urlQuery", "http request url.", GH_ParamAccess.item);
            pManager.AddTextParameter("Meta", "json_meta", "'meta' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddTextParameter("Inputs", "json_inputs", "'inputs' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elevation", "elevation", "Elevation (meter).", GH_ParamAccess.item);

            pManager.AddNumberParameter("Month", "month", "Number of the month (1-12).", GH_ParamAccess.list);
            pManager.AddTextParameter("Time", "time", "Hourly time stamps in the format hh:mm. Could be UTC or Local.", GH_ParamAccess.list);

            // POA
            pManager.AddNumberParameter("Global Clear-Sky Irradiance for POA", "radGlobalCS_POA", "Global clear-sky solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Global Irradiance for POA", "radGlobal_POA", "Global solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Direct Irradiance for POA", "radDirect_POA", "Direct (beam) solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Diffuse Irradiance for POA", "radDiffuse_POA", "Diffuse solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);

            // 2-AXIS
            pManager.AddNumberParameter("Global Clear-Sky Irradiance for 2-Axis", "radGlobalCS_2Axis", "Global clear-sky solar irradiance on the 2-axis tracking plane in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Global Irradiance for 2-Axis", "radGlobal_2Axis", "Global solar irradiance on the 2-axis tracking plane in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Direct Irradiance for 2-Axis", "radDirect_2Axis", "Direct (beam) solar irradiance on the 2-axis tracking plane in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Diffuse Irradiance for 2-Axis", "radDiffuse_2Axis", "Diffuse solar irradiance on the 2-axis tracking plane in W/m2.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Temperature at 2m", "T2m", "Temperature at 2 meters in °C.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            if (!DA.GetData("Run", ref run)) return;
            if (!run) return;

            double latitude = 0;
            double longitude = 0;
            int month = 0;
            bool useHorizon = true;
            var userHorizon = new List<double>();
            double inclination = 0;
            double azimut = 0;
            bool localTime = false;

            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = DailyRadiationQuery.Create(latitude, longitude, "json");

            DA.GetData(2, ref month);
            query.WithMonth(month);

            // HORIZON
            DA.GetData(3, ref useHorizon);
            DA.GetDataList(4, userHorizon);
            query.WithHorizon(useHorizon, userHorizon);

            // POA
            DA.GetData(5, ref inclination);
            DA.GetData(6, ref azimut);
            query.WithOrientation(inclination, azimut);

            // OUTPUTS
            query.WithOutputs(true, true, true, true);
            query.WithTemperature();

            DA.GetData(7, ref localTime);
            if (localTime) query.WithLocalTime();
            
            var dr = Services.GetDailyRadiation(query);

            DA.SetData(0, dr.UrlQuery);
            DA.SetData(1, dr.Meta.ToString(true));
            DA.SetData(2, dr.Inputs.ToString(true));

            DA.SetData(3, dr.Location.Latitude);
            DA.SetData(4, dr.Location.Longitude);
            DA.SetData(5, dr.Location.Elevation);
            
            DA.SetDataList(6, dr.Month);
            DA.SetDataList(7, dr.Time);

            DA.SetDataList(8, dr.GlobalClearSkyIrradiance_POA);
            DA.SetDataList(9, dr.GlobalIrradiance_POA);
            DA.SetDataList(10, dr.DirectIrradiance_POA);
            DA.SetDataList(11, dr.DiffuseIrradiance_POA);
            
            DA.SetDataList(12, dr.GlobalClearSkyIrradiance_2Axis);
            DA.SetDataList(13, dr.GlobalIrradiance_2Axis);
            DA.SetDataList(14, dr.DirectIrradiance_2Axis);
            DA.SetDataList(15, dr.DiffuseIrradiance_2Axis);

            DA.SetDataList(16, dr.Temperature_2m);
        }


    }
}
