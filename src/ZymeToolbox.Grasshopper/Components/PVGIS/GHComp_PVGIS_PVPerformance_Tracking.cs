using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;
using ZymeToolbox.Core.API.PVGIS.Responses;

namespace ZymeToolbox.Grasshopper.Components.PVGIS
{
    public class GHComp_PVGIS_PVPerformance_Tracking : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{82E42C60-77D9-496D-98C7-3CC88CC2A859}");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public GHComp_PVGIS_PVPerformance_Tracking()
          : base("PVGIS Performance of Tracking PV System", "PVGIS Perf Tracking PV", "Get the performance of a tracking PV system from PVGIS API.",
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
            pManager.AddNumberParameter("PV Peak Power", "pvPeakPower", "PV peak power installed in kWp.", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("PV Loss", "pvLoss", "Sum of system losses in %.", GH_ParamAccess.item, 14);

            // TRACKING
            pManager.AddNumberParameter("Vertical-Axis Angle", "vAxis_Angle", "Inclination angle for a single vertical axis system. This is the angle of the PV modules from the horizontal plane, for a fixed (non-tracking) mounting.", GH_ParamAccess.item, 30);
            pManager.AddBooleanParameter("Optimize Vertical-Axis Angle", "vAxis_Optimize", "If true, calculate the optimum angle for a single vertical axis system.", GH_ParamAccess.item, false);

            pManager.AddNumberParameter("Inclined-Axis Angle", "iAxis_Angle", "Inclination angle for a single inclined axis system.", GH_ParamAccess.item, 90);
            pManager.AddBooleanParameter("Optimize Inclined-Axis Angle", "iAxis_Optimize", "If true, calculate the optimum angle for a single inclined axis system. This is the angle of the PV modules from the horizontal plane, for a fixed (non-tracking) mounting.", GH_ParamAccess.item, false);

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

            // Results
            pManager.AddGenericParameter("Performance Results", "results", "Performance results of the the given system.", GH_ParamAccess.list);
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
            int pvTechnology = 0;
            double pvPeakPower = 0;
            double pvLoss = 0;
            double vAxis_Angle = 0;
            bool vAxis_Opt = true;
            double iAxis_Angle = 0;
            bool iAxis_Opt = true;


            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;
            var query = TrackingPVQuery.Create(latitude, longitude, "json");

            // HORIZON
            DA.GetData(2, ref useHorizon);
            DA.GetDataList(3, userHorizon);
            query.WithHorizon(useHorizon, userHorizon);

            // PV
            DA.GetData(4, ref pvTechnology);
            DA.GetData(5, ref pvPeakPower);
            DA.GetData(6, ref pvLoss);
            query.WithPVSystem((PVTechnologyType)pvTechnology, pvPeakPower, pvLoss);

            // Tracking
            DA.GetData(7, ref vAxis_Angle);
            DA.GetData(8, ref vAxis_Opt);
            query.WithPVMouting_VerticalAxis(vAxis_Angle, vAxis_Opt);

            DA.GetData(9, ref iAxis_Angle);
            DA.GetData(10, ref iAxis_Opt);
            query.WithPVMouting_InclinedAxis(iAxis_Angle, iAxis_Opt);

            query.WithPVMouting_TwoAxis(true);

            
            var perf = Services.GetPVPerformance_Tracking(query);

            DA.SetData(0, perf.UrlQuery);
            DA.SetData(1, perf.Meta.ToString(true));
            DA.SetData(2, perf.Inputs.ToString(true));

            DA.SetData(3, perf.Location.Latitude);
            DA.SetData(4, perf.Location.Longitude);
            DA.SetData(5, perf.Location.Elevation);

            DA.SetDataList(6, new List<PVPerformance>() { perf.Performance_VerticalAxis, perf.Performance_InclinedAxis, perf.Performance_TwoAxis });

        }
    }
}
