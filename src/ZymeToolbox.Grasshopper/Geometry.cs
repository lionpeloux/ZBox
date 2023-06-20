using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZymeToolbox.Grasshopper
{
    /// <summary>
    /// Helper class for geometry processing
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// Get multiple iso-value contours curve from a mesh and a given set of vertex values.
        /// </summary>
        public static List<List<Line>> GetMeshContours(Mesh mesh, List<double> vertexValues, List<double> isoValues)
        {
            // https://en.wikipedia.org/wiki/Marching_squares#Contouring_triangle_meshes
            // this code was reverse-engineering from the millipede plugin

            if (mesh.Vertices.Count != vertexValues.Count)
                throw new ArgumentException($"The mesh has {mesh.Faces.Count} but the vertexValues has {vertexValues.Count} items.");

            // triangulate the mesh first
            mesh.Faces.ConvertQuadsToTriangles();
            var isoContours = new List<List<Line>>();

            // for each isovalue
            for (int j = 0; j < isoValues.Count; j++)
            {
                var isoLines = GetTriangularMeshContour(mesh, vertexValues, isoValues[j]);
                isoContours.Add(isoLines);
            }

            return isoContours;
        }

        /// <summary>
        /// Get an iso-value contour curve from a mesh.
        /// </summary>
        /// <param name="triangularMesh">The mesh to plot the iso-value contour on</param>
        /// <param name="vertexValues">A list for the vertex values. Must align with the number of mesh vertices.</param>
        /// <param name="isoValue">The iso-value to build the contour for</param>
        /// <returns>A list of lines representing the iso-value contour curve.</returns>
        private static List<Line> GetTriangularMeshContour(Mesh triangularMesh, List<double> vertexValues, double isoValue)
        {
            // https://en.wikipedia.org/wiki/Marching_squares#Contouring_triangle_meshes
            // this code was reverse-engineering from the millipede plugin

            if (triangularMesh.Vertices.Count != vertexValues.Count)
                throw new ArgumentException($"The mesh has {triangularMesh.Faces.Count} but the vertexValues has {vertexValues.Count} items.");

            var isoLines = new List<Line>();

            int num = 0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            Point3d[] array = new Point3d[3];

            int num5 = 0;
            double num6 = 0.0;
            for (num = 0; num < triangularMesh.Faces.Count; num++)
            {
                int a = triangularMesh.Faces.GetFace(num).A;
                int b = triangularMesh.Faces.GetFace(num).B;
                int c = triangularMesh.Faces.GetFace(num).C;
                num2 = isoValue - vertexValues[a];
                num3 = isoValue - vertexValues[b];
                num4 = isoValue - vertexValues[c];
                num5 = 0;
                if (num2 * num3 < 0.0)
                {
                    num6 = num2 / (vertexValues[b] - vertexValues[a]);
                    array[num5].X = (double)triangularMesh.Vertices[a].X + (double)(triangularMesh.Vertices[b].X - triangularMesh.Vertices[a].X) * num6;
                    array[num5].Y = (double)triangularMesh.Vertices[a].Y + (double)(triangularMesh.Vertices[b].Y - triangularMesh.Vertices[a].Y) * num6;
                    array[num5].Z = (double)triangularMesh.Vertices[a].Z + (double)(triangularMesh.Vertices[b].Z - triangularMesh.Vertices[a].Z) * num6;
                    num5++;
                }
                if (num3 * num4 < 0.0)
                {
                    num6 = num3 / (vertexValues[c] - vertexValues[b]);
                    array[num5].X = (double)triangularMesh.Vertices[b].X + (double)(triangularMesh.Vertices[c].X - triangularMesh.Vertices[b].X) * num6;
                    array[num5].Y = (double)triangularMesh.Vertices[b].Y + (double)(triangularMesh.Vertices[c].Y - triangularMesh.Vertices[b].Y) * num6;
                    array[num5].Z = (double)triangularMesh.Vertices[b].Z + (double)(triangularMesh.Vertices[c].Z - triangularMesh.Vertices[b].Z) * num6;
                    num5++;
                }
                if (num5 != 0)
                {
                    if (num5 != 2 && num4 * num2 < 0.0)
                    {
                        num6 = num4 / (double)(vertexValues[a] - vertexValues[c]);
                        array[num5].X = (double)triangularMesh.Vertices[c].X + (double)(triangularMesh.Vertices[a].X - triangularMesh.Vertices[c].X) * num6;
                        array[num5].Y = (double)triangularMesh.Vertices[c].Y + (double)(triangularMesh.Vertices[a].Y - triangularMesh.Vertices[c].Y) * num6;
                        array[num5].Z = (double)triangularMesh.Vertices[c].Z + (double)(triangularMesh.Vertices[a].Z - triangularMesh.Vertices[c].Z) * num6;
                        num5++;
                    }
                    if (num5 == 2)
                    {
                        isoLines.Add(new Line(array[0], array[1]));
                    }
                }
            }

            return isoLines;
        }
    }
}
