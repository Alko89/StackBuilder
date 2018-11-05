#region Using directives
using System;
using System.IO;

using WkHtmlToXSharp;
#endregion

namespace treeDiM.StackBuilder.Reporting
{
    #region ReporterPDF
    public class ReporterPDF : Reporter
    {
        public ReporterPDF(ReportData inputData, ref ReportNode rnRoot
            , string templatePath, string outputFilePath)
        {
            // absolute output file path
            string absOutputFilePath = string.Empty;
            if (Path.IsPathRooted(outputFilePath))
                absOutputFilePath = outputFilePath;
            else
                absOutputFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), outputFilePath));
            // absolute template path
            string absTemplatePath = string.Empty;
            if (Path.IsPathRooted(templatePath))
                absTemplatePath = templatePath;
            else
                absTemplatePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), templatePath));

            // does output directory exists
            string outDir = Path.GetDirectoryName(absOutputFilePath);
            if (!Directory.Exists(outDir))
            {
                try { Directory.CreateDirectory(outDir); }
                catch (UnauthorizedAccessException /*ex*/)
                { throw new UnauthorizedAccessException(string.Format("User not allowed to write under {0}", Directory.GetParent(outDir).FullName)); }
                catch (Exception ex)
                { throw new Exception(string.Format("Directory {0} does not exist, and could not be created.", outDir), ex); }
            }
            // html file path
            string htmlFilePath = Path.ChangeExtension(absOutputFilePath, "html");
            BuildAnalysisReport(inputData, ref rnRoot, absTemplatePath, htmlFilePath);

            try
            {
                var ignore = Environment.GetEnvironmentVariable("WKHTMLTOXSHARP_NOBUNDLES");

                if (ignore == null || ignore.ToLower() != "true")
                {
                    // Register all available bundles..
                    WkHtmlToXLibrariesManager.Register(new Win32NativeBundle());
                    WkHtmlToXLibrariesManager.Register(new Win64NativeBundle());
                }

                using (IHtmlToPdfConverter converter = new MultiplexingConverter())
                {
                    converter.GlobalSettings.Margin.Top = "0cm";
                    converter.GlobalSettings.Margin.Bottom = "0cm";
                    converter.GlobalSettings.Margin.Left = "0cm";
                    converter.GlobalSettings.Margin.Right = "0cm";
                    converter.GlobalSettings.Orientation = PdfOrientation.Portrait;
                    converter.GlobalSettings.Size.PageSize = PdfPageSize.A4;

                    converter.ObjectSettings.Page = htmlFilePath;
                    converter.ObjectSettings.Web.EnablePlugins = true;
                    converter.ObjectSettings.Web.EnableJavascript = true;
                    converter.ObjectSettings.Web.Background = true;
                    converter.ObjectSettings.Web.LoadImages = true;
                    converter.ObjectSettings.Load.LoadErrorHandling = LoadErrorHandlingType.ignore;

                    byte[] bufferPDF = converter.Convert();
                    File.WriteAllBytes(absOutputFilePath, bufferPDF);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public override bool WriteNamespace => false;
        public override bool WriteImageFiles => true;
        private MultiplexingConverter GetConverter()
        {
            var obj = new MultiplexingConverter();
            obj.Begin += (s, e) => _log.DebugFormat("Conversion begin, phase count: {0}", e.Value);
            obj.Warning += (s, e) => _log.Warn(e.Value);
            obj.Finished += (s, e) => _log.InfoFormat("Finished: {0}", e.Value ? "success" : "failed!");
            return obj;
        }
    }
    #endregion
}