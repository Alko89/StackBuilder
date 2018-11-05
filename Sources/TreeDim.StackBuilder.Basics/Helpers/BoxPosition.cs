#region Using directives
using System;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Box position
    /// </summary>
    public struct BoxPosition
    {
        #region Constructor
        public BoxPosition(Vector3D vPosition, HalfAxis.HAxis dirLength = HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis dirWidth = HalfAxis.HAxis.AXIS_Y_P)
        {
            if (dirLength == dirWidth)
                throw new Exception("Can not create BoxPosition"); 
            Position = vPosition;
            DirectionLength = dirLength;
            DirectionWidth = dirWidth;
        }
        #endregion

        #region Public properties
        public Vector3D Position { get; set; }
        public HalfAxis.HAxis DirectionLength { get; set; }
        public HalfAxis.HAxis DirectionWidth { get; set; }
        public HalfAxis.HAxis DirectionHeight
        {
            get
            {
                if (!IsValid)
                    throw new Exception("Invalid position -> Can not compute DirectionHeight.");
                return HalfAxis.ToHalfAxis(Vector3D.CrossProduct(HalfAxis.ToVector3D(DirectionLength), HalfAxis.ToVector3D(DirectionWidth)));
            }
        }
        public bool IsValid => DirectionLength != DirectionWidth;
        public BBox3D BBox(Vector3D dimensions)
        {
            BBox3D bbox = new BBox3D();
            Vector3D vI = HalfAxis.ToVector3D(DirectionLength);
            Vector3D vJ = HalfAxis.ToVector3D(DirectionWidth);
            Vector3D vK = Vector3D.CrossProduct(vI, vJ);
            Vector3D[] pts = new Vector3D[8];
            pts[0] = Position;
            pts[1] = Position + dimensions.X * vI;
            pts[2] = Position + dimensions.Y * vJ;
            pts[3] = Position + dimensions.X * vI + dimensions.Y * vJ;
            pts[4] = Position + dimensions.Z * vK;
            pts[5] = Position + dimensions.Y * vJ + dimensions.Z * vK;
            pts[6] = Position + HalfAxis.ToVector3D(DirectionWidth) * dimensions.Y;
            pts[7] = Position + HalfAxis.ToVector3D(DirectionLength) * dimensions.X + HalfAxis.ToVector3D(DirectionWidth) * dimensions.Y;
            foreach (Vector3D pt in pts)
                bbox.Extend(pt);
            return bbox;
        }
        #endregion

        #region Static properties
        public static BoxPosition Zero => new BoxPosition(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
        #endregion

        #region Transformation
        public Transform3D Transformation
        {
            get
            {
                // build 4D matrix
                Vector3D vAxisLength = HalfAxis.ToVector3D(DirectionLength);
                Vector3D vAxisWidth = HalfAxis.ToVector3D(DirectionWidth);
                Vector3D vAxisHeight = Vector3D.CrossProduct(vAxisLength, vAxisWidth);
                Matrix4D mat = Matrix4D.Identity;
                mat.M11 = vAxisLength.X; mat.M12 = vAxisWidth.X; mat.M13 = vAxisHeight.X; mat.M14 = Position.X;
                mat.M21 = vAxisLength.Y; mat.M22 = vAxisWidth.Y; mat.M23 = vAxisHeight.Y; mat.M24 = Position.Y;
                mat.M31 = vAxisLength.Z; mat.M32 = vAxisWidth.Z; mat.M33 = vAxisHeight.Z; mat.M34 = Position.Z;
                return new Transform3D(mat);
            }
        }
        public static BoxPosition Transform(BoxPosition boxPosition, Transform3D transform)
        {
            if (!boxPosition.IsValid)
                throw new Exception("Invalid box position : can not transform");
            return new BoxPosition(
                transform.transform(boxPosition.Position)
                , HalfAxis.Transform(boxPosition.DirectionLength, transform)
                , HalfAxis.Transform(boxPosition.DirectionWidth, transform)
                );
        }
        public BoxPosition Transform(Transform3D transform)
        {
            return Transform(this, transform);
        }
        #endregion

        #region Static members
        /// <summary>
        /// This method will be used to build 
        /// </summary>
        /// <param name="pos">Position of left/front/down corner of box</param>
        /// <param name="dimOriented">Projections of box on axes X/Y/Z</param>
        /// <param name="dimOriginal">Box length, width, height</param>
        /// <returns></returns>
        public static BoxPosition FromPositionDimension(Vector3D pos, Vector3D dimOriented, Vector3D dimOriginal)
        {
            BoxPositionIndexed bpi = BoxPositionIndexed.Zero;
            // search for length (dimOriginal[0])
            if (MostlyEqual(dimOriented[0], dimOriginal[0])) 
            {
                // L W H
                if (MostlyEqual(dimOriented[1], dimOriginal[1]))
                    bpi = new BoxPositionIndexed(pos, 1);
                // L H W
                else if (MostlyEqual(dimOriented[1], dimOriginal[2]))
                    bpi = new BoxPositionIndexed(pos, 5);
            }
            else if (MostlyEqual(dimOriented[0], dimOriginal[1]))
            {
                // W L H
                if (MostlyEqual(dimOriented[1], dimOriginal[0]))
                    bpi = new BoxPositionIndexed(pos, 2);
                // W H L
                else if (MostlyEqual(dimOriented[1], dimOriginal[2]))
                    bpi = new BoxPositionIndexed(pos, 3);
            }
            else if (MostlyEqual(dimOriented[0], dimOriginal[2]))
            {
                // H L W
                if (MostlyEqual(dimOriented[1], dimOriginal[0]))
                    bpi = new BoxPositionIndexed(pos, 6);
                // H W L
                else if (MostlyEqual(dimOriented[1], dimOriginal[1]))
                    bpi = new BoxPositionIndexed(pos, 4);
            }
            return bpi.ToBoxPosition(dimOriginal);
        }
        private static bool MostlyEqual(double val0, double val1) => Math.Abs(val1 - val0) < 1.0e-03;
        #endregion

        #region Object method overrides
        public override string ToString() => $"{Position} | ({HalfAxis.ToString(DirectionLength)},{HalfAxis.ToString(DirectionWidth)})";
        #endregion
    }

    public struct BoxPositionIndexed
    {
        #region Constructor
        public BoxPositionIndexed(Vector3D vPosition, int orientation)
        {
            if (orientation < 1 || orientation > 6)
                throw new Exception($"BoxPositionIndexed : cannot have orientation = {orientation}");

            Position = vPosition;
            Orientation = orientation;
        }
        #endregion

        #region Public properties
        public Vector3D Position { get; set; }
        public int Orientation { get; set; }
        public BoxPosition ToBoxPosition(Vector3D dimensions)
        {
            // 1 -> L W H
            // 2 -> W L H
            // 3 -> W H L
            // 4 -> H W L
            // 5 -> L H W
            // 6 -> H L W
            switch (Orientation)
            {
                case 1: return new BoxPosition(Position, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                case 2: return new BoxPosition(Position + dimensions.Y * Vector3D.XAxis, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                case 3: return new BoxPosition(Position, HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_X_P);
                case 4: return new BoxPosition(Position + dimensions.Y * Vector3D.YAxis, HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_Y_N);
                case 5: return new BoxPosition(Position + dimensions.Z * Vector3D.YAxis, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Z_P);
                case 6: return new BoxPosition(Position, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_Z_P);
                default: throw new Exception("BoxPositionIndexed : Invalid orientation!");
            }
        }
        #endregion

        #region Static members
        public static BoxPositionIndexed Zero = new BoxPositionIndexed(Vector3D.Zero, 1);
        #endregion

        #region Object method override
        public override string ToString() => $"{Position} | {Orientation}";
        #endregion
    }
}
