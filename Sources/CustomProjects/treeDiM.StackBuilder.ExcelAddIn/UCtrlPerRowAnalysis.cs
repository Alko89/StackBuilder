#region Using directives
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Excel = Microsoft.Office.Interop.Excel;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Reporting;

using treeDiM.StackBuilder.ExcelAddIn.Properties;
#endregion

namespace treeDiM.StackBuilder.ExcelAddIn
{
    public partial class UCtrlPerRowAnalysis : UserControl
    {
        public UCtrlPerRowAnalysis()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            uCtrlMaxPalletHeight.Value = Settings.Default.LoadedPalletHeight;
            uCtrlOverhang.ValueX = Settings.Default.OverhangX;
            uCtrlOverhang.ValueY = Settings.Default.OverhangY;

            chkbGenerateImageInRow.Checked = Settings.Default.GenerateImageInRow;
            chkbGenerateImageInFolder.Checked = Settings.Default.GenerateImageInFolder;

            OnGenerateImagesChanged(this, null);
            OnGenerateReportChanged(this, null);
        }

        #region Event handlers
        private void OnCompute(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            StringBuilder sbErrors = new StringBuilder();

            try
            {
                string startLetter = Settings.Default.ColumnLetterOutputStart;
                int colStartIndex = ExcelHelpers.ColumnLetterToColumnIndex(Settings.Default.ColumnLetterOutputStart);

                Excel.Worksheet xlSheet = Globals.StackBuilderAddIn.Application.ActiveSheet as Excel.Worksheet;
                Excel.Range range = xlSheet.UsedRange;
                int rowCount = range.Rows.Count;

                // get list of pallets
                List<PalletProperties> pallets = GetSelectedPallets();
                if (0 == pallets.Count)
                {
                    MessageBox.Show(Resources.ID_ERROR_NOPALLETSELECTED);
                    return;
                }
                int palletColStartIndex = colStartIndex - 1;

                // pallet loop
                foreach (PalletProperties palletProperties in pallets)
                {
                    int iOutputFieldCount = palletColStartIndex;
                    int iNoCols = 0;
                    // modify header
                    // count
                    Excel.Range countHeaderCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1);
                    countHeaderCell.Value = Resources.ID_RESULT_NOCASES;
                    ++iNoCols;
                    // load weight
                    Excel.Range loadWeightHeaderCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1);
                    loadWeightHeaderCell.Value = Resources.ID_RESULT_LOADWEIGHT + " (" + UnitsManager.MassUnitString + ")";
                    ++iNoCols;
                    // total pallet weight
                    Excel.Range totalPalletWeightHeaderCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1);
                    totalPalletWeightHeaderCell.Value = Resources.ID_RESULT_TOTALPALLETWEIGHT + " (" + UnitsManager.MassUnitString + ")";
                    ++iNoCols;
                    // efficiency
                    Excel.Range efficiencyHeaderCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + 1);
                    efficiencyHeaderCell.Value = Resources.ID_RESULT_EFFICIENCY + " (%)";
                    ++iNoCols;
                    // image
                    if (GenerateImage)
                    {
                        Excel.Range imageHeaderCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 1);
                        imageHeaderCell.Value = Resources.ID_RESULT_IMAGE;
                        ++iNoCols;
                    }
                    // set bold font for all header row
                    Excel.Range headerRange = xlSheet.get_Range("a"+1, ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount)+1);
                    headerRange.Font.Bold = true;
                    // modify range for images
                    if (GenerateImage)
                    {
                        Excel.Range dataRange = xlSheet.get_Range(
                            "a" + 2,
                            ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + rowCount);
                        dataRange.RowHeight = 128;
                        Excel.Range imageRange = xlSheet.get_Range(
                            ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + 2,
                            ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + rowCount);
                        imageRange.ColumnWidth = 24;
                    }
                    double largestDimensionMinimum = LargestDimMin;


                    string colName = Settings.Default.ColumnLetterName;
                    string colDescription = Settings.Default.ColumnLetterDescription;
                    string colLength = Settings.Default.ColumnLetterLength;
                    string colWidth = Settings.Default.ColumnLetterWidth;
                    string colHeight = Settings.Default.ColumnLetterHeight;
                    string colWeight = Settings.Default.ColumnLetterWeight;

                    // loop through rows
                    for (int iRow = 2; iRow <= rowCount; ++iRow)
                    {
                        iOutputFieldCount = palletColStartIndex-1;
                        try
                        {
                            // get name
                            string name = (xlSheet.get_Range(colName + iRow, colName + iRow).Value).ToString();
                            // get description
                            string description = (xlSheet.get_Range(colDescription + iRow, colDescription + iRow).Value).ToString();
                            // get length
                            double length = xlSheet.get_Range(colLength + iRow, colLength + iRow).Value;
                            // get width
                            double width = xlSheet.get_Range(colWidth + iRow, colWidth + iRow).Value;
                            // get height
                            double height = xlSheet.get_Range(colHeight + iRow, colHeight + iRow).Value;

                            double maxDimension = Math.Max(Math.Max(length, width), height);
                            if (maxDimension < largestDimensionMinimum) continue;

                            // get weight
                            double? weight = null;
                            try { weight = xlSheet.get_Range(colWeight + iRow, colWeight + iRow).Value; } catch (Exception /*ex*/) { }
                            // compute stacking
                            int stackCount = 0;
                            double loadWeight = 0.0, totalPalletWeight = 0.0, stackEfficiency = 0.0;
                            string stackImagePath = string.Empty;
                            iOutputFieldCount = palletColStartIndex;
                            // generate result
                            GenerateResult(name, description
                                , length, width, height, weight
                                , palletProperties
                                , ref stackCount
                                , ref loadWeight, ref totalPalletWeight
                                , ref stackEfficiency
                                , ref stackImagePath);
                            // insert count
                            Excel.Range countCel = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow);
                            countCel.Value = stackCount;
                            // insert load weight
                            Excel.Range loadWeightCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow);
                            loadWeightCell.Value = loadWeight;
                            // insert total weight
                            Excel.Range totalWeightCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow);
                            totalWeightCell.Value = totalPalletWeight;
                            // efficiency
                            Excel.Range efficiencyCell = xlSheet.get_Range(ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow);
                            efficiencyCell.Value = Math.Round(stackEfficiency, 2);
                            // insert image 
                            if (GenerateImage)
                            {
                                Excel.Range imageCell = xlSheet.get_Range(
                                    ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount) + iRow,
                                    ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow);
                                xlSheet.Shapes.AddPicture(stackImagePath,
                                    Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,
                                    imageCell.Left + 1, imageCell.Top + 1, imageCell.Width - 2, imageCell.Height - 2);
                            }
                        }
                        catch (OutOfMemoryException ex)
                        {
                            sbErrors.Append(ex.Message);
                        }
                        catch (EngineException ex)
                        {
                            sbErrors.Append(ex.Message);
                        }
                        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException /*ex*/)
                        {
                            iOutputFieldCount = palletColStartIndex;
                            Excel.Range countCel = xlSheet.get_Range(
                                ExcelHelpers.ColumnIndexToColumnLetter(iOutputFieldCount++) + iRow);
                            countCel.Value = string.Format($"ERROR : Invalid input data!");
                        }
                        catch (Exception ex)
                        {
                            throw ex; // rethrow
                        }
                    } // loop row

                    // increment palletColStartIndex
                    palletColStartIndex += iNoCols;

                } // loop pallet
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                switch ((uint)ex.ErrorCode)
                {
                    case 0x800A03EC:
                        MessageBox.Show("NAME_NOT_FOUND : Could not find cell with given name!");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Arrow;
            }
        }
        private void OnGenerateImagesChanged(object sender, EventArgs e)
        {
            folderSelect.Enabled = chkbGenerateImageInFolder.Checked;
        }
        private void OnGenerateReportChanged(object sender, EventArgs e)
        {
            reportFolderSelect.Enabled = chkbGenerateReportInFolder.Checked;
        }
        private void OnEditPallets(object sender, EventArgs e)
        {
            try
            {
                // get the collection of work sheets
                Excel.Sheets sheets = Globals.StackBuilderAddIn.Application.Worksheets;
                Excel.Worksheet worksheet = null;

                // find the "Pallets" worksheet
                try
                {
                    worksheet = (Excel.Worksheet)sheets["Pallets"];
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    worksheet = (Excel.Worksheet)sheets.Add();
                    worksheet.Name = "Pallets";

                    // modify header
                    worksheet.get_Range("a" + 1, "a" + 1).Value = "Name";
                    worksheet.get_Range("b" + 1, "b" + 1).Value = "Description";
                    worksheet.get_Range("c" + 1, "c" + 1).Value = "Length";
                    worksheet.get_Range("d" + 1, "d" + 1).Value = "Width";
                    worksheet.get_Range("e" + 1, "e" + 1).Value = "Height";
                    worksheet.get_Range("f" + 1, "f" + 1).Value = "Weight";
                    worksheet.get_Range("g" + 1, "g" + 1).Value = "Form factor";

                    Excel.Range headerRange = worksheet.get_Range("a" + 1, "g" + 1);
                    headerRange.Font.Bold = true;

                    // initialize pallet sheet
                    string[] palletTypes = PalletData.TypeNames;
                    int i = 2;
                    foreach (string typeName in palletTypes)
                    {
                        PalletData palletData = PalletData.GetByName(typeName);
                        worksheet.get_Range("a" + i, "a" + i).Value = palletData.Name;
                        worksheet.get_Range("b" + i, "b" + i).Value = palletData.Description;
                        worksheet.get_Range("c" + i, "c" + i).Value = palletData.Length;
                        worksheet.get_Range("d" + i, "d" + i).Value = palletData.Width;
                        worksheet.get_Range("e" + i, "e" + i).Value = palletData.Height;
                        worksheet.get_Range("f" + i, "f" + i).Value = palletData.Weight;
                        worksheet.get_Range("g" + i, "g" + i).Value = palletData.Name;
                        ++i;
                    }

                    // fit column width
                    worksheet.get_Range("a" + 1, "g" + (i - 1)).Columns.AutoFit();
                    worksheet.Activate();
                }
                OnRefreshPallets(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private List<PalletProperties> GetSelectedPallets()
        {
            List<PalletProperties> pallets = new List<PalletProperties>();
            foreach (object o in lbPallets.CheckedItems)
            {
                if (o is PalletItem item) pallets.Add(item.PalletProp);
            }
            return pallets;
        }

        private void OnRefreshPallets(object sender, EventArgs e)
        {
            try
            {
                Excel.Sheets sheets = Globals.StackBuilderAddIn.Application.Worksheets;
                Excel.Worksheet worksheet = (Excel.Worksheet)sheets["Pallets"];
                int rowCount = worksheet.UsedRange.Rows.Count;

                List<PalletProperties> listPallets = new List<PalletProperties>();
                StringBuilder cumulatedErrorString = new StringBuilder();
                for (int i = 2; i <= rowCount; ++i)
                {
                    try
                    {
                        // try & get typeName
                        string typeName = "EUR";
                        try { typeName = worksheet.get_Range("g" + i, "g" + i).Value.ToString(); }
                        catch (Exception) {}
                        PalletProperties palletProp = new PalletProperties(null, typeName
                            , ExcelHelpers.ReadDouble("Pallet length", worksheet, "c" + i)
                            , ExcelHelpers.ReadDouble("Pallet width", worksheet, "d" + i)
                            , ExcelHelpers.ReadDouble("Pallet height", worksheet, "e" + i)
                            )
                        {
                            Weight = ExcelHelpers.ReadDouble("Weight", worksheet, "f" + i)
                        };
                        palletProp.ID.SetNameDesc(
                            ExcelHelpers.ReadString("Pallet name", worksheet, "a" + i),
                            ExcelHelpers.ReadString("Pallet description", worksheet, "b" + i)
                            );
                        listPallets.Add(palletProp);
                    }
                    catch (Exception ex)
                    {
                        cumulatedErrorString.Append(ex.Message);
                    }
                }
                // fill list box
                lbPallets.Items.Clear();
                lbPallets.SelectionMode = SelectionMode.One;
                foreach (PalletProperties palletProperties in listPallets)
                {
                    int index = lbPallets.Items.Add(new PalletItem(palletProperties));
                    lbPallets.SetItemChecked(index, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Computing results
        private void GenerateResult(
            string name, string description
            , double length, double width, double height
            , double? weight
            , PalletProperties palletProperties
            , ref int stackCount, ref double loadWeight, ref double totalWeight
            , ref double stackEfficiency
            , ref string stackImagePath)
        {
            stackCount = 0;
            totalWeight = 0.0;
            stackImagePath = string.Empty;

            // generate case
            BoxProperties bProperties = new BoxProperties(null, length, width, height);
            bProperties.ID.SetNameDesc(name, description);
            if (weight.HasValue) bProperties.SetWeight(weight.Value);
            bProperties.SetColor(Color.Chocolate);
            bProperties.TapeWidth = new OptDouble(true, Math.Min(UnitsManager.ConvertLengthFrom(50.0, UnitsManager.UnitSystem.UNIT_METRIC1), 0.5 * width));
            bProperties.TapeColor = Color.Beige;

            Graphics3DImage graphics = null;
            if (GenerateImage || GenerateImageInFolder)
            {
                // generate image path
                stackImagePath = Path.Combine(Path.ChangeExtension(Path.GetTempFileName(), "png"));

                if (GenerateImageInFolder)
                    stackImagePath = Path.ChangeExtension(Path.Combine(Path.Combine(DirectoryPathImages, name)), "png");

                graphics = new Graphics3DImage(new Size(ImageSize, ImageSize))
                {
                    FontSizeRatio = Settings.Default.FontSizeRatio,
                    CameraPosition = Graphics3D.Corner_0
                };
            }

            // compute analysis
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet();
            constraintSet.SetAllowedOrientations(new bool[] { !AllowOnlyZOrientation, !AllowOnlyZOrientation, true });
            constraintSet.SetMaxHeight(new OptDouble(true, PalletMaximumHeight));
            constraintSet.Overhang = Overhang;

            SolverCasePallet solver = new SolverCasePallet(bProperties, palletProperties);
            List<Analysis> analyses = solver.BuildAnalyses(constraintSet, false);
            if (analyses.Count > 0)
            {
                Analysis analysis = analyses[0];
                stackCount = analysis.Solution.ItemCount;
                loadWeight = analysis.Solution.LoadWeight;
                totalWeight = analysis.Solution.Weight;
                stackEfficiency = analysis.Solution.VolumeEfficiency;

                if (stackCount <= StackCountMax)
                {
                    if (GenerateImage || GenerateImageInFolder)
                    {
                        ViewerSolution sv = new ViewerSolution(analysis.Solution);
                        sv.Draw(graphics, Transform3D.Identity);
                        graphics.Flush();
                    }
                    if (GenerateReport)
                    {
                        ReportData inputData = new ReportData(analysis);
                        string outputFilePath = Path.ChangeExtension(Path.Combine(DirectoryPathReports, string.Format("Report_{0}_on_{1}", analysis.Content.Name, analysis.Container.Name)), "doc");

                        ReportNode rnRoot = null;
                        Margins margins = new Margins();
                        Reporter reporter = new ReporterMSWord(inputData, ref rnRoot, Reporter.TemplatePath, outputFilePath, margins);
                    }
                }
            }
            if (GenerateImage)
            {
                Bitmap bmp = graphics.Bitmap;
                bmp.Save(stackImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        #endregion

        #region Private properties (Pane settings)
        private bool GenerateImage { get { return chkbGenerateImageInRow.Checked; } set { chkbGenerateImageInRow.Checked = value; } }
        private bool GenerateImageInFolder { get { return chkbGenerateImageInFolder.Checked; } set { chkbGenerateImageInFolder.Checked = value; } }
        private bool GenerateReport => false;
        private string DirectoryPathImages { get { return folderSelect.Folder; } set { folderSelect.Folder = value; } }
        private string DirectoryPathReports { get { return reportFolderSelect.Folder; } set { reportFolderSelect.Folder = value; } }
        private int ImageSize => Settings.Default.ImageSize;
        private bool AllowOnlyZOrientation => chkbOnlyZOrientation.Checked;
        private double PalletMaximumHeight => uCtrlMaxPalletHeight.Value;
        private int StackCountMax => Settings.Default.StackCountMax;
        private double LargestDimMin => Settings.Default.MinDimensions;
        private Vector2D Overhang => new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY);
        #endregion

        #region Static member functions
        internal static string TaskPaneTitle => "Stackbuilder Per Row Analysis";
        #endregion

        #region Helpers

        #endregion


    }
}
