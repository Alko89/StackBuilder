﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class BoxCasePalletSolutionViewer
    {
        #region Data members
        private BoxCasePalletSolution _caseSolution;
        private bool _showDimensions = true;
        #endregion

        #region Constructor
        public BoxCasePalletSolutionViewer(BoxCasePalletSolution caseSolution)
        {
            _caseSolution = caseSolution;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Draw case solution
        /// </summary>
        public void Draw(Graphics3D graphics)
        {
            if (null == _caseSolution)
                throw new Exception("No case solution defined!");

            // load pallet solution
            BoxProperties caseProperties;

            CasePalletSolution palletSolution = _caseSolution.PalletSolutionDesc.LoadPalletSolution();
            if (null == palletSolution)
                caseProperties = new BoxProperties(null, _caseSolution.CaseLength, _caseSolution.CaseWidth, _caseSolution.CaseHeight);
            else
            {
                CasePalletAnalysis palletAnalysis = palletSolution.Analysis;
                // retrieve case properties 
                caseProperties = palletAnalysis.BProperties as BoxProperties;
            }
            if (null == caseProperties) return;
            // draw case (inside)
            Case case_ = new Case(caseProperties);
            case_.DrawInside(graphics, Transform3D.Identity);
            // get case analysis
            BoxCasePalletAnalysis caseAnalysis = _caseSolution.ParentCaseAnalysis;
            // draw solution
            uint pickId = 0;
            foreach (ILayer layer in _caseSolution)
            {
                Layer3DBox blayer = layer as Layer3DBox;
                if (null != blayer)
                {
                    foreach (BoxPosition bPosition in blayer)
                        graphics.AddBox(new Box(pickId++, caseAnalysis.BoxProperties, bPosition));
                }

                InterlayerPos interlayerPos = layer as InterlayerPos;
                if (null != interlayerPos)
                {
                    Box box = new Box(pickId++, caseAnalysis.InterlayerProperties);
                    // set position
                    box.Position = new Vector3D(0.0, 0.0, interlayerPos.ZLow);
                    // draw
                    graphics.AddBox(box);
                }
            }
            // get case analysis
            if (_showDimensions)
                graphics.AddDimensions(new DimensionCube(_caseSolution.CaseLength, _caseSolution.CaseWidth, _caseSolution.CaseHeight));
        }
        /// <summary>
        /// Draw a 2D representation of first (and second, if solution does not have homogeneous layers) layer(s)
        /// </summary>
        public void Draw(Graphics2D graphics)
        {
            if (null == _caseSolution)
                throw new Exception("No case solution defined!");

            BoxCasePalletAnalysis caseAnalysis = _caseSolution.ParentCaseAnalysis;

            if (_caseSolution.HasHomogeneousLayers)
            {
                graphics.NumberOfViews = 1;
                graphics.SetViewport(0.0f, 0.0f, (float)_caseSolution.CaseLength, (float)_caseSolution.CaseWidth);

                Layer3DBox blayer = _caseSolution[0] as Layer3DBox;
                if (blayer != null)
                {
                    graphics.SetCurrentView(0);
                    graphics.DrawRectangle(Vector2D.Zero, new Vector2D(_caseSolution.CaseLength, _caseSolution.CaseWidth), Color.Black);
                    uint pickId = 0;
                    foreach (BoxPosition bPosition in blayer)
                        graphics.DrawBox(new Box(pickId++, caseAnalysis.BoxProperties, bPosition));
                }
            }
            else 
            {
                graphics.NumberOfViews = 2;
                graphics.SetViewport(0.0f, 0.0f, (float)_caseSolution.CaseLength, (float)_caseSolution.CaseWidth);

                // get first box layer
                if (_caseSolution.Count < 1) return;
                Layer3DBox blayer0 = _caseSolution[0] as Layer3DBox;
                if (blayer0 != null)
                {
                    graphics.SetCurrentView(0);
                    graphics.DrawRectangle(Vector2D.Zero, new Vector2D(_caseSolution.CaseLength, _caseSolution.CaseWidth), Color.Black);
                    uint pickId = 0;
                    foreach (BoxPosition bPosition in blayer0)
                        graphics.DrawBox(new Box(pickId++, caseAnalysis.BoxProperties, bPosition));
                }

                // get second box layer
                if (_caseSolution.Count < 2) return;
                Layer3DBox blayer1 = _caseSolution[1] as Layer3DBox;
                if (null == blayer1 && _caseSolution.Count > 2)
                    blayer1 = _caseSolution[2] as Layer3DBox;
                if (blayer1 != null)
                {
                    graphics.SetCurrentView(1);
                    graphics.DrawRectangle(Vector2D.Zero, new Vector2D(_caseSolution.CaseLength, _caseSolution.CaseWidth), Color.Black);
                    uint pickId = 0;
                    foreach (BoxPosition bPosition in blayer1)
                        graphics.DrawBox(new Box(pickId++, caseAnalysis.BoxProperties, bPosition));
                }
            }
        }
        /// <summary>
        /// Draw layers
        /// Images are used during report generation
        /// </summary>
        public void DrawLayers(Graphics3D graphics, bool showPallet, int layerIndex)
        {
            if (null == _caseSolution)
                throw new Exception("No solution defined!");
            BoxCasePalletAnalysis caseAnalysis = _caseSolution.ParentCaseAnalysis;

            // draw solution
            uint pickId = 0;
            int iLayer = 0, iLayerCount = 0;
            while (iLayerCount <= layerIndex && iLayer < _caseSolution.Count)
            {
                ILayer layer = _caseSolution[iLayer];
                Layer3DBox blayer = layer as Layer3DBox;
                if (null != blayer)
                {
                    foreach (BoxPosition bPosition in blayer)
                        graphics.AddBox(new Box(pickId++, caseAnalysis.BoxProperties, bPosition));
                    ++iLayerCount;
                }
                ++iLayer;
            }
        }
        #endregion

        #region Public properties
        public bool ShowDimensions
        {
            get { return _showDimensions; }
            set { _showDimensions = value; }
        }
        #endregion
    }
}
