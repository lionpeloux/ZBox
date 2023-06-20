using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace ZymeToolbox.Grasshopper.Components.Geometry
{
    public class GHComp_DownloadFile : GH_Component
    {

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{8823B32F-89A6-42FB-AAD3-35C1AAA425E4}");
        public override GH_Exposure Exposure => GH_Exposure.quinary;

        public GHComp_DownloadFile()
          : base("Mesh Contour Field", "Mesh Contour", "Get multiple iso-value contours curve from a mesh and a given set of vertex values",
            "Mesh", "Util")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Input mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Vertex Values", "V", "A scalar field for the mesh, as a list of vertex values", GH_ParamAccess.list);
            pManager.AddNumberParameter("Iso Values", "I", "A list of values to build the contour for", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour", "C", "The iso-value contour curves", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = default;
            var vertexValues = new List<double>();
            var isoValues = new List<double>();

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, vertexValues)) return;
            if (!DA.GetDataList(2, isoValues)) return;

            var contours = Grasshopper.Geometry.GetMeshContours(mesh, vertexValues, isoValues);

            var lines = new DataTree<Line>();

            for (int i = 0; i < contours.Count; i++)
            {
                lines.AddRange(contours[i], new GH_Path(i));
            }

            DA.SetDataTree(0, lines);
        }

    }
}