using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using ZymeToolbox.Core;
using ZymeToolbox.Core.API.PVGIS;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Climat.Grasshopper.Components
{
    public class GHComp_PVGIS_HorizonProfile : GH_Component
    {

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{4C4CA585-C11E-42C5-8654-09732FC7CA00}");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public GHComp_PVGIS_HorizonProfile()
          : base("PVGIS Horizon Profile", "PVGIS Horizon", "Get the horizon profile from PVGIS API.",
            "ZBox", "2 | Climat API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "run", "Sends the HTTP request when set to true.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Url Query", "urlQuery", "HTTP resquest url send to the REST API.", GH_ParamAccess.item);
            pManager.AddTextParameter("Meta", "json_meta", "'meta' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddTextParameter("Inputs", "json_inputs", "'inputs' section of the JSON response.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Latitude", "latitude", "Latitude, South is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Longitude", "longitude", "Longitude, West is negative (decimal degree).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elevation", "elevation", "Elevation (meter).", GH_ParamAccess.item);
            pManager.AddVectorParameter("Horizon Profile", "horizon", "A list of SunPosition (Azimuth, Elevation) describing the horizon profile (December 21th). Values are in degree.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Winter Solstice", "winter", "A list of SunPosition (Azimuth, Elevation) describing the winter solstice (December 21th). Values are in degree.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Summer Solstice", "summer", "A list of SunPosition (Azimuth, Elevation) describing the summer solstice (June 21th). Values are in degree.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            if (!DA.GetData("Run", ref run)) return;
            if (!run) return;

            double latitude = 0;
            double longitude = 0;
            if (!DA.GetData(0, ref latitude)) return;
            if (!DA.GetData(1, ref longitude)) return;

            var query = HorizonProfileQuery.Create(latitude, longitude, "json");
            var response = Services.GetHorizonProfile(query);

            DA.SetData(0, response.UrlQuery);
            DA.SetData(1, response.Meta.ToString(true));
            DA.SetData(2, response.Inputs.ToString(true));

            DA.SetData(3, response.Location.Latitude);
            DA.SetData(4, response.Location.Longitude);
            DA.SetData(5, response.Location.Elevation);
            DA.SetDataList(6, response.HorizonProfile.Select(sp => new Vector2d(sp.Azimuth, sp.Elevation)));
            DA.SetDataList(7, response.WinterSolstice.Select(sp => new Vector2d(sp.Azimuth, sp.Elevation)));
            DA.SetDataList(8, response.SummerSolstice.Select(sp => new Vector2d(sp.Azimuth, sp.Elevation)));
        }

    }
}