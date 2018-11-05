#region Using directives
using System;
using System.Drawing;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    #region Triangle
    public class Triangle
    {
        public Triangle(uint pickId, Vector3D[] vertices, bool[] drawPath)
        {
            PickId = pickId;
            for (int i = 0; i < 3; ++i)
            {
                Points[i] = vertices[i];
                DrawPath[i] = drawPath[i];
            }
        }
        public Triangle(uint pickId, Vector3D pt0, Vector3D pt1, Vector3D pt2, Color colorFill)
        {
            PickId = pickId;
            Points[0] = pt0; Points[1] = pt1; Points[2] = pt2;
            ColorFill = colorFill;
        }
        public Triangle(uint pickId, Vector3D pt0, bool e0, Vector3D pt1, bool e1, Vector3D pt2, bool e2, Color colorFill)
        {
            PickId = pickId;
            Points[0] = pt0; Points[1] = pt1; Points[2] = pt2;
            DrawPath[0] = e0; DrawPath[1] = e1; DrawPath[2] = e2;
            ColorFill = colorFill;
        }
        public Vector3D Center
        {
            get
            {
                Vector3D vCenter = Vector3D.Zero;
                foreach (Vector3D v in Points)
                    vCenter += v;
                return vCenter * (1.0 / Points.Length);
            }
        }
        public Vector3D Normal
        {
            get
            {
                Vector3D vecNormal = Vector3D.CrossProduct(Points[1] - Points[0], Points[2] - Points[0]);
                if (vecNormal.GetLength() < MathFunctions.EpsilonD)
                    throw new GraphicsException("Triangle is degenerated");
                vecNormal.Normalize();
                return vecNormal;
            }
        }
        public uint PickId { get; set; }
        public bool IsSolid { get; set; } = true;
        public Color ColorFill { get; set; } = Color.Red;
        public Color ColorPath { get; set; } = Color.Black;
        public Vector3D[] Points { get; } = new Vector3D[3];
        public bool[] DrawPath = new bool[]{ true, true, true };
        public bool IsFlat => (Vector3D.CrossProduct(Points[1] - Points[0], Points[2] - Points[0]).GetLength() < EPS);
        public int PointRelativePosition(Vector3D pt)
        {
            double dotProd = Vector3D.DotProduct(pt - Center, Normal);
            if (Math.Abs(dotProd) < EPS)
                return 0;
            else
                return (dotProd > 0 ? 1 : -1);
        }
        public bool IsVisible(Vector3D viewDir)
        {
            return Vector3D.DotProduct(viewDir, Normal) < 0.0;
        }
        public bool PointIsBehind(Vector3D pt, Vector3D viewDir)
        {
            return (Vector3D.DotProduct(pt - Center, Normal) * Vector3D.DotProduct(viewDir, Normal)) > EPS;
        }
        public bool PointIsInFront(Vector3D pt, Vector3D viewDir)
        {
            return (Vector3D.DotProduct(pt - Center, Normal) * Vector3D.DotProduct(viewDir, Normal)) < EPS;
        }
        public Triangle Transform(Transform3D transf)
        {
            Vector3D[] points = new Vector3D[3];
            for (int i = 0; i < 3; ++i)
                points[i] = transf.transform(Points[i]);
            return new Triangle(PickId, points, DrawPath)
            {
                IsSolid = this.IsSolid,
                ColorFill = this.ColorFill,
                ColorPath = this.ColorPath
            };
        }
        public void Swap12()
        {
            Vector3D ptTemp = Points[1];
            Points[1] = Points[2];
            Points[2] = ptTemp;

            bool bTemp = DrawPath[0];
            DrawPath[0] = DrawPath[2];
            DrawPath[2] = bTemp;
        }
        public bool EdgeDrawn(int i, int j)
        {
            int iTemp = i, jTemp = j;
            if (i < j) { iTemp = i; jTemp = j; } else { iTemp = j; jTemp = i; }
            if (0 == iTemp)
            {
                if (1 == jTemp)
                    return DrawPath[0];
                else
                    return DrawPath[2];
            }
            else
                return DrawPath[1];
        }
        public override string ToString()
        {
            return $"({Points[0]}, {Points[1]}, {Points[2]})";
        }

        public static readonly double EPS = 0.1;
    }
    #endregion
}
