﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;

using log4net;

using Sharp3D.Math.Core;
using TreeDim.StackBuilder.Basics;
using TreeDim.StackBuilder.Graphics;
using TreeDim.StackBuilder.Desktop.Properties;
using TreeDim.StackBuilder.Reporting;
#endregion

namespace TreeDim.StackBuilder.Desktop
{
    public partial class DockContentReport : DockContent, IView, IItemListener
    {
        #region Data members
        /// <summary>
        /// document
        /// </summary>
        private IDocument _document;
        /// <summary>
        /// Selected solution
        /// </summary>
        private SelSolution _selSolution;
        /// <summary>
        /// Path of html file to show
        /// </summary>
        private string _htmlFilePath;
        /// <summary>
        /// logger
        /// </summary>
        static readonly ILog _log = LogManager.GetLogger(typeof(DockContentAnalysis));
        #endregion

        #region Constructor
        public DockContentReport(IDocument document, SelSolution selSolution, string htmlFilePath)
        {
            _document = document;
            _selSolution = selSolution;
            _selSolution.AddListener(this);
            _htmlFilePath = htmlFilePath;

            InitializeComponent();
        }
        #endregion

        #region IItemListener implementation
        /// <summary>
        /// overrides IItemListener.Update
        /// </summary>
        /// <param name="item"></param>
        public void Update(ItemBase item)
        {
        }
        /// <summary>
        /// overrides IItemListener.Kill
        /// handles analysis removal for any reason (deletion/document closing)
        /// </summary>
        /// <param name="item"></param>
        public void Kill(ItemBase item)
        {
            Close();
        }
        #endregion

        #region IView implementation
        public IDocument Document
        {
            get { return _document; }
        }
        #endregion

        #region Specific properties
        public SelSolution SelSolution
        {
            get { return _selSolution; }
        }
         #endregion

        private void DockContentReport_Load(object sender, EventArgs e)
        {
            // form caption
            this.Text = string.Format("{0} report", _selSolution.Name);
            // display html
            _webBrowser.Navigate(_htmlFilePath, string.Empty, null, string.Empty);
        }
    }
}