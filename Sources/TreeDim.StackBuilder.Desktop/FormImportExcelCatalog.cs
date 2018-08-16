#region Using directives
using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

using System.Drawing;
using System.Collections.Generic;
using System.Net;

using System.Diagnostics;

using log4net;

using Syroot.Windows.IO;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.ExcelReader;

//using treeDiM.PLMPack.DBClient;
//using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormImportExcelCatalog : Form
    {
        #region Constructor
        public FormImportExcelCatalog()
        {
            InitializeComponent();
        }
        #endregion

        #region Override form
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // load file path from properties
            excelFileSelect.FileName = Properties.Settings.Default.ExcelLibraryPath;

            _bgWorker = new BackgroundWorker();
            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            _bgWorker.DoWork += new DoWorkEventHandler(ImportItems);
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunCompleted);
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.WorkerSupportsCancellation = true;

            bnStop.Enabled = false;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Properties.Settings.Default.ExcelLibraryPath = excelFileSelect.FileName;
        }
        #endregion

        #region Private properties
        private bool Overwrite => chkbOverwrite.Checked;
        #endregion

        #region Handlers
        private void OnDownloadExcelTemplate(object sender, EventArgs e)
        {
            string fileURL = Properties.Settings.Default.ExcelTemplateFileURL;
            try
            {
                KnownFolder knownFolder = new KnownFolder(KnownFolderType.Downloads);
                string downloadPath = Path.Combine(knownFolder.Path, Path.GetFileName(fileURL));

                using (var client = new WebClient())
                { client.DownloadFile(fileURL, downloadPath); }

                if (File.Exists(downloadPath))
                    Process.Start(downloadPath);
            }
            catch (WebException ex)
            {
                MessageBox.Show($"Failed to download file {fileURL} : {ex.Message}");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnFileNameChanged(object sender, EventArgs e)
        {
            string filePath = excelFileSelect.FileName;
            bool fileExist = File.Exists(filePath);
            // enable disable import button
            bnImport.Enabled = fileExist;
            UpdateStatus(fileExist ? string.Empty : Properties.Resources.ID_DEFINEVALIDFILEPATH);

            if (fileExist)
            {
                // load file
                try { if (ExcelDataReader_StackBuilder.LoadFile(filePath, ref _listTypes)) { } }
                catch (System.Security.SecurityException ex) { MessageBox.Show(ex.Message); return; }
                catch (Exception ex) { MessageBox.Show(ex.Message); return; }

                bnImport.Enabled = _listTypes.Count > 0;
                progressBar.Maximum = _listTypes.Count > 0 ? _listTypes.Count - 1 : 0;
                UpdateStatus(string.Format("{0} items found", _listTypes.Count));
            }
        }

        private void OnButtonImportClick(object sender, EventArgs e)
        {
            // Warning about expected unit
            if (DialogResult.No == MessageBox.Show(
                string.Format(Properties.Resources.ID_EXCELSHEETUNITSYSTEM,
                Path.GetFileName(excelFileSelect.FileName),
                UnitsManager.SystemUnitString),
                Properties.Resources.ID_UNITSYSTEM,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                return;
            bnImport.Enabled = false;
            bnStop.Enabled = true;
            richTextBoxLog.Clear();
            // Kickoff the worker thread to begin it's DoWork function
            _bgWorker.RunWorkerAsync();
        }
        private void ImportItems(object sender, DoWorkEventArgs e)
        {
            try
            {
                int maxCount = _listTypes.Count;

                _iProgress = 0;
                while (_iProgress < maxCount)
                {
                    try
                    {
                        ImportItem(_listTypes[_iProgress]);
                        ++_iProgress;
                        _bgWorker.ReportProgress((int)Math.Floor((_iProgress * 100.0) / maxCount));
                        if (_bgWorker.CancellationPending)
                        {
                            // Set the e.Cancel flag so that the WorkerCompleted event
                            // knows that the process was cancelled.
                            e.Cancel = true;
                            _bgWorker.ReportProgress(0);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void ImportItem(DataType dt)
        {
            try
            {
                /*using (WCFClient wcfClient = new WCFClient())
                {
                    // does item already exist?
                    if (!CanWrite(wcfClient.Client, dt))
                    {
                        AppendRtb(string.Format(Properties.Resources.ID_IMPORTSKIPPING, dt.Name));
                        return;
                    }
                    else
                        AppendRtb(string.Format(Properties.Resources.ID_IMPORTLOADING, dt.Name));
                    // create case
                    if (dt is DataCase dtCase)
                    {
                        int colorFace = Color.Chocolate.ToArgb();
                        wcfClient.Client.CreateNewCase(
                            new DCSBCase()
                            {
                                Name = dtCase.Name,
                                Description = dtCase.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                DimensionsOuter = new DCSBDim3D()
                                {
                                    M0 = dtCase.OuterDimensions[0],
                                    M1 = dtCase.OuterDimensions[1],
                                    M2 = dtCase.OuterDimensions[2]
                                },
                                HasInnerDims = dtCase.InnerDimensions[0] > 0.0,
                                DimensionsInner = new DCSBDim3D()
                                {
                                    M0 = dtCase.InnerDimensions[0],
                                    M1 = dtCase.InnerDimensions[1],
                                    M2 = dtCase.InnerDimensions[2]
                                },
                                IsCase = true,
                                Colors = new int[] { colorFace, colorFace, colorFace, colorFace, colorFace, colorFace },
                                ShowTape = dtCase.TapeWidth > 0,
                                TapeColor = Color.Beige.ToArgb(),
                                TapeWidth = dtCase.TapeWidth,
                                Weight = dtCase.Weight,
                                NetWeight = dtCase.NetWeight > 0 ? dtCase.NetWeight : (double?)null,
                                MaxWeight = dtCase.MaxWeight > 0 ? dtCase.MaxWeight : (double?)null
                            }
                            );
                    }
                    else if (dt is DataBox dtBox)
                    {
                        int colorFace = Color.Chocolate.ToArgb();
                        wcfClient.Client.CreateNewCase(
                           new DCSBCase()
                           {
                               Name = dtBox.Name,
                               Description = dtBox.Description,
                               UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                               DimensionsOuter = new DCSBDim3D()
                               {
                                   M0 = dtBox.Dimensions[0],
                                   M1 = dtBox.Dimensions[1],
                                   M2 = dtBox.Dimensions[2]
                               },
                               HasInnerDims = false,
                               IsCase = false,
                               Colors = new int[] { colorFace, colorFace, colorFace, colorFace, colorFace, colorFace },
                               ShowTape = false,
                               TapeColor = Color.Beige.ToArgb(),
                               TapeWidth = 0,
                               Weight = dtBox.Weight,
                               NetWeight = dtBox.NetWeight > 0 ? dtBox.NetWeight : (double?)null,
                               MaxWeight = (double?)null
                           }
                           );
                    }
                    else if (dt is DataCylinder dtCylinder)
                    {
                        int colorFaceExt = Color.LightSkyBlue.ToArgb();
                        int colorFaceInt = Color.Chocolate.ToArgb();
                        int colorTop = Color.Gray.ToArgb();
                        wcfClient.Client.CreateNewCylinder(
                            new DCSBCylinder()
                            {
                                Name = dtCylinder.Name,
                                Description = dtCylinder.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                RadiusOuter = 0.5 * dtCylinder.Diameter,
                                RadiusInner = 0.5 * dtCylinder.InnerDiameter,
                                Height = dtCylinder.Height,
                                ColorOuter = colorFaceExt,
                                ColorInner = colorFaceInt,
                                ColorTop = colorTop,
                                Weight = dtCylinder.Weight,
                                NetWeight = dtCylinder.NetWeight > 0 ? dtCylinder.NetWeight : (double?)null
                            }
                            );
                    }
                    else if (dt is DataPallet dtPallet)
                    {
                        int colorPallet = Color.Yellow.ToArgb();
                        wcfClient.Client.CreateNewPallet(
                            new DCSBPallet()
                            {
                                Name = dtPallet.Name,
                                Description = dtPallet.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                Dimensions = new DCSBDim3D()
                                { M0 = dtPallet.Dimensions[0], M1 = dtPallet.Dimensions[1], M2 = dtPallet.Dimensions[2] },
                                Weight = dtPallet.Weight,
                                Color = colorPallet,
                                PalletType = "EUR2"
                            }
                            );
                    }
                    else if (dt is DataPalletCorner dtPalletCorner)
                    {
                        int colorPalletCorner = Color.Khaki.ToArgb();
                        wcfClient.Client.CreateNewPalletCorner(
                            new DCSBPalletCorner()
                            {
                                Name = dtPalletCorner.Name,
                                Description = dtPalletCorner.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                Length = dtPalletCorner.Length,
                                Width = dtPalletCorner.Width,
                                Thickness = dtPalletCorner.Thickness,
                                Weight = dtPalletCorner.Weight,
                                Color = colorPalletCorner
                            }
                            );
                    }
                    else if (dt is DataPalletCap dtPalletCap)
                    {
                        int colorPalletCap = Color.Khaki.ToArgb();
                        wcfClient.Client.CreateNewPalletCap(
                            new DCSBPalletCap()
                            {
                                Name = dtPalletCap.Name,
                                Description = dtPalletCap.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                DimensionsOuter = new DCSBDim3D()
                                { M0 = dtPalletCap.Dimensions[0], M1 = dtPalletCap.Dimensions[1], M2 = dtPalletCap.Dimensions[2] },
                                DimensionsInner = new DCSBDim3D()
                                { M0 = dtPalletCap.InnerDimensions[0], M1 = dtPalletCap.InnerDimensions[1], M2 = dtPalletCap.InnerDimensions[2] },
                                Weight = dtPalletCap.Weight,
                                Color = colorPalletCap
                            }
                            );
                    }
                    else if (dt is DataPalletFilm dtPalletFilm)
                    {
                        int colorPalletFilm = Color.LightSkyBlue.ToArgb();
                        wcfClient.Client.CreateNewPalletFilm(
                            new DCSBPalletFilm()
                            {
                                Name = dtPalletFilm.Name,
                                Description = dtPalletFilm.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                UseHatching = dtPalletFilm.Hatching,
                                HatchingAngle = dtPalletFilm.Angle,
                                HatchingSpace = dtPalletFilm.Spacing,
                                UseTransparency = dtPalletFilm.Transparency,
                                Color = colorPalletFilm
                            }
                            );
                    }
                    else if (dt is DataTruck dtTruck)
                    {
                        int colorTruck = Color.LightBlue.ToArgb();
                        wcfClient.Client.CreateNewTruck(
                            new DCSBTruck()
                            {
                                Name = dtTruck.Name,
                                Description = dtTruck.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                DimensionsInner = new DCSBDim3D()
                                { M0 = dtTruck.InnerDimensions[0], M1 = dtTruck.InnerDimensions[1], M2 = dtTruck.InnerDimensions[2] },
                                AdmissibleLoad = dtTruck.MaxLoad,
                                Color = colorTruck
                            }
                            );
                    }
                    else if (dt is DataBundle dtBundle)
                    {
                        int colorBundle = Color.LightGray.ToArgb();
                        wcfClient.Client.CreateNewBundle(
                            new DCSBBundle()
                            {
                                Name = dtBundle.Name,
                                Description = dtBundle.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                DimensionsUnit = new DCSBDim3D() { M0 = dtBundle.Length, M1 = dtBundle.Width, M2 = dtBundle.UnitThickness },
                                UnitWeight = dtBundle.UnitWeight,
                                Number = dtBundle.NoFlats,
                                Color = colorBundle
                            }
                            );
                    }
                    else if (dt is DataInterlayer dtInterlayer)
                    {
                        int colorInterlayer = Color.Beige.ToArgb();
                        wcfClient.Client.CreateNewInterlayer(
                            new DCSBInterlayer()
                            {
                                Name = dtInterlayer.Name,
                                Description = dtInterlayer.Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                Dimensions = new DCSBDim3D() { M0 = dtInterlayer.Dimensions[0], M1 = dtInterlayer.Dimensions[1], M2 = dtInterlayer.Dimensions[2] },
                                Weight = dtInterlayer.Weight,
                                Color = colorInterlayer
                            }
                            );
                    }
                    else
                        throw new Exception(string.Format(Properties.Resources.ID_UNEXPECTEDTYPE, dt.Name));
                }*/
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            finally
            {
            }
        }
        /*private bool CanWrite(PLMPackServiceClient client, DataType dt)
        {
            DCSBTypeEnum eType = DataTypeToDCSBEnum(dt);
            int itemID = -1;
            bool exist = client.ItemExistsID(eType, dt.Name, ref itemID);
            if (exist && Overwrite)
            {
                client.RemoveItemById(eType, itemID);
                AppendRtb(string.Format(Properties.Resources.ID_REMOVINGEXISTINGTYPE, dt.Name));
            }
            return !exist || Overwrite;
        }

        private DCSBTypeEnum DataTypeToDCSBEnum(DataType dt)
        {
            if (dt is DataCase || dt is DataBox) return DCSBTypeEnum.TCase;
            else if (dt is DataBundle) return DCSBTypeEnum.TBundle;
            else if (dt is DataInterlayer) return DCSBTypeEnum.TInterlayer;
            else if (dt is DataPallet) return DCSBTypeEnum.TPallet;
            else if (dt is DataCylinder) return DCSBTypeEnum.TCylinder;
            else if (dt is DataPalletCorner) return DCSBTypeEnum.TPalletCorner;
            else if (dt is DataPalletCap) return DCSBTypeEnum.TPalletCap;
            else if (dt is DataPalletFilm) return DCSBTypeEnum.TPalletFilm;
            else if (dt is DataTruck) return DCSBTypeEnum.TTruck;
            else throw new Exception(string.Format(Properties.Resources.ID_UNEXPECTEDTYPE, dt.Name));
        }*/
        /// <summary>
        /// On completed do the appropriate task
        /// </summary>
        void WorkerRunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AppendRtb(Properties.Resources.ID_DONE);
            // The background process is complete. We need to inspect
            // our response to see if an error occurred, a cancel was
            // requested or if we completed successfully.  
            if (e.Cancelled)
                UpdateStatus(Properties.Resources.ID_TASKCANCELED);
            else if (e.Error != null)
                UpdateStatus("Error while performing background operation.");
            else
                // Everything completed normally.
                UpdateStatus(string.Empty);

            //Change the status of the buttons on the UI accordingly
            bnImport.Enabled = true;
            bnStop.Enabled = false;
        }

        /// <summary>
        /// Notification is performed here to the progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = Math.Min(_iProgress, progressBar.Maximum);
            UpdateStatus("Processing : " + e.ProgressPercentage + "%");
        }
        private void OnStopWork(object sender, EventArgs e)
        {
            if (_bgWorker.IsBusy)
                _bgWorker.CancelAsync();
        }
        #endregion

        #region Helpers
        public void AppendRtb(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendRtb), new object[] { value });
                return;
            }
            richTextBoxLog.Text += value + Environment.NewLine;
            richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
            richTextBoxLog.ScrollToCaret();
        }

        private void UpdateStatus(string message)
        {
            if (string.IsNullOrEmpty(message))
                statusLabel.Text = "Ready";
            else
                statusLabel.Text = message;
        }
        #endregion

        #region Data members
        protected List<DataType> _listTypes = new List<DataType>();
        protected int _iProgress = 0;
        protected BackgroundWorker _bgWorker;
        protected ILog _log = LogManager.GetLogger(typeof(FormImportExcelCatalog));
        #endregion
    }
}
