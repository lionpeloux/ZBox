using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_MonthlyRadiation : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{50F1BD83-D437-4ED1-801B-ADC725DD9054}");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public GHComp_PVGIS_MonthlyRadiation()
          : base("PVGIS Monthly Radiation", "PVGIS Monthly Radiation", "Get the monthly radiation results from PVGIS API.",
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
            pManager.AddNumberParameter("POA Inclination", "poaInclination", "Inclination angle of the POA from the horizontal plane in degree (0° = hozirontal | 90° = vertical). Not relevant for 2-axis tracking POA", GH_ParamAccess.item, 0);

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

            pManager.AddNumberParameter("Year", "year", "Year.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Month", "month", "Number of the month (1-12).", GH_ParamAccess.list);

            // POA
            pManager.AddNumberParameter("Global Irradiance for Horizontal Plane", "radGlobal_Hori", "Monthly global solar irradiance on the horizontal plane in kWh/m2/month.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Global Irradiance for Optimal Angle", "radGlobal_Opti", "Monthly global solar irradiance on the optimaly oriented plane in kWh/m2/month.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Global Irradiance for POA", "radGlobal_POA", "Monthly global solar irradiance on the (inclined) plane of array in kWh/m2/month.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Direct Irradiance for Normal Plane", "radDirect_Norm", "Monthly direct (beam) solar irradiance on the plane always normal to sun rays in kWh/m2/month.", GH_ParamAccess.list);

            pManager.AddNumberParameter("Diffuse to Global Irradiance Ratio", "D/G", "Ratio of diffuse to global irradiance.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Avertage Temperature at 2m", "T2m_avg", "24 hours average of air temperature at 2 meters in °C.", GH_ParamAccess.list);
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
            double inclination = 0;

            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = MonthlyRadiationQuery.Create(latitude, longitude, "json");

            // YEARS
            if (!DA.GetData(2, ref startYear)) startYear = null;
            if (!DA.GetData(3, ref endYear)) endYear = null;
            query.WithYears(startYear, endYear);

            // HORIZON
            DA.GetData(4, ref useHorizon);
            DA.GetDataList(5, userHorizon);
            query.WithHorizon(useHorizon, userHorizon);

            // POA
            DA.GetData(6, ref inclination);
            query.WithIrradiationOutputs(true, true, true, inclination);

            // OUTPUTS
            query.WithDiffuseToGlobalRatio();
            query.WithAverageTemperature();

            var mr = Services.GetMonthlyRadiation(query);

            DA.SetData(0, mr.UrlQuery);
            DA.SetData(1, mr.Meta.ToString(true));
            DA.SetData(2, mr.Inputs.ToString(true));

            DA.SetData(3, mr.Location.Latitude);
            DA.SetData(4, mr.Location.Longitude);
            DA.SetData(5, mr.Location.Elevation);

            DA.SetDataList(6, mr.Year);
            DA.SetDataList(7, mr.Month);

            DA.SetDataList(8, mr.GlobalIrradiance_Horizontal);
            DA.SetDataList(9, mr.GlobalIrradiance_Optimal);
            DA.SetDataList(10, mr.GlobalIrradiance_POA);
            DA.SetDataList(11, mr.DirectIrradiance_Normal);

            DA.SetDataList(12, mr.DiffuseToGlobalIrradianceRatio);
            DA.SetDataList(13, mr.AverageTemperature_2m);
        }


    }
}
