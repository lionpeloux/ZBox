using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_PVPerformance_OnGrid : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{5D17478A-E01C-4255-9B10-6AA0717348BC}");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public GHComp_PVGIS_PVPerformance_OnGrid()
          : base("PVGIS Performance of Grid-Connected PV System", "PVGIS Perf On-Grid PV", "Get the performance of a grid-connected PV system from PVGIS API.",
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
            pManager.AddIntegerParameter("PV Technology", "pvTechnology", "Type of PV Technology : 0=CrystSi | 1=CIS | 2=CdTe | 3=Unknown.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("PV Peak Power", "pvPeakPower", "PV peak power installed in kWp.", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("PV Loss", "pvLoss", "Sum of system losses in %.", GH_ParamAccess.item, 14);

            // POA
            pManager.AddIntegerParameter("PV Mounting Type", "pvMounting", "Type of PV Mounting : 0=free | 1=building.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("POA Inclination", "poaInclination", "Inclination angle of the POA from the horizontal plane in degree (0° = hozirontal | 90° = vertical). Not relevant for 2-axis tracking POA", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("POA Azimuth", "poaAzimuth", "Azimuth angle of the fixed POA in degree (0° = South, 90° = West, -90° = East). Not relevant for POA with tracking.", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("POA Optimize Inclination", "poaOptx1", "Compute the optimum inclination angle.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("POA Optimize Inclination and Azimiuth", "poaOptx2", "Compute the optimum inclination AND azimuth angles.", GH_ParamAccess.item, false);

            // COST
            pManager.AddNumberParameter("PV system cost", "pvSystemCost", "Overall system cost in user currency, including PV system components (PV modules, mounting, inverters, cables, etc.) and installation costs (planning, installation, ...)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Annual Interest Rate in %", "pvAPR", "Annual percentage rate in %. It's the annual cost of a loan to a borrower.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Expected Lifetime", "pvLifeTime", "Expected liftime of the system in years.", GH_ParamAccess.item, 30);

            pManager.AddBooleanParameter("Run", "run", "Sends the HTTP request when set to true.", GH_ParamAccess.item);

            pManager[3].Optional = true;
            pManager[12].Optional = true;
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
            pManager.AddGenericParameter("Performance Results", "results", "Performance results of the the given system.", GH_ParamAccess.item);
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
            double inclination = 0;
            double azimut = 0;
            bool poaOptx1 = false;
            bool poaOptx2 = false;
            int pvMounting = 0;
            int pvTechnology = 0;
            double pvPeakPower = 0;
            double pvLoss = 0;
            double pvSystemCost = 0;
            double pvAPR = 0;
            int pvLifeTime = 0;

            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = OnGridPVQuery.Create(latitude, longitude, "json");

            // HORIZON
            DA.GetData(2, ref useHorizon);
            DA.GetDataList(3, userHorizon);
            query.WithHorizon(useHorizon, userHorizon);

            // PV
            DA.GetData(4, ref pvTechnology);
            DA.GetData(5, ref pvPeakPower);
            DA.GetData(6, ref pvLoss);
            DA.GetData(7, ref pvMounting);
            query.WithPVSystem(pvPeakPower, pvLoss, (PVMountingType)pvMounting, (PVTechnologyType)pvTechnology);

            // POA
            DA.GetData(8, ref inclination);
            DA.GetData(9, ref azimut);
            DA.GetData(10, ref poaOptx1);
            DA.GetData(11, ref poaOptx2);
            query.WithPVMouting(inclination, azimut, poaOptx1, poaOptx2);

            // COST
            DA.GetData(12, ref pvSystemCost);
            DA.GetData(13, ref pvAPR);
            DA.GetData(14, ref pvLifeTime);
            query.WithPVCostModel(pvSystemCost, pvAPR, pvLifeTime);

            var perf = Services.GetPVPerformance_OnGrid(query);

            DA.SetData(0, perf.UrlQuery);
            DA.SetData(1, perf.Meta.ToString(true));
            DA.SetData(2, perf.Inputs.ToString(true));

            DA.SetData(3, perf.Location.Latitude);
            DA.SetData(4, perf.Location.Longitude);
            DA.SetData(5, perf.Location.Elevation);

            DA.SetData(6, perf.Performance_Fixed);
        }


    }
}
