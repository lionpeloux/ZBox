using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;
using ZymeToolbox.Core.API.PVGIS.Responses;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_PVPerformanceTotalResults : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{F5E79879-C801-4750-832E-279510BFBB96}");
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        public GHComp_PVGIS_PVPerformanceTotalResults()
          : base("PVGIS Get PV Performance Total Results", "PVGIS Perf Total Results", "Get the total performance results of a grid-connected or tracking PV system from PVGIS API.",
            "ZBox", "2 | Climate API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Performance", "performance", "A performance result object to browse.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Label", "label", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Daily Energy Production", "Ed", "Average daily energy production from the given system in kWh/day.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Monthly Energy Production", "Em", "Average monthly energy production from the given system in kWh/month.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Yearly Energy Production", "Ey", "Average yearly energy production from the given system in kWh/year.", GH_ParamAccess.item);

            pManager.AddNumberParameter("Average Daily Sum of Global Irradiance", "Gd", "Average daily sum of global irradiation per square meter received by the modules of the given system in kWh/m2/day.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Monthly Sum of Global Irradiance", "Gm", "Average monthly sum of global irradiation per square meter received by the modules of the given system in kWh/m2/month.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Average Yearly Sum of Global Irradiance", "Gy", "Average yearly sum of global irradiation per square meter received by the modules of the given system in kWh/m2/year.", GH_ParamAccess.item);

            pManager.AddNumberParameter("Deviation of Monthly Energy Production", "SDm", "Standard deviation of the monthly energy production due to year-to-year variation in kWh.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Deviation of Yearly Energy Production", "SDy", "Standard deviation of the yearly energy production due to year-to-year variation in kWh.", GH_ParamAccess.item);

            pManager.AddNumberParameter("Angle of Incidence Loss", "lossAOI", "Angle of incidence loss in %.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spectral Loss", "lossSPEC", "Spectral loss in %.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Temperature and Irradiance Loss", "lossTRAD", "Temperature and irradiance loss in %.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Total Loss", "lossTotal", "Total loss in %.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var obj = new GH_ObjectWrapper();
            DA.GetData(0, ref obj);

            if (obj == null) return;

            var perf = obj.Value as PVPerformance;
            if (perf == null) return;


            DA.SetData(0, perf.Label);
            DA.SetData(1, perf.Totals.AverageDailyEnergyProduction);
            DA.SetData(2, perf.Totals.AverageMonthlyEnergyProduction);
            DA.SetData(3, perf.Totals.AverageYearlyEnergyProduction);
            DA.SetData(4, perf.Totals.AverageDailySumOfGlobalIrradiance);
            DA.SetData(5, perf.Totals.AverageMonthlySumOfGlobalIrradiance);
            DA.SetData(6, perf.Totals.AverageYearlySumOfGlobalIrradiance);
            DA.SetData(7, perf.Totals.DeviationOfMonthlyEnergyProduction);
            DA.SetData(8, perf.Totals.DeviationOYearlyEnergyProduction);
            DA.SetData(9, perf.Totals.AngleOfIncidenceLoss);
            DA.SetData(10, perf.Totals.SpectralLoss);
            DA.SetData(11, perf.Totals.TemperatureAndIrradianceLoss);
            DA.SetData(12, perf.Totals.TotalLoss);


        }
    }
}
