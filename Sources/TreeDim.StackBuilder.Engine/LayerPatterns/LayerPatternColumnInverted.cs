using System;
using System.Collections.Generic;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    class LayerPatternColumnInverted : LayerPatternBox
    {
        public override string Name => "Column Inverted";
        public override int GetNumberOfVariants(Layer2D layer) => 1;
        public override bool IsSymetric => false;
        public override bool CanBeSwapped => false;
        public override bool CanBeInverted => false;

        public override void GenerateLayer(ILayer2D layer, double actualLength, double actualWidth)
        {
            layer.Clear();
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            int sizeY = (int)Math.Floor(palletLength / boxLength);
            int sizeX = (int)Math.Floor(palletWidth / boxWidth);

            double offsetY = 0.5 * (palletLength - actualLength);
            double offsetX = 0.5 * (palletWidth - actualWidth);

            double spaceX = sizeX > 1 ? (actualLength - sizeX * boxLength) / (sizeX - 1) : 0.0;
            double spaceY = sizeY > 1 ? (actualWidth - sizeY * boxWidth) / (sizeY - 1) : 0.0;

            for (int i = 0; i < sizeX; ++i)
                for (int j = 0; j < sizeY; ++j)
                    AddPosition(layer
                        , new Vector2D(
                            offsetX + i * (boxLength + spaceX)
                            , offsetY + j * (boxWidth + spaceY)
                            )
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
            // set spacing to ZERO i.e. no spacing with column layout
            layer.UpdateMaxSpace(spaceX, Name);
            layer.UpdateMaxSpace(spaceY, Name);
        }

        public override bool GetLayerDimensions(ILayer2D layer, out double actualLength, out double actualWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            actualLength = Math.Floor(palletLength / boxLength) * boxLength;
            actualWidth = Math.Floor(palletWidth / boxWidth) * boxWidth;
            return (palletLength >= boxLength) && (palletWidth >= boxWidth);
        }
    }
}
