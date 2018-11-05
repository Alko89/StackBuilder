#region Using directives
using System.Collections.Generic;

using Sharp3D.Math.Core;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class HAnalysisPallet : HAnalysis
    {
        #region Constructor
        public HAnalysisPallet(Document doc) : base(doc)
        {
            ConstraintSet = new HConstraintSetPallet() { MaximumHeight = 1700.0 };
        }
        #endregion

        #region Specific properties
        public PalletProperties Pallet { set { _containers.Clear(); _containers.Add(value); }  }
        #endregion

        #region Override HAnalysis
        public override Vector3D DimContainer(int index)
        {
            if (index < _containers.Count && _containers[index] is PalletProperties palletProperties)
                return new Vector3D(palletProperties.Length, palletProperties.Width, ConstraintSet.MaximumHeight - palletProperties.Height);
            else
                return Vector3D.Zero;
        }
        public override Vector3D Offset(int index)
        {
            if (index < _containers.Count && _containers[index] is PalletProperties palletProperties)
                return new Vector3D(0.0, 0.0, palletProperties.Height);
            else
                return Vector3D.Zero;
        }
        public override BBox3D AdditionalBoudingBox(int index)
        {
            if (index < _containers.Count && _containers[index] is PalletProperties palletProperties)
                return palletProperties.BoundingBox;
            else
                return BBox3D.Initial;
        }
        #endregion
    }
}
