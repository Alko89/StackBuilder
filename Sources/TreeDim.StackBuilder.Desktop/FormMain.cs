#region Using directives
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

using WeifenLuo.WinFormsUI.Docking;
using log4net;
using Utilities;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Reporting;
using treeDiM.StackBuilder.Plugin;
//using treeDiM.PLMPack.DBClient;
//using treeDiM.PLMPack.DBClient.PLMPackSR;
using treeDiM.StackBuilder.Exporters;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormMain
        : Form, IDocumentFactory, IMRUClient, IDocumentListener
    {
        #region Data members
        /// <summary>
        /// Docking manager
        /// </summary>
        private DockContentDocumentExplorer _documentExplorer = new DockContentDocumentExplorer();
        private DockContentLogConsole _logConsole = new DockContentLogConsole();
        private DockContentStartPage _dockStartPage = new DockContentStartPage();
        private ToolStripProfessionalRenderer _defaultRenderer = new ToolStripProfessionalRenderer(new PropertyGridEx.CustomColorScheme());
        private DeserializeDockContent _deserializeDockContent;
        /// <summary>
        /// List of document instance
        /// The document class holds data (boxes/pallets/interlayers/anslyses)
        /// </summary>
        private List<IDocument> _documents = new List<IDocument>();
        static readonly ILog _log = LogManager.GetLogger(typeof(FormMain));
        private static FormMain _instance;
        private MruManager _mruManager;
        #endregion

        #region Constructor
        public FormMain()
        {
            // set static instance
            _instance = this;
            
            // load content
            _deserializeDockContent = new DeserializeDockContent(ReloadContent);

            InitializeComponent();


            // load file passed as argument
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                string joinedString = string.Empty;
                for (int i = 1; i < args.Length; ++i)
                    joinedString += (i > 1 ? " " : "") + args[i];
                if (File.Exists(args[1]))
                    OpenDocument(args[1]);
                else if (File.Exists(joinedString))
                    OpenDocument(joinedString);
            }
            // or show splash sceen 
            else
            {
                bool multithreaded = false;
                if (multithreaded)
                {
                    // --- instantiate and start splach screen thread
                    Thread th = new Thread(new ThreadStart(DoSplash));
                    th.Start();
                    // ---
                }
                else
                    DoSplash();
            }
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // loading plugin
            IPlugin plugin = LoadPlugin();
            if (null != plugin)
            {
                plugin.Initialize();
                plugin.UpdateToolbar(toolStripSplitButtonNew);
                plugin.OpenFile += OpenGeneratedFile;
            }
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            // Apply a gray professional renderer as a default renderer
            ToolStripManager.Renderer = _defaultRenderer;
            _defaultRenderer.RoundedEdges = true;

            // Set DockPanel properties
            var theme = new VS2015BlueTheme();
            dockPanel.Theme = theme;

            dockPanel.ActiveAutoHideContent = null;
            dockPanel.Parent = this;

            dockPanel.SuspendLayout(true);
            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, _deserializeDockContent);
            else
                timerLogin.Start();
            dockPanel.ResumeLayout(true, true);

            // MRUManager
            _mruManager = new MruManager();
            _mruManager.Initialize(
             this,                              // owner form
             mnuFileMRU,                        // Recent Files menu item
             "Software\\TreeDim\\StackBuilder"); // Registry path to keep MRU list

            UpdateToolbarState();

            // show or hide PLMPackLib button
            bool showMenuPLMPackLib = !string.IsNullOrEmpty(UrlPLMPackLib);
            toolStripMIPLMPackLib.Visible = showMenuPLMPackLib;
            toolStripSeparatorPLMPackLib.Visible = showMenuPLMPackLib;

            // windows settings
            if (null != Settings.Default.FormMainPosition)
                Settings.Default.FormMainPosition.Restore(this);

            // connection/disconnection event handling
            if (!Program.UseDisconnected)
            {
                //WCFClient.Connected += OnConnected;
                //WCFClient.Disconnected += OnDisconnected;
            }
            else
            {
                CreateBasicLayout();
                UpdateDisconnectButton();
            }
        }

        private void OpenGeneratedFile(string s)
        {
            OpenDocument(s);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (null == Settings.Default.FormMainPosition)
                    Settings.Default.FormMainPosition = new WindowSettings();
                Settings.Default.FormMainPosition.Record(this);
                Settings.Default.Save();

            }
            catch (System.Configuration.ConfigurationErrorsException ex)
            {
                _log.Error(string.Format("Failed to save user configuration: {0}", ex.ToString()));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            CloseAllDocuments(e);
            base.OnClosing(e);
        }
        #endregion

        #region Plugin loading
        private bool HasPlugin
        {
            get
            {
                string pluginPath = Settings.Default.PluginPath;
                return string.IsNullOrEmpty(pluginPath) && File.Exists(pluginPath);
            }
        }
        private IPlugin LoadPlugin()
        {
            string pluginPath = Properties.Settings.Default.PluginPath;
            if (string.IsNullOrEmpty(pluginPath)
                || string.Equals(Path.GetExtension(pluginPath), "dll")
                || !File.Exists(pluginPath))
                return null;

            Solution.SetSolver(new LayerSolver());

            IPlugin plugin = null;
            try
            {
                // Create a new assembly from the plugin file we're adding..
                Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);
                // Next we'll loop through all the Types found in the assembly
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.IsPublic) // Only look at public types
                    {
                        if (!pluginType.IsAbstract)  // Only look at non-abstract types
                        {
                            // Gets a type object of the interface we need the plugins to match
                            Type typeInterface = pluginType.GetInterface("treeDiM.StackBuilder.Plugin.IPlugin", true);

                            // Make sure the interface we want to use actually exists
                            if (typeInterface != null)
                            {
                                // Create a new available plugin since the type implements the IPlugin interface
                                plugin = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                                if (null != plugin)
                                {
                                    // Call the initialization sub of the plugin
                                    plugin.Initialize();
                                }
                            }
                            typeInterface = null; // cleanup		
                        }
                    }
                }
                pluginAssembly = null; //more cleanup
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            return plugin;
        }
        #endregion

        #region SplashScreen
        public void DoSplash()
        {
            using (FormSplashScreen sp = new FormSplashScreen(this))
            {
                sp.TimerInterval = 1000;
                sp.ShowDialog();
            }
        }
        #endregion

        #region Start page
        private string UrlStartPage => Settings.Default.StartPageUrl;
        private string UrlPLMPackLib => Settings.Default.UrlPLMPackLib;

        public void ShowStartPage(object sender, EventArgs e)
        {
            // do not try to show start page
            // if StartPageUrl is empty or web site is reachable
            if (string.IsNullOrEmpty(UrlStartPage) || !Program.IsWebSiteReachable)
                return;
           _log.InfoFormat("Showing start page (URL : {0})", UrlStartPage);

            try
            {
                _dockStartPage.Show(dockPanel, DockState.Document);
                _dockStartPage.Url = new Uri(UrlStartPage);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
         }
        private void CloseStartPage()
        {
            if (null != _dockStartPage)
                _dockStartPage.Hide();
        }
        private void OnTimerLoginTick(object sender, EventArgs e)
        {
            timerLogin.Stop();
            try
            {
                // show login form
                /*if (!WCFClient.IsConnected && !Program.UseDisconnected)
                {
                    using (WCFClient wcfClient = new WCFClient())
                    {
                        wcfClient.Client.Connect();
                    }
                }*/
                // note : CreateBasicLayout now called by OnConnected()
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Login failed (Exception = {0})", ex.Message));
            }
        }
        #endregion

        #region Docking
        private void CreateBasicLayout()
        {
            _documentExplorer.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            _documentExplorer.DocumentTreeView.AnalysisNodeClicked += new AnalysisTreeView.AnalysisNodeClickHandler(DocumentTreeView_NodeClicked);
            _documentExplorer.DocumentTreeView.SolutionReportClicked += new AnalysisTreeView.AnalysisNodeClickHandler(OnSolutionReportNodeClicked);
            ShowLogConsole();
            if (AssemblyConf != "debug" && Properties.Settings.Default.ShowStartPage)
                ShowStartPage(this, null);
        }

        private void ClearBasicLayout()
        {
            _documentExplorer.Hide();
            _documentExplorer.DocumentTreeView.AnalysisNodeClicked -= DocumentTreeView_NodeClicked;
            _documentExplorer.DocumentTreeView.SolutionReportClicked -= OnSolutionReportNodeClicked;
        }

        public void ShowLogConsole()
        {
            // show or hide log console ?
            if (AssemblyConf == "debug" || Settings.Default.ShowLogConsole)
            {
                _logConsole = new DockContentLogConsole();
                _logConsole.Show(dockPanel, DockState.DockBottom);
            }
            else
            {
                if (null != _logConsole)
                    _logConsole.Close();
                _logConsole = null;
            }
        }

        private IDockContent ReloadContent(string persistString)
        {
            switch (persistString)
            {
                case "frmDocument":
                    return null;
                case "frmSolution":
                    _documentExplorer = new DockContentDocumentExplorer();
                    return _documentExplorer;
                case "frmLogConsole":
                    _logConsole = new DockContentLogConsole();
                    return _logConsole;
                default:
                    return null;
            }
        }
        #endregion

        #region DocumentTreeView event handlers
        // ### AnalysisNodeClicked
        void DocumentTreeView_NodeClicked(object sender, AnalysisTreeViewEventArgs eventArg)
        {
            if (null != eventArg.Analysis)
                CreateOrActivateViewAnalysis(eventArg.Analysis);
            else if (null != eventArg.ItemBase)
            {
                ItemBase itemProp = eventArg.ItemBase;
                if (itemProp.GetType() == typeof(BoxProperties))
                {
                    BoxProperties box = itemProp as BoxProperties;
                    FormNewBox form = new FormNewBox(eventArg.Document, eventArg.ItemBase as BoxProperties);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(box)) return;
                        box.ID.SetNameDesc(form.BoxName, form.Description);
                        box.SetLength(form.BoxLength);
                        box.SetWidth(form.BoxWidth);
                        box.SetHeight(form.BoxHeight);
                        box.SetWeight(form.Weight);
                        box.SetNetWeight(form.NetWeight);
                        box.InsideLength = form.InsideLength;
                        box.InsideWidth = form.InsideWidth;
                        box.InsideHeight = form.InsideHeight;
                        box.SetAllColors(form.Colors);
                        box.TextureList = form.TextureList;
                        box.TapeWidth = form.TapeWidth;
                        box.TapeColor = form.TapeColor;
                        box.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(PackProperties))
                {
                    PackProperties pack = itemProp as PackProperties;
                    FormNewPack form = new FormNewPack(eventArg.Document, eventArg.ItemBase as PackProperties)
                    {
                        Boxes = eventArg.Document.Boxes.ToList()
                    };
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(pack)) return;
                        pack.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        pack.Box = form.SelectedBox;
                        pack.BoxOrientation = form.BoxOrientation;
                        pack.Arrangement = form.Arrangement;
                        pack.Wrap = form.Wrapper;
                        if (form.HasForcedOuterDimensions)
                            pack.ForceOuterDimensions(form.OuterDimensions);
                        pack.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(CylinderProperties))
                {
                    CylinderProperties cylinderProperties = itemProp as CylinderProperties;
                    FormNewCylinder form = new FormNewCylinder(eventArg.Document, cylinderProperties);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (!UserAcknowledgeDependancies(cylinderProperties)) return;
                        cylinderProperties.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        cylinderProperties.RadiusOuter = form.RadiusOuter;
                        cylinderProperties.RadiusInner = form.RadiusInner;
                        cylinderProperties.Height = form.CylinderHeight;
                        cylinderProperties.SetWeight(form.Weight);
                        cylinderProperties.ColorTop = form.ColorTop;
                        cylinderProperties.ColorWallOuter = form.ColorWallOuter;
                        cylinderProperties.ColorWallInner = form.ColorWallInner;
                        cylinderProperties.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(BundleProperties))
                {
                    BundleProperties bundle = itemProp as BundleProperties;
                    FormNewBundle form = new FormNewBundle(eventArg.Document, bundle);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(bundle)) return;
                        bundle.ID.SetNameDesc(form.BundleName, form.Description);
                        bundle.SetLength(form.BundleLength);
                        bundle.SetWidth(form.BundleWidth);
                        bundle.UnitThickness = form.UnitThickness;
                        bundle.UnitWeight = form.UnitWeight;
                        bundle.NoFlats = form.NoFlats;
                        bundle.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(InterlayerProperties))
                {
                    InterlayerProperties interlayer = itemProp as InterlayerProperties;
                    FormNewInterlayer form = new FormNewInterlayer(eventArg.Document, interlayer);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(interlayer)) return;
                        interlayer.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        interlayer.Length = form.InterlayerLength;
                        interlayer.Width = form.InterlayerWidth;
                        interlayer.Thickness = form.Thickness;
                        interlayer.Weight = form.Weight;
                        interlayer.Color = form.Color;
                        interlayer.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletProperties))
                {
                    PalletProperties pallet = itemProp as PalletProperties;
                    FormNewPallet form = new FormNewPallet(eventArg.Document, pallet);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(pallet)) return;
                        pallet.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        pallet.Length = form.PalletLength;
                        pallet.Width = form.PalletWidth;
                        pallet.Height = form.PalletHeight;
                        pallet.Weight = form.Weight;
                        pallet.TypeName = form.PalletTypeName;
                        pallet.Color = form.PalletColor;
                        pallet.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(TruckProperties))
                {
                    TruckProperties truck = itemProp as TruckProperties;
                    FormNewTruck form = new FormNewTruck(eventArg.Document, truck);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(truck)) return;
                        truck.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        truck.Length = form.TruckLength;
                        truck.Width = form.TruckWidth;
                        truck.Height = form.TruckHeight;
                        truck.AdmissibleLoadWeight = form.TruckAdmissibleLoadWeight;
                        truck.Color = form.TruckColor;
                        truck.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletCornerProperties))
                {
                    PalletCornerProperties corner = itemProp as PalletCornerProperties;
                    FormNewPalletCorners form = new FormNewPalletCorners(eventArg.Document, corner);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(corner)) return;
                        corner.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        corner.Length = form.CornerLength;
                        corner.Width = form.CornerWidth;
                        corner.Thickness = form.CornerThickness;
                        corner.Color = form.CornerColor;
                        corner.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletCapProperties))
                {
                    PalletCapProperties cap = itemProp as PalletCapProperties;
                    FormNewPalletCap form = new FormNewPalletCap(eventArg.Document, cap);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(cap)) return;
                        cap.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        cap.Color = form.CapColor;
                        cap.Length = form.CapLength;
                        cap.Width = form.CapWidth;
                        cap.Height = form.CapHeight;
                        cap.InsideLength = form.CapInnerLength;
                        cap.InsideWidth = form.CapInnerWidth;
                        cap.InsideHeight = form.CapInnerHeight;
                        cap.EndUpdate();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletFilmProperties))
                {
                    PalletFilmProperties film = itemProp as PalletFilmProperties;
                    FormNewPalletFilm form = new FormNewPalletFilm(eventArg.Document, film);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        film.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        film.UseTransparency = form.UseTransparency;
                        film.UseHatching = form.UseHatching;
                        film.HatchSpacing = form.HatchSpacing;
                        film.HatchAngle = form.HatchAngle;
                        film.Color = form.FilmColor;
                        film.EndUpdate();
                    }
                }
                else
                    Debug.Assert(false);
            }
        }
        private bool UserAcknowledgeDependancies(ItemBase item)
        {
            if (item.HasDependancies)
            {
                if (DialogResult.Cancel == MessageBox.Show(
                    string.Format(Resources.ID_DEPENDINGANALYSES, item.Name, item.DependancyString)
                    , Application.ProductName
                    , MessageBoxButtons.OKCancel))
                    return false;
            }
            return true;
        }

        public static void GenerateReport(Analysis analysis)
        {
            try
            {
                FormReportDesign form = new FormReportDesign()
                {
                    Analysis = analysis
                };
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); Program.SendCrashReport(ex);
            }
        }
        public static void GenerateExport(Analysis analysis, string extension)
        {
            try
            {
                Exporter exporter = ExporterFactory.GetExporterByExt(extension);

                SaveFileDialog saveFileDialogExportXML = new System.Windows.Forms.SaveFileDialog();
                saveFileDialogExportXML.FileName = analysis.Name + "." + exporter.Extension;
                saveFileDialogExportXML.Filter = exporter.Filter;
                saveFileDialogExportXML.DefaultExt = exporter.Extension;

                if (DialogResult.OK == saveFileDialogExportXML.ShowDialog())
                {
                    string filePath = saveFileDialogExportXML.FileName;
                    exporter.Export(analysis, filePath);

                    if (exporter.Extension == "dae")
                    {
                        // browse with google chrome
                        if (ExporterCollada.ChromeInstalled)
                            ExporterCollada.BrowseWithGoogleChrome(filePath);
                        else
                            MessageBox.Show(Resources.ID_CHROMEISNOTINSTALLED, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); Program.SendCrashReport(ex);
            }
        }

        private void OnSolutionReportNodeClicked(object sender, AnalysisTreeViewEventArgs eventArg)
        {
            if (null != eventArg.Analysis)
                GenerateReport(eventArg.Analysis);
        }

        void OnEditAnalysis(object sender, AnalysisTreeViewEventArgs eventArg)
        {
            try
            {
                if ((eventArg.Document is DocumentSB doc) && (null != eventArg.Analysis))
                    doc.EditAnalysis(eventArg.Analysis);
                CreateOrActivateViewAnalysis(eventArg.Analysis);                      
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); Program.SendCrashReport(ex);
            }
        }
        #endregion

        #region Caption text / toolbar state
        private void UpdateFormUI()
        {
            // form text
            string caption = string.Empty;
            if (ActiveDocument is DocumentSB doc)
            {
                caption += Path.GetFileNameWithoutExtension(doc.FilePath);
                caption += " - ";
            }
            caption += Application.ProductName;
            Text = caption;
        }
        /// <summary>
        /// enables / disable menu/toolbar items
        /// </summary>
        private void UpdateToolbarState()
        {
            DocumentSB doc = (DocumentSB)ActiveDocument;
            // save
            toolStripMenuItemSave.Enabled = (null != doc) && (doc.IsDirty);
            toolStripButtonSave.Enabled = (null != doc) && (doc.IsDirty);
            // save all
            toolStripMenuItemSaveAll.Enabled = OneDocDirty;
            toolStripButtonSaveAll.Enabled = OneDocDirty;
            // save as
            toolStripMenuItemSaveAs.Enabled = (null != doc);
            // close
            toolStripMenuItemClose.Enabled = (null != doc);
            // new box
            toolStripMenuItemBox.Enabled = (null != doc);
            toolStripButtonBox.Enabled = (null != doc);
            // new case
            toolStripMenuItemCase.Enabled = (null != doc);
            toolStripButtonCase.Enabled = (null != doc);
            // new pack
            toolStripMenuItemPack.Enabled = (null != doc) && doc.CanCreatePack;
            toolStripButtonPack.Enabled = (null != doc) && doc.CanCreatePack;
            // new bundle
            toolStripMenuItemBundle.Enabled = (null != doc);
            toolStripButtonBundle.Enabled = (null != doc);
            // new cylinder
            toolStripMenuItemCylinder.Enabled = (null != doc);
            toolStripButtonCylinder.Enabled = (null != doc);
            // new pallet
            toolStripMenuItemPallet.Enabled = (null != doc);
            toolStripButtonPallet.Enabled = (null != doc);
            // pallet decoration
            toolStripSBPalletDeco.Enabled = (null != doc);
            toolStripMenuItemInterlayer.Enabled = (null != doc);
            toolStripMenuItemPalletCap.Enabled = (null != doc);
            toolStripMenuItemPalletFilm.Enabled = (null != doc);
            toolStripMenuItemPalletCorners.Enabled = (null != doc);
            toolStripMIInterlayer.Enabled = (null != doc);
            toolStripMIPalletCap.Enabled = (null != doc);
            toolStripMIPalletCorner.Enabled = (null != doc);
            toolStripMIPalletFilm.Enabled = (null != doc);
            // new truck
            toolStripMenuItemTruck.Enabled = (null != doc);
            toolStripButtonTruck.Enabled = (null != doc);
            // new case/pallet analysis
            toolStripMenuItemNewAnalysisCasePallet.Enabled = (null != doc) && doc.CanCreateAnalysisCasePallet;
            toolStripMIAnalysisCasePallet.Enabled = (null != doc) && doc.CanCreateAnalysisCasePallet;
            toolStripMIAnalysisBundlePallet.Enabled = (null != doc) && doc.CanCreateAnalysisBundlePallet;
            // new cylinder/pallet analysis
            toolStripMenuItemNewAnalysisCylinderPallet.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderPallet;
            toolStripMIAnalysisCylinderPallet.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderPallet;
            // new box/case analysis
            toolStripMenuItemNewAnalysisBoxCase.Enabled = (null != doc) && doc.CanCreateAnalysisBoxCase;
            toolStripMIAnalysisBoxCase.Enabled = (null != doc) && doc.CanCreateAnalysisBoxCase;
            toolStripMIAnalysisBundleCase.Enabled = (null != doc) && doc.CanCreateAnalysisBundleCase;
            // new cylinder/case analysis
            toolStripMenuItemNewAnalysisCylinderCase.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderCase;
            toolStripMIAnalysisCylinderCase.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderCase;
            // new pallet/truck analysis
            toolStripMenuItemAnalysisPalletTruck.Enabled = (null != doc) && doc.CanCreateAnalysisPalletTruck;
            toolStripSBAnalysesTruck.Enabled = (null != doc) && doc.CanCreateAnalysisCaseTruck;
            // split buttons
            toolStripSBAnalysisPallet.Enabled = (null != doc);
            toolStripSBAnalysesCase.Enabled = (null != doc);
            toolStripSBOptimisations.Enabled = (null != doc);
            toolStripSBPalletDeco.Enabled = (null != doc);
            // optimisations
            toolStripMIBestCase.Enabled         = (null != doc) && doc.CanCreateOptiMulticase;
            toolStripMenuItemBestCase.Enabled   = (null != doc) && doc.CanCreateOptiMulticase;
            toolStripMIBestCasePallet.Enabled   = (null != doc) && doc.CanCreateOptiCasePallet;
            toolStripMIBestPack.Enabled         = (null != doc) && doc.CanCreateOptiPack;
            toolStripMenuItemBestPack.Enabled   = (null != doc) && doc.CanCreateOptiPack;
            // disconnected mode
            toolStripMenuItemEditDB.Enabled = !Program.UseDisconnected;
            editPaletSolutionsDB.Enabled = !Program.UseDisconnected;

        }
        #endregion

        #region IDocumentFactory implementation
        public void NewDocument()
        {
            // set solver
            Document.SetSolver(new LayerSolver());
            Solution.SetSolver(new LayerSolver());

            FormNewDocument form = new FormNewDocument();
            if (DialogResult.OK == form.ShowDialog())
            {
                AddDocument(new DocumentSB(form.DocName, form.DocDescription, form.Author, _documentExplorer.DocumentTreeView));
                _log.Debug("New document added!");
            }
            UpdateFormUI();
        }

        public void OpenDocument(string filePath)
        {
            try
            {
                // set solver
                Document.SetSolver(new LayerSolver());
                Solution.SetSolver(new LayerSolver());

                if (!File.Exists(filePath))
                {
                    // update mruFileManager as we failed to load file
                    if (null != _mruManager)
                        _mruManager.Remove(filePath);
                    MessageBox.Show(string.Format(Resources.ID_FILENOTFOUND, filePath));
                    return;
                }
                AddDocument(new DocumentSB(filePath, _documentExplorer.DocumentTreeView));

                // update mruFileManager as file was successfully loaded
                if (null != _mruManager)
                    _mruManager.Add(filePath);

                _log.Debug(string.Format("File {0} loaded!", filePath));
            }
            catch (Exception ex)
            {
                // update mruFileManager as we failed to load file
                if (null != _mruManager)
                    _mruManager.Remove(filePath);

                _log.Error(ex.ToString());   Program.SendCrashReport(ex);
            }
            UpdateFormUI();
        }

        public void AddDocument(IDocument doc)
        {
            // add this form as document listener
            if (doc is DocumentSB docSB)
                docSB.AddListener(this);
            // add document 
            _documents.Add(doc);
            doc.Modified += new EventHandler(OnDocumentModified);
            UpdateToolbarState();
        }

        public void SaveDocument()
        {
            IDocument doc = ActiveDocument;
            if (null == doc) return;
            CancelEventArgs e = new CancelEventArgs();
            SaveDocument(doc, e);
        }

        public void SaveDocument(IDocument doc, CancelEventArgs e)
        {
            if (doc.IsNew)
                SaveDocumentAs(doc, e);
            else
                doc.Save();
        }

        public void SaveAllDocuments()
        {
            CancelEventArgs e = new CancelEventArgs();
            SaveAllDocuments(e);
        }

        public void SaveAllDocuments(CancelEventArgs e)
        {
            if (e.Cancel) return;
            foreach (IDocument doc in Documents)
                SaveDocument(doc, e);
        }

        public void CloseAllDocuments(CancelEventArgs e)
        {
            // exit if already canceled
            if (e.Cancel) return;

            // try and close every opened documents
            while (_documents.Count > 0)
            {
                if (e.Cancel) return;

                IDocument doc = _documents[0];
                CloseDocument(doc, e);
            }
        }

        public void CloseDocument(IDocument doc, CancelEventArgs e)
        {
            // exit if already canceled
            if (e.Cancel)
                return;
            if (doc.IsDirty)
            {
                DialogResult res = MessageBox.Show(string.Format(Resources.ID_SAVEMODIFIEDFILE, doc.Name), Application.ProductName, MessageBoxButtons.YesNoCancel);
                if (DialogResult.Yes == res)
                {
                    SaveDocument(doc, e);
                    e.Cancel = false;
                }
                else if (DialogResult.No == res)
                    e.Cancel = false;
                else if (DialogResult.Cancel == res)
                    e.Cancel = true;
            }
            if (e.Cancel)
                return;
            // close document
            doc.Close();
            // remove from document list
            _documents.Remove(doc);
            // handle the case
            _log.Debug(string.Format("Document {0} closed", doc.Name));
            // update toolbar
            UpdateToolbarState();
        }

        public void SaveDocumentAs(IDocument doc, CancelEventArgs e)
        {
            saveFileDialogSB.FileName = doc.Name + ".stb";
            if (saveFileDialogSB.ShowDialog() == DialogResult.OK)
                doc.SaveAs(saveFileDialogSB.FileName);
            else
                e.Cancel = true;
        }

        public void SaveDocumentAs()
        {
            IDocument doc = ActiveDocument;
            if (null == doc) return;
            CancelEventArgs e = new CancelEventArgs();
            SaveDocumentAs(doc, e);
        }

        public void CloseDocument()
        {
            IDocument doc = ActiveDocument;
            if (null == doc) return;

            // close active document
            CancelEventArgs e = new CancelEventArgs();
            CloseDocument(doc, e);
        }
        /// <summary>
        /// Access list of documents
        /// </summary>
        public List<IDocument> Documents { get { return _documents; } }

        /// <summary>
        /// Get active view based on ActiveMdiChild
        /// </summary>
        public IView ActiveView
        {
            get
            {
                // search ammong existing views
                foreach (IDocument doc in Documents)
                    foreach (IView view in doc.Views)
                    {
                        Form form = view as Form;
                        if (this.ActiveMdiChild == form)
                            return view;
                    }
                return null;
            }            
        }

        public IDocument ActiveDocument
        {
            get
            {
                IView view = ActiveView;
                if (view != null)
                    return view.Document;
                else if (_documents.Count == 1)
                    return _documents[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns true if at least one document is dirty
        /// </summary>
        public bool OneDocDirty
        {
            get
            {
                foreach (IDocument doc in Documents)
                    if (doc.IsDirty)
                        return true;
                return false;
            }
        }
        #endregion

        #region IDocumentListener implementation
        // new
        public void OnNewDocument(Document doc) {}
        public void OnNewTypeCreated(Document doc, ItemBase itemBase) { }
        public void OnNewAnalysisCreated(Document doc, Analysis analysis) => CreateOrActivateViewAnalysis(analysis);
        public void OnAnalysisUpdated(Document doc, Analysis analysis) => CreateOrActivateViewAnalysis(analysis);
        public void OnNewECTAnalysisCreated(Document doc) {  }
        // remove
        public void OnTypeRemoved(Document doc, ItemBase itemBase) { }
        public void OnAnalysisRemoved(Document doc, ItemBase itemBase) { }
        // close
        public void OnDocumentClosed(Document doc) { }
        #endregion

        #region Connection / disconnection
        private void OnConnected()
        {
            /*if (!Program.UseDisconnected && WCFClient.IsConnected)
            {
                using (WCFClient wcfClient = new WCFClient())
                {
                    DCGroup currentGroup = wcfClient.Client.GetCurrentGroup();
                    // update main frame title
                    if (null != currentGroup)
                        Text = Application.ProductName + " - (" + currentGroup.Name + "\\" + wcfClient.User.Name + ")";
                }
                // create basic layout
                CreateBasicLayout();
                UpdateDisconnectButton();
            }*/
         }
        private void OnDisconnected()
        {
            Text = Application.ProductName;
            ClearBasicLayout();
            UpdateDisconnectButton();
        }
        private void UpdateDisconnectButton()
        { 
        }
        #endregion

        #region File menu event handlers
        private void FileClose(object sender, EventArgs e)
        {
            IDocument doc = ActiveDocument;
            CancelEventArgs cea = new CancelEventArgs();
            CloseDocument(doc, cea);
        }
        private void FileNew(object sender, EventArgs e)
        {
            CloseStartPage();
            NewDocument();
        }
        private void FileOpen(object sender, EventArgs e)
        {
            CloseStartPage();
            if (DialogResult.OK == openFileDialogSB.ShowDialog())
                foreach(string fileName in openFileDialogSB.FileNames)
                    OpenDocument(fileName);            
        }
        private void FileSave(object sender, EventArgs e)        {   SaveDocument();                }
        private void FileSaveAs(object sender, EventArgs e)      {   SaveDocumentAs();              }
        private void FileSaveAll(object sender, EventArgs e)     {   SaveAllDocuments();            }
        private void FileExit(object sender, EventArgs e)
        {
            CancelEventArgs cea = new CancelEventArgs();
            CloseAllDocuments(cea);
            Close();
            Application.Exit(); 
        }
        public void OpenMRUFile(string filePath)
        {
            CloseStartPage();

            // instantiate solver
            DocumentSB.SetSolver(new LayerSolver());
            // open file
            OpenDocument(filePath); // -> exception handled in OpenDocument
        }
        #endregion

        #region Tools
        #region User
        private void OnUserClicked(object sender, EventArgs e)
        {
            toolStripSBUser.ShowDropDown();
        }
        #endregion
        #region Items
        private void ToolAddNewBox(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewBoxUI();    }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewCase(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPack(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPackUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewBundle(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewBundleUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewCylinder(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewCylinderUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
         }
        private void ToolAddNewPallet(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewTruck(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Pallet decorations
        private void OnPalletAccessories(object sender, EventArgs e)
        {
            toolStripSBPalletDeco.ShowDropDown();
        }
        private void ToolAddNewInterlayer(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewInterlayerUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletCap(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletCapUI(); }
            catch (Exception ex) { _log.Error(ex.ToString());  Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletCorners(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletCornersUI(); }
            catch (Exception ex) { _log.Error(ex.ToString());  Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletFilm(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletFilmUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Analyses
        private void OnAnalysisPallet(object sender, EventArgs e)
        {
            toolStripSBAnalysisPallet.ShowDropDown();
        }
        private void OnAnalysisCase(object sender, EventArgs e)
        {
            toolStripSBAnalysesCase.ShowDropDown();
        }
        private void OnAnalysisTrucks(object sender, EventArgs e)
        {
            toolStripSBAnalysesTruck.ShowDropDown();
        }
        private void OnAnalysisOpti(object sender, EventArgs e)
        {
            toolStripSBOptimisations.ShowDropDown();
        }
        private void OnNewAnalysisBoxCase(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message:"No active document!"); }
            try { ((DocumentSB)ActiveDocument).CreateNewAnalysisBoxCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCylinderCase(object sender, EventArgs e)
        { 
            if (null == ActiveDocument) { _log.Error(message:"No active document!"); }
            try { ((DocumentSB)ActiveDocument).CreateNewAnalysisCylinderCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisPalletTruck(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message:"No active document!"); }
            try { ((DocumentSB)ActiveDocument).CreateNewAnalysisPalletTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCasePallet(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message: "No active document!"); }
            try { ((DocumentSB)ActiveDocument).CreateNewAnalysisCasePalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCylinderPallet(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message: "No active document!"); }
            try { ((DocumentSB)ActiveDocument).CreateNewAnalysisCylinderPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisBoxCasePallet(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message: "No active document!"); }
            try { ((DocumentSB)ActiveDocument).CreateNewAnalysisBoxCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCaseTruck(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message: "No active document!"); }
            try { AnalysisCaseTruck analysis = ((DocumentSB)ActiveDocument).CreateNewAnalysisCaseTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewHAnalysisPallet(object sender, EventArgs e)
        {
            if (null == ActiveDocument) { _log.Error(message : "No active document!" ); }
            try { HAnalysis hAnalysis = ((DocumentSB)ActiveDocument).CreateNewHAnalysisPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Optimisation
        private void OnOptiPack(object sender, EventArgs e)
        {
            try
            {
                // show optimisation form
                FormOptimizePack form = new FormOptimizePack((DocumentSB)ActiveDocument);
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnOptiSelectCase(object sender, EventArgs e)
        {
            try
            {
                FormOptimiseMultiCase form = new FormOptimiseMultiCase()
                {
                    Document = (DocumentSB)ActiveDocument
                };
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Database / settings / excel sheet
        private void OnShowDatabase(object sender, EventArgs e)
        {
            try
            {
                if (Program.UseDisconnected)
                    return;
                // Form show database
                /*FormShowDatabase form = new FormShowDatabase()
                {
                    Document = ActiveDocument as Document
                };
                form.ShowDialog();*/
                // update toolbar state as database may now be empty
                UpdateToolbarState();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnShowSettings(object sender, EventArgs e)
        {
            try
            {
                // show OptionsFormSettings
                FormOptionsSettings form = new FormOptionsSettings();
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnLoadExcelSheet(object sender, EventArgs e)
        {
            try
            {
                DocumentSB doc = ActiveDocument as DocumentSB;
                if (null == doc) return;
                FormExcelLibrary form = new FormExcelLibrary(doc);
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region PLMPackLib
        private void OnPLMPackLib(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(UrlPLMPackLib))
                    Process.Start("rundll32.exe", "dfshim.dll,ShOpenVerbApplication " + UrlPLMPackLib);
            }
            catch (Exception ex) { _log.Error(ex.Message); }
        }
        #endregion
        #endregion

        #region Document / View status change handlers
        void OnDocumentModified(object sender, EventArgs e)
        {
            UpdateToolbarState();
        }
        #endregion

        #region Form activation/creation
        public void CreateOrActivateViewAnalysis(Analysis analysis)
        { 
            AnalysisCasePallet analysisCasePallet = analysis as AnalysisCasePallet;
            AnalysisBoxCase analysisBoxCase = analysis as AnalysisBoxCase;
            AnalysisPalletTruck analysisPalletTruck = analysis as AnalysisPalletTruck;
            AnalysisCylinderCase analysisCylinderCase = analysis as AnalysisCylinderCase;
            AnalysisCylinderPallet analysisCylinderPallet = analysis as AnalysisCylinderPallet;
            AnalysisCaseTruck analysisCaseTruck = analysis as AnalysisCaseTruck;

            // ---> search among existing views
            // ---> activate if found
            foreach (IDocument doc in Documents)
                foreach (IView view in doc.Views)
                {
                    if (view is DockContentAnalysisEdit formAnalysisEdit && formAnalysisEdit.Analysis == analysis)
                    {
                        formAnalysisEdit.Activate();
                        return;
                    }
                    if (view is DockContentAnalysisPalletTruck formAnalysisPalletTruck && null != analysisPalletTruck)
                    {
                        if (analysisPalletTruck == formAnalysisPalletTruck.Analysis)
                        {
                            formAnalysisPalletTruck.Activate();
                            return;
                        }
                    }
                }

            // ---> not found : create new form
            // get document
            DocumentSB parentDocument = (DocumentSB)analysis.ParentDocument;
            // create form and show
            DockContentView formAnalysis = parentDocument.CreateViewAnalysis(analysis);
            if (null != formAnalysis)
                formAnalysis.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
        }
        /// <summary>
        /// ECT analysis view
        /// </summary>
        public void CreateOrActivateViewECTAnalysis(ECTAnalysis analysis)
        { 
            // search among existing views
            foreach (IDocument doc in Documents)
                foreach (IView view in doc.Views)
                {
                    DockContentECTAnalysis form = view as DockContentECTAnalysis;
                    if (null == form) continue;
                    if (analysis == form.ECTAnalysis)
                    {
                        form.Activate();
                        return;
                    }
                }
            // ---> not found
            // ---> create new form
            // get document
            DocumentSB parentDocument = analysis.ParentDocument as DocumentSB;
            // instantiate form
            DockContentECTAnalysis formECTAnalysis = parentDocument.CreateECTAnalysisView(analysis);
            // show docked
            formECTAnalysis.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
        }
        #endregion

        #region Helpers
        public string AssemblyConf
        {
            get
            {
                object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyConfigurationAttribute confAttribute = (AssemblyConfigurationAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (!string.IsNullOrEmpty(confAttribute.Configuration))
                        return confAttribute.Configuration.ToLower();
                }
                return "release";
            }
        }
        #endregion

        #region Help menu event handlers
        private void OnAbout(object sender, EventArgs e)
        {
            AboutBox form = new AboutBox()
            {
                CompanyUrl = Settings.Default.CompanyUrl,
                SupportEmail = Settings.Default.EmailSupport
            };
            form.ShowDialog();
        }
        private void OnOnlineHelp(object sender, EventArgs e)
        {
            try { Process.Start(Settings.Default.HelpPageUrl); }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnDisconnect(object sender, EventArgs e)
        {
            try
            {
                //WCFClient.Disconnect();
                // start login again
                timerLogin.Start();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        #endregion

        #region Static instance accessor
        public static FormMain GetInstance()  { return _instance; }
        #endregion

    }
}
