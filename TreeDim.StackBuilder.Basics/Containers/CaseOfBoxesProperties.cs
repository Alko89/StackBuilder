﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class CaseOfBoxesProperties : BoxProperties
    {
        #region Data members
        private BoxProperties _boxProperties;
        private CaseDefinition _caseDefinition;
        private CaseOptimConstraintSet _constraintSet;
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiate a new case from a box, a case definition and a case optimization constraintset
        /// </summary>
        /// <param name="document">Parent document</param>
        /// <param name="bProperties">Box properties</param>
        /// <param name="constraintSet">Case optimization constraint set</param>
        public CaseOfBoxesProperties(Document document
            , BoxProperties boxProperties
            , CaseDefinition caseDefinition
            , CaseOptimConstraintSet constraintSet)
            : base(document)
        {
            _boxProperties = boxProperties;
            _boxProperties.AddDependancy(this);
            _caseDefinition = caseDefinition;
            _constraintSet = constraintSet;

            base.Weight = _caseDefinition.CaseEmptyWeight(_boxProperties, _constraintSet);

            OnAttributeModified(boxProperties);
        }
        #endregion

        #region Public properties
        public BoxProperties InsideBoxProperties
        { get { return _boxProperties; } }
        public CaseDefinition CaseDefinition
        { get { return _caseDefinition; } }
        public CaseOptimConstraintSet CaseOptimConstraintSet
        { get { return _constraintSet; } }
        public double WeightEmpty
        {
            get { return _caseDefinition.CaseEmptyWeight(_boxProperties, _constraintSet); }
        }
        /// <summary>
        /// override weight method
        /// </summary>
        public override double Weight
        {
            get { return WeightEmpty + _caseDefinition.Arrangement.Number * _boxProperties.Weight; }
        }

        public int NumberOfBoxes
        {
            get { return _caseDefinition.Arrangement.Number; }
        }
        #endregion

        #region Dependancies
        public override void OnAttributeModified(ItemBase modifiedAttribute)
        {
            Vector3D outerDim = _caseDefinition.OuterDimensions(_boxProperties, _constraintSet);
            Length = outerDim.X;
            Width = outerDim.Y;
            Height = outerDim.Z;

            Vector3D innerDim = _caseDefinition.InnerDimensions(_boxProperties);
            InsideLength = innerDim.X;
            InsideWidth = innerDim.Y;
            InsideHeight = innerDim.Z;
        }
        protected override void RemoveItselfFromDependancies()
        {
            _boxProperties.RemoveDependancy(this);
            base.RemoveItselfFromDependancies();
        }
        public override void OnEndUpdate(ItemBase updatedAttribute)
        {
            Modify();
            base.OnEndUpdate(updatedAttribute);
        }
        #endregion
    }
}
