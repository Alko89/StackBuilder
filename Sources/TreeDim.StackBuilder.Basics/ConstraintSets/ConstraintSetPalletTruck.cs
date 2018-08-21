using System;
using System.Collections.Generic;
using System.Linq;

using Sharp3D.Math.Core;

using log4net;

namespace treeDiM.StackBuilder.Basics
{
    public class ConstraintSetPalletTruck : ConstraintSetAbstract
    {
        public ConstraintSetPalletTruck(IPackContainer container)
        {
            _container = container;
        }
        public override bool AllowOrientation(HalfAxis.HAxis axisOrtho)
        {
            switch (axisOrtho)
            {
                case HalfAxis.HAxis.AXIS_Z_N:
                case HalfAxis.HAxis.AXIS_Z_P:
                    return true;
                default:
                    return false;
            }
        }

        public override string AllowedOrientationsString
        {
            get => "0,0,1";
            set => throw new InvalidOperationException("Setting this property is not supported.");
        }

        public override OptDouble OptMaxHeight
        {
            get
            {
                TruckProperties truck = _container as TruckProperties;
                return new OptDouble(true, truck.Height - MinDistanceLoadRoof);
            } 
        }
        public override OptDouble OptMaxWeight
        {
            get
            {
                TruckProperties truck = _container as TruckProperties;
                return new OptDouble(true, truck.AdmissibleLoadWeight);
            }
        }
        public override bool AllowUncompleteLayer => true;
        public override bool Valid => true;

        public Vector2D MinDistanceLoadWall { get; set; }
        public double MinDistanceLoadRoof { get; set; }
        public bool AllowMultipleLayers { get; set; }
        public override bool CritLayerNumberReached(int layerNumber)
        {
            return AllowMultipleLayers ? false : layerNumber > 1;
        }

        #region Non-Public Members
        private IPackContainer _container;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(ConstraintSetPalletTruck));
        #endregion
    }
}
