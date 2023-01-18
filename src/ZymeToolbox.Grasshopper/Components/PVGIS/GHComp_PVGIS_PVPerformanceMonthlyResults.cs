using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using ZymeToolbox.Core.API.PVGIS.Responses;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_PVPerformanceMonthlyResults : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{96002C6E-CDD9-4FD8-A54B-F6AA5275C8AD}");
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        public GHComp_PVGIS_PVPerformanceMonthlyResults()
          : base("PVGIS Get PV Performance Monthly Results", "PVGIS Perf Monthly Results", "Get the monthly performance results of a grid-connected or tracking PV system from PVGIS API.",
            "ZBox", "2 | Climate API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Performance", "performance", "A performance result object to browse.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // Monthy Results
            pManager.AddTextParameter("Label", "label", "", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Month", "month", "Number of the month (1-12).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Average Daily Energy Production", "E_daily", "Average daily energy production from the given system in kWh/day.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Average Monthly Energy Production", "E_montly", "Average monthly energy production from the given system in kWh/month.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Average Daily Sum of Global Irradiance", "G_daily", "Average daily sum of global irradiance per square meter received by the modules of the given system in kWh/m2/day.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Average Monthly Sum of Global Irradiance", "G_montly", "Average monthly sum of global irradiance per square meter received by the modules of the given system in kWh/m2/month.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Deviation of Monthly Energy Production", "SD", "Standard deviation of the monthly energy production due to year-to-year variation in kWh.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var obj = new GH_ObjectWrapper();
            DA.GetData(0, ref obj);

            if (obj == null) return;

            var perf = obj.Value as PVPerformance;
            if (perf == null) return;

            DA.SetData(0, perf.Label);
            DA.SetDataList(1, perf.Month);
            DA.SetDataList(2, perf.AverageDailyEnergyProduction);
            DA.SetDataList(3, perf.AverageMonthlyEnergyProduction);
            DA.SetDataList(4, perf.AverageDailySumOfGlobalIrradiance_POA);
            DA.SetDataList(5, perf.AverageMonthlySumOfGlobalIrradiance_POA);
            DA.SetDataList(6, perf.DeviationOfMonthlyEnergyProduction);
        }
    }
}
