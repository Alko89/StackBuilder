﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.Xsl;
using System.Xml.Schema;
using System.IO;

using System.Drawing;

using System.Diagnostics;
using System.ComponentModel;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
#endregion

namespace treeDiM.StackBuilder.Reporting
{
    #region MyXmlUrlResolver
    class MyXmlUrlResolver : XmlUrlResolver
    {
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (baseUri != null)
                return base.ResolveUri(baseUri, relativeUri);
            else
                return base.ResolveUri(new Uri(@"K:\GitHub\StackBuilder\treeDiM.StackBuilder.Reporting\ReportTemplates"), relativeUri);
        }
    }
    #endregion

    #region ReportData
    /// <summary>
    /// class used to encapsulate an analysis and a solution
    /// </summary>
    public class ReportData
    {
        #region Data members
        private Analysis _analysis;
        private Solution _solution;

        private CasePalletAnalysis _palletAnalysis;
        private CasePalletSolution _palletSolution;
        private SelCasePalletSolution _selSolution;

        private PackPalletAnalysis _packPalletAnalysis;
        private SelPackPalletSolution _selPackPalletSolution;

        private CylinderPalletAnalysis _cylinderPalletAnalysis;
        private SelCylinderPalletSolution _selCylinderPalletSolution;

        private HCylinderPalletAnalysis _hCylinderPalletAnalysis;
        private SelHCylinderPalletSolution _selHCylinderPalletSolution;

        private BoxCasePalletAnalysis _caseAnalysis;
        private SelBoxCasePalletSolution _selCaseSolution;

        private BoxCaseAnalysis _boxCaseAnalysis;
        private SelBoxCaseSolution _selBoxCaseSolution;
        #endregion

        #region Constructors
        public ReportData(Solution solution)
        {
            _solution = solution;
            _analysis = solution.Analysis;


        }
        public ReportData(CasePalletAnalysis palletAnalysis, SelCasePalletSolution selSolution)
        {
            _palletAnalysis = palletAnalysis;
            _selSolution = selSolution;
        }
        public ReportData(PackPalletAnalysis packPalletAnalysis, SelPackPalletSolution selPackPalletSolution)
        {
            _packPalletAnalysis = packPalletAnalysis;
            _selPackPalletSolution = selPackPalletSolution;
        }
        public ReportData(BoxCasePalletAnalysis caseAnalysis, SelBoxCasePalletSolution selCaseSolution)
        {
            _caseAnalysis = caseAnalysis;
            _selCaseSolution = selCaseSolution;
        }
        public ReportData(BoxCaseAnalysis boxCaseAnalysis, SelBoxCaseSolution selBoxCaseSolution)
        {
            _boxCaseAnalysis = boxCaseAnalysis;
            _selBoxCaseSolution = selBoxCaseSolution;
        }
        public ReportData(
            CasePalletAnalysis palletAnalysis, SelCasePalletSolution selSolution
            , CylinderPalletAnalysis cylinderPalletAnalysis, SelCylinderPalletSolution selCylinderPalletSolution
            , HCylinderPalletAnalysis hCylinderPalletAnalysis, SelHCylinderPalletSolution selHCylinderPalletSolution
            , BoxCaseAnalysis boxCaseAnalysis, SelBoxCaseSolution selBoxCaseSolution
            , BoxCasePalletAnalysis caseAnalysis, SelBoxCasePalletSolution selCaseSolution
            , PackPalletAnalysis packPalletAnalysis, SelPackPalletSolution selPackPalletSolution
            )
        {
            // case/pallet analysis
            _palletAnalysis = palletAnalysis;
            _selSolution = selSolution;
            // box/case/pallet analysis
            _caseAnalysis = caseAnalysis;
            _selCaseSolution = selCaseSolution;
            // Cylinder analysis
            _cylinderPalletAnalysis = cylinderPalletAnalysis;
            _selCylinderPalletSolution = selCylinderPalletSolution;
            // HCylinder analysis
            _hCylinderPalletAnalysis = hCylinderPalletAnalysis;
            _selHCylinderPalletSolution = selHCylinderPalletSolution;
            // Box case analysis
            _boxCaseAnalysis = boxCaseAnalysis;
            _selBoxCaseSolution = selBoxCaseSolution;
            // Pack pallet analysis
            _packPalletAnalysis = packPalletAnalysis;
            _selPackPalletSolution = selPackPalletSolution; 
        }
        #endregion

        #region Public accessors
        public Document Document
        {
            get
            {
                if (null != _analysis)
                    return _analysis.ParentDocument;
                if (null != _palletAnalysis)
                    return _palletAnalysis.ParentDocument;
                if (null != _packPalletAnalysis)
                    return _packPalletAnalysis.ParentDocument;
                if (null != _cylinderPalletAnalysis)
                    return _cylinderPalletAnalysis.ParentDocument;
                if (null != _hCylinderPalletAnalysis)
                    return _hCylinderPalletAnalysis.ParentDocument;
                if (null != _caseAnalysis)
                    return _caseAnalysis.ParentDocument;
                if (null != _boxCaseAnalysis)
                    return _boxCaseAnalysis.ParentDocument;
                return null;
            }
        }

        public string Title
        {
            get
            {
                if (null != _analysis)
                    return _analysis.Name;

                if (null != _selSolution)
                    return _selSolution.Name;
                else if (null != _selPackPalletSolution)
                    return _selPackPalletSolution.Name;
                else if (null != _selCylinderPalletSolution)
                    return _selCylinderPalletSolution.Name;
                else if (null != _selCaseSolution)
                    return _selCaseSolution.Name;
                else if (null != _selBoxCaseSolution)
                    return _selBoxCaseSolution.Name;
                return null;
            }
        }

        public AnalysisCasePallet AnalysisCasePallet
        { get { return _analysis as AnalysisCasePallet; } }

        public Solution Solution { get { return _solution; } }

        public CasePalletAnalysis CasePalletAnalysis
        {
            get
            {
                if (null != _palletAnalysis)
                    return _palletAnalysis;
                else if (null != _caseAnalysis)
                    return CasePalletSolution.Analysis;
                else
                    return null;
            }
        }
        public PackPalletAnalysis PackPalletAnalysis
        { get { return _packPalletAnalysis; } }
        public SelPackPalletSolution SelPackPalletSolution
        { get { return _selPackPalletSolution; } }

        public CylinderPalletAnalysis CylinderPalletAnalysis
        { get { return _cylinderPalletAnalysis; } }
        public HCylinderPalletAnalysis HCylinderPalletAnalysis
        { get { return _hCylinderPalletAnalysis; } }
        public SelCasePalletSolution SelSolution
        {
            get
            {
                if (null != _palletAnalysis)
                    return _selSolution;
                else if (null != _caseAnalysis)
                    return CasePalletAnalysis.GetSelSolutionBySolutionIndex(0);
                return null;
            }
        }
        public SelCylinderPalletSolution SelCylinderPalletSolution
        {
            get
            {
                if (null != _cylinderPalletAnalysis)
                    return _selCylinderPalletSolution;
                else
                    return null;
            }
        }
        public SelHCylinderPalletSolution SelHCylinderPalletSolution
        {
            get
            {
                if (null != _hCylinderPalletAnalysis)
                    return _selHCylinderPalletSolution;
                else
                    return null;
            }        
        }
        public SelBoxCasePalletSolution SelCaseSolution
        {   get { return _selCaseSolution; } }

        public BoxCasePalletAnalysis CaseAnalysis
        { get { return _caseAnalysis; } }
        public bool IsBoxCaseAnalysis
        { get { return (null != _boxCaseAnalysis) && (null != _selBoxCaseSolution);} }
        public bool IsCasePalletAnalysis
        { get { return (null != _palletAnalysis) && (null != _selSolution);} }
        public bool IsPackPalletAnalysis
        { get { return (null != _packPalletAnalysis) && (null != _selPackPalletSolution); } }
        public bool IsCylinderPalletAnalysis
        { get { return (null != _cylinderPalletAnalysis) && (null != _selCylinderPalletSolution); } }
        public bool IsHCylinderPalletAnalysis
        { get { return (null != _hCylinderPalletAnalysis) && (null != _selHCylinderPalletSolution); } }
        public bool IsBoxCasePalletAnalysis
        { get { return (null != _caseAnalysis) && (null != _selCaseSolution); } }
        public bool IsValid
        { get { return IsBoxCasePalletAnalysis ^ IsBoxCaseAnalysis ^ IsCasePalletAnalysis ^ IsCylinderPalletAnalysis ^ IsHCylinderPalletAnalysis ^ IsPackPalletAnalysis; } }
        public double ActualCaseWeight
        {
            get
            {
                if (IsCasePalletAnalysis)
                    return _palletAnalysis.BProperties.Weight;
                else if (IsBoxCasePalletAnalysis)
                    return _selCaseSolution.Solution.CaseWeight;
                else
                    return 0.0; // invalid
            }
        }
        public double ActualPalletWeight
        {
            get
            {
                if (IsCasePalletAnalysis)
                    return _selSolution.Solution.PalletWeight;
                else if (IsCylinderPalletAnalysis)
                    return _selCylinderPalletSolution.Solution.PalletWeight;
                else if (IsHCylinderPalletAnalysis)
                    return _selHCylinderPalletSolution.Solution.PalletWeight;
                else if (IsBoxCasePalletAnalysis)
                    return _selCaseSolution.Solution.PalletWeight;
                else
                    return 0.0; // invalid
            }
        }
        #endregion

        #region IItemListener related methods
        public void AddListener(IItemListener listener)
        {
            if (null != _selSolution)
                _selSolution.AddListener(listener);
            else if (null != _selCaseSolution)
                _selCaseSolution.AddListener(listener);
            else if (null != _selCylinderPalletSolution)
                _selCylinderPalletSolution.AddListener(listener);
        }

        public void RemoveListener(IItemListener listener)
        {
            if (null != _selSolution)
                _selSolution.RemoveListener(listener);
            else if (null != _selCaseSolution)
                _selCaseSolution.RemoveListener(listener);
        }
        #endregion

        #region Object override
        public override bool Equals(object obj)
        {
            if (obj is ReportData)
            {
                ReportData reportObject = obj as ReportData;
                return _selSolution == reportObject._selSolution
                    && _selCaseSolution == reportObject._selCaseSolution;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _selSolution.GetHashCode() ^ _selCaseSolution.GetHashCode();
        }
        #endregion

        #region CasePalletSolution
        public CasePalletSolution CasePalletSolution
        {
            get
            {
                // case analysis
                if (null != _caseAnalysis)
                {
                    if (null == _palletSolution)
                        _palletSolution = _selCaseSolution.Solution.AttachedPalletSolution;
                    return _palletSolution;
                }
                // pallet analysis 
                else if (null != _palletAnalysis)
                    return _selSolution.Solution;
                // error!
                else
                    return null;
            }
        }
        public PackPalletSolution PackPalletSolution
        {
            get
            {
                if (null != _selPackPalletSolution)
                    return _selPackPalletSolution.Solution;
                else
                    return null;
            }
        }
        public CylinderPalletSolution CylinderPalletSolution
        {
            get
            {
                if (null != _selCylinderPalletSolution)
                    return _selCylinderPalletSolution.Solution;
                else
                    return null;
            }
        }
        public HCylinderPalletSolution HCylinderPalletSolution
        {
            get
            {
                if (null != _selHCylinderPalletSolution)
                    return _selHCylinderPalletSolution.Solution;
                else
                    return null;
            }
        }
        #endregion

        #region BoxCaseAnalysis
        /// <summary>
        /// BoxCaseAnalysis
        /// </summary>
        public BoxCaseAnalysis BoxCaseAnalysis
        {
            get
            {
                if (IsBoxCaseAnalysis)
                    return _boxCaseAnalysis;
                else
                    return null;
            }
        }
        /// <summary>
        /// SelBoxCaseSolution
        /// </summary>
        public SelBoxCaseSolution SelBoxCaseSolution
        {
            get
            {
                if (IsBoxCaseAnalysis)
                    return _selBoxCaseSolution;
                else
                    return null;
            }
        }
        #endregion
    }
    #endregion

    #region Reporter
    /// <summary>
    /// Generates pallet analyses reports
    /// </summary>
    abstract public class Reporter
    {
        #region Enums
        public enum eImageSize
        {
            IMAGESIZE_DEFAULT
            , IMAGESIZE_SMALL
        };
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(Reporter));
        protected static bool _validateAgainstSchema = false;
        protected string _imageDirectory;
        protected static string _companyLogo;
        protected static eImageSize _imageSize = eImageSize.IMAGESIZE_DEFAULT;
        #endregion

        #region Abstract members
        abstract public bool WriteNamespace { get; }
        abstract public bool WriteImageFiles { get; }
        #endregion

        #region Public properties
        static public string CompanyLogo
        {
            get { return _companyLogo; }
            set { _companyLogo = File.Exists(value) ? value : string.Empty; }
        }
        static public eImageSize ImageSizeSetting
        {
            get { return _imageSize; }
            set { _imageSize = value; }
        }
        #endregion

        #region Private properties
        private string ImageDirectory
        {
            set {   _imageDirectory = value;}
            get {   return _imageDirectory;  }
        }
        private int ImageSizeDetail
        {
            get
            {
                switch (_imageSize)
                {
                    case eImageSize.IMAGESIZE_DEFAULT: return 256;
                    case eImageSize.IMAGESIZE_SMALL: return 128;
                    default: return 256;
                }
            }
        }
        private int ImageSizeWide
        {
            get
            {
                switch (_imageSize)
                {
                    case eImageSize.IMAGESIZE_DEFAULT: return 768;
                    case eImageSize.IMAGESIZE_SMALL: return 384;
                    default: return 768;
                }
            }
        }
        #endregion

        #region Report generation
        public void BuildAnalysisReport(ReportData inputData, string reportTemplatePath, string outputFilePath)
        {
            // verify if inputData is a valid entry
            if (!inputData.IsValid)
                throw new Exception("Reporter.BuildAnalysisReport(): ReportData argument is invalid!");
            // absolute path
            string absOutputFilePath = ToAbsolute(outputFilePath);
            string absReportTemplatePath = ToAbsolute(reportTemplatePath);
            // create directory if needed
            ImageDirectory = Path.Combine(Path.GetDirectoryName(absOutputFilePath), "Images");
            if (WriteImageFiles && !Directory.Exists(ImageDirectory))
                Directory.CreateDirectory(ImageDirectory); 
            // create xml data file + XmlTextReader
            string xmlFilePath = Path.ChangeExtension(System.IO.Path.GetTempFileName(), "xml");
            CreateAnalysisDataFile(inputData, xmlFilePath, WriteNamespace);
            XmlTextReader xmlData = new XmlTextReader(new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read));
            // validate against schema
            // note xml file validation against xml schema produces a large number of errors
            // For the moment, I can not remove all errors
            if (_validateAgainstSchema)
                Reporter.ValidateXmlDocument(xmlData, Path.Combine(Path.GetDirectoryName(absReportTemplatePath), "ReportSchema.xsd"));
            // check availibility of files
            if (!File.Exists(absReportTemplatePath))
                throw new System.IO.FileNotFoundException(string.Format("Report template path ({0}) is invalid", absReportTemplatePath));
            // load generated xslt
            XmlTextReader xsltReader = new XmlTextReader(new FileStream(absReportTemplatePath, FileMode.Open, FileAccess.Read));
            string threeLetterLanguageAbbrev = System.Globalization.CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName;
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(absReportTemplatePath), threeLetterLanguageAbbrev + ".xml")))
            {
                _log.Warn(string.Format("Language file {0}.xml could not be found! Trying ENU.xml...", threeLetterLanguageAbbrev));
                threeLetterLanguageAbbrev = "ENU";
            }
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(absReportTemplatePath), threeLetterLanguageAbbrev + ".xml")))
                _log.Warn(string.Format("Language file {0}.xml could not be found!", threeLetterLanguageAbbrev));
            // generate word document
            byte[] wordDoc = GetReport(xmlData, xsltReader, Path.Combine(Path.GetDirectoryName(absReportTemplatePath), threeLetterLanguageAbbrev));
            // write resulting array to HDD, show process information
            using (FileStream fs = new FileStream(absOutputFilePath, FileMode.Create))
                fs.Write(wordDoc, 0, wordDoc.Length);
        }
        #endregion

        #region Xml transformation using xsl template
        /// <summary>
        /// Creates document from XML using XSLT
        /// </summary>
        /// <param name="xmlData">Report data as XML</param>
        /// <param name="xsltReader">XSLT transformation as Stream, used for document creation</param>
        /// <returns>Resulting document as byte[]</returns>
        private static byte[] GetReport(XmlReader xmlData, XmlReader xsltReader, string threeLetterLanguageAbbrev)
        {
            // Initialize needed variables
            XslCompiledTransform xslt = new XslCompiledTransform();
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("lang", "", threeLetterLanguageAbbrev);

            XsltSettings xslt_set = new XsltSettings();
            xslt_set.EnableScript = true;
            xslt_set.EnableDocumentFunction = true;
          

            using (MemoryStream swResult = new MemoryStream())
            {
                // Load XSLT to reader and perform transformation
                xslt.Load(xsltReader, xslt_set, new MyXmlUrlResolver());
                xslt.Transform(xmlData, args, swResult);
                return swResult.ToArray();
            }
        }
        #endregion

        #region Static methods to build xml report
        public void CreateAnalysisDataFile(ReportData inputData, string xmlDataFilePath, bool writeNamespace)
        {
            // instantiate XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            // set declaration
            XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "no");
            xmlDoc.AppendChild(declaration);
            // report (root) element
            XmlElement elemDocument;
            if (writeNamespace)
                elemDocument = xmlDoc.CreateElement("report", "http://treeDim/StackBuilder/ReportSchema.xsd");
            else
                elemDocument = xmlDoc.CreateElement("report");
            xmlDoc.AppendChild(elemDocument);

            string ns = xmlDoc.DocumentElement.NamespaceURI;
            Document doc = inputData.Document;
            // name element
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = doc.Name;
            elemDocument.AppendChild(elemName);
            // description element
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = doc.Description;
            elemDocument.AppendChild(elemDescription);
            // author element
            XmlElement elemAuthor = xmlDoc.CreateElement("author", ns);
            elemAuthor.InnerText = doc.Author;
            elemDocument.AppendChild(elemAuthor);
            // date of creation element
            XmlElement elemDateOfCreation = xmlDoc.CreateElement("dateOfCreation", ns);
            elemDateOfCreation.InnerText = doc.DateOfCreation.Year < 2000 ? DateTime.Now.ToShortDateString() : doc.DateOfCreation.ToShortDateString();
            elemDocument.AppendChild(elemDateOfCreation);
            // CompanyLogo
            if (!string.IsNullOrEmpty(CompanyLogo))
            {
                System.Drawing.Bitmap logoBitmap = new Bitmap(System.Drawing.Bitmap.FromFile(CompanyLogo));

                XmlElement elemCompanyLogo = xmlDoc.CreateElement("companyLogo", ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemCompanyLogo.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(logoBitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", 100, 100);
                elemCompanyLogo.Attributes.Append(styleAttribute);
                elemDocument.AppendChild(elemCompanyLogo);

                SaveImageAs(logoBitmap, "CompanyLogo.png");
            }
            // case analysis
            AppendCaseAnalysisElement(inputData, elemDocument, xmlDoc);
            // box/case analysis
            AppendBoxCaseAnalysisElement(inputData, elemDocument, xmlDoc);
            // case/pallet analysis
            AppendCasePalletAnalysisElement(inputData, elemDocument, xmlDoc);
            // pack/pallet analysis
            AppendPackPalletAnalysisElement(inputData, elemDocument, xmlDoc);
            // cylinder/pallet analysis
            AppendCylinderPalletAnalysisElement(inputData, elemDocument, xmlDoc);
            // hcylinder/pallet analysis
            AppendHCylinderPalletAnalysisElement(inputData, elemDocument, xmlDoc);
            // truckAnalysis
            AppendTruckAnalysisElement(inputData, elemDocument, xmlDoc);
            // ectAnalysis
            AppendEctAnalysisElement(inputData, elemDocument, xmlDoc);

            // finally save xml document
            _log.Debug(string.Format("Generating xml data file {0}", xmlDataFilePath));
            xmlDoc.Save(xmlDataFilePath);
        }
        #endregion

        #region Analyses
        private void AppendAnalysisCasePalletElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;

            AnalysisCasePallet analysis = inputData.AnalysisCasePallet;
            Solution solution = inputData.Solution;

            // analysisCasePallet
            XmlElement eltAnalysisCasePallet = xmlDoc.CreateElement("analysisCasePallet", ns);
            elemDocument.AppendChild(eltAnalysisCasePallet);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = analysis.Name;
            eltAnalysisCasePallet.AppendChild(elemName);
            // description




        }

        private void AppendCasePalletAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            if (!inputData.IsCasePalletAnalysis && !inputData.IsBoxCasePalletAnalysis)
                return;
            string ns = xmlDoc.DocumentElement.NamespaceURI;

            CasePalletAnalysis analysis = inputData.CasePalletAnalysis;
            SelCasePalletSolution selSolution = inputData.SelSolution;

            // palletAnalysis
            XmlElement eltPalletAnalysis = xmlDoc.CreateElement("casePalletAnalysis", ns);
            elemDocument.AppendChild(eltPalletAnalysis);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = analysis.Name;
            eltPalletAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = analysis.Description;
            eltPalletAnalysis.AppendChild(elemDescription);
            // pallet
            AppendPalletElement(analysis.PalletProperties, eltPalletAnalysis, xmlDoc);
            // case
            if (analysis.BProperties is CaseOfBoxesProperties)
            {
                AppendInsideBoxElement(analysis, selSolution.Solution, eltPalletAnalysis, xmlDoc);
                AppendCaseOfBoxesElement(analysis, selSolution.Solution, eltPalletAnalysis, xmlDoc);
            }
            else if (analysis.BProperties is BoxProperties && inputData.IsCasePalletAnalysis)
                AppendCaseElement(analysis, selSolution.Solution, eltPalletAnalysis, xmlDoc);
            else if (analysis.BProperties is BundleProperties)
                AppendBundleElement(analysis.BProperties as BundleProperties, eltPalletAnalysis, xmlDoc);
            // interlayer
            AppendInterlayerElement(analysis.InterlayerProperties, eltPalletAnalysis, xmlDoc);
            // pallet corners
            AppendPalletCornerElement(analysis.PalletCornerProperties, eltPalletAnalysis, xmlDoc);
            // pallet cap
            AppendPalletCapElement(analysis.PalletCapProperties, eltPalletAnalysis, xmlDoc);
            // pallet film
            AppendPalletFilmElement(analysis.PalletFilmProperties, analysis, eltPalletAnalysis, xmlDoc);
            // constraintSet
            AppendConstraintSet(analysis, selSolution.Solution, eltPalletAnalysis, xmlDoc);
            // solution
            AppendSolutionElement(inputData, eltPalletAnalysis, xmlDoc);
        }

        private void AppendPackPalletAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            if (!inputData.IsPackPalletAnalysis)
                return;
            string ns = xmlDoc.DocumentElement.NamespaceURI;

            PackPalletAnalysis analysis = inputData.PackPalletAnalysis;
            SelPackPalletSolution selSolution = inputData.SelPackPalletSolution;

            // packPalletAnalysis element
            XmlElement eltPackPalletAnalysis = xmlDoc.CreateElement("packPalletAnalysis", ns);
            elemDocument.AppendChild(eltPackPalletAnalysis);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = analysis.Name;
            eltPackPalletAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = analysis.Description;
            eltPackPalletAnalysis.AppendChild(elemDescription);
            // pack
            AppendPackElement(analysis.PackProperties, eltPackPalletAnalysis, xmlDoc);
            // pallet
            AppendPalletElement(analysis.PalletProperties, eltPackPalletAnalysis, xmlDoc);
            // interlayer
            AppendInterlayerElement(analysis.InterlayerProperties, eltPackPalletAnalysis, xmlDoc);
            // solution
            AppendPackPalletSolutionElement(inputData, eltPackPalletAnalysis, xmlDoc);
        }

        private void AppendCylinderPalletAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            if (!inputData.IsCylinderPalletAnalysis)
                return;
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // cylinder/pallet analysis
            CylinderPalletAnalysis analysis = inputData.CylinderPalletAnalysis;
            SelCylinderPalletSolution selSolution = inputData.SelCylinderPalletSolution;

            // cylinder/pallet analysis
            XmlElement elemCylinderPalletAnalysis = xmlDoc.CreateElement("cylinderPalletAnalysis", ns);
            elemDocument.AppendChild(elemCylinderPalletAnalysis);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = analysis.Name;
            elemCylinderPalletAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = analysis.Description;
            elemCylinderPalletAnalysis.AppendChild(elemDescription);
            // pallet
            AppendPalletElement(analysis.PalletProperties, elemCylinderPalletAnalysis, xmlDoc);
            // cylinder
            AppendCylinderElement(analysis.CylinderProperties, elemCylinderPalletAnalysis, xmlDoc);
            // interlayer
            AppendInterlayerElement(analysis.InterlayerProperties, elemCylinderPalletAnalysis, xmlDoc);
            // constraintSet
            AppendCylinderPalletConstraintSet(analysis.ConstraintSet, elemCylinderPalletAnalysis, xmlDoc);
            // solution
            AppendCylinderPalletSolutionElement(inputData, elemCylinderPalletAnalysis, xmlDoc);
        }

        private void AppendHCylinderPalletAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            if (!inputData.IsHCylinderPalletAnalysis)
                return;
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // cylinder/pallet analysis
            HCylinderPalletAnalysis analysis = inputData.HCylinderPalletAnalysis;
            SelHCylinderPalletSolution selSolution = inputData.SelHCylinderPalletSolution;

            // cylinder/pallet analysis
            XmlElement elemCylinderPalletAnalysis = xmlDoc.CreateElement("hCylinderPalletAnalysis", ns);
            elemDocument.AppendChild(elemCylinderPalletAnalysis);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = analysis.Name;
            elemCylinderPalletAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = analysis.Description;
            elemCylinderPalletAnalysis.AppendChild(elemDescription);
            // pallet
            AppendPalletElement(analysis.PalletProperties, elemCylinderPalletAnalysis, xmlDoc);
            // cylinder
            AppendCylinderElement(analysis.CylinderProperties, elemCylinderPalletAnalysis, xmlDoc);
            // constraintSet
            AppendHCylinderPalletConstraintSet(analysis.ConstraintSet, elemCylinderPalletAnalysis, xmlDoc);
            // solution
            AppendHCylinderPalletSolutionElement(inputData, elemCylinderPalletAnalysis, xmlDoc);      
        
        }

        private void AppendPalletElement(PalletProperties palletProp, XmlElement elemAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            if (null == palletProp) return;
            // pallet
            XmlElement elemPallet = xmlDoc.CreateElement("pallet", ns);
            elemAnalysis.AppendChild(elemPallet);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = palletProp.Name;
            elemPallet.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = palletProp.Description;
            elemPallet.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemPallet, "length", UnitsManager.UnitType.UT_LENGTH, palletProp.Length);
            AppendElementValue(xmlDoc, elemPallet, "width", UnitsManager.UnitType.UT_LENGTH, palletProp.Width);
            AppendElementValue(xmlDoc, elemPallet, "height", UnitsManager.UnitType.UT_LENGTH, palletProp.Height);
            AppendElementValue(xmlDoc, elemPallet, "weight", UnitsManager.UnitType.UT_MASS, palletProp.Weight);
            AppendElementValue(xmlDoc, elemPallet, "admissibleLoad", UnitsManager.UnitType.UT_MASS, palletProp.AdmissibleLoadWeight);

            // type
            XmlElement elemType = xmlDoc.CreateElement("type", ns);
            elemType.InnerText = palletProp.TypeName;
            elemPallet.AppendChild(elemType);
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(512, 512));
            graphics.CameraPosition = Graphics3D.Corner_0;
            Pallet pallet = new Pallet(palletProp);
            pallet.Draw(graphics, Transform3D.Identity);
            graphics.AddDimensions(new DimensionCube(palletProp.Length, palletProp.Width, palletProp.Height));
            graphics.Flush();
            // ---
            // view_pallet_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_pallet_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemPallet.AppendChild(elemImage);
            // save image
            SaveImageAs(graphics.Bitmap, "view_pallet_iso.png");
        }

        private void AppendCaseElement(CasePalletAnalysis analysis, CasePalletSolution sol, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // get BoxProperties
            BoxProperties boxProp = analysis.BProperties as BoxProperties;
            if (null == boxProp) return;
            // case
            XmlElement elemCase = xmlDoc.CreateElement("case", ns);
            elemPalletAnalysis.AppendChild(elemCase);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = boxProp.Name;
            elemCase.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = boxProp.Description;
            elemCase.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemCase, "length", UnitsManager.UnitType.UT_LENGTH, boxProp.Length);
            AppendElementValue(xmlDoc, elemCase, "width", UnitsManager.UnitType.UT_LENGTH, boxProp.Width);
            AppendElementValue(xmlDoc, elemCase, "height", UnitsManager.UnitType.UT_LENGTH, boxProp.Height);
            AppendElementValue(xmlDoc, elemCase, "weight", UnitsManager.UnitType.UT_MASS, boxProp.Weight);
            AppendElementValue(xmlDoc, elemCase, "admissibleLoadOnTop", UnitsManager.UnitType.UT_MASS, 0.0);
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Box box = new Box(0, boxProp);
            graphics.AddBox(box);
            DimensionCube dc = new DimensionCube(box.Length, box.Width, box.Height);    dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // ---
            // view_case_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_case_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemCase.AppendChild(elemImage);
            // save image
            SaveImageAs(graphics.Bitmap, "view_case_iso.png");
        }

        private void AppendCylinderElement(CylinderProperties cylProperties, XmlElement elemAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // get CylinderProperties
            XmlElement elemCylinder = xmlDoc.CreateElement("cylinder", ns);
            elemAnalysis.AppendChild(elemCylinder);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = cylProperties.Name;
            elemCylinder.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = cylProperties.Description;
            elemCylinder.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemCylinder, "radius", UnitsManager.UnitType.UT_LENGTH, cylProperties.RadiusOuter);
            AppendElementValue(xmlDoc, elemCylinder, "width", UnitsManager.UnitType.UT_LENGTH, cylProperties.Height);
            AppendElementValue(xmlDoc, elemCylinder, "height", UnitsManager.UnitType.UT_MASS, cylProperties.Weight);
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Cylinder cyl = new Cylinder(0, cylProperties);
            graphics.AddCylinder(cyl);
            DimensionCube dc = new DimensionCube(cyl.DiameterOuter, cyl.DiameterOuter, cyl.Height);   dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // ---
            // view_case_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_cylinder_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemCylinder.AppendChild(elemImage);
            // save image
            SaveImageAs(graphics.Bitmap, "view_cylinder_iso.png");             
        }

        private void AppendBundleElement(BundleProperties bundleProp, XmlElement elemAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            if (null == bundleProp) return;
            // bundle
            XmlElement elemBundle = CreateElement("bundle", null, elemAnalysis, xmlDoc, ns);
            elemAnalysis.AppendChild(elemBundle);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = bundleProp.Name;
            elemBundle.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = bundleProp.Description;
            elemBundle.AppendChild(elemDescription);
            // length / width / number of flats / unit thickness / unit weight / total thickness / total weight
            AppendElementValue(xmlDoc, elemBundle, "length", UnitsManager.UnitType.UT_LENGTH, bundleProp.Length);
            AppendElementValue(xmlDoc, elemBundle, "width", UnitsManager.UnitType.UT_LENGTH, bundleProp.Width);
            AppendElementValue(xmlDoc, elemBundle, "numberOfFlats", bundleProp.NoFlats);
            AppendElementValue(xmlDoc, elemBundle, "unitThickness", UnitsManager.UnitType.UT_LENGTH, bundleProp.UnitThickness);
            AppendElementValue(xmlDoc, elemBundle, "unitWeight", UnitsManager.UnitType.UT_MASS, bundleProp.UnitWeight);
            AppendElementValue(xmlDoc, elemBundle, "totalThickness", UnitsManager.UnitType.UT_LENGTH, bundleProp.UnitThickness * bundleProp.NoFlats);
            AppendElementValue(xmlDoc, elemBundle, "totalWeight", UnitsManager.UnitType.UT_MASS, bundleProp.UnitWeight * bundleProp.NoFlats);
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            Box box = new Box(0, bundleProp);
            graphics.AddBox(box);
            DimensionCube dc = new DimensionCube(bundleProp.Length, bundleProp.Width, bundleProp.Height);   dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // ---
            // view_bundle_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_bundle_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemBundle.AppendChild(elemImage);
            // save images ?
            SaveImageAs(graphics.Bitmap, "view_bundle_iso.png");
        }

        private void AppendConstraintSet(CasePalletAnalysis analysis, CasePalletSolution sol, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            PalletConstraintSet cs = analysis.ConstraintSet;
            // solution
            XmlElement elemConstraintSet = xmlDoc.CreateElement("constraintSet", ns);
            elemPalletAnalysis.AppendChild(elemConstraintSet);
            // overhangX, overhangY
            AppendElementValue(xmlDoc, elemConstraintSet, "overhangX", UnitsManager.UnitType.UT_LENGTH, cs.OverhangX);
            AppendElementValue(xmlDoc, elemConstraintSet, "overhangY", UnitsManager.UnitType.UT_LENGTH, cs.OverhangY);
            // allowedPatterns
            XmlElement elemAllowedPatterns = xmlDoc.CreateElement("allowedPatterns", ns);
            elemAllowedPatterns.InnerText = cs.AllowedPatternString;
            elemConstraintSet.AppendChild(elemAllowedPatterns);
            // allowedBoxAxis
            XmlElement elemAllowedBoxAxis = xmlDoc.CreateElement("allowedOrthoAxis", ns);
            elemAllowedBoxAxis.InnerText = cs.AllowOrthoAxisString;
            elemConstraintSet.AppendChild(elemAllowedBoxAxis);
            // allowAlternateLayers
            XmlElement elemAllowAlternateLayers = xmlDoc.CreateElement("allowAlternateLayers", ns);
            elemAllowAlternateLayers.InnerText = cs.AllowAlternateLayers.ToString();
            elemConstraintSet.AppendChild(elemAllowAlternateLayers);
            // allowAlignedLayers
            XmlElement elemAllowAlignedLayers = xmlDoc.CreateElement("allowAlignedLayers", ns);
            elemAllowAlignedLayers.InnerText = cs.AllowAlignedLayers.ToString();
            elemConstraintSet.AppendChild(elemAllowAlignedLayers);
            // interlayerPeriod
            if (cs.HasInterlayer)
            {
                XmlElement elemInterlayerPeriodGroup = xmlDoc.CreateElement("interlayerPeriodGroup", ns);
                elemConstraintSet.AppendChild(elemInterlayerPeriodGroup);
                XmlElement elemInterlayerPeriod = xmlDoc.CreateElement("interlayerPeriod", ns);
                elemInterlayerPeriod.InnerText = string.Format("{0}", cs.InterlayerPeriod);
                elemInterlayerPeriodGroup.AppendChild(elemInterlayerPeriod);
            }
            // stopCriterion
            if (cs.UseMaximumHeight)
            {
                XmlElement maximumPalletHeightGroup = xmlDoc.CreateElement("maximumPalletHeightGroup", ns);
                elemConstraintSet.AppendChild(maximumPalletHeightGroup);
                // maximum pallet height
                AppendElementValue(xmlDoc, maximumPalletHeightGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_LENGTH, cs.MaximumHeight);
            }
            if (cs.UseMaximumNumberOfCases)
            {
                XmlElement maximumNumberOfItemsGroup = xmlDoc.CreateElement("maximumNumberOfItemsGroup", ns);
                elemConstraintSet.AppendChild(maximumNumberOfItemsGroup);
                XmlElement maximumNumberOfItems = xmlDoc.CreateElement("maximumNumberOfItems", ns);
                maximumNumberOfItems.InnerText = string.Format("{0}", cs.MaximumNumberOfItems);
                maximumNumberOfItemsGroup.AppendChild(maximumNumberOfItems);
            }
            if (cs.UseMaximumPalletWeight)
            {
                XmlElement maximumPalletWeightGroup = xmlDoc.CreateElement("maximumPalletWeightGroup", ns);
                elemConstraintSet.AppendChild(maximumPalletWeightGroup);
                // pallet weight
                AppendElementValue(xmlDoc, maximumPalletWeightGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_MASS, cs.MaximumPalletWeight);
            }
            if (cs.UseMaximumWeightOnBox)
            {
                XmlElement maximumWeightOnBoxGroup = xmlDoc.CreateElement("maximumWeightOnBoxGroup", ns);
                elemConstraintSet.AppendChild(maximumWeightOnBoxGroup);
                // admissible load on top
                AppendElementValue(xmlDoc, maximumWeightOnBoxGroup, "admissibleLoadOnTop", UnitsManager.UnitType.UT_MASS, cs.MaximumWeightOnBox);
            }
        }
        private void AppendCylinderPalletConstraintSet(CylinderPalletConstraintSet cs, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // solution
            XmlElement elemConstraintSet = xmlDoc.CreateElement("constraintSet", ns);
            elemPalletAnalysis.AppendChild(elemConstraintSet);
            AppendElementValue(xmlDoc, elemConstraintSet, "overhangX", UnitsManager.UnitType.UT_LENGTH, cs.OverhangX);
            AppendElementValue(xmlDoc, elemConstraintSet, "overhangY", UnitsManager.UnitType.UT_LENGTH, cs.OverhangY);
            // stopCriterion
            if (cs.UseMaximumPalletHeight)
            {
                XmlElement maximumPalletHeightGroup = xmlDoc.CreateElement("maximumPalletHeightGroup", ns);
                elemConstraintSet.AppendChild(maximumPalletHeightGroup);
                // max pallet height
                AppendElementValue(xmlDoc, maximumPalletHeightGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_LENGTH, cs.MaximumPalletHeight);
            }
            if (cs.UseMaximumNumberOfItems)
            {
                XmlElement maximumNumberOfItemsGroup = xmlDoc.CreateElement("maximumNumberOfItemsGroup", ns);
                elemConstraintSet.AppendChild(maximumNumberOfItemsGroup);
                XmlElement maximumNumberOfItems = xmlDoc.CreateElement("maximumNumberOfItems", ns);
                maximumNumberOfItems.InnerText = string.Format("{0}", cs.MaximumNumberOfItems);
                maximumNumberOfItemsGroup.AppendChild(maximumNumberOfItems);
            }
            if (cs.UseMaximumPalletWeight)
            {
                XmlElement maximumPalletWeightGroup = xmlDoc.CreateElement("maximumPalletWeightGroup", ns);
                elemConstraintSet.AppendChild(maximumPalletWeightGroup);
                // max pallet weight
                AppendElementValue(xmlDoc, maximumPalletWeightGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_MASS, cs.MaximumPalletWeight);
            }
            if (cs.UseMaximumLoadOnLowerCylinder)
            {
                XmlElement maximumWeightOnBoxGroup = xmlDoc.CreateElement("maximumWeightOnCylinderGroup", ns);
                elemConstraintSet.AppendChild(maximumWeightOnBoxGroup);
                // admissible load on top
                AppendElementValue(xmlDoc, maximumWeightOnBoxGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_MASS, cs.MaximumLoadOnLowerCylinder);
            }
        }

        private void AppendHCylinderPalletConstraintSet(HCylinderPalletConstraintSet cs, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // solution
            XmlElement elemConstraintSet = xmlDoc.CreateElement("constraintSet", ns);
            elemPalletAnalysis.AppendChild(elemConstraintSet);
            AppendElementValue(xmlDoc, elemConstraintSet, "overhangX", UnitsManager.UnitType.UT_LENGTH, cs.OverhangX);
            AppendElementValue(xmlDoc, elemConstraintSet, "overhangY", UnitsManager.UnitType.UT_LENGTH, cs.OverhangY);
            // stopCriterion
            if (cs.UseMaximumPalletHeight)
            {
                XmlElement maximumPalletHeightGroup = xmlDoc.CreateElement("maximumPalletHeightGroup", ns);
                elemConstraintSet.AppendChild(maximumPalletHeightGroup);
                // max pallet height
                AppendElementValue(xmlDoc, maximumPalletHeightGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_LENGTH, cs.MaximumPalletHeight);
            }
            if (cs.UseMaximumNumberOfItems)
            {
                XmlElement maximumNumberOfItemsGroup = xmlDoc.CreateElement("maximumNumberOfItemsGroup", ns);
                elemConstraintSet.AppendChild(maximumNumberOfItemsGroup);
                XmlElement maximumNumberOfItems = xmlDoc.CreateElement("maximumNumberOfItems", ns);
                maximumNumberOfItems.InnerText = string.Format("{0}", cs.MaximumNumberOfItems);
                maximumNumberOfItemsGroup.AppendChild(maximumNumberOfItems);
            }
            if (cs.UseMaximumPalletWeight)
            {
                XmlElement maximumPalletWeightGroup = xmlDoc.CreateElement("maximumPalletWeightGroup", ns);
                elemConstraintSet.AppendChild(maximumPalletWeightGroup);
                // max pallet weight
                AppendElementValue(xmlDoc, maximumPalletWeightGroup, "maximumPalletHeight", UnitsManager.UnitType.UT_MASS, cs.MaximumPalletWeight);
            }
        }
        #endregion

        #region Dimensions
        private void AppendInsideBoxElement(CasePalletAnalysis analysis, CasePalletSolution sol, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // get caseOfBoxProperties
            CaseOfBoxesProperties caseOfBoxes = analysis.BProperties as CaseOfBoxesProperties;
            // get box properties
            BoxProperties boxProperties = caseOfBoxes.InsideBoxProperties;
            // elemBoxes
            XmlElement elemBox = xmlDoc.CreateElement("box", ns);
            elemPalletAnalysis.AppendChild(elemBox);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = boxProperties.Name;
            elemBox.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = boxProperties.Description;
            elemBox.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemBox, "length", UnitsManager.UnitType.UT_LENGTH, boxProperties.Length);
            AppendElementValue(xmlDoc, elemBox, "width", UnitsManager.UnitType.UT_LENGTH, boxProperties.Width);
            AppendElementValue(xmlDoc, elemBox, "height", UnitsManager.UnitType.UT_LENGTH, boxProperties.Height);
            AppendElementValue(xmlDoc, elemBox, "weight", UnitsManager.UnitType.UT_MASS, boxProperties.Weight);

            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Box box = new Box(0, boxProperties);
            graphics.AddBox(box);
            DimensionCube dc = new DimensionCube(box.Length, box.Width, box.Height);    dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // ---
            // view_box_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_box_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemBox.AppendChild(elemImage);
            // save image ?
            SaveImageAs(graphics.Bitmap, "view_box_iso.png");
        }
        private void AppendCaseOfBoxesElement(CasePalletAnalysis analysis, CasePalletSolution sol, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // get CaseOfBoxProperties
            CaseOfBoxesProperties caseOfBoxes = analysis.BProperties as CaseOfBoxesProperties;
            if (null == caseOfBoxes) return;
            // elemCaseOfBoxes
            XmlElement elemCaseOfBoxes = xmlDoc.CreateElement("caseOfBoxes", ns);
            elemPalletAnalysis.AppendChild(elemCaseOfBoxes);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = caseOfBoxes.Name;
            elemCaseOfBoxes.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = caseOfBoxes.Description;
            elemCaseOfBoxes.AppendChild(elemDescription);
            // length
            XmlElement elemNoX = xmlDoc.CreateElement("noX", ns);
            elemNoX.InnerText = string.Format("{0}", caseOfBoxes.CaseDefinition.Arrangement._iLength);
            elemCaseOfBoxes.AppendChild(elemNoX);
            // width
            XmlElement elemNoY = xmlDoc.CreateElement("noY", ns);
            elemNoY.InnerText = string.Format("{0}", caseOfBoxes.CaseDefinition.Arrangement._iWidth);
            elemCaseOfBoxes.AppendChild(elemNoY);
            // height
            XmlElement elemNoZ = xmlDoc.CreateElement("noZ", ns);
            elemNoZ.InnerText = string.Format("{0}", caseOfBoxes.CaseDefinition.Arrangement._iHeight);
            elemCaseOfBoxes.AppendChild(elemNoZ);
            // number of boxes
            XmlElement elemNoBoxes = xmlDoc.CreateElement("numberOfBoxes", ns);
            elemNoBoxes.InnerText = string.Format("{0}", caseOfBoxes.NumberOfBoxes);
            elemCaseOfBoxes.AppendChild(elemNoBoxes);
            // dim0
            XmlElement eltDim0 = xmlDoc.CreateElement("dim0", ns);
            eltDim0.InnerText = string.Format("{0}", caseOfBoxes.CaseDefinition.Dim0);
            elemCaseOfBoxes.AppendChild(eltDim0);
            // dim1
            XmlElement eltDim1 = xmlDoc.CreateElement("dim1", ns);
            eltDim1.InnerText = string.Format("{0}", caseOfBoxes.CaseDefinition.Dim1);
            elemCaseOfBoxes.AppendChild(eltDim1);

            AppendElementValue(xmlDoc, elemCaseOfBoxes, "innerLength", UnitsManager.UnitType.UT_LENGTH, caseOfBoxes.InsideLength);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "innerWidth", UnitsManager.UnitType.UT_LENGTH, caseOfBoxes.InsideWidth);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "innerHeight", UnitsManager.UnitType.UT_LENGTH, caseOfBoxes.InsideHeight);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "innerVolume", UnitsManager.UnitType.UT_VOLUME, caseOfBoxes.InsideVolume * UnitsManager.FactorCubeLengthToVolume);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "outerLength", UnitsManager.UnitType.UT_LENGTH, caseOfBoxes.Length);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "outerWidth", UnitsManager.UnitType.UT_LENGTH, caseOfBoxes.Width);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "outerHeight", UnitsManager.UnitType.UT_LENGTH, caseOfBoxes.Height);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "outerVolume", UnitsManager.UnitType.UT_VOLUME, caseOfBoxes.Volume * UnitsManager.FactorCubeLengthToVolume);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "emptyWeight", UnitsManager.UnitType.UT_MASS, caseOfBoxes.WeightEmpty);
            AppendElementValue(xmlDoc, elemCaseOfBoxes, "weight", UnitsManager.UnitType.UT_MASS, caseOfBoxes.Weight);

            // type converter
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            // view case of boxes iso1
            Graphics3DImage graphics1 = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics1.CameraPosition = Graphics3D.Corner_0;
            CaseDefinitionViewer viewer = new CaseDefinitionViewer(caseOfBoxes.CaseDefinition, caseOfBoxes.InsideBoxProperties, caseOfBoxes.CaseOptimConstraintSet);
            viewer.CaseProperties = caseOfBoxes;
            viewer.Orientation = sol.FirstCaseOrientation;
            viewer.Draw(graphics1);
            graphics1.Flush();
            // view case of boxes iso2
            Graphics3DImage graphics2 = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics2.CameraPosition = Graphics3D.Corner_0;
            Box box = new Box(0, caseOfBoxes);
            graphics2.AddBox(box);
            graphics2.AddDimensions(new DimensionCube(caseOfBoxes.Length, caseOfBoxes.Width, caseOfBoxes.Height));
            graphics2.Flush();
            // view_caseOfBoxes_iso1
            XmlElement elemImage1 = xmlDoc.CreateElement("view_caseOfBoxes_iso1", ns);
            elemImage1.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics1.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute1 = xmlDoc.CreateAttribute("style");
            styleAttribute1.Value = string.Format("width:{0}pt;height:{1}pt", graphics1.Bitmap.Width / 3, graphics1.Bitmap.Height / 3);
            elemImage1.Attributes.Append(styleAttribute1);
            elemCaseOfBoxes.AppendChild(elemImage1);
            // save image
            SaveImageAs(graphics1.Bitmap, "view_caseOfBoxes_iso1.png");
            // view_caseOfBoxes_iso2
            XmlElement elemImage2 = xmlDoc.CreateElement("view_caseOfBoxes_iso2", ns);
            elemImage2.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics2.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute2 = xmlDoc.CreateAttribute("style");
            styleAttribute2.Value = string.Format("width:{0}pt;height:{1}pt", graphics2.Bitmap.Width / 3, graphics2.Bitmap.Height / 3);
            elemImage2.Attributes.Append(styleAttribute2);
            elemCaseOfBoxes.AppendChild(elemImage2);
            // save image
            SaveImageAs(graphics2.Bitmap, "view_caseOfBoxes_iso2.png");
        }

        private void AppendInterlayerElement(InterlayerProperties interlayerProp, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            // sanity check
            if (null == interlayerProp) return;
            // namespace
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // interlayer
            XmlElement elemInterlayer = xmlDoc.CreateElement("interlayer", ns);
            elemPalletAnalysis.AppendChild(elemInterlayer);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = interlayerProp.Name;
            elemInterlayer.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = interlayerProp.Description;
            elemInterlayer.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemInterlayer, "length", UnitsManager.UnitType.UT_LENGTH, interlayerProp.Length);
            AppendElementValue(xmlDoc, elemInterlayer, "width", UnitsManager.UnitType.UT_LENGTH, interlayerProp.Width);
            AppendElementValue(xmlDoc, elemInterlayer, "thickness", UnitsManager.UnitType.UT_LENGTH, interlayerProp.Thickness);
            AppendElementValue(xmlDoc, elemInterlayer, "weight", UnitsManager.UnitType.UT_MASS, interlayerProp.Weight);

            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            Box box = new Box(0, interlayerProp);
            graphics.AddBox(box);
            graphics.Flush();
            // ---
            // view_interlayer_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_interlayer_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemInterlayer.AppendChild(elemImage);
            // save image ?
            SaveImageAs(graphics.Bitmap, "view_interlayer_iso.png");
        }

        private void AppendPalletCornerElement(PalletCornerProperties palletCornerProp, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            // sanity check
            if (null == palletCornerProp) return;
            // namespace
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // interlayer
            XmlElement elemPalletCorner = xmlDoc.CreateElement("palletCorner", ns);
            elemPalletAnalysis.AppendChild(elemPalletCorner);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = palletCornerProp.Name;
            elemPalletCorner.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = palletCornerProp.Description;
            elemPalletCorner.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemPalletCorner, "length", UnitsManager.UnitType.UT_LENGTH, palletCornerProp.Length);
            AppendElementValue(xmlDoc, elemPalletCorner, "width", UnitsManager.UnitType.UT_LENGTH, palletCornerProp.Width);
            AppendElementValue(xmlDoc, elemPalletCorner, "thickness", UnitsManager.UnitType.UT_LENGTH, palletCornerProp.Thickness);
            AppendElementValue(xmlDoc, elemPalletCorner, "weight", UnitsManager.UnitType.UT_LENGTH, palletCornerProp.Weight);
            // ---
            // view_palletCorner_iso
            // build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            Corner corner = new Corner(0, palletCornerProp);
            corner.Draw(graphics);
            graphics.Flush();
            // save image ?
            SaveImageAs(graphics.Bitmap, "view_palletCorner_iso.png");
            // ---
            // view_palletCorner_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_palletCorner_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemPalletCorner.AppendChild(elemImage);
        }

        private void AppendPalletCapElement(PalletCapProperties palletCapProp, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            // sanity check
            if (null == palletCapProp) return;
            // namespace
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // interlayer
            XmlElement elemPalletCap = xmlDoc.CreateElement("palletCap", ns);
            elemPalletAnalysis.AppendChild(elemPalletCap);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = palletCapProp.Name;
            elemPalletCap.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = palletCapProp.Description;
            elemPalletCap.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemPalletCap, "length", UnitsManager.UnitType.UT_LENGTH, palletCapProp.Length);
            AppendElementValue(xmlDoc, elemPalletCap, "width", UnitsManager.UnitType.UT_LENGTH, palletCapProp.Width);
            AppendElementValue(xmlDoc, elemPalletCap, "height", UnitsManager.UnitType.UT_LENGTH, palletCapProp.Height);
            AppendElementValue(xmlDoc, elemPalletCap, "innerLength", UnitsManager.UnitType.UT_LENGTH, palletCapProp.InsideLength);
            AppendElementValue(xmlDoc, elemPalletCap, "innerWidth", UnitsManager.UnitType.UT_LENGTH, palletCapProp.InsideWidth);
            AppendElementValue(xmlDoc, elemPalletCap, "innerHeight", UnitsManager.UnitType.UT_LENGTH, palletCapProp.InsideHeight);
            AppendElementValue(xmlDoc, elemPalletCap, "weight", UnitsManager.UnitType.UT_MASS, palletCapProp.Weight);
            // ---
            // view_palletCap_iso
            // build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            PalletCap palletCap = new PalletCap(0, palletCapProp, Vector3D.Zero);
            palletCap.Draw(graphics);
            graphics.AddDimensions(new DimensionCube(palletCapProp.Length, palletCapProp.Width, palletCapProp.Height));
            graphics.Flush();
            // save image ?
            SaveImageAs(graphics.Bitmap, "view_palletCap_iso.png");
            // ---
            // view_palletCap_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_palletCap_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemPalletCap.AppendChild(elemImage);
        }

        private void AppendPalletFilmElement(PalletFilmProperties palletFilmProp, CasePalletAnalysis analyis, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            // sanity check
            if (null == palletFilmProp) return;
            // namespace
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // interlayer
            XmlElement elemPalletFilm = xmlDoc.CreateElement("palletFilm", ns);
            elemPalletAnalysis.AppendChild(elemPalletFilm);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = palletFilmProp.Name;
            elemPalletFilm.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = palletFilmProp.Description;
            elemPalletFilm.AppendChild(elemDescription);
            // number of turns
            XmlElement elemNumberOfTurns = xmlDoc.CreateElement("numberOfTurns", ns);
            elemNumberOfTurns.InnerText = analyis.ConstraintSet.PalletFilmTurns.ToString();
            elemPalletFilm.AppendChild(elemNumberOfTurns);
        }

        private void AppendSolutionElement(ReportData inputData, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;

            SelCasePalletSolution selSolution = inputData.SelSolution;
            CasePalletSolution sol = inputData.CasePalletSolution;

            // solution
            XmlElement elemSolution = xmlDoc.CreateElement("palletSolution", ns);
            elemPalletAnalysis.AppendChild(elemSolution);
            // title
            XmlElement elemTitle = xmlDoc.CreateElement("title", ns);
            elemTitle.InnerText = sol.Title;
            elemSolution.AppendChild(elemTitle);
            // homogeneousLayer
            XmlElement elemHomogeneousLayer = xmlDoc.CreateElement("homogeneousLayer", ns);
            elemHomogeneousLayer.InnerText = sol.HasHomogeneousLayers.ToString();
            elemSolution.AppendChild(elemHomogeneousLayer);
            // efficiency
            XmlElement elemEfficiency = xmlDoc.CreateElement("efficiency", ns);
            elemEfficiency.InnerText = string.Format("{0:F}", sol.VolumeEfficiencyCases);
            elemSolution.AppendChild(elemEfficiency);

            AppendElementValue(xmlDoc, elemSolution, "palletWeight", UnitsManager.UnitType.UT_MASS, inputData.ActualPalletWeight);
            AppendElementValue(xmlDoc, elemSolution, "palletLength", UnitsManager.UnitType.UT_LENGTH, sol.PalletLength);
            AppendElementValue(xmlDoc, elemSolution, "palletWidth", UnitsManager.UnitType.UT_LENGTH, sol.PalletWidth);
            AppendElementValue(xmlDoc, elemSolution, "palletHeight", UnitsManager.UnitType.UT_LENGTH, sol.PalletHeight);

            // caseCount
            XmlElement elemCaseCount = xmlDoc.CreateElement("caseCount", ns);
            elemCaseCount.InnerText = string.Format("{0}", sol.CaseCount);
            elemSolution.AppendChild(elemCaseCount);
            // if case of boxes, add box count + box efficiency
            if (sol.Analysis.BProperties is CaseOfBoxesProperties)
            {
                CaseOfBoxesProperties caseOfBoxes = sol.Analysis.BProperties as CaseOfBoxesProperties;
                XmlElement elemBoxCount = xmlDoc.CreateElement("boxCount", ns);
                elemBoxCount.InnerText = string.Format("{0:F}", caseOfBoxes.NumberOfBoxes);
                elemSolution.AppendChild(elemBoxCount);
                XmlElement elemBoxEfficiency = xmlDoc.CreateElement("boxEfficiency", ns);
                elemBoxEfficiency.InnerText = string.Format("{0:F}", sol.VolumeEfficiencyCases);
            }
            // layerCount
            XmlElement elemLayerCount = xmlDoc.CreateElement("layerCount", ns);
            elemLayerCount.InnerText = string.Format("{0}", sol.CaseLayersCount);
            elemSolution.AppendChild(elemLayerCount);
            // layer1_caseCount / layer2_caseCount
            XmlElement elemLayer1_caseCount = xmlDoc.CreateElement("layer1_caseCount", ns);
            elemLayer1_caseCount.InnerText = string.Format("{0}", sol.CaseLayerFirst.BoxCount);
            elemSolution.AppendChild(elemLayer1_caseCount);
            if (sol.Count > 1 && (sol.CaseLayerFirst.BoxCount != sol.CaseLayerSecond.BoxCount))
            {
                XmlElement elemLayer2_caseCount = xmlDoc.CreateElement("layer2_caseCount", ns);
                elemLayer2_caseCount.InnerText = string.Format("{0}", sol.CaseLayerSecond.BoxCount);
                elemSolution.AppendChild(elemLayer2_caseCount);
            }
            // interlayer count
            if (sol.Analysis.ConstraintSet.HasInterlayer)
            {
                XmlElement elemInterlayerCount = xmlDoc.CreateElement("interlayerCount", ns);
                elemInterlayerCount.InnerText = string.Format("{0}", sol.InterlayerCount);
                elemSolution.AppendChild(elemInterlayerCount);
            }
            // --- layer images
            for (int i = 0; i < Math.Min(sol.Count, (sol.HasHomogeneousLayers ? 1 : 2)); ++i)
            {
                XmlElement elemLayer = xmlDoc.CreateElement("layer", ns);
                // layerId
                XmlElement xmlLayerId = xmlDoc.CreateElement("layerId", ns);
                xmlLayerId.InnerText = string.Format("{0}", i + 1);
                elemLayer.AppendChild(xmlLayerId);
                // --- build layer image
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
                // set camera position 
                graphics.CameraPosition = Graphics3D.Top;
                // instantiate solution viewer
                CasePalletSolutionViewer sv = new CasePalletSolutionViewer(sol);
                sv.DrawLayers(graphics, true, i /* layer index*/);
                // ---
                // layerImage
                XmlElement elemLayerImage = xmlDoc.CreateElement("layerImage", ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemLayerImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 2, graphics.Bitmap.Height / 2);
                elemLayerImage.Attributes.Append(styleAttribute);
                elemLayer.AppendChild(elemLayerImage);
                // layerCaseCount
                XmlElement elemLayerBoxCount = xmlDoc.CreateElement("layerCaseCount", ns);
                elemLayerBoxCount.InnerText = sol[i].BoxCount.ToString();
                elemLayer.AppendChild(elemLayerBoxCount);

                elemSolution.AppendChild(elemLayer);
                // save image
                SaveImageAs(graphics.Bitmap, string.Format("layerImage{0}.png", i + 1));
            }
            // --- pallet images
            for (int i = 0; i < 5; ++i)
            {
                // initialize drawing values
                string viewName = string.Empty;
                Vector3D cameraPos = Vector3D.Zero;
                int imageWidth = ImageSizeDetail;
                bool showDimensions = false;
                switch (i)
                {
                    case 0:
                        viewName = "view_palletsolution_front"; cameraPos = Graphics3D.Front; imageWidth = ImageSizeDetail;
                        break;
                    case 1:
                        viewName = "view_palletsolution_left"; cameraPos = Graphics3D.Left; imageWidth = ImageSizeDetail;
                        break;
                    case 2:
                        viewName = "view_palletsolution_right"; cameraPos = Graphics3D.Right; imageWidth = ImageSizeDetail;
                        break;
                    case 3:
                        viewName = "view_palletsolution_back"; cameraPos = Graphics3D.Back; imageWidth = ImageSizeDetail;
                        break;
                    case 4:
                        viewName = "view_palletsolution_iso"; cameraPos = Graphics3D.Corner_180; imageWidth = ImageSizeWide; showDimensions = true;
                        break;
                    default:
                        break;
                }
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(imageWidth, imageWidth));
                // set camera position 
                graphics.CameraPosition = cameraPos;
                // instantiate solution viewer
                CasePalletSolutionViewer sv = new CasePalletSolutionViewer(sol);
                sv.ShowDimensions = showDimensions;
                sv.Draw(graphics);
                graphics.Flush();
                SaveImageAs(graphics.Bitmap, viewName + ".png");
                // ---
                XmlElement elemImage = xmlDoc.CreateElement(viewName, ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 3, graphics.Bitmap.Height / 3);
                elemImage.Attributes.Append(styleAttribute);
                elemSolution.AppendChild(elemImage);
            }
        }

        private void AppendPackPalletSolutionElement(ReportData inputData, XmlElement elemPackPalletAnalysis, XmlDocument xmlDoc)
        {
            if (!inputData.IsPackPalletAnalysis) return;

            string ns = xmlDoc.DocumentElement.NamespaceURI;

            SelPackPalletSolution selSolution = inputData.SelPackPalletSolution;
            PackPalletSolution sol = inputData.PackPalletSolution;

            // solution
            XmlElement elemSolution = xmlDoc.CreateElement("packPalletSolution", ns);
            elemPackPalletAnalysis.AppendChild(elemSolution);
            // title
            XmlElement elemTitle = xmlDoc.CreateElement("title", ns);
            elemTitle.InnerText = sol.Title;
            elemSolution.AppendChild(elemTitle);
            // efficiency
            XmlElement elemEfficiency = xmlDoc.CreateElement("efficiency", ns);
            elemEfficiency.InnerText = string.Format( "{0:F}", sol.VolumeEfficiency );
            elemSolution.AppendChild(elemEfficiency);
            // length / width / height
            AppendElementValue(xmlDoc, elemSolution, "length", UnitsManager.UnitType.UT_LENGTH, sol.PalletLength);
            AppendElementValue(xmlDoc, elemSolution, "width", UnitsManager.UnitType.UT_LENGTH, sol.PalletWidth);
            AppendElementValue(xmlDoc, elemSolution, "height", UnitsManager.UnitType.UT_LENGTH, sol.PalletHeight);
            // counts
            AppendElementValue(xmlDoc, elemSolution, "palletPackCount", sol.PackCount);
            AppendElementValue(xmlDoc, elemSolution, "palletCSUCount", sol.CSUCount);
            AppendElementValue(xmlDoc, elemSolution, "palletInterlayerCount", sol.InterlayerCount);
            // 
            AppendElementValue(xmlDoc, elemSolution, "palletWeight", UnitsManager.UnitType.UT_MASS, sol.PalletWeight);
            AppendElementValue(xmlDoc, elemSolution, "palletLoadWeight", UnitsManager.UnitType.UT_MASS, sol.PalletLoadWeight);
            AppendElementValue(xmlDoc, elemSolution, "palletNetWeight", UnitsManager.UnitType.UT_MASS, sol.PalletNetWeight);
            AppendElementValue(xmlDoc, elemSolution, "overhangX", UnitsManager.UnitType.UT_LENGTH, sol.OverhangX);
            AppendElementValue(xmlDoc, elemSolution, "overhangY", UnitsManager.UnitType.UT_LENGTH, sol.OverhangY);
            // --- pallet images
            for (int i = 0; i < 5; ++i)
            {
                // initialize drawing values
                string viewName = string.Empty;
                Vector3D cameraPos = Vector3D.Zero;
                int imageWidth = ImageSizeDetail;
                bool showDimensions = false;
                switch (i)
                {
                    case 0: viewName = "view_palletsolution_front"; cameraPos = Graphics3D.Front; imageWidth = ImageSizeDetail; break;
                    case 1: viewName = "view_palletsolution_left"; cameraPos = Graphics3D.Left; imageWidth = ImageSizeDetail;   break;
                    case 2: viewName = "view_palletsolution_right"; cameraPos = Graphics3D.Right; imageWidth = ImageSizeDetail; break;
                    case 3: viewName = "view_palletsolution_back"; cameraPos = Graphics3D.Back; imageWidth = ImageSizeDetail;   break;
                    case 4: viewName = "view_palletsolution_iso"; cameraPos = Graphics3D.Corner_180; imageWidth = ImageSizeWide; showDimensions = true; break;
                    default:  break;
                }
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(imageWidth, imageWidth));
                // set camera position 
                graphics.CameraPosition = cameraPos;
                // instantiate solution viewer
                PackPalletSolutionViewer sv = new PackPalletSolutionViewer(sol);
                sv.ShowDimensions = showDimensions;
                sv.Draw(graphics);
                graphics.Flush();
                SaveImageAs(graphics.Bitmap, viewName + ".png");
                // ---
                XmlElement elemImage = xmlDoc.CreateElement(viewName, ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 3, graphics.Bitmap.Height / 3);
                elemImage.Attributes.Append(styleAttribute);
                elemSolution.AppendChild(elemImage);
            }

            // --- layers
            for (int i=0; i<sol.NoLayerTypes; ++i)
            {
                XmlElement elemLayerPack = xmlDoc.CreateElement("layerPack", ns);
                elemSolution.AppendChild(elemLayerPack);

                LayerType layerType = sol.GetLayerType(i);
                AppendElementValue(xmlDoc, elemLayerPack, "layerPackCount", layerType.PackCount);
                AppendElementValue(xmlDoc, elemLayerPack, "layerCSUCount", layerType.CSUCount);
                AppendElementValue(xmlDoc, elemLayerPack, "layerWeight", UnitsManager.UnitType.UT_MASS, layerType.LayerWeight);
                AppendElementValue(xmlDoc, elemLayerPack, "layerNetWeight", UnitsManager.UnitType.UT_MASS, layerType.LayerNetWeight);
                AppendElementValue(xmlDoc, elemLayerPack, "layerLength", UnitsManager.UnitType.UT_LENGTH, layerType.Length);
                AppendElementValue(xmlDoc, elemLayerPack, "layerWidth", UnitsManager.UnitType.UT_LENGTH, layerType.Width);
                AppendElementValue(xmlDoc, elemLayerPack, "layerHeight", UnitsManager.UnitType.UT_LENGTH, layerType.Height);
                AppendElementValue(xmlDoc, elemLayerPack, "maximumSpace", UnitsManager.UnitType.UT_LENGTH, layerType.MaximumSpace);
                AppendElementValue(xmlDoc, elemLayerPack, "layerIndexes", layerType.LayerIndexes);

                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
                // set camera position 
                graphics.CameraPosition = Graphics3D.Corner_180;
                // instantiate solution viewer
                PackPalletSolutionViewer sv = new PackPalletSolutionViewer(sol);
                sv.ShowDimensions = true;
                sv.DrawLayer(graphics, i);
                graphics.Flush();
                string viewName = string.Format("view_layer_iso{0}", i);
                SaveImageAs(graphics.Bitmap, viewName + ".png");
                // ---
                XmlElement elemImage = xmlDoc.CreateElement("imagePackLayer", ns);
                elemImage.InnerText = "images\\" + viewName + ".png";
                elemLayerPack.AppendChild(elemImage);
            }
        }

        private void AppendCylinderPalletSolutionElement(ReportData inputData, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            if (!inputData.IsCylinderPalletAnalysis) return;

            string ns = xmlDoc.DocumentElement.NamespaceURI;

            SelCylinderPalletSolution selSolution = inputData.SelCylinderPalletSolution;
            CylinderPalletSolution sol = inputData.CylinderPalletSolution;

            // solution
            XmlElement elemSolution = xmlDoc.CreateElement("palletSolution", ns);
            elemPalletAnalysis.AppendChild(elemSolution);
            // title
            XmlElement elemTitle = xmlDoc.CreateElement("title", ns);
            elemTitle.InnerText = sol.Title;
            elemSolution.AppendChild(elemTitle);
            // efficiency
            XmlElement elemEfficiency = xmlDoc.CreateElement("efficiency", ns);
            elemEfficiency.InnerText = string.Format("{0:F}", sol.VolumeEfficiency);
            elemSolution.AppendChild(elemEfficiency);

            AppendElementValue(xmlDoc, elemSolution, "palletWeight", UnitsManager.UnitType.UT_MASS, inputData.ActualPalletWeight);
            AppendElementValue(xmlDoc, elemSolution, "palletHeight", UnitsManager.UnitType.UT_LENGTH, sol.PalletHeight);

            // cylinderCount
            XmlElement elemCaseCount = xmlDoc.CreateElement("cylinderCount", ns);
            elemCaseCount.InnerText = string.Format("{0}", sol.CylinderCount);
            elemSolution.AppendChild(elemCaseCount);
            // layerCount
            XmlElement elemLayerCount = xmlDoc.CreateElement("layerCount", ns);
            elemCaseCount.InnerText = string.Format("{0}", sol.CylinderLayersCount);
            elemSolution.AppendChild(elemCaseCount);
            // interlayer count
            if (sol.Analysis.ConstraintSet.HasInterlayer)
            {
                XmlElement elemInterlayerCount = xmlDoc.CreateElement("interlayerCount", ns);
                elemInterlayerCount.InnerText = string.Format("{0}", sol.InterlayerCount);
                elemSolution.AppendChild(elemInterlayerCount);
            }
            // layer (single)
            {
                // --- layer image
                XmlElement elemLayer = xmlDoc.CreateElement("layer", ns);
                // layerId
                XmlElement xmlLayerId = xmlDoc.CreateElement("layerId", ns);
                xmlLayerId.InnerText = string.Format("{0}", 1);
                elemLayer.AppendChild(xmlLayerId);
                // --- build layer image
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
                // set camera position 
                graphics.CameraPosition = Graphics3D.Top;
                // instantiate solution viewer
                CylinderPalletSolutionViewer sv = new CylinderPalletSolutionViewer(sol);
                sv.DrawLayers(graphics, true, 0 /* layer index*/);
                // ---
                // layerImage
                XmlElement elemLayerImage = xmlDoc.CreateElement("layerImage", ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemLayerImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 2, graphics.Bitmap.Height / 2);
                elemLayerImage.Attributes.Append(styleAttribute);
                elemLayer.AppendChild(elemLayerImage);
                // layerCylinderCount
                XmlElement elemLayerBoxCount = xmlDoc.CreateElement("layerCylinderCount", ns);
                elemLayerBoxCount.InnerText = string.Format("{0}", sol.CylinderPerLayerCount);
                elemLayer.AppendChild(elemLayerBoxCount);
                elemSolution.AppendChild(elemLayer);
                // save image
                SaveImageAs(graphics.Bitmap, string.Format("layerImage{0}.png", 1));
            }
            // --- pallet images
            for (int i = 0; i < 5; ++i)
            {
                // initialize drawing values
                string viewName = string.Empty;
                Vector3D cameraPos = Vector3D.Zero;
                int imageWidth = ImageSizeDetail;
                bool showDimensions = false;
                switch (i)
                {
                    case 0:
                        viewName = "view_palletsolution_front"; cameraPos = Graphics3D.Front; imageWidth = ImageSizeDetail;
                        break;
                    case 1:
                        viewName = "view_palletsolution_left"; cameraPos = Graphics3D.Left; imageWidth = ImageSizeDetail;
                        break;
                    case 2:
                        viewName = "view_palletsolution_right"; cameraPos = Graphics3D.Right; imageWidth = ImageSizeDetail;
                        break;
                    case 3:
                        viewName = "view_palletsolution_back"; cameraPos = Graphics3D.Back; imageWidth = ImageSizeDetail;
                        break;
                    case 4:
                        viewName = "view_palletsolution_iso"; cameraPos = Graphics3D.Corner_0; imageWidth = ImageSizeWide; showDimensions = true;
                        break;
                    default:
                        break;
                }
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(imageWidth, imageWidth));
                // set camera position 
                graphics.CameraPosition = cameraPos;
                // instantiate solution viewer
                CylinderPalletSolutionViewer sv = new CylinderPalletSolutionViewer(sol);
                sv.ShowDimensions = showDimensions;
                sv.Draw(graphics);
                // ---
                XmlElement elemImage = xmlDoc.CreateElement(viewName, ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 3, graphics.Bitmap.Height / 3);
                elemImage.Attributes.Append(styleAttribute);
                elemSolution.AppendChild(elemImage);
                // Save image ?
                SaveImageAs(graphics.Bitmap, viewName + ".png");
            }
        }

        private void AppendHCylinderPalletSolutionElement(ReportData inputData, XmlElement elemPalletAnalysis, XmlDocument xmlDoc)
        {
            if (!inputData.IsHCylinderPalletAnalysis) return;

            string ns = xmlDoc.DocumentElement.NamespaceURI;

            SelHCylinderPalletSolution selSolution = inputData.SelHCylinderPalletSolution;
            HCylinderPalletSolution sol = inputData.HCylinderPalletSolution;

            // solution
            XmlElement elemSolution = xmlDoc.CreateElement("palletSolution", ns);
            elemPalletAnalysis.AppendChild(elemSolution);
            // title
            XmlElement elemTitle = xmlDoc.CreateElement("title", ns);
            elemTitle.InnerText = sol.Title;
            elemSolution.AppendChild(elemTitle);

            AppendElementValue(xmlDoc, elemSolution, "palletWeight", UnitsManager.UnitType.UT_MASS, inputData.ActualPalletWeight);
            AppendElementValue(xmlDoc, elemSolution, "palletHeight", UnitsManager.UnitType.UT_LENGTH, sol.PalletHeight);

            // cylinderCount
            XmlElement elemCaseCount = xmlDoc.CreateElement("cylinderCount", ns);
            elemCaseCount.InnerText = string.Format("{0}", sol.CylinderCount);
            elemSolution.AppendChild(elemCaseCount);
            // --- pallet images
            for (int i = 0; i < 5; ++i)
            {
                // initialize drawing values
                string viewName = string.Empty;
                Vector3D cameraPos = Vector3D.Zero;
                int imageWidth = ImageSizeDetail;
                bool showDimensions = false;
                switch (i)
                {
                    case 0:
                        viewName = "view_palletsolution_front"; cameraPos = Graphics3D.Front; imageWidth = ImageSizeDetail;
                        break;
                    case 1:
                        viewName = "view_palletsolution_left"; cameraPos = Graphics3D.Left; imageWidth = ImageSizeDetail;
                        break;
                    case 2:
                        viewName = "view_palletsolution_right"; cameraPos = Graphics3D.Right; imageWidth = ImageSizeDetail;
                        break;
                    case 3:
                        viewName = "view_palletsolution_back"; cameraPos = Graphics3D.Back; imageWidth = ImageSizeDetail;
                        break;
                    case 4:
                        viewName = "view_palletsolution_iso"; cameraPos = Graphics3D.Corner_0; imageWidth = ImageSizeWide; showDimensions = true;
                        break;
                    default:
                        break;
                }
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(imageWidth, imageWidth));
                // set camera position 
                graphics.CameraPosition = cameraPos;
                // instantiate solution viewer
                HCylinderPalletSolutionViewer sv = new HCylinderPalletSolutionViewer(sol);
                sv.ShowDimensions = showDimensions;
                sv.Draw(graphics);
                // ---
                XmlElement elemImage = xmlDoc.CreateElement(viewName, ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 3, graphics.Bitmap.Height / 3);
                elemImage.Attributes.Append(styleAttribute);
                elemSolution.AppendChild(elemImage);
                // Save image ?
                SaveImageAs(graphics.Bitmap, viewName + ".png");
            }
        }

        private void AppendTruckAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            if (!inputData.IsCasePalletAnalysis) return;
            CasePalletAnalysis analysis = inputData.CasePalletAnalysis;
            SelCasePalletSolution selSolution = inputData.SelSolution;

            // retrieve truck analysis if any
            if (!selSolution.HasTruckAnalyses) return;  // no truck analysis available -> exit

            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // truckAnalysis
            XmlElement elemTruckAnalysis = xmlDoc.CreateElement("truckAnalysis", ns);
            elemDocument.AppendChild(elemTruckAnalysis);

            TruckAnalysis truckAnalysis = selSolution.TruckAnalyses[0];
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = analysis.Name;
            elemTruckAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = analysis.Description;
            elemTruckAnalysis.AppendChild(elemDescription);
            // truck
            AppendTruckElement(truckAnalysis, elemTruckAnalysis, xmlDoc);
            // solution
            AppendTruckSolutionElement(inputData, elemTruckAnalysis, xmlDoc);
        }

        private void AppendEctAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            if (!inputData.IsCasePalletAnalysis)
                return;
            CasePalletAnalysis analysis = inputData.CasePalletAnalysis;
            SelCasePalletSolution selSolution = inputData.SelSolution;

            // retrieve ect analysis if any
            if (!selSolution.HasECTAnalyses) return;

            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // ectAnalysis
            XmlElement elemEctAnalysis = xmlDoc.CreateElement("ectAnalysis", ns);
            elemDocument.AppendChild(elemEctAnalysis);

            ECTAnalysis ectAnalysis = selSolution.EctAnalyses[0];
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = ectAnalysis.Name;
            elemEctAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = ectAnalysis.Description;
            elemEctAnalysis.AppendChild(elemDescription);
             // cardboard
            XmlElement elemCardboard = xmlDoc.CreateElement("cardboard", ns);
            elemEctAnalysis.AppendChild(elemCardboard);
            XmlElement elemCardboardName = xmlDoc.CreateElement("name", ns);
            elemCardboardName.InnerText = ectAnalysis.Cardboard.Name;
            elemCardboard.AppendChild(elemCardboardName);

            AppendElementValue(xmlDoc, elemCardboard, "thickness", UnitsManager.UnitType.UT_LENGTH, ectAnalysis.Cardboard.Thickness);

            XmlElement elemCardboadECT = xmlDoc.CreateElement("ect", ns);
            elemCardboadECT.InnerText = string.Format("{0:0.00}", ectAnalysis.Cardboard.ECT);
            elemCardboard.AppendChild(elemCardboadECT);
            XmlElement elemCardboardStiffnessX = xmlDoc.CreateElement("stiffnessX", ns);
            elemCardboardStiffnessX.InnerText = string.Format("{0:0.00}", ectAnalysis.Cardboard.RigidityDX);
            elemCardboard.AppendChild(elemCardboardStiffnessX);
            XmlElement elemCardboardStiffnessY = xmlDoc.CreateElement("stiffnessY", ns);
            elemCardboardStiffnessY.InnerText = string.Format("{0:0.00}", ectAnalysis.Cardboard.RigidityDY);
            elemCardboard.AppendChild(elemCardboardStiffnessY);
            // case type
            XmlElement elemCaseType = xmlDoc.CreateElement("caseType", ns);
            elemCaseType.InnerText = ectAnalysis.CaseType;
            elemEctAnalysis.AppendChild(elemCaseType);
            // printed surface
            XmlElement elemPrintedSurface = xmlDoc.CreateElement("printedSurface", ns);
            elemPrintedSurface.InnerText = ectAnalysis.PrintSurface;
            elemEctAnalysis.AppendChild(elemPrintedSurface);
            // mc kee formula mode
            XmlElement elemMcKeeFormula = xmlDoc.CreateElement("mcKeeFormulaMode", ns);
            elemMcKeeFormula.InnerText = ectAnalysis.McKeeFormulaText;
            elemEctAnalysis.AppendChild(elemMcKeeFormula);
            // bct_static
            XmlElement elemStaticBCT = xmlDoc.CreateElement("bct_static", ns);
            elemEctAnalysis.AppendChild(elemStaticBCT);
            XmlElement elemStaticValue = xmlDoc.CreateElement("static_value", ns);
            elemStaticValue.InnerText = string.Format("{0:0.00}", ectAnalysis.StaticBCT);
            elemStaticBCT.AppendChild(elemStaticValue);
            // bct_dynamic
            XmlElement elemDynamicBCT = xmlDoc.CreateElement("bct_dynamic", ns);
            elemEctAnalysis.AppendChild(elemDynamicBCT);
            Dictionary<KeyValuePair<string, string>, double> ectDictionary = ectAnalysis.DynamicBCTDictionary;
            foreach (string storageKey in treeDiM.EdgeCrushTest.McKeeFormula.StockCoefDictionary.Keys)
            {
                XmlElement elemBCTStorage = xmlDoc.CreateElement("bct_dynamic_storage", ns);
                elemDynamicBCT.AppendChild(elemBCTStorage);
                // duration
                XmlElement elemStorageDuration = xmlDoc.CreateElement("duration", ns);
                elemStorageDuration.InnerText = storageKey;
                elemBCTStorage.AppendChild(elemStorageDuration);
                // humidity rate -> values
                string[] elementHumidityNames
                    = {
                      "humidity_0_45"
                      , "humidity_46_55"
                      , "humidity_56_65"
                      , "humidity_66_75"
                      , "humidity_76_85"
                      , "humidity_86_100"
                   };
                int indexHumidity = 0;
                foreach (string humidityKey in treeDiM.EdgeCrushTest.McKeeFormula.HumidityCoefDictionary.Keys)
                {
                    // get value of ect for "storage time" + "humidity"
                    double ectValue = ectDictionary[new KeyValuePair<string, string>(storageKey, humidityKey)];
                    XmlElement elemHumidity = xmlDoc.CreateElement(elementHumidityNames[indexHumidity++], ns);
                    elemHumidity.InnerText = string.Format("{0:0.00}", ectValue);
                    elemBCTStorage.AppendChild(elemHumidity);

                    // attribute stating if value is correct or below admissible value
                    XmlAttribute attributeAdmissible = xmlDoc.CreateAttribute("admissible");
                    attributeAdmissible.Value = selSolution.Solution.AverageLoadOnFirstLayerCase < ectValue ? "true" : "false";
                    elemHumidity.Attributes.Append(attributeAdmissible);
                } 
            }
        }

        private void AppendTruckElement(TruckAnalysis truckAnalysis, XmlElement elemTruckAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // get PalletProperties
            TruckProperties truckProp = truckAnalysis.TruckProperties;
            if (null == truckProp) return;
            // create "truck" element
            XmlElement elemTruck = xmlDoc.CreateElement("truck", ns);
            elemTruckAnalysis.AppendChild(elemTruck);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = truckProp.Name;
            elemTruck.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = truckProp.Description;
            elemTruck.AppendChild(elemDescription);

            AppendElementValue(xmlDoc, elemTruck, "length", UnitsManager.UnitType.UT_LENGTH, truckProp.Length);
            AppendElementValue(xmlDoc, elemTruck, "width", UnitsManager.UnitType.UT_LENGTH, truckProp.Width);
            AppendElementValue(xmlDoc, elemTruck, "height", UnitsManager.UnitType.UT_LENGTH, truckProp.Height);
            AppendElementValue(xmlDoc, elemTruck, "admissibleLoad", UnitsManager.UnitType.UT_MASS, truckProp.AdmissibleLoadWeight);

            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            Truck truck = new Truck(truckProp);
            truck.DrawBegin(graphics);
            truck.DrawEnd(graphics);
            DimensionCube dc = new DimensionCube(truckProp.Length, truckProp.Width, truckProp.Height);      dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // ---
            // view_truck_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_truck_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemTruck.AppendChild(elemImage);
            // Save image ?
            SaveImageAs(graphics.Bitmap, "view_truck_iso.png");
        }
        private void AppendTruckSolutionElement(ReportData inputData, XmlElement elemTruckAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;

            CasePalletSolution palletSolution = inputData.CasePalletSolution;
            TruckAnalysis truckAnalysis = inputData.SelSolution.TruckAnalyses[0];

            // retrieve selected truckSolution
            TruckSolution truckSolution = truckAnalysis.SelectedSolution;
            if (null == truckSolution) return;
            // create "truckSolution" element
            XmlElement elemTruckSolution = xmlDoc.CreateElement("truckSolution", ns);
            elemTruckAnalysis.AppendChild(elemTruckSolution);
            if (!string.IsNullOrEmpty(truckSolution.Title))
            {
                // title
                XmlElement elemTitle = xmlDoc.CreateElement("title", ns);
                elemTitle.InnerText = truckSolution.Title;
                elemTruckSolution.AppendChild(elemTitle);
            }
            // palletCount
            XmlElement elemPalletCount = xmlDoc.CreateElement("palletCount", ns);
            elemPalletCount.InnerText = string.Format("{0}", truckSolution.PalletCount);
            elemTruckSolution.AppendChild(elemPalletCount);
            // boxCount
            XmlElement elemBoxCount = xmlDoc.CreateElement("caseCount", ns);
            elemBoxCount.InnerText = string.Format("{0}", truckSolution.BoxCount);
            elemTruckSolution.AppendChild(elemBoxCount);

            double loadWeight = truckSolution.PalletCount * inputData.ActualPalletWeight;
            AppendElementValue(xmlDoc, elemTruckSolution, "loadWeight", UnitsManager.UnitType.UT_MASS, loadWeight);

            // loadEfficiency
            XmlElement elemLoadEfficiency = xmlDoc.CreateElement("loadEfficiency", ns);
            elemLoadEfficiency.InnerText = string.Format("{0:F}", 100.0 * loadWeight / truckAnalysis.TruckProperties.AdmissibleLoadWeight);
            elemTruckSolution.AppendChild(elemLoadEfficiency);
            // volumeEfficiency
            XmlElement elemVolumeEfficiency = xmlDoc.CreateElement("volumeEfficiency", ns);
            elemVolumeEfficiency.InnerText = string.Format("{0:F}", truckSolution.Efficiency);
            elemTruckSolution.AppendChild(elemVolumeEfficiency);

            // --- truck images
            for (int i = 0; i < 2; ++i)
            {
                // initialize drawing values
                string viewName = string.Empty;
                Vector3D cameraPos = Vector3D.Zero;
                int imageWidth = ImageSizeWide;
                switch (i)
                {
                    case 0: viewName = "view_trucksolution_top"; cameraPos = Graphics3D.Top; imageWidth = ImageSizeWide; break;
                    case 1: viewName = "view_trucksolution_iso"; cameraPos = Graphics3D.Corner_0; imageWidth = ImageSizeWide; break;
                    default: break;
                }
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(imageWidth, imageWidth));
                // set camera position 
                graphics.CameraPosition = cameraPos;
                // dimensions
                if (1 == i)
                {
                    TruckProperties truckProp = truckSolution.ParentTruckAnalysis.TruckProperties;
                    graphics.AddDimensions(new DimensionCube(truckSolution.LoadBoundingBox, Color.Red, false));
                    graphics.AddDimensions(new DimensionCube(Vector3D.Zero, truckProp.Length, truckProp.Width, truckProp.Height, Color.Black, true));
                }
                // instantiate solution viewer
                TruckSolutionViewer sv = new TruckSolutionViewer(truckSolution);
                sv.Draw(graphics);
                graphics.Flush();
                // ---
                XmlElement elemImage = xmlDoc.CreateElement(viewName, ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 3, graphics.Bitmap.Height / 3);
                elemImage.Attributes.Append(styleAttribute);
                elemTruckSolution.AppendChild(elemImage);
                // Save image ?
                SaveImageAs(graphics.Bitmap, viewName + ".png");
            }
        }
        #endregion

        #region Case analysis
        private void AppendCaseAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            // check if case analysis
            if (!inputData.IsBoxCasePalletAnalysis)
                return;
            BoxCasePalletAnalysis caseAnalysis = inputData.CaseAnalysis;
            SelBoxCasePalletSolution selSolution = inputData.SelCaseSolution;

            if (null == selSolution.Solution.PalletSolutionDesc.LoadPalletSolution())
                return;
            
            // namespace
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // caseAnalysis
            XmlElement elemCaseAnalysis = xmlDoc.CreateElement("boxCasePalletAnalysis", ns);
            elemDocument.AppendChild(elemCaseAnalysis);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = caseAnalysis.Name;
            elemCaseAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = caseAnalysis.Description;
            elemCaseAnalysis.AppendChild(elemDescription);
            // box
            AppendBoxElement(caseAnalysis.BoxProperties, elemCaseAnalysis, xmlDoc);
            // case
            AppendCaseElement(selSolution, elemCaseAnalysis, xmlDoc);
            // constraint set
            AppendCaseConstraintSet(caseAnalysis, elemCaseAnalysis, xmlDoc);
            // case solution
            AppendCaseSolutionElement(selSolution, elemCaseAnalysis, xmlDoc);
        }
        #endregion

        #region BoxCaseAnalysis
        private void AppendBoxCaseAnalysisElement(ReportData inputData, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            // check if case analysis
            if (!inputData.IsBoxCaseAnalysis)
                return;
            BoxCaseAnalysis boxCaseAnalysis = inputData.BoxCaseAnalysis;
            SelBoxCaseSolution selBoxCaseSolution = inputData.SelBoxCaseSolution;
            // namespace
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // caseAnalysis
            XmlElement elemBoxCaseAnalysis = xmlDoc.CreateElement("boxCaseAnalysis", ns);
            elemDocument.AppendChild(elemBoxCaseAnalysis);
            // name
            XmlElement elemName = xmlDoc.CreateElement("name", ns);
            elemName.InnerText = boxCaseAnalysis.Name;
            elemBoxCaseAnalysis.AppendChild(elemName);
            // description
            XmlElement elemDescription = xmlDoc.CreateElement("description", ns);
            elemDescription.InnerText = boxCaseAnalysis.Description;
            elemBoxCaseAnalysis.AppendChild(elemDescription);
            // box
            if (boxCaseAnalysis.BProperties is BoxProperties)
                AppendBoxElement(boxCaseAnalysis.BProperties as BoxProperties, elemBoxCaseAnalysis, xmlDoc);
            else if (boxCaseAnalysis.BProperties is BundleProperties)
                AppendBundleElement(boxCaseAnalysis.BProperties as BundleProperties, elemBoxCaseAnalysis, xmlDoc);
            // case
            AppendCaseElement(boxCaseAnalysis.CaseProperties, elemBoxCaseAnalysis, xmlDoc);
            // constraint set
            AppendBoxCaseConstraintSet(boxCaseAnalysis.ConstraintSet, elemBoxCaseAnalysis, xmlDoc);
            // solution
            AppendBoxCaseSolutionElement(selBoxCaseSolution.Solution, elemBoxCaseAnalysis, xmlDoc);
        }
        #endregion

        #region BoxProperties / PackProperties
        private void AppendCaseElement(BoxProperties caseProperties, XmlElement elemCaseAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // case element
            XmlElement elemCase = CreateElement("caseWithInnerDims", null, elemCaseAnalysis, xmlDoc, ns);
            // name
            CreateElement("name", caseProperties.Name, elemCase, xmlDoc, ns);
            // description
            CreateElement("description", caseProperties.Description, elemCase, xmlDoc, ns);
            // unit width
            // length / width /height
            AppendElementValue(xmlDoc, elemCase, "length", UnitsManager.UnitType.UT_LENGTH, caseProperties.Length);
            AppendElementValue(xmlDoc, elemCase, "width", UnitsManager.UnitType.UT_LENGTH, caseProperties.Width);
            AppendElementValue(xmlDoc, elemCase, "height", UnitsManager.UnitType.UT_LENGTH, caseProperties.Height);
            // innerLength / innerWidth / innerHeight
            AppendElementValue(xmlDoc, elemCase, "innerLength", UnitsManager.UnitType.UT_LENGTH, caseProperties.InsideLength);
            AppendElementValue(xmlDoc, elemCase, "innerWidth", UnitsManager.UnitType.UT_LENGTH, caseProperties.InsideWidth);
            AppendElementValue(xmlDoc, elemCase, "innerHeight", UnitsManager.UnitType.UT_LENGTH, caseProperties.InsideHeight);
            // weight
            AppendElementValue(xmlDoc, elemCase, "weight", UnitsManager.UnitType.UT_MASS, caseProperties.Weight);
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Box box = new Box(0, caseProperties);
            graphics.AddBox(box);
            DimensionCube dc = new DimensionCube(box.Length, box.Width, box.Height);        dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // ---
            // view_case_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_case_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemCase.AppendChild(elemImage);
            // save image
            SaveImageAs(graphics.Bitmap, "view_case_iso.png");
        }
        private void AppendPackElement(PackProperties packProperties, XmlElement elemPackAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // pack element
            XmlElement elemPack = CreateElement("pack", null, elemPackAnalysis, xmlDoc, ns);
            // name
            CreateElement("name", packProperties.Name, elemPack, xmlDoc, ns);
            // description
            CreateElement("description", packProperties.Description, elemPack, xmlDoc, ns);
            // arrangement
            PackArrangement arrangement = packProperties.Arrangement;
            CreateElement(
                "arrangement"
                , string.Format("{0} * {1} * {2}", arrangement.Length, arrangement.Width, arrangement.Height)
                , elemPack, xmlDoc, ns);
            // length / width /height
            AppendElementValue(xmlDoc, elemPack, "length", UnitsManager.UnitType.UT_LENGTH, packProperties.Length);
            AppendElementValue(xmlDoc, elemPack, "width", UnitsManager.UnitType.UT_LENGTH, packProperties.Width);
            AppendElementValue(xmlDoc, elemPack, "height", UnitsManager.UnitType.UT_LENGTH, packProperties.Height);
            // weight
            AppendElementValue(xmlDoc, elemPack, "netWeight", UnitsManager.UnitType.UT_MASS, packProperties.NetWeight);
            AppendElementValue(xmlDoc, elemPack, "wrapperWeight", UnitsManager.UnitType.UT_MASS, packProperties.Wrap.Weight);
            AppendElementValue(xmlDoc, elemPack, "weight", UnitsManager.UnitType.UT_MASS, packProperties.Weight);
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Pack pack = new Pack(0, packProperties);
            pack.ForceTransparency = true;
            graphics.AddBox(pack);
            DimensionCube dc = new DimensionCube(pack.Length, pack.Width, pack.Height); dc.FontSize = 6.0f;
            graphics.AddDimensions(dc);
            graphics.Flush();
            // view_pack_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_pack_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemPack.AppendChild(elemImage);
            // save image
            SaveImageAs(graphics.Bitmap, "view_pack_iso.png");
        }

        private void AppendBoxElement(BoxProperties boxProperties, XmlElement elemCaseAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // box element
            XmlElement elemBox = CreateElement("box", null, elemCaseAnalysis, xmlDoc, ns);
            // name
            CreateElement("name", boxProperties.Name, elemBox, xmlDoc, ns);
            // description
            CreateElement("description", boxProperties.Description, elemBox, xmlDoc, ns);

            AppendElementValue(xmlDoc, elemBox, "length", UnitsManager.UnitType.UT_LENGTH, boxProperties.Length);
            AppendElementValue(xmlDoc, elemBox, "width", UnitsManager.UnitType.UT_LENGTH, boxProperties.Width);
            AppendElementValue(xmlDoc, elemBox, "height", UnitsManager.UnitType.UT_LENGTH, boxProperties.Height);
            AppendElementValue(xmlDoc, elemBox, "weight", UnitsManager.UnitType.UT_MASS, boxProperties.Weight);
            // view_box_iso
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Box box = new Box(0, boxProperties);
            graphics.AddBox(box);
            graphics.AddDimensions(new DimensionCube(box.Length, box.Width, box.Height));
            graphics.Flush();
            // ---
            // view_box_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_box_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemBox.AppendChild(elemImage);
            // save image ?
            SaveImageAs(graphics.Bitmap, "view_box_iso.png");
        }
        
        private void AppendCaseElement(SelBoxCasePalletSolution caseSolution, XmlElement elemCaseAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            BoxProperties boxProp = caseSolution.Solution.AttachedPalletSolution.Analysis.BProperties as BoxProperties;
            // case element
            XmlElement elemCase = CreateElement("caseWithInnerDims", null, elemCaseAnalysis, xmlDoc, ns);
            // name
            CreateElement("name", boxProp.Name, elemCase, xmlDoc, ns);
            // description
            CreateElement("description", boxProp.Description, elemCase, xmlDoc, ns);

            AppendElementValue(xmlDoc, elemCase, "length", UnitsManager.UnitType.UT_LENGTH, boxProp.Length);
            AppendElementValue(xmlDoc, elemCase, "width", UnitsManager.UnitType.UT_LENGTH, boxProp.Width);
            AppendElementValue(xmlDoc, elemCase, "height", UnitsManager.UnitType.UT_LENGTH, boxProp.Height);
            AppendElementValue(xmlDoc, elemCase, "innerLength", UnitsManager.UnitType.UT_LENGTH, boxProp.InsideLength);
            AppendElementValue(xmlDoc, elemCase, "innerWidth", UnitsManager.UnitType.UT_LENGTH, boxProp.InsideWidth);
            AppendElementValue(xmlDoc, elemCase, "innerHeight", UnitsManager.UnitType.UT_LENGTH, boxProp.InsideHeight);
            AppendElementValue(xmlDoc, elemCase, "weight", UnitsManager.UnitType.UT_MASS, boxProp.Height);
 
            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(ImageSizeDetail, ImageSizeDetail));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            Box box = new Box(0, boxProp);
            graphics.AddBox(box);
            graphics.AddDimensions(new DimensionCube(box.Length, box.Width, box.Height));
            graphics.Flush();
            // ---
            // view_case_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_case_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemCase.AppendChild(elemImage);
            // save image
            SaveImageAs(graphics.Bitmap, "view_case_iso.png");
        }
        #endregion

        #region Constraint sets : Box/Case
        private void AppendBoxCaseConstraintSet(BCaseConstraintSet bCaseConstraintSet, XmlElement elemDocument, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // caseConstraintSet element
            XmlElement elemCaseConstraintSet = null;
            if (bCaseConstraintSet is BoxCaseConstraintSet)
            {
                elemCaseConstraintSet = CreateElement("boxCaseConstraintSet", null, elemDocument, xmlDoc, ns);
                // down cast
                BoxCaseConstraintSet boxCaseConstraintSet = bCaseConstraintSet as BoxCaseConstraintSet;
                // allowedOrthoAxis
                CreateElement("allowedOrthoAxis", boxCaseConstraintSet.AllowOrthoAxisString, elemCaseConstraintSet, xmlDoc, ns);
            }
            else if (bCaseConstraintSet is BundleCaseConstraintSet)
            {
                elemCaseConstraintSet = CreateElement("bundleCaseConstraintSet", null, elemDocument, xmlDoc, ns);
            }
            // maximumCaseWeightGroup
            if (bCaseConstraintSet.UseMaximumCaseWeight)
            {
                XmlElement maximumCaseWeightGroup = CreateElement("maximumCaseWeightGroup", null, elemCaseConstraintSet, xmlDoc, ns);
                AppendElementValue(xmlDoc, maximumCaseWeightGroup, "maximumCaseWeight", UnitsManager.UnitType.UT_MASS, bCaseConstraintSet.MaximumCaseWeight);
            }
            // minimumBoxPerCaseGroup
            if (bCaseConstraintSet.UseMaximumNumberOfBoxes)
            {
                XmlElement maximumBoxPerCaseGroup = CreateElement("MaximumNumberOfBoxesGroup", null, elemCaseConstraintSet, xmlDoc, ns);
                CreateElement("maximumBoxPerCase", bCaseConstraintSet.MaximumNumberOfBoxes, maximumBoxPerCaseGroup, xmlDoc, ns);
            }
        }

        private void AppendBoxCaseSolutionElement(BoxCaseSolution solution, XmlElement elemBoxCaseAnalysis, XmlDocument xmlDoc)
        {
            BoxCaseAnalysis analysis = solution.Analysis;
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            // solution
            XmlElement elemSolution = xmlDoc.CreateElement("boxCaseSolution", ns);
            elemBoxCaseAnalysis.AppendChild(elemSolution);
            // title
            XmlElement elemTitle = xmlDoc.CreateElement("title", ns);
            elemTitle.InnerText = solution.Title;
            elemSolution.AppendChild(elemTitle);
            // boxPerLayerCount
            XmlElement elemBoxPerLayerCount = xmlDoc.CreateElement("boxPerLayerCount", ns);
            elemBoxPerLayerCount.InnerText = solution.BoxPerLayerCount.ToString();
            elemSolution.AppendChild(elemBoxPerLayerCount);
            // boxPerCaseCount
            XmlElement elemBoxPerCaseCount = xmlDoc.CreateElement("boxPerCaseCount", ns);
            elemBoxPerCaseCount.InnerText = solution.BoxPerCaseCount.ToString();
            elemSolution.AppendChild(elemBoxPerCaseCount);
            // BoxLayersCount
            XmlElement elemBoxLayersCount = xmlDoc.CreateElement("boxLayersCount", ns);
            elemBoxLayersCount.InnerText = solution.BoxLayersCount.ToString();
            elemSolution.AppendChild(elemBoxLayersCount);
            // criterions
            // load weight
            AppendElementValue(xmlDoc, elemSolution, "LoadWeight", UnitsManager.UnitType.UT_MASS, solution.BoxPerCaseCount * analysis.BProperties.Weight);
            // case weight
            AppendElementValue(xmlDoc, elemSolution, "CaseWeight", UnitsManager.UnitType.UT_MASS, solution.CaseWeight);
            // EfficiencyWeight
            XmlElement elemEfficiencyWeight = xmlDoc.CreateElement("EfficiencyWeight", ns);
            if (solution.CaseWeight > 0)
                elemEfficiencyWeight.InnerText = string.Format("{0:F}", solution.EfficiencyWeight);
            else
                elemEfficiencyWeight.InnerText = string.Empty;
            elemSolution.AppendChild(elemEfficiencyWeight);
            // EfficiencyVolume
            XmlElement elemEfficiencyVolume = xmlDoc.CreateElement("EfficiencyVolume", ns);
            elemEfficiencyVolume.InnerText = string.Format("{0:F}", solution.EfficiencyVolume);
            elemSolution.AppendChild(elemEfficiencyVolume);
            // LimitReached
            string limitReached = string.Empty;
            switch (solution.LimitReached)
            {
                case BoxCaseSolution.Limit.LIMIT_MAXHEIGHTREACHED: limitReached="Maximum height"; break;
                case BoxCaseSolution.Limit.LIMIT_MAXNUMBERREACHED: limitReached="Maximum number"; break;
                case BoxCaseSolution.Limit.LIMIT_MAXWEIGHTREACHED: limitReached="Maximum weight"; break;
                default: break;
            }
            XmlElement elemLimitReached = xmlDoc.CreateElement("LimitReached", ns);
            elemLimitReached.InnerText = limitReached; 
            elemSolution.AppendChild(elemLimitReached);
            // --- case images
            for (int i = 0; i < 5; ++i)
            {
                // initialize drawing values
                string viewName = string.Empty;
                Vector3D cameraPos = Vector3D.Zero;
                int imageWidth = ImageSizeDetail;
                bool showDimensions = false;
                switch (i)
                {
                    case 0: viewName = "view_caseSolution_front"; cameraPos = Graphics3D.Front; imageWidth = ImageSizeDetail; break;
                    case 1: viewName = "view_caseSolution_left"; cameraPos = Graphics3D.Left; imageWidth = ImageSizeDetail; break;
                    case 2: viewName = "view_caseSolution_right"; cameraPos = Graphics3D.Right; imageWidth = ImageSizeDetail; break;
                    case 3: viewName = "view_caseSolution_back"; cameraPos = Graphics3D.Back; imageWidth = ImageSizeDetail; break;
                    case 4:  viewName = "view_caseSolution_iso"; cameraPos = Graphics3D.Corner_0; imageWidth = ImageSizeWide; showDimensions = true; break;
                    default: break;
                }
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(new Size(imageWidth, imageWidth));
                // set camera position 
                graphics.CameraPosition = cameraPos;
                // instantiate solution viewer
                BoxCaseSolutionViewer sv = new BoxCaseSolutionViewer(solution);
                sv.ShowDimensions = showDimensions;
                sv.Draw(graphics);
                graphics.Flush();
                // ---
                XmlElement elemImage = xmlDoc.CreateElement(viewName, ns);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 3, graphics.Bitmap.Height / 3);
                elemImage.Attributes.Append(styleAttribute);
                elemSolution.AppendChild(elemImage);
                // Save image ?
                SaveImageAs(graphics.Bitmap, viewName + ".png");
            }
        }
        #endregion // BoxCaseAnalysis

        #region Case constraint set
        private void AppendCaseConstraintSet(BoxCasePalletAnalysis caseAnalysis, XmlElement elemCaseAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            BoxCasePalletConstraintSet cs = caseAnalysis.ConstraintSet;
            // caseConstraintSet element
            XmlElement elemCaseConstraintSet = CreateElement("caseConstraintSet", null, elemCaseAnalysis, xmlDoc, ns);
            // allowedPatterns
            CreateElement("allowedPatterns", cs.AllowedPatternString, elemCaseConstraintSet, xmlDoc, ns);
            // allowedOrthoAxis
            CreateElement("allowedOrthoAxis", cs.AllowOrthoAxisString, elemCaseConstraintSet, xmlDoc, ns);
            // allowAlignedLayers
            CreateElement("allowAlignedLayers", cs.AllowAlignedLayers ? "Y" : "N", elemCaseConstraintSet, xmlDoc, ns);
            // allowAlternateLayers
            CreateElement("allowAlternateLayers", cs.AllowAlternateLayers ? "Y" : "N", elemCaseConstraintSet, xmlDoc, ns);
            // maximumCaseWeightGroup
            if (cs.UseMaximumCaseWeight)
            {
                XmlElement maximumCaseWeightGroup = CreateElement("maximumCaseWeightGroup", null, elemCaseConstraintSet, xmlDoc, ns);
                AppendElementValue(xmlDoc, maximumCaseWeightGroup, "maximumCaseWeight", UnitsManager.UnitType.UT_MASS, cs.MaximumCaseWeight);
            }
            // minimumBoxPerCaseGroup
            if (cs.UseMinimumNumberOfItems)
            {
                XmlElement minimumBoxPerCaseGroup = CreateElement("minimumBoxPerCaseGroup", null, elemCaseConstraintSet, xmlDoc, ns);
                CreateElement("minimumBoxPerCase", cs.MinimumNumberOfItems, minimumBoxPerCaseGroup, xmlDoc, ns);
            }
            // maximumBoxPerCaseGroup
            if (cs.UseMaximumNumberOfItems)
            {
                XmlElement maximumBoxPerCaseGroup = CreateElement("minimumBoxPerCaseGroup", null, elemCaseConstraintSet, xmlDoc, ns);
                CreateElement("maximumBoxPerCase", cs.MinimumNumberOfItems, maximumBoxPerCaseGroup, xmlDoc, ns);
            }
        }
        #endregion

        #region Case solution element
        private void AppendCaseSolutionElement(SelBoxCasePalletSolution selSolution, XmlElement elemCaseAnalysis, XmlDocument xmlDoc)
        {
            string ns = xmlDoc.DocumentElement.NamespaceURI;
            BoxCasePalletSolution caseSolution = selSolution.Solution;
            // caseSolution element
            XmlElement elemCaseSolution = CreateElement("boxCasePalletSolution", null, elemCaseAnalysis, xmlDoc, ns);
            // title
            CreateElement("title", caseSolution.Title, elemCaseSolution, xmlDoc, ns);
            // homogeneousLayer
            CreateElement("homogeneousLayer", caseSolution.HasHomogeneousLayers ? "Y" : "N", elemCaseSolution, xmlDoc, ns);
            // casePerPalletCount
            CreateElement("casePerPalletCount", caseSolution.CasePerPalletCount, elemCaseSolution, xmlDoc, ns);
            // boxPerCaseCount
            CreateElement("boxPerCaseCount", caseSolution.BoxPerCaseCount, elemCaseSolution, xmlDoc, ns);
            // boxPerPalletCount
            CreateElement("boxPerPalletCount", caseSolution.BoxPerPalletCount, elemCaseSolution, xmlDoc, ns);
            // caseEfficiency
            CreateElement("caseEfficiency", caseSolution.CaseEfficiency, elemCaseSolution, xmlDoc, ns);
            // palletEfficiency
            CreateElement("palletEfficiency", caseSolution.PalletEfficiency, elemCaseSolution, xmlDoc, ns);

            AppendElementValue(xmlDoc, elemCaseSolution, "caseWeight", UnitsManager.UnitType.UT_MASS, caseSolution.CaseWeight);
            AppendElementValue(xmlDoc, elemCaseSolution, "palletWeight", UnitsManager.UnitType.UT_MASS, caseSolution.PalletWeight);

            // --- build image
            Graphics3DImage graphics = new Graphics3DImage(new Size(512, 512));
            graphics.CameraPosition = Graphics3D.Corner_0;
            graphics.Target = Vector3D.Zero;
            // instantiate solution viewer
            BoxCasePalletSolutionViewer sv = new BoxCasePalletSolutionViewer(caseSolution);
            sv.Draw(graphics);
            graphics.Flush();
            SaveImageAs(graphics.Bitmap, "view_caseSolution_iso.png");
            // ---
            // view_caseSolution_iso
            XmlElement elemImage = xmlDoc.CreateElement("view_caseSolution_iso", ns);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            elemImage.InnerText = Convert.ToBase64String((byte[])converter.ConvertTo(graphics.Bitmap, typeof(byte[])));
            XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
            styleAttribute.Value = string.Format("width:{0}pt;height:{1}pt", graphics.Bitmap.Width / 4, graphics.Bitmap.Height / 4);
            elemImage.Attributes.Append(styleAttribute);
            elemCaseSolution.AppendChild(elemImage);
        }
        #endregion

        #region Helpers
        private static XmlElement CreateElement(string eltName, string innerValue, XmlElement parentElt, XmlDocument xmlDoc, string ns)
        {
            XmlElement elt = xmlDoc.CreateElement(eltName, ns);
            if (!string.IsNullOrEmpty(innerValue))
                elt.InnerText = innerValue;
            parentElt.AppendChild(elt);
            return elt;
        }
        private static XmlElement CreateElement(string eltName, double v, XmlElement parentElt, XmlDocument xmlDoc, string ns)
        {
            XmlElement elt = xmlDoc.CreateElement(eltName, ns);
            elt.InnerText = string.Format("{0:F}", v);
            parentElt.AppendChild(elt);
            return elt;
        }
        private static XmlElement CreateElement(string eltName, int v, XmlElement parentElt, XmlDocument xmlDoc, string ns)
        {
            XmlElement elt = xmlDoc.CreateElement(eltName, ns);
            elt.InnerText = string.Format("{0}", v);
            parentElt.AppendChild(elt);
            return elt;
        }
        private static void AppendElementValue(XmlDocument xmlDoc, XmlElement parent, string eltName, UnitsManager.UnitType unitType, OptDouble optValue)
        {
            if (optValue.Activated)
            {
                // eltName
                XmlElement createdElement = xmlDoc.CreateElement(eltName, xmlDoc.DocumentElement.NamespaceURI);
                parent.AppendChild(createdElement);
                // unit
                XmlElement unitElement = xmlDoc.CreateElement("unit", xmlDoc.DocumentElement.NamespaceURI);
                unitElement.InnerText = UnitsManager.UnitString(unitType);
                createdElement.AppendChild(unitElement);
                // value
                XmlElement valueElement = xmlDoc.CreateElement("value", xmlDoc.DocumentElement.NamespaceURI);
                valueElement.InnerText = string.Format(UnitsManager.UnitFormat(unitType), optValue.Value);
                createdElement.AppendChild(valueElement);
            }
        }
        private static void AppendElementValue(XmlDocument xmlDoc, XmlElement parent, string eltName, UnitsManager.UnitType unitType, double eltValue)
        {
            // eltName
            XmlElement createdElement = xmlDoc.CreateElement(eltName, xmlDoc.DocumentElement.NamespaceURI);
            parent.AppendChild(createdElement);
            // unit
            XmlElement unitElement = xmlDoc.CreateElement("unit", xmlDoc.DocumentElement.NamespaceURI);
            unitElement.InnerText = UnitsManager.UnitString(unitType);
            createdElement.AppendChild(unitElement);
            // value
            XmlElement valueElement = xmlDoc.CreateElement("value", xmlDoc.DocumentElement.NamespaceURI);
            valueElement.InnerText = string.Format(UnitsManager.UnitFormat(unitType), eltValue);
            createdElement.AppendChild(valueElement);
        }
        private static void AppendElementValue(XmlDocument xmlDoc, XmlElement parent, string eltName, double eltValue)
        {
            XmlElement createdElement = xmlDoc.CreateElement(eltName, xmlDoc.DocumentElement.NamespaceURI);
            createdElement.InnerText = string.Format("{0:0.#}", eltValue);
            parent.AppendChild(createdElement);
        }
        private static void AppendElementValue(XmlDocument xmlDoc, XmlElement parent, string eltName, int eltValue)
        {
            XmlElement createdElement = xmlDoc.CreateElement(eltName, xmlDoc.DocumentElement.NamespaceURI);
            createdElement.InnerText = string.Format("{0}", eltValue);
            parent.AppendChild(createdElement);
        }
        private static void AppendElementValue(XmlDocument xmlDoc, XmlElement parent, string eltName, string eltValue)
        {
            XmlElement createdElement = xmlDoc.CreateElement(eltName, xmlDoc.DocumentElement.NamespaceURI);
            createdElement.InnerText = string.Format("{0}", eltValue);
            parent.AppendChild(createdElement);
        }

        private void SaveImageAs(Bitmap bmp, string fileName)
        {
            if (!WriteImageFiles)
                return;
            try { bmp.Save(Path.Combine(ImageDirectory, fileName), System.Drawing.Imaging.ImageFormat.Png); }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }

        public string ToAbsolute(string pathString)
        {
            if (Path.IsPathRooted(pathString))
                return pathString;
            else
                return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), pathString));
        }
        #endregion

        #region Deleting methods
        public void DeleteImageDirectory()
        {
            try { System.IO.Directory.Delete(_imageDirectory, true); }
            catch (Exception ex) { _log.Error(ex.Message); }
        }
        #endregion

        #region XML validator
        // Validation Error Count
        static int ErrorsCount = 0;
        // Validation Error Message
        static string ErrorMessage = "";

        private static void ValidateXmlDocument(XmlReader documentToValidate, string schemaPath)
        {
            XmlSchema schema;
            using (var schemaReader = XmlReader.Create(schemaPath))
            {
                schema = XmlSchema.Read(schemaReader, ValidationEventHandler);
            }

            var schemas = new XmlSchemaSet();
            schemas.Add(schema);
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ValidationFlags =
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationEventHandler;

            using (var validationReader = XmlReader.Create(documentToValidate, settings))
            { while (validationReader.Read()) { } }

            if (ErrorsCount > 0)
                throw new Exception(ErrorMessage);
        }

        private static void ValidationEventHandler(
            object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                ErrorMessage = ErrorMessage + args.Message + "\r\n";
                ErrorsCount++;
            }
        }
        #endregion
    }
    #endregion
}
