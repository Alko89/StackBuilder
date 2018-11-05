#region Using directives
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Resources;

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

            CategoryPath = Properties.Resources.ID_OPTIONSMEASUREMENTSYSTEM;
            DisplayName = Properties.Resources.ID_DISPLAYMEASUREMENTSYSTEM;

            // -- fill cbLanguages
            int iSel = -1, i = -1;
            ReadOnlyCollection<CultureInfo> cultureInfos = GetAvailableCultures();
            foreach (CultureInfo ci in cultureInfos)
            {
                cbLanguages.Items.Add(new ComboCultureWrapper(ci));
                ++i;
                if (ci.ThreeLetterWindowsLanguageName == CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName)
                    iSel = i;
            }
            cbLanguages.SelectedIndex = iSel;
            // --

            // set current unit system
            cbUnitSystem.SelectedIndex = (int)UnitsManager.CurrentUnitSystem;
        }
        #endregion

        #region Helpers
        private static ReadOnlyCollection<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> list = new List<CultureInfo>();

            string startupDir = Application.StartupPath;
            Assembly asm = Assembly.GetEntryAssembly();

            CultureInfo neutralCulture = CultureInfo.InvariantCulture;
            if (asm != null)
            {
                NeutralResourcesLanguageAttribute attr = Attribute.GetCustomAttribute(asm, typeof(NeutralResourcesLanguageAttribute)) as NeutralResourcesLanguageAttribute;
                if (attr != null)
                    neutralCulture = CultureInfo.GetCultureInfo(attr.CultureName);
            }
            // -- rather than neutral culture choose en-us
            neutralCulture = CultureInfo.GetCultureInfo("en-US");
            list.Add(neutralCulture);

            if (asm != null)
            {
                string baseName = asm.GetName().Name;
                foreach (string dir in Directory.GetDirectories(startupDir))
                {
                    // Check that the directory name is a valid culture
                    DirectoryInfo dirinfo = new DirectoryInfo(dir);
                    CultureInfo tCulture = null;
                    try
                    {
                        tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);
                    }
                    // Not a valid culture : skip that directory
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    // Check that the directory contains satellite assemblies
                    if (dirinfo.GetFiles(baseName + ".resources.dll").Length > 0)
                    {
                        list.Add(tCulture);
                    }

                }
            }
            return list.AsReadOnly();
        }
        #endregion

        #region Handlers
        private void OnComboSelectionChanged(object sender, EventArgs e)
        {
            // unit system
            Properties.Settings.Default.UnitSystem = cbUnitSystem.SelectedIndex;
            // culture
            if (cbLanguages.SelectedItem is ComboCultureWrapper cCultWrapper)
                Properties.Settings.Default.CultureToUse = cCultWrapper.ToCultureInfoString();
            Properties.Settings.Default.Save();

            // tells the user that the application must restart
            ApplicationMustRestart = true;
        }
        #endregion

        #region ComboCultureWrapper
        internal class ComboCultureWrapper
        {
            public ComboCultureWrapper(CultureInfo ci)
            {
                _ci = ci;
            }
            public override string ToString()
            {
                return _ci.DisplayName;
            }
            public string ToCultureInfoString()
            {
                return _ci.ToString();
            }
            private CultureInfo _ci;
        }
        #endregion
    }
}
