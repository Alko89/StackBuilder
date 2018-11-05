#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using log4net;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    internal class BSPNodeTri
    {
        #region Constructor
        public BSPNodeTri(Triangle t)
        {
            Triangle = t;
            InitEquation(t, ref A, ref B, ref C, ref D);
        }
        #endregion

        #region Public accessors
        public Triangle Triangle { get; set; }
        public BSPNodeTri NodeLeft { get; set; }
        public BSPNodeTri NodeRight { get; set; }
        #endregion

        #region Public method
        public void Insert(Triangle t)
        {
            if (t.IsFlat)
                return;
            List<Triangle> side_left = new List<Triangle>();
            List<Triangle> side_right = new List<Triangle>();
            Split(t, ref side_left, ref side_right);

            foreach (var tr in side_left)
            {
                if (tr.IsFlat)
                    continue;
                if (null == NodeLeft)
                    NodeLeft = new BSPNodeTri(tr);
                else
                    NodeLeft.Insert(tr);
            }
            foreach (var tr in side_right)
            {
                if (tr.IsFlat)
                    continue;
                if (null == NodeRight)
                    NodeRight = new BSPNodeTri(tr);
                else
                    NodeRight.Insert(tr);
            }
        }
        public void RecursFillTriangleList(ref List<Triangle> triangles)
        {
            NodeRight?.RecursFillTriangleList(ref triangles);
            triangles.Add(Triangle);
            NodeLeft?.RecursFillTriangleList(ref triangles);
        }
        #endregion

        #region Helpers
        private void InitEquation(Triangle tr, ref double a, ref double b, ref double c, ref double d)
        {
            Vector3D n = tr.Normal;
            if (tr.IsFlat)
                throw new GraphicsException("Triangle is flat");
            if (n.Z < 0) n = -n;
            d = Vector3D.DotProduct(n, tr.Points[0]);
            a = n.X;
            b = n.Y;
            c = n.Z;
        }
        private int Split(Triangle t, ref List<Triangle> t1, ref List<Triangle> t2)
        {
            Vector3D normal = t.Normal;
            int[] v = new int[3];
            int val = 0;
            int i = 0;
            foreach (Vector3D pt in t.Points)
            {
                v[i] = TestPoint(pt);
                val += v[i];
                i++;
            }

            switch (val)
            {
                case 3:
                    t1.Add(t);
                    return 1;
                case -3:
                    t2.Add(t);
                    return 1;
                case 2:
                    t1.Add(t);
                    return 1;
                case -2:
                    t2.Add(t);
                    return 1;
                case 0:
                    if (v[0] != 0 || v[1] != 0 || v[2] != 0)
                    {
                        int pivot = 0, positive = 0, negative = 0;
                        if (0 == v[0])
                        {
                            pivot = 0;
                            if (v[1] > 0) { positive = 1; negative = 2; }
                            else { positive = 2; negative = 1; }
                        }
                        if (0 == v[1])
                        {
                            pivot = 1;
                            if (v[0] > 0) { positive = 0; negative = 2; }
                            else { positive = 2; negative = 0; }
                        }
                        if (0 == v[2])
                        {
                            pivot = 2;
                            if (v[0] > 0) { positive = 0; negative = 1; }
                            else { positive = 1; negative = 0; }
                        }
                        // here positive, pivot and negative are ready
                        Vector3D ptInter = Vector3D.Zero;
                        TestLine(t.Points[positive], t.Points[negative], ref ptInter);                        
                        t1.Add(new Triangle(t.PickId, t.Points[positive], t.EdgeDrawn(positive, negative), ptInter, false, t.Points[pivot], t.EdgeDrawn(pivot, positive), t.ColorFill));
                        t2.Add(new Triangle(t.PickId, t.Points[negative], t.EdgeDrawn(negative, pivot), t.Points[pivot], false, ptInter, t.EdgeDrawn(positive, negative), t.ColorFill));
                        return 2;
                    }
                    else
                    {
                        t1.Add(t);
                        return 1;
                    }
                case -1:
                    if (0 == v[0] * v[1] * v[2])
                    {
                        t2.Add(t);
                        return 1;
                    }
                    {
                        int positive = 0, negative1 = 0, negative2 = 0;
                        if (v[0] == 1) { positive = 0; negative1 = 1; negative2 = 2; }
                        if (v[1] == 1) { positive = 1; negative1 = 0; negative2 = 2; }
                        if (v[2] == 1) { positive = 2; negative1 = 0; negative2 = 1; }
                        Vector3D ptInter1 = Vector3D.Zero, ptInter2 = Vector3D.Zero;
                        TestLine(t.Points[negative1], t.Points[positive], ref ptInter1);
                        TestLine(t.Points[negative2], t.Points[positive], ref ptInter2);
                        Triangle t11 = new Triangle(t.PickId, t.Points[positive], t.EdgeDrawn(positive, negative1), ptInter1, false, ptInter2, t.EdgeDrawn(negative2, positive), t.ColorFill);
                        Triangle t21 = new Triangle(t.PickId, t.Points[negative2], t.EdgeDrawn(negative2, positive), ptInter2, false, ptInter1, false, t.ColorFill);
                        Triangle t22 = new Triangle(t.PickId, t.Points[negative2], false, ptInter1, t.EdgeDrawn(positive, negative1), t.Points[negative1], t.EdgeDrawn(negative1, negative2), t.ColorFill);

                        if (!t11.IsFlat)
                            t1.Add(AlignNormal(t11, normal));
                        if (!t21.IsFlat)
                            t2.Add(AlignNormal(t21, normal));
                        if (!t22.IsFlat)
                            t2.Add(AlignNormal(t22, normal));
                        return 3;
                    }
                case 1:
                    if (0 == v[0] * v[1] * v[2])
                    {
                        t1.Add(t);
                        return 1;
                    }
                    {
                        int negative = 0, positive1 = 0, positive2 = 0;
                        if (v[0] == -1) { negative = 0; positive1 = 1; positive2 = 2; }
                        if (v[1] == -1) { negative = 1; positive1 = 0; positive2 = 2; }
                        if (v[2] == -1) { negative = 2; positive1 = 0; positive2 = 1; }
                        Vector3D ptInter1 = Vector3D.Zero, ptInter2 = Vector3D.Zero;
                        TestLine(t.Points[positive1], t.Points[negative], ref ptInter1);
                        TestLine(t.Points[positive2], t.Points[negative], ref ptInter2);
                        Triangle t11 = new Triangle(t.PickId, t.Points[positive1], t.EdgeDrawn(positive1, negative), ptInter1, false, ptInter2, false, t.ColorFill);
                        Triangle t12 = new Triangle(t.PickId, t.Points[positive1], false, ptInter2, t.EdgeDrawn(negative, positive2), t.Points[positive2], t.EdgeDrawn(positive2, positive1), t.ColorFill);
                        Triangle t21 = new Triangle(t.PickId, t.Points[negative], t.EdgeDrawn(negative, positive2), ptInter2, false, ptInter1, t.EdgeDrawn(positive1, negative), t.ColorFill);

                        if (!t11.IsFlat)
                            t1.Add(AlignNormal(t11, normal));
                        if (!t12.IsFlat)
                            t1.Add(AlignNormal(t12, normal));
                        if (!t21.IsFlat)
                            t2.Add(AlignNormal(t21, normal));                        
                        return 3;
                    }
                default:
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (Vector3D pt in t.Points)
                            sb.AppendLine(pt.ToString());
                        throw new Exception(sb.ToString());
                    }
            }
        }
        public double ClassifyPoint(Vector3D pt)
        {
            return Vector3D.DotProduct(pt, new Vector3D(A, B, C)) - D;
        }
        private int TestPoint(Vector3D pt)
        {
            double Z = pt.X * A + pt.Y * B + pt.Z * C - D;
            if (Z < Triangle.EPS) return -1;
            else if (Z > Triangle.EPS) return 1;
            else return 0;
        }
        private bool TestLine(Vector3D pt0, Vector3D pt1, ref Vector3D ptInter)
        {
            Vector3D normal = Triangle.Normal;
            if (Math.Abs(Vector3D.DotProduct(normal, (pt1 - pt0))) < Triangle.EPS)
            {
                ptInter = pt0;
                return false;
            }
            double s = (Vector3D.DotProduct(normal, Triangle.Points[0] - pt0) / (Vector3D.DotProduct(normal, pt1 - pt0)));
            ptInter = pt0 + s * (pt1 - pt0);
            return true;
        }
        private Triangle AlignNormal(Triangle t, Vector3D normal)
        {
            if (Vector3D.DotProduct(t.Normal, normal) < 0.0)
                t.Swap12();            
            return t;
        }
        #endregion

        #region Private data members
        private readonly double A;
        private readonly double B;
        private readonly double C;
        private readonly double D;

        private static readonly ILog _log = LogManager.GetLogger(typeof(BSPNodeTri));
        #endregion
    }

    internal class BSPNode
    {
        // constructor
        public BSPNode(Face f)
        {
            Face = f;
            InitEquation(f, ref A, ref B, ref C, ref D);
        }
        // insertion methods
        public void Insert(Face f)
        {
            if (f.IsDegenerate)
                return;

            // check on which side the polygon is, posibly split
            List<Face> side_left = new List<Face>();
            List<Face> side_right = new List<Face>();
            Split(f, ref side_left, ref side_right);

            // insert triangles
            foreach (Face face in side_left)
            {
                if (null == NodeLeft)
                    NodeLeft = new BSPNode(face);
                else
                    NodeLeft.Insert(face);
            }
            foreach (Face face in side_right)
            {
                if (null == NodeRight)
                    NodeRight = new BSPNode(face);
                else
                    NodeRight.Insert(face);
            }
        }
        private void RecursFillFaceArray(List<Face> faces)
        {
            if (null != NodeRight)
            {
                NodeRight.RecursFillFaceArray(faces);
            }
            faces.Add(Face);
            if (null != NodeLeft)
            {
                NodeLeft.RecursFillFaceArray(faces);
            }
        }
        // split methods
        private int TestPoint(Vector3D pt)
        {
            double Z = pt.X * A + pt.Y * B + pt.Z * C - D;

            if (Z < -Triangle.EPS) return -1;
            else if (Z > Triangle.EPS) return 1;
            else return 0;
        }
        private bool TestLine(Vector3D pt0, Vector3D pt1, ref Vector3D ptInter)
        {
            Vector3D normal = Face.Normal;
            if (Math.Abs(Vector3D.DotProduct(normal, pt1 - pt0)) < Triangle.EPS)
            {
                ptInter = pt0;
                return false;
            }
            double s = Vector3D.DotProduct(normal, Face.Points[0] - pt0) / Vector3D.DotProduct(normal, pt1 - pt0);
            ptInter = pt0 + s * (pt1 - pt0);
            return true;
        }
        private int Split(Face f, ref List<Face> f1, ref List<Face> f2)
        {
            int[] v = new int[4];
            int val = 0;
            int i = 0;
            foreach (Vector3D pt in f.Points)
            {
                v[i] = TestPoint(pt);
                val += v[i++];
            }
            switch (val)
            {
                case 4:
                    f1.Add(f);
                    return 1;
                case -4:
                    f2.Add(f);
                    return 1;
                case 2:
                    f1.Add(f);
                    return 1;
                case -2:
                    f2.Add(f);
                    return 1;
                case 0:
                    if (0 != v[0] && 0 != v[1] && 0 != v[2] && 0 != v[3])
                    {
                        for (i = 0; i < 4; ++i)
                        {
                            if (v[i % 4] + v[(i + 1) % 4] == 2)
                            {
                                Vector3D ptInter1 = Vector3D.Zero, ptInter2 = Vector3D.Zero;
                                TestLine(f.Points[(i + 1) % 4], f.Points[(i + 2) % 4], ref ptInter1);
                                TestLine(f.Points[(i + 3) % 4], f.Points[(i + 4) % 4], ref ptInter2);

                                Vector3D[] v1 = new Vector3D[4];
                                v1[0] = f.Points[i % 4];
                                v1[1] = f.Points[(i + 1) % 4];
                                v1[2] = ptInter1;
                                v1[3] = ptInter2;
                                f1.Add(new Face(f.PickingId, v1, f.ColorFill, f.ColorPath, f.IsSolid));
                                Vector3D[] v2 = new Vector3D[4];
                                v2[0] = ptInter2;
                                v2[1] = ptInter1;
                                v2[2] = f.Points[(i + 2) % 4];
                                v2[3] = f.Points[(i + 3) % 4];
                                f2.Add(new Face(f.PickingId, v2, f.ColorFill, f.ColorPath, f.IsSolid));
                            }
                        }
                        return 2;
                    }
                    else // triangle is inside plane, assign to positive node
                    {
                        f1.Add(f);
                        return 1;
                    }
                default:
                    _log.WarnFormat("Split: val = {0}", val);
                    throw new GraphicsException("Split failed!");
            }
        }
        // classify point
        public double ClassifyPoint(Vector3D pt)
        {
            Vector3D normal = Face.Normal;
            return Vector3D.DotProduct(pt, normal) - D;
        }

        public BSPNode NodeLeft { get; set; }
        public BSPNode NodeRight { get; set; }
        public Face Face { get; set; }

        #region Private members
        private void InitEquation(Face f, ref double a, ref double b, ref double c, ref double d)
        {
            Vector3D n = f.Normal;
            if (f.IsDegenerate)
                throw new GraphicsException("Face is degenerate");
            if (n.Z < 0) n = -n;
            d = Vector3D.DotProduct(n, f.Points[0]);
            a = n.X;
            b = n.Y;
            c = n.Z;
        }
        private readonly double A;
        private readonly double B;
        private readonly double C;
        private readonly double D;
        private static readonly ILog _log = LogManager.GetLogger(typeof(BSPNode));
        #endregion
    }
}
