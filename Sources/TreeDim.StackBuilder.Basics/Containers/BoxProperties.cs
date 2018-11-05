using System;
using System.Collections.Generic;
using System.Drawing;

using Sharp3D.Math.Core;

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Box properties (dimensions, colors, textures)
    /// </summary>
    public class BoxProperties : BProperties, IContainer
    {
        /// <summary>
        /// Constructor 1
        /// </summary>
        /// <param name="document">Parent document</param>
        public BoxProperties(Document document)
            : base(document)
        {
            _length = 0.0; _width = 0.0; _height = 0.0;
            HasInsideDimensions = true;
        }
        
        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <param name="document">Parent document</param>
        /// <param name="length">Outer length</param>
        /// <param name="width">Outer width</param>
        /// <param name="height">Outer height</param>
        public BoxProperties(Document document, double length, double width, double height)
            : base(document)
        {
            _length     = length;
            _width      = width;
            _height     = height;
            HasInsideDimensions = false;
        }

        public BoxProperties(Document document, double[] dimensions)
            : base(document)
        {
            _length = dimensions[0];
            _width = dimensions[1];
            _height = dimensions[2];
            HasInsideDimensions = false;

        }

        /// <summary>
        /// Constructor 3
        /// </summary>
        /// <param name="document">Parent document</param>
        /// <param name="length">Outer length</param>
        /// <param name="width">Outer width</param>
        /// <param name="height">Outer height</param>
        /// <param name="insideLength">Inner length</param>
        /// <param name="insideWidth">Inner width</param>
        /// <param name="insideHeight">Inner height</param>
        public BoxProperties(Document document
            , double length, double width, double height
            , double insideLength, double insideWidth, double insideHeight)
            : base(document)
        {
            _length     = length;
            _width      = width;
            _height     = height;
            _insideLength = insideLength;
            _insideWidth = insideWidth;
            _insideHeight = insideHeight;
            HasInsideDimensions = true;
        }

        public override bool IsCase => HasInsideDimensions;

        // Dimensions
        public override double Height => _height;
        public void SetHeight(double height)
        {
            _height = height;
            Modify();
        }

        // InsideDimensions
        public bool HasInsideDimensions { get; }
        public double InsideLength
        {
            get => HasInsideDimensions ? _insideLength : _length;
            set
            {
                _insideLength = value;
                Modify();
            }
        }
        public double InsideWidth
        {
            get => HasInsideDimensions ? _insideWidth : _width;
            set
            {
                _insideWidth = value;
                Modify();
            }
        }
        public double InsideHeight
        {
            get => HasInsideDimensions ? _insideHeight : _height;
            set
            {
                _insideHeight = value;
                Modify();
            }
        }
        public double InsideVolume => InsideLength * InsideWidth * InsideHeight;
        public virtual Vector3D InsideDimensions => new Vector3D(InsideLength, InsideWidth, InsideHeight);
        public virtual double[] OuterDimensionsArray => new double[] { _length, _width, _height };
        public virtual double[] InsideDimensionsArray => new double[] { InsideLength, InsideWidth, InsideHeight };

        public OptDouble MaxWeight
        {
            get { return IsCase ? _maxWeight : new OptDouble(false, 0.0); }
            set { _maxWeight = value; }
        }

        public bool FitsIn(BoxProperties caseProperties)
        {
            double[] dimItem = OuterDimensionsArray;
            Array.Sort(dimItem);
            double[] dimCase = caseProperties.InsideDimensionsArray;
            Array.Sort(dimCase);
            return dimItem[0] <= dimCase[0]
                && dimItem[1] <= dimCase[1]
                && dimItem[2] <= dimCase[2];        
        }
        public Vector3D GetStackingDimensions(ConstraintSetAbstract constraintSet)
        {
            return InsideDimensions;
        }
        public bool FitsIn(BoxProperties caseProperties, bool allowVerticalX, bool allowVerticalY, bool allowVerticalZ)
        {
            double[] dimItem = OuterDimensionsArray;
            double[] dimCase = caseProperties.InsideDimensionsArray;
            if (allowVerticalX)
            {
                if (
                    (dimItem[0] <= dimCase[0]) && (
                        ( (dimItem[1] <= dimCase[1]) && (dimItem[2] <= dimCase[2]) )
                    || ( (dimItem[1] <= dimCase[2]) && (dimItem[2] <= dimCase[1]) ) )
                    )
                return true;
            }
            if (allowVerticalY)
            {
                if (
                    (dimItem[1] <= dimCase[1]) && (
                        ((dimItem[0] <= dimCase[0]) && (dimItem[2] <= dimCase[2]))
                    || ((dimItem[0] <= dimCase[2]) && (dimItem[2] <= dimCase[0])))
                    )
                    return true;
            }
            if (allowVerticalZ)
            {
                if (
                    (dimItem[2] <= dimCase[2]) && (
                        ((dimItem[0] <= dimCase[0]) && (dimItem[1] <= dimCase[1]))
                    || ((dimItem[0] <= dimCase[1]) && (dimItem[1] <= dimCase[0])))
                    )
                    return true;
            }
            return false;
        }

        // Colors
        public override void SetColor(Color color)
        {
            for (int i = 0; i < _colors.Length; ++i)
                _colors[i] = color;
            Modify();
        }
        public override Color GetColor(HalfAxis.HAxis axis)
        {
            return _colors[(int)axis];
        }

        public override Color[] Colors => _colors;
        public void SetColor(HalfAxis.HAxis axis, Color color)
        {
            _colors[(int)axis] = color;
            Modify();
        }
        public void SetAllColors(Color[] color)
        {
            for (int i = 0; i < _colors.Length; ++i)
                _colors[i] = color[i];
            Modify();
        }
        public bool UniqueColor
        {
            get
            {
                for (int i = 1; i < _colors.Length; ++i)
                    if (_colors[0] != _colors[i])
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Texture pair
        /// </summary>
        /// <param name="axis">Face normal axis</param>
        /// <param name="position">Position</param>
        /// <param name="size">Size</param>
        /// <param name="angle">Angle</param>
        /// <param name="bmp">Image used as texture</param>
        public void AddTexture(HalfAxis.HAxis axis, Vector2D position, Vector2D size, double angle, Bitmap bmp)
        {
            _textures.Add(new Pair<HalfAxis.HAxis, Texture>(axis, new Texture(bmp, position, size, angle)));
            Modify();
        }


        /// <summary>
        /// Get / set face/texture pairs
        /// </summary>
        public List<Pair<HalfAxis.HAxis, Texture>> TextureList
        {
            get => _textures;
            set
            {
                _textures.Clear();
                if (null != value)
                    _textures.AddRange(value);
            }
        }
        public List<Pair<HalfAxis.HAxis, Texture>> TextureListCopy
        {
            get
            {
                var list = new List<Pair<HalfAxis.HAxis, Texture>>();
                foreach (var tex in _textures)
                    list.Add(new Pair< HalfAxis.HAxis, Texture >(tex.first, tex.second.Clone()));
                return list;
            }
        }

        // Tape properties
        public OptDouble TapeWidth { get; set; }
        public Color TapeColor { get; set; }

        /// <summary>
        /// Flag to display a label on the top of the box
        /// </summary>
        public bool ShowTopLabel { get; set; }

        public override bool IsBundle => false;
        protected override string TypeName => IsCase ? Properties.Resources.ID_NAMECASE : Properties.Resources.ID_NAMEBOX;

        #region Non-Public Members
        private double _height;
        private double _insideLength, _insideWidth, _insideHeight;
        private Color[] _colors = new Color[6];
        private List<Pair<HalfAxis.HAxis, Texture>> _textures = new List<Pair<HalfAxis.HAxis, Texture>>();
        private OptDouble _maxWeight;
        #endregion
    }
}
