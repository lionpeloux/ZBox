using Geolocation;
using Grasshopper.Kernel;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using ZymeToolbox.Core.API.DOE;
using ZymeToolbox.Core.FileFormat;

namespace ZymeToolbox.Grasshopper.Components.IO
{
    public class GHComp_EPWLocation : GH_Component
    {

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{1E0228EE-FDC0-41CD-AC25-D0140B6A8C8A}");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public GHComp_EPWLocation()
          : base("EPW Location From File", "EPW Location", "Get the location of an .epw file.",
            "ZBox", "1 | I/O")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Url to EPW File", "url", "Url to the .epw file. Url can actually be a path to a local file.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "info", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string url = "";
            if (!DA.GetData(0, ref url)) return;
            
            var location = EPW.GetLocationFromEPWFile(url);

            DA.SetData(0, location.ToString());
        }

    }
}