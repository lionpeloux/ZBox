using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_HourlyRadiation : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{236D48C8-2BF0-44DC-A52A-66122EF29A94}");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public GHComp_PVGIS_HourlyRadiation()
          : base("PVGIS Hourly Radiation", "PVGIS Hourly Radiation", "Get the hourly radiation results from PVGIS API.",
            "ZBox", "2 | Climate API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);

            // Year
            pManager.AddIntegerParameter("Start Year", "startYear", "Start year of the serie. If not provided will default to first available year.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("End Year", "endYear", "End year of the serie. If not provided will default to last available year.", GH_ParamAccess.item);

            // Horizon
            pManager.AddBooleanParameter("Use Horizon", "useHorizon", "Calculation will take into account shadows from high horizon if set to true.", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("User Horizon", "userHorizon", "User specified horizon. Specify the height of the horizon at equidistant directions around the point of interest, in degrees", GH_ParamAccess.list);

            // POA
            pManager.AddIntegerParameter("POA Tracking", "poaTracking", "Type of suntracking : 0=fixed | 1=single horizontal axis aligned north-south | 2=two-axis tracking | 3=vertical axis tracking | 4=single horizontal axis aligned east-west | 5=single inclined axis aligned north-south.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("POA Inclination", "poaInclination", "Inclination angle of the POA from the horizontal plane in degree (0° = hozirontal | 90° = vertical). Not relevant for 2-axis tracking POA", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("POA Azimuth", "poaAzimuth", "Azimuth angle of the fixed POA in degree (0° = South, 90° = West, -90° = East). Not relevant for POA with tracking.", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("POA Optimize Inclination", "poaOptx1", "Compute the optimum inclination angle.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("POA Optimize Inclination and Azimiuth", "poaOptx2", "Compute the optimum inclination AND azimuth angles.", GH_ParamAccess.item, false);

            // PV
            pManager.AddIntegerParameter("PV Mounting Type", "pvMounting", "Type of PV Mounting : 0=free | 1=building.", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("PV Technology", "pvTechnology", "Type of PV Technology : 0=CrystSi | 1=CIS | 2=CdTe | 3=Unknown.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("PV Peak Power", "pvPeakPower", "PV peak power in kW.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("PV Loss", "pvLoss", "Sum of system losses in %.", GH_ParamAccess.item, 14);

            pManager.AddBooleanParameter("Run", "run", "Sends the HTTP request when set to true.", GH_ParamAccess.item);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Url Query", "urlQuery", "HTTP resquest url send to the REST API.", GH_ParamAccess.item);
            pManager.AddTextParameter("Meta", "json_meta", "'meta' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddTextParameter("Inputs", "json_inputs", "'inputs' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elevation", "elevation", "Elevation (meter).", GH_ParamAccess.item);

            pManager.AddTextParameter("Time", "time", "Hourly time stamps.", GH_ParamAccess.list);

            pManager.AddNumberParameter("PV Power", "pvPower", "PV system power in W. If requested.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Direct Irradiance", "radDirect", "Direct (beam) solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Diffuse Irradiance", "radDiffuse", "Diffuse solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Reflected Irradiance", "radReflected", "Reflected (ground) solar irradiance on the (inclined) plane of array in W/m2.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Sun Heigh", "sunHeight", "Sun height in degree.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Temperature at 2m", "T2m", "Temperature at 2 meters in °C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Wind Speed at 10m", "WS10m", "Wind speed at 10 meters in m/s.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Is Reconstructer", "reconstructed", "Indicates whether the solar radiation values are reconstructed or not.", GH_ParamAccess.list);
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
            int tracking = 0;
            double inclination = 0;
            double azimut = 0;
            bool poaOptx1 = false;
            bool poaOptx2 = false;
            int pvMounting = 0;
            int pvTechnology = 0;
            double pvPeakPower = 0;
            double pvLoss = 0;

            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = HourlyRadiationQuery.Create(latitude, longitude, true, "json");

            if (!DA.GetData(2, ref startYear)) startYear = null;
            if (!DA.GetData(3, ref endYear)) endYear = null;
            query.WithYears(startYear, endYear);

            // HORIZON
            DA.GetData(4, ref useHorizon);
            DA.GetDataList(5, userHorizon);
            query.WithHorizon(useHorizon, userHorizon);

            // POA
            DA.GetData(6, ref tracking);
            DA.GetData(7, ref inclination);
            DA.GetData(8, ref azimut);
            DA.GetData(9, ref poaOptx1);
            DA.GetData(10, ref poaOptx2);
            query.WithPVMouting((PVTrackingType)tracking, inclination, azimut, poaOptx1, poaOptx2);

            // PV
            DA.GetData(11, ref pvMounting);
            DA.GetData(12, ref pvTechnology);
            DA.GetData(13, ref pvPeakPower);
            DA.GetData(14, ref pvLoss);
            query.WithPVSystem(pvPeakPower, pvLoss, (PVMountingType)pvMounting, (PVTechnologyType)pvTechnology);

            var hr = Services.GetHourlyRadiation(query);

            DA.SetData(0, hr.UrlQuery);
            DA.SetData(1, hr.Meta.ToString(true));
            DA.SetData(2, hr.Inputs.ToString(true));

            DA.SetData(3, hr.Location.Latitude);
            DA.SetData(4, hr.Location.Longitude);
            DA.SetData(5, hr.Location.Elevation);
            DA.SetDataList(6, hr.Time);
            DA.SetDataList(7, hr.PVPower);
            DA.SetDataList(8, hr.DirectIrradiance_POA);
            DA.SetDataList(9, hr.DiffuseIrradiance_POA);
            DA.SetDataList(10, hr.ReflectedIrradiance_POA);
            DA.SetDataList(11, hr.SunHeight);
            DA.SetDataList(12, hr.Temperature_2m);
            DA.SetDataList(13, hr.WindSpeed_10m);
            DA.SetDataList(14, hr.IsReconstructed);
        }




    }
}
