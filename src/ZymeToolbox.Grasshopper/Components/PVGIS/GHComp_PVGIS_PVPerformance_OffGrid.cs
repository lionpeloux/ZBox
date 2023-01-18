using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_PVPerformance_OffGrid : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{B31EFB7E-DE2E-4AFE-8F3B-0F838D97A11C}");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public GHComp_PVGIS_PVPerformance_OffGrid()
          : base("PVGIS Performance of Off-Grid PV System", "PVGIS Perf Off-Grid PV", "Get the performance of an off-grid PV system from PVGIS API.",
            "ZBox", "2 | Climate API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);

            // Horizon
            pManager.AddBooleanParameter("Use Horizon", "useHorizon", "Calculation will take into account shadows from high horizon if set to true.", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("User Horizon", "userHorizon", "User specified horizon. Specify the height of the horizon at equidistant directions around the point of interest, in degrees", GH_ParamAccess.list);

            // PV
            pManager.AddNumberParameter("PV Peak Power", "pvPeakPower", "PV peak power installed in Wp.", GH_ParamAccess.item, 50.0);
            pManager.AddNumberParameter("PV Battery Capacity", "pvBatteryCapacity", "PV battery capacity in Wh.", GH_ParamAccess.item, 400.0);
            pManager.AddNumberParameter("PV Battery Cutoff", "pvBatteryCutoff", "PV battery discharge cutoff limit in %.", GH_ParamAccess.item, 40.0);

            // Consumption
            pManager.AddNumberParameter("PV Daily Consumption", "pvConsumption", "Either a single daily value or a list of 24 hourly values for the electrical consumption in Wh.", GH_ParamAccess.list, new List<double>() { 300 });

            // POA
            pManager.AddNumberParameter("POA Inclination", "poaInclination", "Inclination angle of the POA from the horizontal plane in degree (0° = hozirontal | 90° = vertical). Not relevant for 2-axis tracking POA", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("POA Azimuth", "poaAzimuth", "Azimuth angle of the fixed POA in degree (0° = South, 90° = West, -90° = East). Not relevant for POA with tracking.", GH_ParamAccess.item, 0);

            // Trigger
            pManager.AddBooleanParameter("Run", "run", "Sends the HTTP request when set to true.", GH_ParamAccess.item);

            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Url Query", "urlQuery", "HTTP resquest url send to the REST API.", GH_ParamAccess.item);
            pManager.AddTextParameter("Meta", "json_meta", "'meta' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddTextParameter("Inputs", "json_inputs", "'inputs' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elevation", "elevation", "Elevation (meter).", GH_ParamAccess.item);

            // Monthy Results
            pManager.AddIntegerParameter("Month", "month", "Number of the month (1-12).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Average Daily Energy Production", "E_daily", "Average daily energy production from the given system in kWh/day.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Average Dailty Energy not Captured", "E_lost", "Average daily energy not captured from the given system in kWh/day.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Full-Battery Days Ratio", "batteryFullDaysRatio", "Percentage of days when the battery became full in %.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Empty-Battery Days Ratio", "batteryEmptyDaysRatio", "Percentage of days when the battery became empty in %.", GH_ParamAccess.list);

            // Histogram
            pManager.AddVectorParameter("Probability of Daily Battery Charge State", "batteryChargeHisto", "Probability of battery charge state at the end of the day in %. Each value is a vector where [X,Y] gives the charge state interval and Z the corresponding percentage of days. The sum of all Z values should be 100%.", GH_ParamAccess.list);

            // Totals
            pManager.AddNumberParameter("Number of Days", "_total_NumOfDays", "Number of days used for the calculation.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Full-Battery Days Ratio", "_total_BatFull", "Percentage of days when the battery became full in %.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Empty-Battery Days Ratio", "_total_BatEmpty", "Percentage of days when the battery became empty in %.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Daily Energy not Captured", "_total_Elost", "Average energy not captured per day in Wh/d.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Daily Energy Missing", "_total_Emiss", "Average energy missing per day in Wh/d.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            if (!DA.GetData("Run", ref run)) return;
            if (!run) return;

            double latitude = 0;
            double longitude = 0;
            bool useHorizon = true;
            var userHorizon = new List<double>();
            double pvPeakPower = 0;
            double pvBatteryCapacity = 0;
            double pvBatteryCutoff = 0;
            var pvConsumption = new List<double>();
            double inclination = 0;
            double azimut = 0;

            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = OffGridPVQuery.Create(latitude, longitude, "json");

            // HORIZON
            DA.GetData(2, ref useHorizon);
            DA.GetDataList(3, userHorizon);
            query.WithHorizon(useHorizon, userHorizon);

            // PV
            DA.GetData(4, ref pvPeakPower);
            DA.GetData(5, ref pvBatteryCapacity);
            DA.GetData(6, ref pvBatteryCutoff);
            query.WithPVSystem(pvPeakPower, pvBatteryCapacity, pvBatteryCutoff);

            // CONSUMPTION
            DA.GetDataList(7, pvConsumption);
            query.WithConsumption(pvConsumption);

            // POA
            DA.GetData(8, ref inclination);
            DA.GetData(9, ref azimut);
            query.WithPVMouting(inclination, azimut);

            var perf = Services.GetPVPerformance_OffGrid(query);

            DA.SetData(0, perf.UrlQuery);
            DA.SetData(1, perf.Meta.ToString(true));
            DA.SetData(2, perf.Inputs.ToString(true));

            DA.SetData(3, perf.Location.Latitude);
            DA.SetData(4, perf.Location.Longitude);
            DA.SetData(5, perf.Location.Elevation);

            DA.SetDataList(6, perf.Month);
            DA.SetDataList(7, perf.AverageDailyEnergyProduction);
            DA.SetDataList(8, perf.AverageDailyEnergyProductionLost);
            DA.SetDataList(9, perf.FullBatteryRatio);
            DA.SetDataList(10, perf.EmptyBatteryRatio);

            var histo = new List<Vector3d>();
            int index = 0;
            foreach (var z in perf.Histo_ChargeState_Percentil)
            {
                var x = perf.Histo_ChargeState_Min[index];
                var y = perf.Histo_ChargeState_Max[index];
                histo.Add(new Vector3d(x, y, z));
                index++;
            }
            DA.SetDataList(11, histo);

            DA.SetData(12, perf.Totals.NumberOfDays);
            DA.SetData(13, perf.Totals.FullBatteryDaysRatio);
            DA.SetData(14, perf.Totals.EmptyBatteryDaysRatio);
            DA.SetData(15, perf.Totals.AverageDailyEnergyLost);
            DA.SetData(16, perf.Totals.AverageDailyMissingEnergy);



        }
    }
}
