﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region CylinderPalletAnalysis
    public class CylinderPalletAnalysis : ItemBase
    {
        #region Data members
        private InterlayerProperties _interlayerProperties, _interlayerPropertiesAntiSlip;
        private CylinderProperties _cylinderProperties;
        private PalletProperties _palletProperties;
        private CylinderPalletConstraintSet _constraintSet;
        private List<CylinderPalletSolution> _solutions = new List<CylinderPalletSolution>();
        private List<SelCylinderPalletSolution> _selectedSolutions = new List<SelCylinderPalletSolution>();
        private static ICylinderAnalysisSolver _solver;
        static readonly ILog _log = LogManager.GetLogger(typeof(CylinderPalletAnalysis));
        #endregion

        #region Delegates
        public delegate void ModifyAnalysis(CylinderPalletAnalysis analysis);
        public delegate void SelectSolution(CylinderPalletAnalysis analysis, SelCylinderPalletSolution selSolution);
        #endregion

        #region Events
        public event ModifyAnalysis Modified;
        public event SelectSolution SolutionSelected;
        public event SelectSolution SolutionSelectionRemoved;
        #endregion

        #region Constructor
        public CylinderPalletAnalysis(
            CylinderProperties cylProperties
            , PalletProperties palletProperties
            , InterlayerProperties interlayerProperties
            , InterlayerProperties interlayerPropertiesAntiSlip
            , CylinderPalletConstraintSet constraintSet)
            : base(cylProperties.ParentDocument)
        {
            // setting members
            CylinderProperties = cylProperties;
            PalletProperties = palletProperties;
            InterlayerProperties = interlayerProperties;
            InterlayerPropertiesAntiSlip = interlayerPropertiesAntiSlip;
            _constraintSet = constraintSet;
            // has interlayer ?
            _constraintSet.HasInterlayer = null != interlayerProperties;
            _constraintSet.HasInterlayerAntiSlip = null != interlayerPropertiesAntiSlip;
        }
        #endregion

        #region Public properties

        public CylinderProperties CylinderProperties
        {
            get { return _cylinderProperties; }
            set
            {
                if (value == _cylinderProperties) return;
                if (null != _cylinderProperties) _cylinderProperties.RemoveDependancy(this);
                _cylinderProperties = value;
                _cylinderProperties.AddDependancy(this);
            }
        }
        public PalletProperties PalletProperties
        {
            get { return _palletProperties; }
            set
            {
                if (_palletProperties == value) return;
                if (null != _palletProperties) _palletProperties.RemoveDependancy(this);
                _palletProperties = value;
                _palletProperties.AddDependancy(this);
            }
        }
        public bool HasInterlayer
        {
            get { return (null != _interlayerProperties); }
        }
        public bool HasInterlayerAntiSlip
        {
            get { return (null != _interlayerPropertiesAntiSlip); }
        }
        public InterlayerProperties InterlayerProperties
        {
            get { return _interlayerProperties; }
            set
            {
                if (_interlayerProperties == value) return;
                if (null != _interlayerProperties) _interlayerProperties.RemoveDependancy(this);
                _interlayerProperties = value;
                if (null != _interlayerProperties)
                    _interlayerProperties.AddDependancy(this);
            }
        }
        public InterlayerProperties InterlayerPropertiesAntiSlip
        {
            get { return _interlayerPropertiesAntiSlip; }
            set
            {
                if (_interlayerPropertiesAntiSlip == value) return;
                if (null != _interlayerPropertiesAntiSlip) _interlayerPropertiesAntiSlip.RemoveDependancy(this);
                _interlayerPropertiesAntiSlip = value;
                if (null != _interlayerPropertiesAntiSlip)
                    _interlayerPropertiesAntiSlip.AddDependancy(this);
            }
        }
        public CylinderPalletConstraintSet ConstraintSet
        {
            set { _constraintSet = value; }
            get { return _constraintSet; }
        }
        public List<CylinderPalletSolution> Solutions
        {
            get { return _solutions; }
            set
            {
                _solutions = value;
                foreach (CylinderPalletSolution sol in _solutions)
                    sol.Analysis = this;
            }
        }

        public static ICylinderAnalysisSolver Solver
        { set { _solver = value; } }
        #endregion

        #region Solution selection
        public void SelectSolutionByIndex(int index)
        {
            if (index < 0 || index > _solutions.Count)
                return;  // no solution with this index
            if (HasSolutionSelected(index)) return;             // solution already selected
            // instantiate new SelSolution
            SelCylinderPalletSolution selSolution = new SelCylinderPalletSolution(ParentDocument, this, _solutions[index]);
            // insert in list
            _selectedSolutions.Add(selSolution);
            // fire event
            if (null != SolutionSelected)
                SolutionSelected(this, selSolution);
            // set document modified (not analysis, otherwise selected solutions are erased)
            ParentDocument.Modify();
        }
        public void UnselectSolutionByIndex(int index)
        {
            UnSelectSolution(GetSelSolutionBySolutionIndex(index));
        }
        public void UnSelectSolution(SelCylinderPalletSolution selSolution)
        {
            if (null == selSolution) return; // this solution not selected
            // remove from list
            _selectedSolutions.Remove(selSolution);
            ParentDocument.RemoveItem(selSolution);
            // fire event
            if (null != SolutionSelectionRemoved)
                SolutionSelectionRemoved(this, selSolution);
            // set document modified (not analysis, otherwise selected solutions are erased)
            ParentDocument.Modify();
        }
        public bool HasSolutionSelected(int index)
        {
            return (null != GetSelSolutionBySolutionIndex(index));
        }
        public SelCylinderPalletSolution GetSelSolutionBySolutionIndex(int index)
        {
            if (index < 0 || index > _solutions.Count) return null;  // no solution with this index
            return _selectedSolutions.Find(delegate(SelCylinderPalletSolution selSol) { return selSol.Solution == _solutions[index]; });
        }
        #endregion

        #region Dependancies
        public override void OnEndUpdate(ItemBase updatedAttribute)
        {
            if (null != Modified)
                Modified(this);
            // clear selected solutions
            while (_selectedSolutions.Count > 0)
                UnSelectSolution(_selectedSolutions[0]);
            // clear solutions
            _solutions.Clear();
            // get default analysis solver
            if (null != _solver)
                _solver.ProcessAnalysis(this);
            else
                _log.Error("_solver == null : solver was not set");
            if (_solutions.Count == 0)
                _log.Debug("Recomputed analysis has no solutions");
            // set modified / propagate modifications
            Modify();
        }
        #endregion
    }

    public interface ICylinderAnalysisSolver
    {
        void ProcessAnalysis(CylinderPalletAnalysis analysis);
    }
    #endregion

    #region HCylinderPalletAnalysis
    public class HCylinderPalletAnalysis : ItemBase
    {
        #region Data members
        private CylinderProperties _cylinderProperties;
        private PalletProperties _palletProperties;
        private HCylinderPalletConstraintSet _constraintSet;
        private List<HCylinderPalletSolution> _solutions = new List<HCylinderPalletSolution>();
        private List<SelHCylinderPalletSolution> _selectedSolutions = new List<SelHCylinderPalletSolution>();
        private static IHCylinderAnalysisSolver _solver;
        static readonly ILog _log = LogManager.GetLogger(typeof(HCylinderPalletAnalysis));
        #endregion

        #region Constructor
        public HCylinderPalletAnalysis(
            CylinderProperties cylProperties,
            PalletProperties palletProperties,
            HCylinderPalletConstraintSet constraintSet)
            : base(cylProperties.ParentDocument)
        {
            CylinderProperties = cylProperties;
            PalletProperties = palletProperties;
            _constraintSet = constraintSet;
        }
        #endregion

        #region Delegates
        public delegate void ModifyAnalysis(HCylinderPalletAnalysis analysis);
        public delegate void SelectSolution(HCylinderPalletAnalysis analysis, SelHCylinderPalletSolution selSolution);
        #endregion

        #region Events
        public event ModifyAnalysis Modified;
        public event SelectSolution SolutionSelected;
        public event SelectSolution SolutionSelectionRemoved;
        #endregion

        #region Public properties
        public CylinderProperties CylinderProperties
        {
            get { return _cylinderProperties; }
            set
            {
                if (value == _cylinderProperties) return;
                if (null != _cylinderProperties) _cylinderProperties.RemoveDependancy(this);
                _cylinderProperties = value;
                _cylinderProperties.AddDependancy(this);
            }
        }
        public PalletProperties PalletProperties
        {
            get { return _palletProperties; }
            set
            {
                if (_palletProperties == value) return;
                if (null != _palletProperties) _palletProperties.RemoveDependancy(this);
                _palletProperties = value;
                _palletProperties.AddDependancy(this);
            }
        }
        public HCylinderPalletConstraintSet ConstraintSet
        {
            set { _constraintSet = value; }
            get { return _constraintSet; }
        }
        public List<HCylinderPalletSolution> Solutions
        {
            get { return _solutions; }
            set
            {
                _solutions = value;
                foreach (HCylinderPalletSolution sol in _solutions)
                    sol.Analysis = this;
            }
        }
        public static IHCylinderAnalysisSolver Solver
        { set { _solver = value; } }
        #endregion

        #region Solution selection
        public void SelectSolutionByIndex(int index)
        {
            if (index < 0 || index > _solutions.Count)
                return;  // no solution with this index
            if (HasSolutionSelected(index)) return;             // solution already selected
            // instantiate new SelSolution
            SelHCylinderPalletSolution selSolution = new SelHCylinderPalletSolution(ParentDocument, this, _solutions[index]);
            // insert in list
            _selectedSolutions.Add(selSolution);
            // fire event
            if (null != SolutionSelected)
                SolutionSelected(this, selSolution);
            // set document modified (not analysis, otherwise selected solutions are erased)
            ParentDocument.Modify();
        }
        public void UnselectSolutionByIndex(int index)
        {
            UnSelectSolution(GetSelSolutionBySolutionIndex(index));
        }
        public void UnSelectSolution(SelHCylinderPalletSolution selSolution)
        {
            if (null == selSolution) return; // this solution not selected
            // remove from list
            _selectedSolutions.Remove(selSolution);
            ParentDocument.RemoveItem(selSolution);
            // fire event
            if (null != SolutionSelectionRemoved)
                SolutionSelectionRemoved(this, selSolution);
            // set document modified (not analysis, otherwise selected solutions are erased)
            ParentDocument.Modify();
        }
        public bool HasSolutionSelected(int index)
        {
            return (null != GetSelSolutionBySolutionIndex(index));
        }
        public SelHCylinderPalletSolution GetSelSolutionBySolutionIndex(int index)
        {
            if (index < 0 || index > _solutions.Count) return null;  // no solution with this index
            return _selectedSolutions.Find(delegate(SelHCylinderPalletSolution selSol) { return selSol.Solution == _solutions[index]; });
        }
        #endregion

        #region Dependancies
        public override void OnEndUpdate(ItemBase updatedAttribute)
        {
            if (null != Modified)
                Modified(this);
            // clear selected solutions
            while (_selectedSolutions.Count > 0)
                UnSelectSolution(_selectedSolutions[0]);
            // clear solutions
            _solutions.Clear();
            // get default analysis solver
            if (null != _solver)
                _solver.ProcessAnalysis(this);
            else
                _log.Error("_solver == null : solver was not set");
            if (_solutions.Count == 0)
                _log.Debug("Recomputed analysis has no solutions");
            // set modified / propagate modifications
            Modify();
        }
        #endregion
    }
    
    public interface IHCylinderAnalysisSolver
    {
        void ProcessAnalysis(HCylinderPalletAnalysis analysis);
    }
    #endregion
}
