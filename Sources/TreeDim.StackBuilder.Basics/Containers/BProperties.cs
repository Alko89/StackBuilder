using System;
using System.Collections.Generic;
using System.Drawing;

namespace treeDiM.StackBuilder.Basics
{
    public abstract class BProperties : PackableBrickNamed
    {
        public override double Length => _length;
        public override double Width => _width;
        public override double Weight => _weight;
        public abstract Color[] Colors { get; }
        public abstract bool IsBundle { get; }

        public abstract Color GetColor(HalfAxis.HAxis axis);
        public abstract void SetColor(Color color);

        public virtual void SetLength(double length)
        {
            _length = length;
            Modify();
        }

        public virtual void SetWidth(double width)
        {
            _width = width;
            Modify();
        }

        #region Non-Public Members
        protected double _length;
        protected double _width;

        protected BProperties(Document document)
            : base(document)
        {
        }
        protected BProperties(Document document, string name, string description)
            : base(document, name, description)
        {
        }

        #endregion
    }
}
