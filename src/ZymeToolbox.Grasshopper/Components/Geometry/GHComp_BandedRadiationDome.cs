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
    public class GHComp_BandedRadiationDome : GH_Component
    {

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{8AD694D1-7967-4664-A30D-E05A136F8ED5}");
        public override GH_Exposure Exposure => GH_Exposure.quinary;

        public GHComp_BandedRadiationDome()
          : base("Banded Radiation Dome", "Banded Radiation Dome", "Banded Radiation Dome",
            "ZBox", "Ladybug")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "mesh", "Input mesh", GH_ParamAccess.item);
            pManager.AddCurveParameter("Compass", "compass", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Dir Values", "dir_values", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Iso Values", "iso_values", "Iso values must be sorted from min to max.", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Bands", "bands", "The iso-value bands as surfaces. Sorted accordingly to iso_values.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Iso Contours", "contours", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = default;
            var compass = new List<Curve>();
            var dirValues = new List<double>();
            var isoValues = new List<double>();

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, compass)) return;
            if (!DA.GetDataList(2, dirValues)) return;
            if (!DA.GetDataList(3, isoValues)) return;

            double tol = 1e-6;
            Circle circle;

            if (!compass[0].TryGetCircle(out circle))
                throw new ArgumentException("not a circle");

            var R = circle.Radius;
            
            var disk = Brep.CreatePlanarBreps(circle.ToNurbsCurve(), tol)[0];

            var isoLines = Grasshopper.Geometry.GetMeshContours(mesh, dirValues, isoValues);
            var isoCrvs = new List<Curve>();

            var geom = new List<GeometryBase>() { circle.ToNurbsCurve() };

            for (int i = 0; i < isoLines.Count; i++)
            {
                var crv = Curve.JoinCurves(isoLines[i].Select(l => l.ToNurbsCurve()))[0];
                crv = crv.Rebuild(12, 2, true);
                if (!crv.IsClosed)
                {
                    crv = crv.Extend(CurveEnd.Both, CurveExtensionStyle.Arc, geom);
                    //crv = crv.Extend(CurveEnd.Both, R / 100, CurveExtensionStyle.Arc);

                }
                isoCrvs.Add(crv);
            }

            var isoBands = disk.Split(isoCrvs, tol);
            var sortedBands = SortBands(isoCrvs, isoBands.ToList(), tol); 


            DA.SetDataList(0, sortedBands);
            DA.SetDataList(1, isoCrvs);
        }

        private Brep[] SortBands(List<Curve> isoCurves, List<Brep> isoBandes, double tol = 1e-6)
        {
            // which faces are adjacent to which iso-contour ?
            List<int>[] FacesByCurve = new List<int>[isoCurves.Count];

            // which iso-contours are adjacent to which face ?
            List<int>[] CurvesByFace = new List<int>[isoBandes.Count];

            DataTree<int> matchT = new DataTree<int>();

            for (int i = 0; i < isoCurves.Count; i++)
            {
                Curve c = isoCurves[i];
                Point3d pt = c.PointAtStart;
                FacesByCurve[i] = new List<int>();
                for (int j = 0; j < isoBandes.Count; j++)
                {
                    Brep f = isoBandes[j];
                    var cpt = f.ClosestPoint(pt);
                    // c is a boundary for
                    if (pt.DistanceTo(cpt) < tol)
                    {
                        FacesByCurve[i].Add(j);

                        if (CurvesByFace[j] == null)
                            CurvesByFace[j] = new List<int>();

                        CurvesByFace[j].Add(j);
                    }
                }
                matchT.AddRange(FacesByCurve[i], new GH_Path(i));
            }


            // sort faces
            int[] sortedFacesIndex = new int[isoBandes.Count];
            Brep[] sortedFaces = new Brep[isoBandes.Count];
            int f_prev = 0;
            // first face
            foreach (var fi in FacesByCurve.First())
            {
                if (CurvesByFace[fi].Count == 1)
                {
                    sortedFaces[0] = isoBandes[fi];
                    sortedFacesIndex[0] = fi;
                }
                else
                {
                    f_prev = fi;
                }
            }

            // last face
            foreach (var fi in FacesByCurve.Last())
            {
                if (CurvesByFace[fi].Count == 1)
                {
                    sortedFaces[sortedFaces.Length - 1] = isoBandes[fi];
                    sortedFacesIndex[sortedFaces.Length - 1] = fi;
                }
            }

            // middle
            for (int i = 1; i < isoCurves.Count; i++)
            {
                int f0 = FacesByCurve[i][0];
                int f1 = FacesByCurve[i][1];
                if (f0 == f_prev)
                {
                    sortedFaces[i] = isoBandes[f0];
                    sortedFacesIndex[i] = f0;
                    f_prev = f1;
                }
                else
                {
                    sortedFaces[i] = isoBandes[f1];
                    sortedFacesIndex[i] = f1;
                    f_prev = f0;
                }
            }

            return sortedFaces;

        }

    }
}