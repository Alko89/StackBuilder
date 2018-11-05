#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    #region Segment
    public class Segment
    {
        #region Constructor
        public Segment(Vector3D pt0, Vector3D pt1, Color color)
        {
            Points[0] = pt0; Points[1] = pt1; Color = color;
        }
        #endregion

        #region Public properties
        public Color Color { get; set; } = Color.Black;
        public Vector3D[] Points { get; } = new Vector3D[2];
        public uint PickingId { get; } = 0;
        #endregion

        #region Public methods
        #endregion

        #region Object override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Segment => PickId : {0} Points : ", PickingId));
            foreach (Vector3D v in Points)
                sb.Append(v.ToString());
            return sb.ToString();
        }
        #endregion
    }
    #endregion

    #region Face
    public class Face
    {
        #region Private members
        private static readonly double eps = 0.0001;
        #endregion

        #region Constructor
        public Face(uint pickId, Vector3D[] vertices, bool isSolid)
        {
            Points = vertices;
            if (Points.Length < 3)
                throw new GraphicsException("Face is degenerated");
            PickingId = pickId;
            IsSolid = isSolid;
        }
        public Face(uint pickId, Vector3D[] vertices, Color colorFill, Color colorPath, bool isSolid)
        {
            Points = vertices;
            if (Points.Length < 3)
                throw new GraphicsException("Face is degenerated");
            PickingId = pickId;
            ColorFill = colorFill;
            ColorPath = colorPath;
            IsSolid = isSolid;
        }
        /// <summary>
        /// Face constructor
        /// </summary>
        /// <param name="pickId">Picking id</param>
        /// <param name="pt0">Lower left point</param>
        /// <param name="pt1">Lower right point</param>
        /// <param name="pt2">Upper right point</param>
        /// <param name="pt3">Upper left point</param>
        /// <param name="isSolid">Is solid</param>
        public Face(uint pickId, Vector3D pt0, Vector3D pt1, Vector3D pt2, Vector3D pt3, bool isSolid)
        {
            PickingId = pickId;
            Points = new Vector3D[4];
            Points[0] = pt0;
            Points[1] = pt1;
            Points[2] = pt2;
            Points[3] = pt3;
            IsSolid = isSolid;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// fill color of face
        /// </summary>
        public Color ColorFill { get; set; } = Color.Red;
        /// <summary>
        /// path color of face
        /// </summary>
        public Color ColorPath { get; set; } = Color.Black;
        public bool IsSolid { set; get; } = false;
        /// <summary>
        /// The array of 3D Points corresponding to the corners of this Face
        /// </summary>
        public Vector3D[] Points { get; }

        /// <summary>
        /// The center point of the face
        /// </summary>
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
        /// <summary>
        /// The unit normal of the face
        /// </summary>
        public Vector3D Normal
        {
            get
            {
                Vector3D vecNormal = Vector3D.CrossProduct(Points[1] - Points[0], Points[2] - Points[0]);
                if (vecNormal.GetLength() < MathFunctions.EpsilonD)
                    throw new GraphicsException("Face is degenerated");
                vecNormal.Normalize();
                return vecNormal;
            }
        }
        /// <summary>
        /// Is face degenerate
        /// </summary>
        public bool IsDegenerate
        {   get { return (Vector3D.CrossProduct(Points[1] - Points[0], Points[2] - Points[0]).GetLength() < MathFunctions.EpsilonD); } }
        /// <summary>
        /// The picking id of the face
        /// </summary>
        public uint PickingId { get; } = 0;
        /// <summary>
        /// Has bitmap
        /// </summary>
        public bool HasBitmap
        {
            get { return (null != Textures) && (Textures.Count > 0); }
        }
        /// <summary>
        /// Bitmap
        /// </summary>
        public List<Texture> Textures { set; get; } = new List<Texture>();

        public void ExtractFaceBitmap(double dimX, double dimY, int bmpWidth, string filename)
        {
            // extract bitmap
            double aspectRatio = dimY / dimX;
            using (Bitmap bmp = new Bitmap(bmpWidth, (int)(bmpWidth * aspectRatio)))
            {
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                g.Clear(ColorFill);

                foreach (Texture texture in Textures)
                {
                    double cosAngle = Math.Cos(texture.Angle * Math.PI / 180.0);
                    double sinAngle = Math.Sin(texture.Angle * Math.PI / 180.0);

                    Vector2D[] pts = new Vector2D[4];
                    Vector2D vecO = Vector2D.Zero;
                    Vector2D vecI = Vector2D.XAxis;
                    Vector2D vecJ = Vector2D.YAxis;

                    pts[0] = vecO + texture.Position.X * vecI + texture.Position.Y * vecJ;
                    pts[1] = vecO + (texture.Position.X + texture.Size.X * cosAngle) * vecI + (texture.Position.Y + texture.Size.X * sinAngle) * vecJ;
                    pts[2] = vecO + (texture.Position.X + texture.Size.X * cosAngle - texture.Size.Y * sinAngle) * vecI + (texture.Position.Y + texture.Size.X * sinAngle + texture.Size.Y * cosAngle) * vecJ;
                    pts[3] = vecO + (texture.Position.X - texture.Size.Y * sinAngle) * vecI + (texture.Position.Y + texture.Size.Y * cosAngle) * vecJ;

                    double coefX = bmpWidth / dimX; double coefY = bmpWidth * aspectRatio / dimY;
                    Point[] ptf = new Point[3];
                    ptf[0] = new Point((int)(pts[0].X * coefX), (int)(pts[0].Y * coefY));
                    ptf[1] = new Point((int)(pts[1].X * coefX), (int)(pts[1].Y * coefY));
                    ptf[2] = new Point((int)(pts[3].X * coefX), (int)(pts[3].Y * coefY));

                    g.DrawImage(texture.Bitmap, ptf);
                }
                bmp.Save(filename);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gives point placement relative to face
        /// </summary>
        /// <param name="pt">Tested point</param>
        /// <returns>
        /// -> 1 : interior
        /// -> 0 : on face
        /// -> -1 : exterior
        /// </returns>
        public int PointRelativePosition(Vector3D pt)
        {
            double dotProd = Vector3D.DotProduct(pt - Center, Normal);
            if (Math.Abs(dotProd) < eps)
                return 0;
            else if (dotProd > 0)
                return 1; // ext
            else
                return -1; // int
        }

        public bool IsVisible(Vector3D viewDir)
        {
            return Vector3D.DotProduct(viewDir, Normal) < 0.0;
        }

        public bool PointIsBehind(Vector3D pt, Vector3D viewDir)
        {
            return (Vector3D.DotProduct(pt - Center, Normal) * Vector3D.DotProduct(viewDir, Normal)) > eps;
        }

        public bool PointIsInFront(Vector3D pt, Vector3D viewDir)
        {
            return (Vector3D.DotProduct(pt - Center, Normal) * Vector3D.DotProduct(viewDir, Normal)) < eps;
        }

        public bool RayIntersect(Ray ray, out Vector3D ptInter)
        {
            ptInter = Vector3D.Zero;
            if (IntersectTriangle(ray, Points[0], Points[1], Points[2], out double u, out double v))
            {
                Vector3D vecU = Points[1] - Points[0];
                Vector3D vecV = Points[2] - Points[0];
                ptInter = Points[0] + u * vecU + v * vecV;
                return true;
            }
            else if (IntersectTriangle(ray, Points[2], Points[3], Points[0], out u, out v))
            {
                Vector3D vecU = Points[3] - Points[2];
                Vector3D vecV = Points[0] - Points[2];
                ptInter = Points[2] + u * vecU + v * vecV;
                return true;
            }
            else
                return false;
        }

        public bool IntersectTriangle(Ray ray, Vector3D pt0, Vector3D pt1, Vector3D pt2, out double u, out double v)
        {
            u = 0.0; v = 0.0;
            Vector3D edge1 = pt1 - pt0;
            Vector3D edge2 = pt2 - pt0;

            // begin calculating determinant - also used to calculate U parameter
            Vector3D pvec = Vector3D.CrossProduct(ray.Direction, edge2);
            double det = Vector3D.DotProduct(edge1, pvec);
            // if determinant is near zero, ray lies in plane of triangle
            double EPSILON = 0.00001;
            if (det > -EPSILON && det < EPSILON)
                return false;
            double invdet = 1.0 / det;

            // calculate distance from vert0 to ray origin
            Vector3D tvec = pt0 - ray.Origin;

            // calculate U parameter and test bounds
            u = -Vector3D.DotProduct(tvec, pvec) * invdet;
            if (u < 0.0 || u > 1.0)
                return false;

            // prepare to test V parameter
            Vector3D qvec = Vector3D.CrossProduct(tvec, edge1);

            // calculate vparameter
            v = -Vector3D.DotProduct(ray.Direction, qvec) * invdet;
            if (v < 0.0 || u + v > 1.0)
                return false;

            // calculate t, ray intersects triangle
            double t = Vector3D.DotProduct(edge2, qvec) * invdet;
            return true;
        }

        public Face Transform(Transform3D transf)
        {
            Vector3D[] points = new Vector3D[4];
            for (int i = 0; i < 4; ++i)
                points[i] = transf.transform(Points[i]);
            Face faceNew = new Face(PickingId, points, ColorFill, ColorPath, IsSolid);
            foreach (Texture t in Textures)
                faceNew.Textures.Add(t);
            return faceNew;
        }
        #endregion

        #region Object override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Face => PickId : {0} Points : ", PickingId));
            foreach (Vector3D point in Points)
                sb.Append(point.ToString());
            return sb.ToString();
        }
        #endregion
    }
    #endregion

    #region Face comparison
    public class FaceComparison : IComparer<Face>
    {
        #region Constructor
        /// <summary>
        /// Constructor FaceComparison
        /// </summary>
        /// <param name="transform">Transform</param>
        public FaceComparison(Transform3D transform)
        {
            _transform = transform;
        }
        #endregion

        #region Implementation IComparer
        /// <summary>
        /// Implementation of IComparer
        /// </summary>
        /// <param name="f1">Face f1</param>
        /// <param name="f2">Face f2</param>
        /// <returns>integer "f1>f2 => 1", "f1<f2 => -1", "f1==f2 => 0"  </returns>
        public int Compare(Face f1, Face f2)
        {
            int mode = 0;
            if (0 == mode) // use barycenter.Z in global coordinate then in distance
            {
                if (f1.Center.Z > f2.Center.Z)
                    return 1;
                else if (f1.Center.Z == f2.Center.Z)
                {
                    if (_transform.transform(f1.Center).Z < _transform.transform(f2.Center).Z)
                        return 1;
                    else if (_transform.transform(f1.Center).Z == _transform.transform(f2.Center).Z)
                        return 0;
                    else
                        return -1;
                }
                else
                    return -1;
            }
            else if (1 == mode) // use barycenter distance to eye only
            {
                if (_transform.transform(f1.Center).Z < _transform.transform(f2.Center).Z)
                    return 1;
                else if (_transform.transform(f1.Center).Z == _transform.transform(f2.Center).Z)
                    return 0;
                else
                    return -1;
            }
            else
            {
                double length1 = 0.0, length2 = 0.0;
                foreach (Vector3D pt in f1.Points)
                    length1 += _transform.transform(pt).Z;
                foreach (Vector3D pt in f2.Points)
                    length2 += _transform.transform(pt).Z;

                if (length1 < length2)
                    return 1;
                else if (length1 == length2)
                    return 0;
                else
                    return -1;
            }
        }
        #endregion

        #region Data members
        Transform3D _transform;
        #endregion
    }
    #endregion
}
