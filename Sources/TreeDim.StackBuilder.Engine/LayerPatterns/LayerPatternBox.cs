using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Basics.Extensions;

namespace treeDiM.StackBuilder.Engine
{
    internal abstract class LayerPatternBox : LayerPattern
    {
        // This is OK as long as LayerPatternBox objects are immutable
        public static IReadOnlyList<LayerPatternBox> All => ImmutableList.CreateRange(new LayerPatternBox[] {
            new LayerPatternColumn()
            //, new LayerPatternColumnInverted()
            , new LayerPatternInterlocked()
            , new LayerPatternInterlockedSymetric()
            , new LayerPatternInterlockedFilled()
            , new LayerPatternTrilock()
            , new LayerPatternDiagonale()
            , new LayerPatternSpirale()
            , new LayerPatternEnlargedSpirale()
            , new LayerPatternOpposed()
        });

        public static LayerPatternBox GetByName(string patternName)
        {
            return All[GetPatternNameIndex(patternName)];
        }

        public static int GetPatternNameIndex(string patternName)
        {
            int index = All.FindIndex(x => string.Equals(x.Name, patternName, StringComparison.OrdinalIgnoreCase));
            return index != -1
                ? index
                : throw new ArgumentException($"Invalid pattern name = {patternName}", nameof(patternName));
        }

        public abstract int GetNumberOfVariants(Layer2D layer);
        public abstract bool CanBeInverted { get; }
        public abstract bool IsSymetric { get; }

        public double GetBoxLength(ILayer2D layer)
        {
            return ((Layer2D)layer).BoxLength;
        }
        public double GetBoxWidth(ILayer2D layer)
        {
            return ((Layer2D)layer).BoxWidth;
        }
        public void AddPosition(ILayer2D layer, Vector2D vPosition, HalfAxis.HAxis lengthAxis, HalfAxis.HAxis widthAxis)
        {
            Matrix4D matRot = Matrix4D.Identity;
            Vector3D vTranslation = Vector3D.Zero;

            if (layer.Swapped)
            {
                matRot = new Matrix4D(
                    0.0, -1.0, 0.0, 0.0
                    , 1.0, 0.0, 0.0, 0.0
                    , 0.0, 0.0, 1.0, 0.0
                    , 0.0, 0.0, 0.0, 1.0
                    );
                vTranslation = new Vector3D(layer.Length, 0.0, 0.0);
            }
            Transform3D transfRot = new Transform3D(matRot);
            HalfAxis.HAxis lengthAxisSwapped = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(lengthAxis)));
            HalfAxis.HAxis widthAxisSwapped = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(widthAxis)));

            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];

            Transform3D transfRotTranslation = new Transform3D(matRot);
            Vector3D vPositionSwapped = transfRotTranslation.transform(new Vector3D(vPosition.X, vPosition.Y, 0.0));

            Layer2D layerBox = layer as Layer2D;
            if (!layerBox.IsValidPosition(new Vector2D(vPositionSwapped.X, vPositionSwapped.Y), lengthAxisSwapped, widthAxisSwapped))
            {
                _log.Warn(string.Format("Attempt to add an invalid position in pattern = {0}, Swapped = {1}", Name, layer.Swapped));
                return;
            }
            layerBox.AddPosition(new Vector2D(vPositionSwapped.X, vPositionSwapped.Y), lengthAxisSwapped, widthAxisSwapped);
        }

        #region Non-Public Members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(LayerPatternBox));
        #endregion
    }
}