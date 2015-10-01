﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class OptionPanelUnits : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelUnits()
        {
            InitializeComponent();
            // set current unit system
            cbUnitSystem.SelectedIndex = (int)UnitsManager.CurrentUnitSystem;
        }
        #endregion

        #region Handlers
        private void cbUnitSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            // save property
            Properties.Settings.Default.UnitSystem = cbUnitSystem.SelectedIndex;
            // tells the user that the application must restart
            this.ApplicationMustRestart = true;
        }
        #endregion
    }
}
