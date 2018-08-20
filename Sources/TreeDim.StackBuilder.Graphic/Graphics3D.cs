#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public abstract class Graphics3D
    {
        #region Enums
        enum PaintingAlgorithm
        {
            ALGO_PAINTER,
            ALGO_BSPTREE
        }
        public enum FaceDir
        {
            FRONT
            , BACK
        }

        #endregion
        #region Data members

        /// <summary>
        /// Viewport
        /// </summary>
        private readonly float[] _viewport = new float[4];
        /// <summary>
        /// Compute viewport automatically if enabled
        /// </summary>
        private readonly bool _autoViewport = true;

        /// <summary>
        /// face in the background
        /// </summary>
        private List<Face> _facesBackground = new List<Face>();

        /// <summary>
        /// image inst
        /// </summary>
        private List<ImageInst> _listImageInst = new List<ImageInst>();
        private List<ImageCached> _listImageCached = new List<ImageCached>();
        /// <summary>
        /// dimensions cube
        /// </summary>
        private List<DimensionCube> _dimensions = new List<DimensionCube>();
        private uint _boxDrawingCounter = 0;
        private static double _cameraDistance = 100000.0;

        public static readonly Vector3D Front = new Vector3D(_cameraDistance, 0.0, 0.0);
        public static readonly Vector3D Back = new Vector3D(-_cameraDistance, 0.0, 0.0);
        public static readonly Vector3D Left = new Vector3D(0.0, -_cameraDistance, 0.0);
        public static readonly Vector3D Right = new Vector3D(0.0, _cameraDistance, 0.0);
        public static readonly Vector3D Top = new Vector3D(0.0, 0.0, _cameraDistance);
        public static readonly Vector3D Corner_0 = new Vector3D(
                Math.Cos(45.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
                , Math.Sin(45.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
                , _cameraDistance);
        public static readonly Vector3D Corner_90 = new Vector3D(
             Math.Cos(135.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , Math.Sin(135.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , _cameraDistance);
        public static readonly Vector3D Corner_180 = new Vector3D(
             Math.Cos(225.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , Math.Sin(225.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , _cameraDistance);
        public static readonly Vector3D Corner_270 = new Vector3D(
             Math.Cos(315.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , Math.Sin(315.0 * Math.PI / 180.0) * Math.Sqrt(2.0) * _cameraDistance
             , _cameraDistance);

        private static ILog _log = LogManager.GetLogger(typeof(Graphics3D));
        #endregion

        #region Constructors
        public Graphics3D()
        {
            _viewport[0] = -500.0f;
            _viewport[1] = -500.0f;
            _viewport[2] = 500.0f;
            _viewport[3] = 500.0f;

            GridFontSize = 8;
        }
        #endregion

        #region Public properties
        public float FontSizeRatio { get; set; } = 0.015f;
        public float FontSize
        {
            get
            {
                if (null == Size)
                    return 0.0f;
                else
                    return FontSizeRatio * Size.Height;
            }
        }
        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;
        /// <summary>
        /// View point (position of the observer's eye)
        /// </summary>
        public Vector3D CameraPosition { get; set; } = new Vector3D(0.0, 0.0, -1000.0);
        /// <summary>
        /// Target point
        /// </summary>
        public Vector3D Target { get; set; } = Vector3D.Zero;
        /// <summary>
        /// Light direction
        /// </summary>
        public Vector3D VLight
        {
            get
            {
                Vector3D vLight = (CameraPosition + 5000 * Vector3D.ZAxis) - Target;
                vLight.Normalize();
                return vLight;
            }
        }
        /// <summary>
        /// View direction
        /// </summary>
        public Vector3D ViewDirection
        {
            get
            {
                Vector3D viewDir = Target - CameraPosition;
                viewDir.Normalize();
                return viewDir;
            }
        }
        public double MarginPercentage { get; set; } = 0.01;

        /// <summary>
        /// boolean to show box ids
        /// To be used when debugging
        /// </summary>
        public bool ShowBoxIds { get; set; } = false;
        /// <summary>
        /// boolean to show case testures
        /// </summary>
        public bool ShowTextures { get; set; } = true;
        /// <summary>
        ///  boolean to show or hide dimensions
        /// </summary>
        public bool ShowDimensions { get; set; } = true;
        /// <summary>
        /// gets or sets boxel order status
        /// If set to true the boxel order will be used when drawing box layers
        /// </summary>
        public bool UseBoxelOrderer { get; set; } = true;
        /// <summary>
        /// sort faces using face comparer (painter algo)
        /// </summary>
        public bool EnableFaceSorting { get; set; } = true;

        public Point Offset
        {
            get { return TransformPoint(GetCurrentTransformation(), Vector3D.Zero); }
        }
        #endregion

        #region Private properties
        private int GridFontSize { get; set; }
        #endregion

        #region Helpers
        private Point[] TransformPoint(Transform3D transform, Vector3D[] points3d)
        {
            Point[] points = new Point[points3d.Length];
            int i = 0;
            foreach (Vector3D v in points3d)
            {
                Vector3D vt = transform.transform(v);
                points[i] = new Point((int)vt.X, (int)vt.Y);
                ++i;
            }
            return points;
        }
        private Point TransformPoint(Transform3D transform, Vector3D point3d)
        {
            Vector3D vt = transform.transform(point3d);
            return new Point((int)vt.X, (int)vt.Y);
        }
        public Transform3D GetWorldToEyeTransformation()
        {
            /*
            Orthographic transformation chain
            • Start with coordinates in object’s local coordinates
            • Transform into world coords (modeling transform, Mm)
            • Transform into eye coords (camera xf., Mcam = Fc–1)
            • Orthographic projection, Morth
            • Viewport transform, Mvp

            ps = Mvp*Morth*Mcam*Mm*po
            */
            Vector3D zaxis = CameraPosition - Target;
            zaxis.Normalize();
            Vector3D up = Vector3D.ZAxis;
            Vector3D xaxis = Vector3D.CrossProduct(up, zaxis);
            if (Vector3D.CrossProduct(up, zaxis).GetLengthSquared() < 0.0001)
            {
                up = Vector3D.ZAxis;
                xaxis = Vector3D.XAxis;
            }
            xaxis.Normalize();
            Vector3D yaxis = Vector3D.CrossProduct(zaxis, xaxis);
            Matrix4D Mcam = new Matrix4D(
                    xaxis.X, xaxis.Y, xaxis.Z, -Vector3D.DotProduct(CameraPosition - Target, xaxis),
                    yaxis.X, yaxis.Y, yaxis.Z, -Vector3D.DotProduct(CameraPosition - Target, yaxis),
                    -zaxis.X, -zaxis.Y, -zaxis.Z, -Vector3D.DotProduct(CameraPosition - Target, -zaxis),
                    0.0, 0.0, 0.0, 1.0
                );
            return new Transform3D(Mcam);
        }
        private Transform3D GetOrthographicProjection(Vector3D vecMin, Vector3D vecMax)
        {
            double[] sizeMin = new double[3];
            sizeMin[0] = 1.0;
            sizeMin[1] = Size.Height;
            sizeMin[2] = 0.0;

            double[] sizeMax = new double[3];
            sizeMax[0] = Size.Width;
            sizeMax[1] = 1.0;
            sizeMax[2] = 1.0;
            return Transform3D.OrthographicProjection(vecMin, vecMax, sizeMin, sizeMax);
        }
        /// <summary>
        /// Background faces
        /// </summary>
        /// <param name="face">Face to be drawn before other faces</param>
        public void AddFaceBackground(Face face)
        {
            _facesBackground.Add(face);
        }
        /// <summary>
        /// add face
        /// </summary>
        /// <param name="face">Face item</param>
        public void AddFace(Face face)
        {
            Faces.Add(face);
        }
        public void AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
        }
        public void AddBox(Box box)
        {
            if (!box.IsValid)
                throw new GraphicsException("Box is invalid and cannot be drawn!");
            Boxes.Add(box);
        }

        public void AddCylinder(Cylinder cylinder)
        {
            Cylinders.Add(cylinder);
        }

        public void AddDimensions(DimensionCube dimensionCube)
        {
            _dimensions.Add(dimensionCube);
        }
        #endregion

        #region Abstract methods and properties
        abstract public Size Size { get; }
        abstract public System.Drawing.Graphics Graphics { get; }
        public List<Cylinder> Cylinders { get; set; } = new List<Cylinder>();
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public List<Segment> SegmentsBackground { get; set; } = new List<Segment>();
        public List<Box> Boxes { get; set; } = new List<Box>();
        public List<Face> Faces { get; set; } = new List<Face>();
        public List<Triangle> Triangles { get; set; } = new List<Triangle>();
        public Transform3D CurrentTransf { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// SetViewport
        /// </summary>
        /// <param name="xmin">xmin -> bottom</param>
        /// <param name="ymin">ymin -> left</param>
        /// <param name="xmax">xmax -> right</param>
        /// <param name="ymax">ymax -> top</param>
        public void SetViewport(float xmin, float ymin, float xmax, float ymax)
        {
            _viewport[0] = xmin;
            _viewport[1] = ymin;
            _viewport[2] = xmax;
            _viewport[3] = ymax;
        }

        /// <summary>
        /// Draw all entities stored in buffer
        /// </summary>
        public void Flush()
        {
            // initialize
            Vector3D vLight = CameraPosition - Target; vLight.Normalize();
            _boxDrawingCounter = 0;
            CurrentTransf = null;
            System.Drawing.Graphics g = Graphics;
            g.Clear(BackgroundColor);

            if (EnableFaceSorting)
            {
                // sort face list
                FaceComparison faceComparer = new FaceComparison(GetWorldToEyeTransformation());
                Faces.Sort(faceComparer);
            }
            // draw background segments
            foreach (Segment s in SegmentsBackground)
                Draw(s);
            // draw background faces
            foreach (Face face in _facesBackground)
                Draw(face, FaceDir.FRONT);
            // draw all faces using solid / transparency
            foreach (Face face in Faces)
                Draw(face, FaceDir.BACK);
            // draw triangles
            foreach (Triangle tr in Triangles)
                Draw(tr, FaceDir.FRONT);

            // sort box list
            if (UseBoxelOrderer)
            {
                BoxelOrderer boxelOrderer = new BoxelOrderer(Boxes) { Direction = Target - CameraPosition };
                Boxes = boxelOrderer.GetSortedList();
            }
            else
                Boxes.Sort(new BoxComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));

            // sort cylinder list
            Cylinders.Sort(new CylinderComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));

            if (Cylinders.Count > 0)
            {
                // sort by Z
                List<Drawable> drawableList = new List<Drawable>();
                drawableList.AddRange(Boxes);
                drawableList.AddRange(Cylinders);
                drawableList.Sort(new DrawableComparerSimplifiedPainterAlgo());

                List<Box> boxes = new List<Box>();
                List<Cylinder> cylinders = new List<Cylinder>();
                bool processingBox = drawableList[0] is Box;
                foreach (Drawable drawable in drawableList)
                {
                    Box b = drawable as Box;
                    Cylinder c = drawable as Cylinder;

                    if ((null != b) && processingBox)
                        boxes.Add(b);
                    else if ((null == b) && !processingBox)
                        cylinders.Add(c);
                    else
                    {
                        if (boxes.Count > 0)
                        {
                            BoxelOrderer boxelOrderer = new BoxelOrderer(boxes) { Direction = Target - CameraPosition };
                            boxes = boxelOrderer.GetSortedList();
                            // draw boxes
                            foreach (Box bb in boxes)
                                Draw(bb);
                            // clear
                            boxes.Clear();
                        }
                        if (cylinders.Count > 0)
                        {
                            cylinders.Sort(new CylinderComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));
                            // draw cylinders
                            foreach (Cylinder cc in cylinders)
                                Draw(cc);
                            // clear
                            cylinders.Clear();
                        }
                        if (null != b)
                        {
                            boxes.Add(b);
                            processingBox = true;
                        }
                        else
                        {
                            cylinders.Add(c);
                            processingBox = false;
                        }
                    }
                }

                // remaining boxes
                BoxelOrderer boxelOrdererRem = new BoxelOrderer(boxes) { Direction = Target - CameraPosition };
                boxes = boxelOrdererRem.GetSortedList();
                // draw boxes
                foreach (Box bb in boxes)
                    Draw(bb);

                // remaining cylinders
                cylinders.Sort(new CylinderComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));
                // draw cylinders
                foreach (Cylinder cc in cylinders)
                    Draw(cc);
                // clear
                boxes.Clear();
            }
            else
            {
                // draw all boxes
                foreach (Box box in Boxes)
                    Draw(box);
                // draw all triangles
                foreach (Triangle tr in Triangles)
                    Draw(tr, FaceDir.FRONT);
            }
            // images inst
            if (_listImageInst.Count > 0)
            {
                // --- sort image inst
                Analysis analysis = _listImageInst[0].Analysis;
                BBox3D bbox = analysis.Solution.BBoxGlobal;
                List<Box> boxesImage = new List<Box>();
                foreach (ImageInst imageInst in _listImageInst)
                    boxesImage.Add(imageInst.ToBox());

                if (UseBoxelOrderer && false) // NOT WORKING ? 
                {
                    BoxelOrderer boxelOrderer = new BoxelOrderer(boxesImage);
                    boxelOrderer.TuneParam = 10.0;
                    boxelOrderer.Direction = Target - CameraPosition;
                    boxesImage = boxelOrderer.GetSortedList();
                }
                else
                    boxesImage.Sort(new BoxComparerSimplifiedPainterAlgo(GetWorldToEyeTransformation()));
                // ---

                List<ImageInst> listImageInstSorted = new List<ImageInst>();
                foreach (Box b in boxesImage)
                    listImageInstSorted.Add(new ImageInst(analysis, new Vector3D(b.Length, b.Width, b.Height), b.BPosition));
                
                // draw image inst
                foreach (ImageInst im in listImageInstSorted)
                    Draw(im);
            }
            // draw faces : end
            foreach (Face face in Faces)
                Draw(face, FaceDir.FRONT);

            // draw segment list (e.g. hatching)
            foreach (Segment seg in Segments)
                Draw(seg);

            // draw cotation cubes
            if (ShowDimensions)
            {
                foreach (DimensionCube qc in _dimensions)
                    qc.Draw(this);
            }
        }

        public Transform3D GetCurrentTransformation()
        {
            if (null == CurrentTransf)
            {
                // get transformations
                Transform3D world2eye = GetWorldToEyeTransformation();
                Transform3D orthographicProj = GetOrthographicProjection(
                    new Vector3D(_viewport[0], _viewport[1], -10000.0)
                    , new Vector3D(_viewport[2], _viewport[3], 10000.0));

                // build automatic viewport
                if (_autoViewport)
                {
                    Vector3D vecMin = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
                    Vector3D vecMax = new Vector3D(double.MinValue, double.MinValue, double.MinValue);

                    // boxes
                    foreach (Box box in Boxes)
                        foreach (Vector3D pt in box.Points)
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }
                    // triangles
                    foreach (var tr in Triangles)
                        foreach (Vector3D pt in tr.Points)
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }
                    // cylinders
                    foreach (Cylinder cyl in Cylinders)
                    {
                        foreach (Vector3D pt in cyl.BottomPoints)
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }
                        foreach (Vector3D pt in cyl.TopPoints)
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }
                    }
                    // faces
                    foreach (Face face in Faces)
                        foreach (Vector3D pt in face.Points)
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }

                    // segments
                    foreach (Segment seg in Segments)
                        foreach (Vector3D pt in seg.Points)
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }

                    // cube dimensions
                    foreach (DimensionCube dimCube in _dimensions)
                        foreach (Vector3D pt in dimCube.DrawingPoints(this))
                        {
                            Vector3D ptT = world2eye.transform(pt);
                            vecMin.X = Math.Min(vecMin.X, ptT.X);
                            vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                            vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                            vecMax.X = Math.Max(vecMax.X, ptT.X);
                            vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                            vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                        }

                    Vector3D vecMin1 = vecMin, vecMax1 = vecMax;
                    // adjust width/height
                    if ((vecMax.Y - vecMin.Y) / Size.Height > (vecMax.X - vecMin.X) / Size.Width)
                    {
                        double actualWidth = (vecMax.Y - vecMin.Y) * Size.Width / Size.Height;
                        vecMin1.X = 0.5 * (vecMin.X + vecMax.X) - 0.5 * actualWidth;
                        vecMax1.X = 0.5 * (vecMin.X + vecMax.X) + 0.5 * actualWidth;
                    }
                    else
                    {
                        double actualHeight = (vecMax.X - vecMin.X) * Size.Height / Size.Width;
                        vecMin1.Y = 0.5 * (vecMin.Y + vecMax.Y) - 0.5 * actualHeight;
                        vecMax1.Y = 0.5 * (vecMin.Y + vecMax.Y) + 0.5 * actualHeight;
                    }
                    // set margins
                    double width = vecMax1.X - vecMin1.X;
                    vecMin1.X -= MarginPercentage * width;
                    vecMax1.X += MarginPercentage * width;
                    double height = vecMax1.Y - vecMin1.Y;
                    vecMin1.Y -= MarginPercentage * height;
                    vecMax1.Y += MarginPercentage * height;

                    orthographicProj = GetOrthographicProjection(vecMin1, vecMax1);
                }
                CurrentTransf = orthographicProj * world2eye;
            }
            return CurrentTransf;
        }
        public Transform3D GetCurrentTransformation(List<Vector3D> points)
        {
            if (null == CurrentTransf)
            {
                // get transformations
                Transform3D world2eye = GetWorldToEyeTransformation();
                Transform3D orthographicProj = GetOrthographicProjection(
                    new Vector3D(_viewport[0], _viewport[1], -10000.0)
                    , new Vector3D(_viewport[2], _viewport[3], 10000.0));

                // build automatic viewport
                if (_autoViewport)
                {
                    Vector3D vecMin = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
                    Vector3D vecMax = new Vector3D(double.MinValue, double.MinValue, double.MinValue);

                    foreach (Vector3D pt in points)
                    {
                        Vector3D ptT = world2eye.transform(pt);
                        vecMin.X = Math.Min(vecMin.X, ptT.X);
                        vecMin.Y = Math.Min(vecMin.Y, ptT.Y);
                        vecMin.Z = Math.Min(vecMin.Z, ptT.Z);
                        vecMax.X = Math.Max(vecMax.X, ptT.X);
                        vecMax.Y = Math.Max(vecMax.Y, ptT.Y);
                        vecMax.Z = Math.Max(vecMax.Z, ptT.Z);
                    }

                    Vector3D vecMin1 = vecMin, vecMax1 = vecMax;
                    // adjust width/height
                    if ((vecMax.Y - vecMin.Y) / Size.Height > (vecMax.X - vecMin.X) / Size.Width)
                    {
                        double actualWidth = (vecMax.Y - vecMin.Y) * Size.Width / Size.Height;
                        vecMin1.X = 0.5 * (vecMin.X + vecMax.X) - 0.5 * actualWidth;
                        vecMax1.X = 0.5 * (vecMin.X + vecMax.X) + 0.5 * actualWidth;
                    }
                    else
                    {
                        double actualHeight = (vecMax.X - vecMin.X) * Size.Height / Size.Width;
                        vecMin1.Y = 0.5 * (vecMin.Y + vecMax.Y) - 0.5 * actualHeight;
                        vecMax1.Y = 0.5 * (vecMin.Y + vecMax.Y) + 0.5 * actualHeight;
                    }
                    // set margins
                    double width = vecMax1.X - vecMin1.X;
                    vecMin1.X -= MarginPercentage * width;
                    vecMax1.X += MarginPercentage * width;
                    double height = vecMax1.Y - vecMin1.Y;
                    vecMin1.Y -= MarginPercentage * height;
                    vecMax1.Y += MarginPercentage * height;

                    orthographicProj = GetOrthographicProjection(vecMin1, vecMax1);
                }
                CurrentTransf = orthographicProj * world2eye;
            }
            return CurrentTransf;
        }

        public void AddSegmentBackgound(Segment seg)
        { 
            SegmentsBackground.Add(seg);
        }
        public void AddSegment(Segment seg)
        {
            Segments.Add(seg);
        }
        public void AddImage(Analysis analysis, Vector3D vDimensions, BoxPosition boxPosition)
        {
            _listImageInst.Add(new ImageInst(analysis, vDimensions, boxPosition));
        }
        #endregion

        #region Draw box
        /// <summary>
        /// Draw a line segment
        /// </summary>
        /// <param name="seg">Segment object to be drawn</param>
        internal void Draw(Segment seg)
        {
            System.Drawing.Graphics g = Graphics;
            Brush brush = new SolidBrush(seg.Color);
            Pen pen = new Pen(brush);
            Point[] pt = TransformPoint(GetCurrentTransformation(), seg.Points);
            g.DrawLine(pen, pt[0], pt[1]);
        }
        /// <summary>
        /// Draw a text string at a 3D location
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">3D point on which to center the text string</param>
        /// <param name="color">Color of solid brush used to draw text</param>
        /// <param name="fontSize">Size of font used to draw text</param>
        internal void Draw(string text, Vector3D position, Color color, float fontSize)
        {
            System.Drawing.Graphics g = Graphics;
            Point pt = TransformPoint(GetCurrentTransformation(), position);
            Font font = new Font("Arial", fontSize > 1.0f ? fontSize : 1.0f);
            SizeF sizeF = g.MeasureString(text, font);
            g.DrawString(text
                , font
                , new SolidBrush(color)
                , new Point(pt.X - (int)(0.5f * sizeF.Width), pt.Y - (int)(0.5f * sizeF.Height))
                , StringFormat.GenericDefault);
        }

        internal void Draw(Triangle tr, FaceDir dir)
        {
            System.Drawing.Graphics g = Graphics;

            // test if triangle can actually be seen
            if ((Vector3D.DotProduct(tr.Normal, CameraPosition - Target) > 0.0 && dir == FaceDir.BACK)
                || (Vector3D.DotProduct(tr.Normal, CameraPosition - Target) < 0.0 && dir == FaceDir.FRONT))
                return;
            // compute triangle color
            double cosA = Math.Abs(Vector3D.DotProduct(tr.Normal, VLight));
            Color color = Color.FromArgb(
                tr.IsSolid ? 255 : (dir == FaceDir.FRONT ? 64 : 255)
                , (int)(tr.ColorFill.R * cosA)
                , (int)(tr.ColorFill.G * cosA)
                , (int)(tr.ColorFill.B * cosA));
            Brush brush = new SolidBrush(color);
            // draw filled triangle
            Point[] pt = TransformPoint(GetCurrentTransformation(), tr.Points);
            g.FillPolygon(brush, pt);
            // draw path
            Brush brush0 = new SolidBrush(tr.ColorPath);
            for (int i = 1; i < pt.Length; ++i)
                g.DrawLine(new Pen(brush0, 1.5f), pt[i - 1], pt[i]);
            g.DrawLine(new Pen(brush0, 1.5f), pt[pt.Length - 1], pt[0]);
        }

        /// <summary>
        /// Draw a face
        /// </summary>
        /// <param name="face">Face object to be drawn</param>
        internal void Draw(Face face, FaceDir dir)
        {
            System.Drawing.Graphics g = Graphics;

            // test if face can actually be seen
            if ((Vector3D.DotProduct(face.Normal, CameraPosition - Target) > 0.0 && dir == FaceDir.BACK)
                || (Vector3D.DotProduct(face.Normal, CameraPosition - Target) < 0.0 && dir == FaceDir.FRONT))
                return;

            // compute face color
            double cosA = Math.Abs(Vector3D.DotProduct(face.Normal, VLight));
            Color color = Color.FromArgb(
                face.IsSolid ? 255 : (dir == FaceDir.FRONT ? 64 : 255)
                , (int)(face.ColorFill.R * cosA)
                , (int)(face.ColorFill.G * cosA)
                , (int)(face.ColorFill.B * cosA));
            Point[] pt = TransformPoint(GetCurrentTransformation(), face.Points);

            Brush brush = new SolidBrush(color);
            g.FillPolygon(brush, pt);
            // draw path
            Brush brush0 = new SolidBrush(face.ColorPath);
            int ptCount = pt.Length;
            for (int i = 1; i < ptCount; ++i)
            {
                // there is a bug here!
                // -> a polygon that result from first split will lose all edges
                // when split a second time
                g.DrawLine(new Pen(brush0, 1.5f), pt[i - 1], pt[i]);
            }
            g.DrawLine(new Pen(brush0, 1.5f), pt[ptCount - 1], pt[0]);
        }

        internal void Draw(Face face, FaceDir dir, Color colorApply, bool transparent)
        {
            System.Drawing.Graphics g = Graphics;

            // test if face can actuallt be seen
            if ((Vector3D.DotProduct(face.Normal, CameraPosition - Target) > 0.0 && dir == FaceDir.BACK)
                || (Vector3D.DotProduct(face.Normal, CameraPosition - Target) < 0.0 && dir == FaceDir.FRONT))
                return;

            // compute face color
            double cosA = Math.Abs(Vector3D.DotProduct(face.Normal, VLight));
            Color color = Color.FromArgb(
                transparent ? 64 : 255
                , (int)(colorApply.R * cosA)
                , (int)(colorApply.G * cosA)
                , (int)(colorApply.B * cosA));
            Point[] pt = TransformPoint(GetCurrentTransformation(), face.Points);

            Brush brush = new SolidBrush(color);
            g.FillPolygon(brush, pt);
            // draw path
            float fThickness = transparent ? 2.0f : 1.0f;
            Brush brush0 = new SolidBrush(face.ColorPath);
            int ptCount = pt.Length;
            for (int i = 1; i < ptCount; ++i)
            {
                // there is a bug here!
                // -> a polygon that result from first split will lose all edges
                // when split a second time
                g.DrawLine(new Pen(brush0, fThickness), pt[i - 1], pt[i]);
            }
            g.DrawLine(new Pen(brush0, fThickness), pt[ptCount - 1], pt[0]);
        }

        internal void Draw(Box box)
        {
            System.Drawing.Graphics g = Graphics;

            if (box is Pack)
            {
                Pack pack = box as Pack;
                pack.Draw(this);
            }
            else
            {
                Vector3D[] points = box.Points;

                Face[] faces = box.Faces;
                for (int i = 0; i < 6; ++i)
                {
                    // Face
                    Face face = faces[i];
                    // face normal
                    Vector3D normal = face.Normal;
                    // visible ?
                    if (!faces[i].IsVisible(Target - CameraPosition))
                        continue;
                    // color
                    faces[i].ColorFill = box.Colors[i];
                    double cosA = System.Math.Abs(Vector3D.DotProduct(faces[i].Normal, VLight));
                    Color color = Color.FromArgb((int)(faces[i].ColorFill.R * cosA), (int)(faces[i].ColorFill.G * cosA), (int)(faces[i].ColorFill.B * cosA));
                    // points
                    Vector3D[] points3D = faces[i].Points;
                    Point[] pt = TransformPoint(GetCurrentTransformation(), points3D);
                    //  draw solid face
                    Brush brush = new SolidBrush(color);
                    g.FillPolygon(brush, pt);
                    // draw textures
                    if (null != face.Textures && ShowTextures)
                        foreach (Texture texture in face.Textures)
                        {
                            Point[] ptsImage = TransformPoint(GetCurrentTransformation(), box.PointsImage(i, texture));
                            Point[] pts = new Point[3];
                            pts[0] = ptsImage[3];
                            pts[1] = ptsImage[2];
                            pts[2] = ptsImage[0];
                            g.DrawImage(texture.Bitmap, pts);
                        }
                    // draw path
                    Brush brushPath = new SolidBrush(faces[i].ColorPath);
                    Pen penPathThick = new Pen(brushPath, box.IsBundle ? 2.0f : 1.5f);
                    int ptCount = pt.Length;
                    for (int j = 1; j < ptCount; ++j)
                        g.DrawLine(penPathThick, pt[j - 1], pt[j]);
                    g.DrawLine(penPathThick, pt[ptCount - 1], pt[0]);
                    // draw bundle lines
                    if (box.IsBundle && i < 4)
                    {
                        Pen penPathThin = new Pen(brushPath, 1.5f);
                        int noSlice = Math.Min(box.BundleFlats, 4);
                        for (int iSlice = 0; iSlice < noSlice - 1; ++iSlice)
                        {
                            Vector3D[] ptSlice = new Vector3D[2];
                            ptSlice[0] = points3D[0] + ((double)(iSlice + 1) / (double)noSlice) * (points3D[3] - points3D[0]);
                            ptSlice[1] = points3D[1] + ((double)(iSlice + 1) / (double)noSlice) * (points3D[2] - points3D[1]);

                            Point[] pt2D = TransformPoint(GetCurrentTransformation(), ptSlice);
                            g.DrawLine(penPathThin, pt2D[0], pt2D[1]);
                        }
                    }
                }

                // draw box tape
                if (box.ShowTape && faces[5].IsVisible(Target - CameraPosition))
                {
                    // get color
                    double cosA = Math.Abs(Vector3D.DotProduct(faces[5].Normal, VLight));
                    Color color = Color.FromArgb((int)(box.TapeColor.R * cosA), (int)(box.TapeColor.G * cosA), (int)(box.TapeColor.B * cosA));
                    // instantiate brush
                    Brush brushTape = new SolidBrush(color);
                    // get tape points
                    Point[] pts = TransformPoint(GetCurrentTransformation(), box.TapePoints);
                    // fill polygon
                    g.FillPolygon(brushTape, pts);
                    // draw path
                    Brush brushPath = new SolidBrush(faces[5].ColorPath);
                    Pen penPathThick = new Pen(brushPath, 1.5f);
                    int ptCount = pts.Length;
                    for (int j = 1; j < ptCount; ++j)
                        g.DrawLine(penPathThick, pts[j - 1], pts[j]);
                    g.DrawLine(penPathThick, pts[ptCount - 1], pts[0]);
                }

                // drav custom text on boxes
                if (CameraPosition.X < 0.1 && CameraPosition.Y < 0.1 && CameraPosition.Z == 100000 && box.ShowTopLabel)
                {
                    //Draw box text
                    double elipseSize = (box.Length < box.Width ? box.Length : box.Width) / 2;

                    Brush brushElipse = new SolidBrush(Color.White);
                    Cylinder c = new Cylinder(Int32.MaxValue, elipseSize, elipseSize, box.TopFace.Center.Z, Color.White, Color.White, Color.White);
                    Vector3D[] elipseVectors = c.TopPoints;
                    for (int i = 0; i < elipseVectors.Length; i++)
                    {
                        elipseVectors[i].X += (int)box.TopFace.Center.X;
                        elipseVectors[i].Y += (int)box.TopFace.Center.Y;
                    }
                    Point[] elipsePoints = TransformPoint(GetCurrentTransformation(), elipseVectors);
                    g.FillPolygon(brushElipse, elipsePoints);

                    Font fW = GetAdjustedFont(g, box.Weight.ToString(), new Font("Arial", 12, FontStyle.Bold), (int)(elipsePoints[0].X - elipsePoints[elipsePoints.Length / 2].X), 40, 3, true);

                    Point ptId = TransformPoint(GetCurrentTransformation(), box.TopFace.Center);

                    // draw box weight
                    SizeF adjustedSizeNew = g.MeasureString(box.Weight.ToString(), fW);

                    g.DrawString(
                        box.Weight.ToString()
                        , fW
                        , Brushes.Black
                        , new Point((int)(ptId.X - adjustedSizeNew.Width / 2), (int)(ptId.Y - adjustedSizeNew.Height / 2))
                        , StringFormat.GenericDefault);
                }
            }
            if (ShowBoxIds)
            {
                // draw box id
                Point ptId = TransformPoint(GetCurrentTransformation(), box.TopFace.Center);
                g.DrawString(
                    box.PickId.ToString()
                    , new Font("Arial", GridFontSize)
                    , Brushes.Black
                    , new Rectangle(ptId.X - 15, ptId.Y - 10, 30, 20)
                    , StringFormat.GenericDefault);
                g.DrawString(
                    _boxDrawingCounter.ToString()
                    , new Font("Arial", GridFontSize)
                    , Brushes.Red
                    , new Rectangle(ptId.X + 5, ptId.Y - 10, 30, 20)
                    , StringFormat.GenericDefault);
            }

            ++_boxDrawingCounter;
        }

        public Font GetAdjustedFont(System.Drawing.Graphics graphicRef, string graphicString, Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail)
        {
            Font testFont = null;
            // We utilize MeasureString which we get via a control instance
            for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                // Test the string with the new size
                SizeF adjustedSizeNew = graphicRef.MeasureString(graphicString, testFont);

                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    // Good font, return it
                    return testFont;
                }
            }

            // If you get here there was no fontsize that worked
            // return minimumSize or original?
            if (smallestOnFail)
            {
                return testFont;
            }
            else
            {
                return originalFont;
            }
        }

        internal void DrawWireFrame(Box box)
        {
        }

        internal void Draw(Pack pack)
        {
            System.Drawing.Graphics g = Graphics;
            Vector3D[] points = pack.Points;
        }

        internal void Draw(ImageInst img)
        {
            Point ptImg = TransformPoint(GetCurrentTransformation(), img.PointBase);

            Bitmap bmp = null;
            Point ptOffset = Point.Empty;
            GetCachedBitmap(img, ref bmp, ref ptOffset);
            
            System.Drawing.Graphics g = Graphics;
            g.DrawImage(bmp, new Point(ptImg.X - ptOffset.X, ptImg.Y - ptOffset.Y));
        }
        internal void GetCachedBitmap(ImageInst img, ref Bitmap bmp, ref Point offset)
        {
            ImageCached imgCached = _listImageCached.Find(delegate(ImageCached imgc) { return imgc.Matches(img); });
            if (null == imgCached)
            {
                imgCached = new ImageCached(img.Analysis, img.AxisLength, img.AxisWidth);
                _listImageCached.Add(imgCached);
            }
            Analysis analysis = imgCached.Analysis;

            // *** get size in pixels
            int xmin = int.MaxValue, ymin = int.MaxValue, xmax = int.MinValue, ymax=int.MinValue;
            BBox3D bbox = analysis.Solution.BBoxGlobal;
            foreach (Vector3D vPt in bbox.Corners)
            {
                Point pt = TransformPoint(GetCurrentTransformation() * imgCached.RelativeTransf, vPt);
                xmin = Math.Min(xmin, pt.X);
                xmax = Math.Max(xmax, pt.X);
                ymin = Math.Min(ymin, pt.Y);
                ymax = Math.Max(ymax, pt.Y);
            }
            Size s = new Size(xmax-xmin + 1, ymax-ymin + 1);
            // ***

            Point ptZero = TransformPoint(GetCurrentTransformation(), Vector3D.Zero);
            bmp = imgCached.Image(s, CameraPosition, Target, ref offset);
        }
        #endregion

        #region Draw cylinder
        internal void Draw(Cylinder cyl)
        {
            System.Drawing.Graphics g = Graphics;

            // build pen path
            Brush brushPath = new SolidBrush(cyl.ColorPath);
            Pen penPathThick = new Pen(brushPath, 1.7f);
            Pen penPathThin = new Pen(brushPath, 1.5f);

            // bottom (draw only path)
            Point[] ptsBottom = TransformPoint(GetCurrentTransformation(), cyl.BottomPoints);
            g.DrawPolygon(penPathThick, ptsBottom);
            // top
            Point[] ptsTop = TransformPoint(GetCurrentTransformation(), cyl.TopPoints);
            g.DrawPolygon(penPathThick, ptsTop);

            // outer wall
            Face[] facesWalls = cyl.FacesWalls;
            foreach (Face face in facesWalls)
            {
                try
                {
                    Vector3D normal = face.Normal;
                    // visible ?
                    if (!face.IsVisible(Target - CameraPosition))
                        continue;

                    // color
                    double cosA = System.Math.Abs(Vector3D.DotProduct(face.Normal, VLight));
                    if (cosA < 0 || cosA > 1) cosA = 1.0;
                    Color color = Color.FromArgb((int)(face.ColorFill.R * cosA), (int)(face.ColorFill.G * cosA), (int)(face.ColorFill.B * cosA));
                    // brush
                    Brush brush = new SolidBrush(color);
                    // draw polygon
                    Point[] ptsFace = TransformPoint(GetCurrentTransformation(), face.Points);
                    g.FillPolygon(brush, ptsFace);
                }
                catch (Exception ex)
                { _log.Error(ex.ToString()); }
            }
            // top
            double cosTop = System.Math.Abs(Vector3D.DotProduct(HalfAxis.ToVector3D(cyl.Position.Direction), VLight));
            Color colorTop = Color.FromArgb((int)(cyl.ColorTop.R * cosTop), (int)(cyl.ColorTop.G * cosTop), (int)(cyl.ColorTop.B * cosTop));
            Brush brushTop = new SolidBrush(colorTop);
            bool topVisible = Vector3D.DotProduct(HalfAxis.ToVector3D(cyl.Position.Direction), Target - CameraPosition) < 0;

            if (cyl.DiameterInner > 0)
            {
                Face[] facesTop = cyl.FacesTop;
                foreach (Face face in facesTop)
                {
                    try
                    {
                        Vector3D normal = face.Normal;

                        // visible ?
                        if (!face.IsVisible(Target - CameraPosition))
                            continue;
                        // color
                        // draw polygon
                        Point[] ptsFace = TransformPoint(GetCurrentTransformation(), face.Points);
                        g.FillPolygon(brushTop, ptsFace);
                    }
                    catch (Exception ex)
                    { _log.Error(ex.ToString()); }
                }
            }
            else
            {
                if (topVisible)
                    g.FillPolygon(brushTop, ptsTop);
                else
                    g.FillPolygon(brushTop, ptsBottom);
            }

            if (topVisible)
                g.DrawPolygon(penPathThin, ptsTop);
            else
                g.DrawPolygon(penPathThin, ptsBottom);

            ++_boxDrawingCounter;
        }
        #endregion
    }
}
