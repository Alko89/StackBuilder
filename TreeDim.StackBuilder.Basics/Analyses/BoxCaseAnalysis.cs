﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using log4net;
#endregion

namespace TreeDim.StackBuilder.Basics
{
    #region BoxCaseAnalysis
    public class BoxCaseAnalysis : ItemBase
    {
        #region Data members
        BProperties _bProperties;
        BoxProperties _caseProperties;
        BCaseConstraintSet _constraintSet;
        /// <summary>
        /// List of solutions
        /// </summary>
        List<BoxCaseSolution> _solutions;
        /// <summary>
        /// selected solution
        /// </summary>
        private List<SelBoxCaseSolution> _selectedSolutions = new List<SelBoxCaseSolution>();
        /// <summary>
        /// reference to solver used when updating is needed
        /// </summary>
        private static IBoxCaseAnalysisSolver _solver;
        /// <summary>
        /// logger
        /// </summary>
        static readonly ILog _log = LogManager.GetLogger(typeof(BoxCaseAnalysis));
        #endregion

        #region Delegates
        public delegate void ModifyAnalysis(BoxCaseAnalysis analysis);
        public delegate void SelectSolution(BoxCaseAnalysis analysis, SelBoxCaseSolution selSolution);
        #endregion

        #region Events
        public event ModifyAnalysis Modified;
        public event SelectSolution SolutionSelected;
        public event SelectSolution SolutionSelectionRemoved;
        #endregion

        #region Constructor
        public BoxCaseAnalysis(BProperties bProperties, BoxProperties caseProperties, BCaseConstraintSet constraintSet)
            : base(bProperties.ParentDocument)
        {
            if (!constraintSet.IsValid)
                throw new Exception("Using invalid box/case constraintset -> Can not instantiate box/case analysis!");
            this.BProperties = bProperties;
            this.CaseProperties = caseProperties;
            _constraintSet = constraintSet;
        }
        #endregion

        #region Public properties
        public bool IsBundleAnalysis { get { return _bProperties is BundleProperties; } }
        public bool IsBoxAnalysis { get { return _bProperties is BoxProperties; } }
        public BProperties BProperties
        {
            get { return _bProperties; }
            set
            {
                if (value == _bProperties) return;
                if (null != _bProperties) _bProperties.RemoveDependancy(this);
                _bProperties = value;
                _bProperties.AddDependancy(this);
            }
        }
        public BoxProperties CaseProperties
        {
            get { return _caseProperties; }
            set
            {
                if (value == _caseProperties) return;
                if (null != _caseProperties) _caseProperties.RemoveDependancy(this);
                _caseProperties = value;
                _caseProperties.AddDependancy(this);
            }
        }
        public BCaseConstraintSet ConstraintSet
        { get { return _constraintSet; } }

        public List<BoxCaseSolution> Solutions
        {
            get { return _solutions; }
            set
            {
                _solutions = value;
                foreach (BoxCaseSolution sol in _solutions)
                    sol.Analysis = this;
            }
        }
        public static IBoxCaseAnalysisSolver Solver
        {   set { _solver = value; } }
        #endregion

        #region Solution selection
        public void SelectSolutionByIndex(int index)
        {
            if (index < 0 || index > _solutions.Count)
                return;  // no solution with this index
            if (HasSolutionSelected(index)) return;             // solution already selected
            // instantiate new SelSolution
            SelBoxCaseSolution selSolution = new SelBoxCaseSolution(ParentDocument, this, _solutions[index]);
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
        public void UnSelectSolution(SelBoxCaseSolution selSolution)
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
        public SelBoxCaseSolution GetSelSolutionBySolutionIndex(int index)
        {
            if (index < 0 || index > _solutions.Count) return null;  // no solution with this index
            return _selectedSolutions.Find(delegate(SelBoxCaseSolution selSol) { return selSol.Solution == _solutions[index]; });
        }
        #endregion

        #region Depandancies
        protected override void OnDispose()
        {
            base.OnDispose();
        }
        protected override void RemoveItselfFromDependancies()
        {
            _bProperties.RemoveDependancy(this);
            _caseProperties.RemoveDependancy(this);
            base.RemoveItselfFromDependancies();
        }
        public override void OnAttributeModified(ItemBase modifiedAttribute)
        {
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

        public override void OnEndUpdate(ItemBase updatedAttribute)
        {
            if (null != Modified)
                Modified(this);
            // get default analysis solver
            if (null != _solver)
            {
                // clear solutions
                _solutions.Clear();
                _solver.ProcessAnalysis(this);
            }
            else
                _log.Error("_solver == null : solver was not set");

            if (_solutions.Count == 0)
                _log.Debug("Recomputed analysis has no solutions");
            // set modified / propagate modifications
            Modify();
        }
        #endregion
    }
    #endregion

    #region IBoxCaseAnalysisSolver
    public interface IBoxCaseAnalysisSolver
    {
        void ProcessAnalysis(BoxCaseAnalysis analysis);
    }
    #endregion
}
