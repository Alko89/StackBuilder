using System;

namespace treeDiM.StackBuilder.Basics
{
    public abstract class PackableBrickNamed : PackableBrick
    {
        public PackableBrickNamed(Document parentDocument)
            : base(parentDocument)
        {
        }
        public PackableBrickNamed(Document parentDocument, string name, string description)
            : base(parentDocument)
        {
            ID.SetNameDesc(name, description);
        }

        public override GlobID ID => _id;
        public override double Weight => _weight;
        public override OptDouble NetWeight => _netWeight;

        public virtual void SetWeight(double weight)
        {
            _weight = weight;
            Modify();
        }

        public virtual void SetNetWeight(OptDouble netWeight)
        {
            _netWeight = netWeight;
            Modify();
        }

        public virtual StrapperSet Strappers
        {
            get => _strapperSet; set => _strapperSet = value;
        }

        #region Non-Public Members
        protected GlobID _id = new GlobID();
        protected double _weight;
        protected OptDouble _netWeight = OptDouble.Zero;
        protected StrapperSet _strapperSet;
        #endregion
    }
}