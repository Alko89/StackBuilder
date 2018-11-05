using System;
using System.Text;
using Sharp3D.Math.Core;

namespace treeDiM.StackBuilder.Basics
{
    public abstract class PackableBrick : Packable
    {
        public PackableBrick(Document doc)
            : base(doc)
        {
        }

        public override double Volume { get { return Length * Width * Height; } }
        public abstract double Length { get; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public override Vector3D OuterDimensions => new Vector3D(Length, Width, Height);
        public virtual bool OrientationAllowed(HalfAxis.HAxis axis) { return true; }
        public override bool IsBrick => true;

        public double Dim(int index)
        {
            switch (index)
            {
                case 0: return Length;
                case 1: return Width;
                case 2: return Height;
                default: throw new Exception("Invalid index...");
            }
        }

        public double Dimension(HalfAxis.HAxis axis)
        {
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N:
                case HalfAxis.HAxis.AXIS_X_P:
                    return Length;
                case HalfAxis.HAxis.AXIS_Y_N:
                case HalfAxis.HAxis.AXIS_Y_P:
                    return Width;
                case HalfAxis.HAxis.AXIS_Z_N:
                case HalfAxis.HAxis.AXIS_Z_P:
                    return Height;
                default:
                    return 0.0;
            }
        }

        public double[] Dimensions => new double[] { Length, Width, Height };

        public override string ToString()
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append(base.ToString());
            sBuilder.AppendFormat("Packable => Length = {0}      Width = {1}     Height = {2}"
                , Length, Width, Height
                );
            return sBuilder.ToString();
        }
    }
}