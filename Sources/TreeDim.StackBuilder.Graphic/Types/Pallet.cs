#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using Sharp3D.Math.Core;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    #region Position
    public struct Position
    {
        #region Constructor
        public Position(int index, Vector3D xyz, HalfAxis.HAxis axis1, HalfAxis.HAxis axis2)
        {
            Index = index; XYZ = xyz; Axis1 = axis1; Axis2 = axis2;
        }
        #endregion

        #region Public properties
        public Vector3D XYZ { get; }
        public HalfAxis.HAxis Axis1 { get; }
        public HalfAxis.HAxis Axis2 { get; }
        public int Index { get; }
        #endregion
    }
    #endregion

    #region PalletData
    public class PalletData
    {
        #region Constructor
        private PalletData(string name, string description, Vector3D[] lumbers, Position[] positions, Vector3D dimensions, double weight, Color color)
        {
            Name = name;
            Description = description;
            Lumbers = new List<Vector3D>(lumbers);
            Positions = new List<Position>(positions);
            Dimensions = dimensions;
            Weight = weight;
            Color = color;
        }
        #endregion

        #region Public properties
        public string Name { get; }
        public string Description { get; }
        public Vector3D Dimensions { get; }
        public double Length { get { return Dimensions.X; } }
        public double Width { get { return Dimensions.Y; } }
        public double Height { get { return Dimensions.Z; } }
        public double Weight { get; }
        public Color Color { get; }
        #endregion

        #region Static pool methods
        private static void Initialize()
        {
            if (null == Pool)
            {
                Pool = new List<PalletData>();
                #region Block
                // --------------------------------------------------------------------------------
                // Block
                {
                    Vector3D[] lumbers = {
                        new Vector3D(1200, 1000, 150)
                    };
                    Vector3D dimensions = new Vector3D(1200.0, 1000.0, 150.0);
                    Position[] positions = {
                        new Position(0, Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                    };
                    Pool.Add(new PalletData("Block", "Block", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region Standard UK
                // --------------------------------------------------------------------------------
                // Standard UK
                {
                    Vector3D[] lumbers = {
                         new Vector3D(1000.0, 98.0, 18.0)
                         , new Vector3D(138.0, 98.0, 95.0)
                         , new Vector3D(1200.0, 95.0, 18.0)
                         , new Vector3D(1000.0, 120.0, 19.0)
                    };
                    Vector3D dimensions = new Vector3D(1200.0, 1000.0, 150.0);
                    double xStep0 = (dimensions.X - lumbers[0].Y) / 2.0;
                    double xStep1 = (dimensions.X - lumbers[0].Y) / 2.0;
                    double yStep1 = (dimensions.Y - lumbers[1].X) / 2.0;
                    double yStep2 = lumbers[2].Y + (dimensions.Y - 3.0 * lumbers[2].Y) / 2.0;
                    double xStep3 = lumbers[3].Y + (dimensions.X - 7.0 * lumbers[3].Y) / 6.0;
                    Position[] positions = {
                         // first layer
                         new Position(0, new Vector3D(lumbers[0].Y, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(0, new Vector3D(lumbers[0].Y + xStep0, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(0, new Vector3D(lumbers[0].Y + 2 * xStep0, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         // second layer
                         , new Position(1, new Vector3D(lumbers[1].Y, 0.0, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y, yStep1, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y, 2.0 * yStep1, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y + xStep1, 0.0, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y + xStep1, yStep1, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y + xStep1, 2.0 * yStep1, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y + 2.0 * xStep1, 0.0, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y + 2.0 * xStep1, yStep1, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(1, new Vector3D(lumbers[1].Y + 2.0 * xStep1, 2.0 * yStep1, 18.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         // third layer
                         , new Position(2, new Vector3D(0.0, 0.0, 113.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(2, new Vector3D(0.0, yStep2, 113.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(2, new Vector3D(0.0, 2.0 * yStep2, 113.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         // fourth layer
                         , new Position(3, new Vector3D(lumbers[3].Y, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 1.0 * xStep3, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 2.0 * xStep3, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 3.0 * xStep3, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 4.0 * xStep3, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 5.0 * xStep3, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 6.0 * xStep3, 0.0, 131.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)

                    };
                    Pool.Add(new PalletData("UK Standard", "UK Standard", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region GMA 48*40
                // --------------------------------------------------------------------------------
                // GMA 48*40
                {
                    Vector3D[] lumbers = {
                            new Vector3D(40.0 * 25.4,   3.5 * 25.4, 0.625 * 25.4)
                            , new Vector3D(40.0 * 25.4, 5.5 * 25.4, 0.625 * 25.4)
                            , new Vector3D(48.0 * 25.4, 1.375 * 25.4, 3.5 * 25.4)
                        };

                    Vector3D dimensions = new Vector3D(48.0 * 25.4, 40.0 * 25.4, 4.75 * 25.4);

                    Position[] positions = {
                            // bottom deck                            
                            new Position(1, new Vector3D(lumbers[1].Y, 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(1, new Vector3D(dimensions.X, 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X - 3 * lumbers[0].Y), 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 1 * lumbers[0].Y), 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 5 * lumbers[0].Y), 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            // stringers
                            , new Position(2, new Vector3D(0.0, 0.0, 0.625 * 25.4)
                                , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                            , new Position(2, new Vector3D(0.0, 0.5 * (dimensions.Y - lumbers[2].Y), 0.625 * 25.4)
                                , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                            , new Position(2, new Vector3D(0.0, dimensions.Y - lumbers[2].Y, 0.625 * 25.4)
                                , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                            // top deck
                            , new Position(1, new Vector3D(lumbers[1].Y, 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(1, new Vector3D(dimensions.X, 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X - 7 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X - 3 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 1 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 5 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 9 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };
                    Pool.Add(new PalletData("GMA 48x40", "Grocery Manufacturer Association (North America)", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region CHEP AU
                {
                    Vector3D[] lumbers = {
                            new Vector3D(1165.0, 3.5 * 25.4, 0.625 * 25.4)
                            , new Vector3D(1165.0, 5.5 * 25.4, 0.625 * 25.4)
                            , new Vector3D(1165.0, 1.375 * 25.4, 3.5 * 25.4)
                        };

                    Vector3D dimensions = new Vector3D(1165.0, 1165.0, 150.0);

                    Position[] positions = {
                            // bottom deck                            
                            new Position(1, new Vector3D(lumbers[1].Y, 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(1, new Vector3D(dimensions.X, 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X - 3 * lumbers[0].Y), 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 1 * lumbers[0].Y), 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 5 * lumbers[0].Y), 0.0, 0.0)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            // stringers
                            , new Position(2, new Vector3D(0.0, 0.0, 0.625 * 25.4)
                                , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                            , new Position(2, new Vector3D(0.0, 0.5 * (dimensions.Y - lumbers[2].Y), 0.625 * 25.4)
                                , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                            , new Position(2, new Vector3D(0.0, dimensions.Y - lumbers[2].Y, 0.625 * 25.4)
                                , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                            // top deck
                            , new Position(1, new Vector3D(lumbers[1].Y, 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(1, new Vector3D(dimensions.X, 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X - 7 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X - 3 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 1 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 5 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                            , new Position(0, new Vector3D(0.5 * (dimensions.X + 9 * lumbers[0].Y), 0.0, (0.625+3.5) * 25.4)
                                , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };
                    Pool.Add(new PalletData("CHEP AU", "AU (W)", lumbers, positions, dimensions, 43, Color.FromArgb(51, 102, 255)));
                }
                #endregion
                #region CHEP NZ
                {
                    Vector3D[] lumbers = {
                         new Vector3D(1000.0, 98.0, 25.0)       // 0
                         , new Vector3D(1000.0, 98.0, 25.0)     // 1
                         , new Vector3D(160.0, 95.0, 77.0)      // 2
                         , new Vector3D(95.0, 95.0, 77.0)       // 3
                         , new Vector3D(1200.0, 98.0, 25.0)     // 4
                         , new Vector3D(1000.0, 112.0, 17.0)    // 5
                         , new Vector3D(1000.0, 135.0, 17.0)    // 6
                         , new Vector3D(1000.0, 112.0, 17.0)    // 7
                         , new Vector3D(1000.0, 112.0, 17.0)    // 8
                    };

                    Vector3D dimensions = new Vector3D(1200.0, 1000.0, 25.0 + 77.0 + 25.0 + 17.0);
                    double xInit0 = (dimensions.X - lumbers[0].X) / 2.0;
                    double yStep0 = (dimensions.Y - lumbers[0].Y) / 2.0;
                    double yStep1 = (dimensions.Y - lumbers[2].Y) / 2.0;
                    double yStep2 = (dimensions.Y - lumbers[4].Y) / 2.0;
                    double xStep3 = (dimensions.X - 2.0 * lumbers[5].Y - 2.0 * lumbers[6].Y - 5.0 * lumbers[7].Y) / 6.0;
                    Position[] positions = {
                        // 1st layer
                        // x dir
                        new Position(0, new Vector3D(xInit0, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(xInit0, 1.0 * yStep0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(xInit0, 2.0 * yStep0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                       // y dir
                        , new Position(1, new Vector3D(lumbers[1].Y, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(dimensions.X, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // 2 nd layer
                        , new Position(2, new Vector3D(0.0, 0.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 1.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 2.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(dimensions.X - lumbers[2].X, 0.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(dimensions.X - lumbers[2].X, 1.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(dimensions.X - lumbers[2].X, 2.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D((dimensions.X - lumbers[3].X) / 2.0, 0.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D((dimensions.X - lumbers[3].X) / 2.0, 1.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D((dimensions.X - lumbers[3].X) / 2.0, 2.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 3 rd layer
                        , new Position(4, new Vector3D(0.0, 0.0 * yStep2, lumbers[0].Z + lumbers[2].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(4, new Vector3D(0.0, 1.0 * yStep2, lumbers[0].Z + lumbers[2].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(4, new Vector3D(0.0, 2.0 * yStep2, lumbers[0].Z + lumbers[2].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 4 th layer
                        , new Position(5, new Vector3D(lumbers[5].Y, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(5, new Vector3D(dimensions.X, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(6, new Vector3D(lumbers[5].Y + lumbers[6].Y, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(6, new Vector3D(dimensions.X-lumbers[5].Y, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 1.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 2.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 3.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 4.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 5.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };
                    Pool.Add(new PalletData("CHEP NZ", "NZ (W)", lumbers, positions, dimensions, 28, Color.FromArgb(51, 102, 255)));
                }
                #endregion
                #region EUR
                // --------------------------------------------------------------------------------
                // EUR
                {
                    Vector3D[] lumbers = {
                                 new Vector3D(1200.0, 100.0, 22.0)  // lower plane side
                                 , new Vector3D(1200.0, 145.0, 22.0) // upper plane side 
                                 , new Vector3D(1200.0, 145.0, 22.0) // lower plane central base
                                 , new Vector3D(800.0, 145.0, 22.0) // cross members
                                 , new Vector3D(1200.0, 145.0, 22.0) // upper plant central board
                                 , new Vector3D(1200.0, 100.0, 22.0) // upper plane intermediate boards
                                 , new Vector3D(145.0, 100.0, 78.0) // outside blocks
                                 , new Vector3D(145.0, 145.0, 78.0) // central blocks
                            };
                    Vector3D dimensions = new Vector3D(1200, 800, 144);

                    double yy0 = (dimensions.Y - 2.0 * lumbers[0].Y - lumbers[1].Y) / 2.0;
                    double yy1 = (dimensions.Y - 2.0 * lumbers[6].Y - lumbers[7].Y) / 2.0;
                    double xStep1 = (dimensions.X - lumbers[6].X) / 2.0;
                    double yStep1 = lumbers[6].Y + yy1;
                    double xStep2 = (dimensions.X - lumbers[4].Y) / 2.0;
                    double yy3 = 0.25 * (dimensions.Y - 2.0 * lumbers[1].Y - 2.0 * lumbers[5].Y - lumbers[4].Y);
                    Position[] positions = {
                         // first layer
                         new Position(0, new Vector3D(0.0, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(2, new Vector3D(0.0, yy0 + lumbers[0].Y, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(0.0, 2 * yy0 + lumbers[0].Y + lumbers[2].Y, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         // second layer
                         , new Position(6, new Vector3D(0.0, 0.0, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(7, new Vector3D(0.0, yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(6, new Vector3D(0.0,lumbers[6].Y + lumbers[7].Y + 2.0 * yy1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(6, new Vector3D(0.0 + xStep1, 0.0, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(7, new Vector3D(xStep1, yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(6, new Vector3D(0.0 + xStep1, lumbers[6].Y + lumbers[7].Y + 2.0 * yy1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(6, new Vector3D(0.0 + 2.0 * xStep1, 0.0, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(7, new Vector3D(2.0 * xStep1, yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(6, new Vector3D(0.0 + 2.0 * xStep1, lumbers[6].Y + lumbers[7].Y + 2.0 * yy1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         // third layer
                         , new Position(3, new Vector3D(lumbers[3].Y + 0.0, 0.0, lumbers[0].Z + lumbers[6].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + xStep2, 0.0, lumbers[0].Z + lumbers[6].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         , new Position(3, new Vector3D(lumbers[3].Y + 2.0 * xStep2, 0.0, lumbers[0].Z + lumbers[6].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                         // fourth layer
                         , new Position(2, new Vector3D(0.0, 0.0, lumbers[0].Z + lumbers[6].Z + lumbers[3].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(5, new Vector3D(0.0, lumbers[1].Y + yy3, lumbers[0].Z + lumbers[6].Z + lumbers[3].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(4, new Vector3D( 0.0, lumbers[1].Y + lumbers[5].Y + 2.0 * yy3,lumbers[0].Z + lumbers[6].Z + lumbers[3].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(5, new Vector3D(0.0, lumbers[1].Y + lumbers[5].Y + lumbers[4].Y + 3.0 * yy3, lumbers[0].Z + lumbers[6].Z + lumbers[3].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(2, new Vector3D(0.0, lumbers[1].Y + 2.0 * lumbers[5].Y + lumbers[4].Y + 4.0 * yy3, lumbers[0].Z + lumbers[6].Z + lumbers[3].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)

                        };
                    Pool.Add(new PalletData("EUR", "EUR-EPAL (European Pallet Association)", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region EUR2
                // --------------------------------------------------------------------------------
                // EUR2
                {
                    Vector3D[] lumbers = {
                         new Vector3D(1000.0, 98.0, 25.0)
                         , new Vector3D(1000.0, 98.0, 25.0)
                         , new Vector3D(160.0, 95.0, 77.0)
                         , new Vector3D(95.0, 95.0, 77.0)
                         , new Vector3D(1200.0, 98.0, 25.0)
                         , new Vector3D(1000.0, 112.0, 17.0)
                         , new Vector3D(1000.0, 135.0, 17.0)
                         , new Vector3D(1000.0, 112.0, 17.0)
                         , new Vector3D(1000.0, 112.0, 17.0)
                    };

                    Vector3D dimensions = new Vector3D(1200.0, 1000.0, 144.0);
                    double xInit0 = (dimensions.X - lumbers[0].X) / 2.0;
                    double yStep0 = (dimensions.Y - lumbers[0].Y) / 2.0;
                    double yStep1 = (dimensions.Y - lumbers[2].Y) / 2.0;
                    double yStep2 = (dimensions.Y - lumbers[4].Y) / 2.0;
                    double xStep3 = (dimensions.X - 2.0 * lumbers[5].Y - 2.0 * lumbers[6].Y - 5.0 * lumbers[7].Y) / 6.0;
                    Position[] positions = {
                        // 1st layer
                        // x dir
                        new Position(0, new Vector3D(xInit0, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(xInit0, 1.0 * yStep0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(xInit0, 2.0 * yStep0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                       // y dir
                        , new Position(1, new Vector3D(lumbers[1].Y, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(dimensions.X, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // 2 nd layer
                        , new Position(2, new Vector3D(0.0, 0.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 1.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 2.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(dimensions.X - lumbers[2].X, 0.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(dimensions.X - lumbers[2].X, 1.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(dimensions.X - lumbers[2].X, 2.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D((dimensions.X - lumbers[3].X) / 2.0, 0.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D((dimensions.X - lumbers[3].X) / 2.0, 1.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D((dimensions.X - lumbers[3].X) / 2.0, 2.0 * yStep1, lumbers[0].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 3 rd layer
                        , new Position(4, new Vector3D(0.0, 0.0 * yStep2, lumbers[0].Z + lumbers[2].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(4, new Vector3D(0.0, 1.0 * yStep2, lumbers[0].Z + lumbers[2].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(4, new Vector3D(0.0, 2.0 * yStep2, lumbers[0].Z + lumbers[2].Z)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 4 th layer
                        , new Position(5, new Vector3D(lumbers[5].Y, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(5, new Vector3D(dimensions.X, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(6, new Vector3D(lumbers[5].Y + lumbers[6].Y, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(6, new Vector3D(dimensions.X-lumbers[5].Y, 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 1.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 2.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 3.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 4.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(7, new Vector3D(lumbers[5].Y + lumbers[6].Y + 5.0 * (xStep3 + lumbers[7].Y), 0.0, lumbers[0].Z + lumbers[2].Z + lumbers[4].Z)
                             , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };
                    Pool.Add(new PalletData("EUR2", "EUR2-EPAL (European Pallet Association)", lumbers, positions, dimensions, 33, Color.Yellow));
                }
                #endregion
                #region EUR3
                // --------------------------------------------------------------------------------
                // EUR3
                {
                    Vector3D[] lumbers = {
                         new Vector3D(1200.0, 145.0, 22.0)
                         , new Vector3D(145.0, 145.0, 78.0)
                         , new Vector3D(1000.0, 145.0, 22.0)
                         , new Vector3D(1200.0, 145.0, 22.0)
                         , new Vector3D(1200.0, 145.0, 22.0)
                         , new Vector3D(1200.0, 100.0, 22.0)
                    };

                    Vector3D dimensions = new Vector3D(1200.0, 1000.0, 144.0);
                    double yStep0 = (dimensions.Y - lumbers[0].Y) / 2.0;
                    double xStep1 = (dimensions.X - lumbers[1].X) / 2.0;
                    double yStep1 = (dimensions.Y - lumbers[1].Y) / 2.0;
                    double xStep2 = (dimensions.X - lumbers[2].Y) / 2.0;
                    double yStep3 = (dimensions.Y - lumbers[3].Y) / 2.0;
                    double yyStep3 = (dimensions.Y - 2.0 * lumbers[3].Y - lumbers[4].Y) / 4.0;

                    Position[] positions = {
                         // 1st layer
                         new Position(0, new Vector3D(0.0, 0.0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(0.0, 1.0 * yStep0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         , new Position(0, new Vector3D(0.0, 2.0 * yStep0, 0.0)
                             , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                         // 2 nd layer
                        , new Position(1, new Vector3D(0.0,0.0,lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(0.0, 1.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(0.0, 2.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(1.0 * xStep1, 0.0,lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(1.0 * xStep1, 1.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(1.0 * xStep1, 2.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(2.0 * xStep1, 0.0,lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(2.0 * xStep1, 1.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(2.0 * xStep1, 2.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 3 rd layer
                        , new Position(2, new Vector3D(lumbers[2].Y,0.0, lumbers[0].Z+lumbers[1].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(2, new Vector3D(lumbers[2].Y + 1.0 * xStep2, 0.0, lumbers[0].Z+lumbers[1].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(2, new Vector3D(lumbers[2].Y + 2.0 * xStep2, 0.0, lumbers[0].Z+lumbers[1].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // 4 th layer
                        // outer and middle boards
                        , new Position(3, new Vector3D(0.0, 0.0,lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(4, new Vector3D(0.0, 1.0 * yStep3, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(3, new Vector3D(0.0, 2.0 * yStep3, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // intermediate boards
                        , new Position(5, new Vector3D(0.0, lumbers[3].Y, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(5, new Vector3D(0.0, lumbers[3].Y + yyStep3, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(5, new Vector3D(0.0, dimensions.Y - lumbers[3].Y - lumbers[5].Y, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(5, new Vector3D(0.0, dimensions.Y - lumbers[3].Y - lumbers[5].Y - yyStep3, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                    };
                    Pool.Add(new PalletData("EUR3", "EUR3-EPAL (European Pallet Association)", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region EUR6
                // --------------------------------------------------------------------------------
                // EUR6
                {
                    Vector3D[] lumbers = {
                         new Vector3D(600.0, 78.0, 22.0)
                         , new Vector3D(78.0, 78.0, 78.0)
                         , new Vector3D(800.0, 78.0, 22.0)
                         , new Vector3D(600.0, 98.0, 22.0)
                    };

                    Vector3D dimensions = new Vector3D(800, 600, 144);

                    double xStep0 = (dimensions.X - lumbers[0].Y) / 2.0;
                    double xStep1 = (dimensions.X - lumbers[1].X) / 2.0;
                    double yStep1 = (dimensions.Y - lumbers[1].Y) / 2.0;
                    double yStep2 = (dimensions.Y - lumbers[2].Y) / 2.0;
                    double xStep3 = (dimensions.X - lumbers[3].Y) / 6.0;

                    Position[] positions = {
                        // 1st layer
                        new Position(0, new Vector3D(lumbers[0].Y,0.0,0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(0, new Vector3D(lumbers[0].Y + xStep0, 0.0, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(0, new Vector3D(lumbers[0].Y + 2.0 * xStep0, 0.0, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // 2nd layer
                        , new Position(1, new Vector3D(0.0,0.0,lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(0.0, 1.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(0.0, 2.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(1.0 * xStep1, 0.0,lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(1.0 * xStep1, 1.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(1.0 * xStep1, 2.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(2.0 * xStep1, 0.0,lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(2.0 * xStep1, 1.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(1, new Vector3D(2.0 * xStep1, 2.0 * yStep1, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 3 rd layer
                        , new Position(2, new Vector3D(0.0, 0.0, lumbers[0].Z+lumbers[1].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 1.0 * yStep2, lumbers[0].Z+lumbers[1].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 2.0 * yStep2, lumbers[0].Z+lumbers[1].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // 4 th layer
                        , new Position(3, new Vector3D(lumbers[3].Y, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 1.0 * xStep3, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 2.0 * xStep3, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 3.0 * xStep3, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 4.0 * xStep3, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 5.0 * xStep3, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 6.0 * xStep3, 0.0, lumbers[0].Z+lumbers[1].Z+lumbers[2].Z)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)

                    };
                    Pool.Add(new PalletData("EUR6", "EUR6-EPAL (European Pallet Association)", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region TYPE 03
                {
                    Vector3D[] lumbers = {
                        new Vector3D(25.5 * 25.4, 5.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(25.5 * 25.4, 3.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(49.0 * 25.4, 1.5 * 25.4, 3.5 * 25.4)
                        , new Vector3D(27.5 * 25.4, 3.5 * 25.4, 0.675 * 25.4)
                    };
                    Vector3D dimensions = new Vector3D(49 * 25.4, 27.5 * 25.4, 4.75 * 25.4);

                    double step0 = (dimensions.X - 10 * lumbers[3].Y) / 9.0;
                    double offsetY = 0.5 * (lumbers[3].X - lumbers[0].X);
                    double startX = 0.5 * dimensions.X - 2.0 * lumbers[1].Y - 1.5 * step0;

                    double offsetTop = lumbers[0].Z + lumbers[2].Z;
                    double stepTop = step0 + lumbers[3].Y;

                    Position[] positions = {
                        // bottom
                        new Position(0, new Vector3D(lumbers[0].Y, offsetY, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(0, new Vector3D(dimensions[0], offsetY, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(startX + 1 * lumbers[1].Y, offsetY, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(startX + 2 * lumbers[1].Y + step0, offsetY, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(startX + 3 * lumbers[1].Y + 2 * step0, offsetY, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(startX + 4 * lumbers[1].Y + 3 * step0, offsetY, 0.0)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // stringers
                        , new Position(2, new Vector3D(0.0, offsetY, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 0.5 * dimensions.Y- 0.5 * lumbers[2].Y, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, dimensions.Y - lumbers[2].Y - offsetY, lumbers[0].Z)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // top deck
                        , new Position(3, new Vector3D(lumbers[3].Y + 0 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 1 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 2 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 3 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 4 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 5 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 6 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 7 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 8 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 9 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                  };
                    Pool.Add(new PalletData("TYPE 03", "TYPE 03", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region TYPE 04
                {
                    Vector3D[] lumbers = {
                        new Vector3D(38.0 * 25.4, 5.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(38.0 * 25.4, 3.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(42.0 * 25.4, 1.5 * 25.4, 3.5 * 25.4)
                        , new Vector3D(42.0 * 25.4, 3.5 * 25.4, 0.625 * 25.4)
                    };
                    Vector3D dimensions = new Vector3D(42.0 * 25.4, 42.0 * 25.4, 4.75 * 25.4);

                    double offsetY = 0.5 * (lumbers[3].X - lumbers[0].X);
                    double offsetTop = lumbers[0].Z + lumbers[2].Z;
                    double stepTop = (dimensions.X - 9 * lumbers[3].Y) / 6.0 + lumbers[3].Y;

                    Position[] positions = {
                        // bottom
                        new Position(0, new Vector3D(lumbers[0].Y, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(0, new Vector3D(dimensions.X, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(0.5 * dimensions.X - 0.5 * lumbers[1].Y - 1.0 * 25.4, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(0.5 * dimensions.X + 0.5 * lumbers[1].Y, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(0.5 * dimensions.X + 1.5 * lumbers[1].Y + 1.0 * 25.4, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // stringers
                        , new Position(2, new Vector3D(0.0, offsetY, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 0.5 * dimensions.Y- 0.5 * lumbers[2].Y, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, dimensions.Y - lumbers[2].Y - offsetY, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // top deck
                        , new Position(3, new Vector3D(lumbers[3].Y, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(dimensions.X, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 0 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 1 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 2 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 3 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 4 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 5 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(2.0 * lumbers[3].Y + 6 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };
                    Pool.Add(new PalletData("TYPE 04", "TYPE 04", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region TYPE 05
                {
                    Vector3D[] lumbers = {
                        new Vector3D(33.0 * 25.4, 5.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(33.0 * 25.4, 3.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(49.0 * 25.4, 1.5 * 25.4, 3.5 * 25.4)
                        , new Vector3D(36.0 * 25.4, 3.5 * 25.4, 0.625 * 25.4)
                    };
                    Vector3D dimensions = new Vector3D(49.0 * 25.4, 36.0 * 25.4, 4.75 * 25.4);
                    double offsetY = 0.5 * (lumbers[3].X - lumbers[0].X);
                    double offsetTop = lumbers[0].Z + lumbers[2].Z;
                    double stepTop = (dimensions.X - 10 * lumbers[3].Y) / 9.0 + lumbers[3].Y;
                    double stepStringers = (dimensions.Y - 4 * lumbers[2].Y - 2.0 * offsetY) / 3.0 + lumbers[2].Y;


                    Position[] positions = {
                        // bottom
                        new Position(0, new Vector3D(lumbers[0].Y, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(0, new Vector3D(dimensions.X, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(0.5 * dimensions.X - 0.5 * lumbers[1].Y - 1.0 * 25.4, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(0.5 * dimensions.X + 0.5 * lumbers[1].Y, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(0.5 * dimensions.X + 1.5 * lumbers[1].Y + 1.0 * 25.4, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // stringers
                        , new Position(2, new Vector3D(0.0, offsetY + 0 * stepStringers, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, offsetY + 1 * stepStringers, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, offsetY + 2 * stepStringers, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, offsetY + 3 * stepStringers, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // top deck
                        , new Position(3, new Vector3D(lumbers[3].Y + 0 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 1 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 2 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 3 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 4 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 5 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 6 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 7 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 8 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 9 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };

                    Pool.Add(new PalletData("TYPE 05", "TYPE 05", lumbers, positions, dimensions, 20, Color.Yellow));
                }
                #endregion
                #region TYPE 06
                {
                    Vector3D[] lumbers = {
                        new Vector3D(30.0 * 25.4, 5.5 * 25.4, 0.625 * 25.4)
                        , new Vector3D(30.0 * 25.4, 33 * 25.4, 0.625 * 25.4)
                        , new Vector3D(44.0 * 25.4, 1.5 * 25.4, 3.5 * 25.4)
                        , new Vector3D(31.5 * 25.4, 3.5 * 25.4, 0.625 * 25.4)
                    };
                    Vector3D dimensions = new Vector3D(44.0 * 25.4, 31.5 * 25.4, 4.75 * 25.4);
                    double offsetY = 0.5 * (lumbers[3].X - lumbers[0].X);
                    double offsetTop = lumbers[0].Z + lumbers[2].Z;
                    double stepTop = (dimensions.X - 10 * lumbers[3].Y) / 9.0 + lumbers[3].Y;


                    Position[] positions = {
                        // bottom
                        new Position(0, new Vector3D(lumbers[0].Y, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(0, new Vector3D(dimensions.X, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(1, new Vector3D(lumbers[0].Y + lumbers[1].Y, offsetY, 0.0), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        // stringers
                        , new Position(2, new Vector3D(0.0, offsetY, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, 0.5 * dimensions.Y- 0.5 * lumbers[2].Y, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        , new Position(2, new Vector3D(0.0, dimensions.Y - lumbers[2].Y - offsetY, lumbers[0].Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                        // top deck
                        , new Position(3, new Vector3D(lumbers[3].Y + 0 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 1 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 2 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 3 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 4 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 5 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 6 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 7 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 8 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                        , new Position(3, new Vector3D(lumbers[3].Y + 9 * stepTop, 0.0, offsetTop), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N)
                    };
                    Pool.Add(new PalletData("TYPE 06", "TYPE 06 orange pallet", lumbers, positions, dimensions, 20, Color.Orange));
                }
                #endregion
                // --------------------------------------------------------------------------------
            }
        }

        public static PalletData GetByName(string name)
        {
            Initialize();
            return Pool.Find(delegate(PalletData type) { return string.Compare(type.Name, name, true) == 0; });
        }

        public static string[] TypeNames
        {
            get
            {
                Initialize();
                List<string> typeNames = new List<string>();
                foreach (PalletData palletType in Pool)
                    typeNames.Add(palletType.Name);
                return typeNames.ToArray();
            }
        }

        public List<Vector3D> Lumbers { get; set; }
        public List<Position> Positions { get; set; }
        public static List<PalletData> Pool { get; set; }
        #endregion

        #region Drawing
        public void Draw(Graphics3D graphics, Vector3D dimensions, Color color, Transform3D t)
        {
            double coefX = dimensions.X / Dimensions.X;
            double coefY = dimensions.Y / Dimensions.Y;
            double coefZ = dimensions.Z / Dimensions.Z;
            uint pickId = 0;
            foreach (Position pos in Positions)
            {
                double coef0 = coefX, coef1 = coefY, coef2 = coefZ;
                if (pos.Axis1 == HalfAxis.HAxis.AXIS_X_P && pos.Axis2 == HalfAxis.HAxis.AXIS_Y_P)
                { coef0 = coefX; coef1 = coefY; }
                else if (pos.Axis1 == HalfAxis.HAxis.AXIS_Y_P && pos.Axis2 == HalfAxis.HAxis.AXIS_X_N)
                { coef0 = coefY; coef1 = coefX; }
                Vector3D dim = Lumbers[pos.Index];
                Box box = new Box(pickId++, dim.X * coef0, dim.Y * coef1, dim.Z * coef2
                    , new BoxPosition(
                        t.transform(
                            new Vector3D(pos.XYZ.X * coefX, pos.XYZ.Y * coefY, pos.XYZ.Z * coefZ))
                            , HalfAxis.Transform(pos.Axis1, t)
                            , HalfAxis.Transform(pos.Axis2, t)));
                box.SetAllFacesColor(color);
                graphics.AddBox(box);
            }
        }

        // list of boxes
        public List<Box> BuildListOfBoxes(Vector3D dimensions, Color color, Transform3D t)
        {
            List<Box> listPalletLumbers = new List<Box>();

            double coefX = dimensions.X / Dimensions.X;
            double coefY = dimensions.Y / Dimensions.Y;
            double coefZ = dimensions.Z / Dimensions.Z;
            uint pickId = 0;
            foreach (Position pos in Positions)
            {
                double coef0 = coefX, coef1 = coefY, coef2 = coefZ;
                if (pos.Axis1 == HalfAxis.HAxis.AXIS_X_P && pos.Axis2 == HalfAxis.HAxis.AXIS_Y_P)
                { coef0 = coefX; coef1 = coefY; }
                else if (pos.Axis1 == HalfAxis.HAxis.AXIS_Y_P && pos.Axis2 == HalfAxis.HAxis.AXIS_X_N)
                { coef0 = coefY; coef1 = coefX; }
                Vector3D dim = Lumbers[pos.Index];
                Box box = new Box(pickId++, dim.X * coef0, dim.Y * coef1, dim.Z * coef2
                    , new BoxPosition(
                        t.transform(new Vector3D(pos.XYZ.X * coefX, pos.XYZ.Y * coefY, pos.XYZ.Z * coefZ))
                        , HalfAxis.Transform(pos.Axis1, t), HalfAxis.Transform(pos.Axis2, t))
                        );
                box.SetAllFacesColor(color);
                listPalletLumbers.Add(box);
            }
            return listPalletLumbers;
        }
        #endregion
    }
    #endregion

    #region Pallet
    public class Pallet
    {
        #region Constructor
        public Pallet(PalletProperties palletProperties)
        {
            Length = palletProperties.Length;
            Width = palletProperties.Width;
            Height = palletProperties.Height;
            Color = palletProperties.Color;
            TypeName = palletProperties.TypeName;
        }
        #endregion

        #region Overrides
        public void Draw(Graphics3D graphics, Transform3D t)
        {
            if (Length == 0.0 || Width == 0.0 || Height == 0.0)
                return;

            PalletData palletType = PalletData.GetByName(TypeName);
            if (null != palletType)
                palletType.Draw(graphics, new Vector3D(Length, Width, Height), Color, t);
        }

        public List<Box> BuildListOfBoxes()
        {
            PalletData palletType = PalletData.GetByName(TypeName);
            if (Length == 0.0 || Width == 0.0 || Height == 0.0 || null == palletType)
                return new List<Box>();
            return palletType.BuildListOfBoxes(new Vector3D(Length, Width, Height), Color, Transform3D.Identity);
        }
        #endregion

        #region Public properties
        public Face[] Faces
        {
            get
            {
                Face[] faces = new Face[6];
                // points
                Vector3D[] points = new Vector3D[8];
                points[0] = Vector3D.Zero;
                points[1] = Length * Vector3D.XAxis;
                points[2] = Length * Vector3D.XAxis + Width * Vector3D.YAxis;
                points[3] = Width * Vector3D.YAxis;
                points[4] = Height * Vector3D.ZAxis;
                points[5] = Length * Vector3D.XAxis + Height * Vector3D.ZAxis;
                points[6] = Length * Vector3D.XAxis + Width * Vector3D.YAxis + Height * Vector3D.ZAxis;
                points[7] = Width * Vector3D.YAxis + Height * Vector3D.ZAxis;

                faces[0] = new Face(0, new Vector3D[] { points[3], points[0], points[4], points[7] }, true); // AXIS_X_N
                faces[1] = new Face(0, new Vector3D[] { points[1], points[2], points[6], points[5] }, true); // AXIS_X_P
                faces[2] = new Face(0, new Vector3D[] { points[0], points[1], points[5], points[4] }, true); // AXIS_Y_N
                faces[3] = new Face(0, new Vector3D[] { points[2], points[3], points[7], points[6] }, true); // AXIS_Y_P
                faces[4] = new Face(0, new Vector3D[] { points[3], points[2], points[1], points[0] }, true); // AXIS_Z_N
                faces[5] = new Face(0, new Vector3D[] { points[4], points[5], points[6], points[7] }, true); // AXIS_Z_P

                foreach (Face face in faces)
                    face.ColorFill = Color;

                return faces;
            }
        }

        public Color Color { get; set; }
        public string TypeName { get; set; }
        public double Length { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        #endregion
    }
    #endregion
}
