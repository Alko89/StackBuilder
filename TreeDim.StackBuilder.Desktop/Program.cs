﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Resources;
// CrashReporter.NET
using CrashReporterDotNET;
// log4net
using log4net;
using log4net.Config;
// treeDiM
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;

using treeDiM.AutoUpdater;
#endregion

#region File association
using Utilities;
using System.Threading;
using System.Reflection;
using System.IO;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    static class Program
    {
        #region Private members
        static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        #endregion

        #region Main
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // set up a simple logging configuration
            XmlConfigurator.Configure();
            if (!LogManager.GetRepository().Configured)
                Debug.Fail("Logging not configured!\n Press ignore to continue");

            // 
            MessageFilter oFilter = new MessageFilter();
            System.Windows.Forms.Application.AddMessageFilter(
                                (IMessageFilter)oFilter);

            // note: arguments are handled within FormMain constructor
            // using Environment.GetCommandLineArgs()
            // force CultureToUse culture if specified in config file
            string specifiedCulture = treeDiM.StackBuilder.Desktop.Properties.Settings.Default.CultureToUse;
            if (!string.IsNullOrEmpty(specifiedCulture))
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(specifiedCulture);
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(specifiedCulture);
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Specified culture in config file ({0}) appears to be invalid: {1}", specifiedCulture, ex.Message));
                }
            }

            // get current culture
            _log.Info(string.Format("Starting {0} with user culture {1}", Application.ProductName, Thread.CurrentThread.CurrentUICulture));

            // set unit system
            UnitsManager.CurrentUnitSystem = (UnitsManager.UnitSystem)Properties.Settings.Default.UnitSystem; 

            // file association
            RegisterFileType();

            // initialize database with containing folder
            PalletSolutionDatabase.Directory = Settings.Default.PalletSolutionsPath;

            // *** crash reporting
            Application.ThreadException += (sender, threadargs) => SendCrashReport(threadargs.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, threadargs) =>
            {
                SendCrashReport((Exception)threadargs.ExceptionObject);
                Environment.Exit(0);
            };
            // *** crash reporting

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
 
            // if this application does not need updating,
            // show main form
            Updater updater = new Updater();
            if (!updater.Update())
                Application.Run(new FormMain());

            _log.Info("Closing " + Application.ProductName);
        }
        #endregion

        #region Culture
        public static bool IsCurrentCultureSupported()
        {
            // get list of available cultures
            ReadOnlyCollection<CultureInfo> listOfAvailableCultures = GetAvailableCultures();
            // check if current culture belongs to available cultures
            return listOfAvailableCultures.Contains(Thread.CurrentThread.CurrentUICulture);
        }

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

        #region Exception reporting
        /// <summary>
        /// Report an exception
        /// </summary>
        public static void SendCrashReport(Exception exception)
        {
            var reportCrash = new ReportCrash
            {
                FromEmail = "treedim@gmail.com",
                ToEmail = "treedim@gmail.com",
                SmtpHost = "smtp.gmail.com",
                Port = 587,
                UserName = "treedim@gmail.com",
                Password = "Knowledge_1",
                EnableSSL = true,
            };

            reportCrash.Send(exception);
        }
        #endregion

        #region Contextual help
        internal class MessageFilter : IMessageFilter
        {
            #region IMessageFilter Members
            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                // Use a switch so we can trap other messages in the future
                switch (m.Msg)
                {
                    case 0x100: // WM_KEYDOWN

                        if ((int)m.WParam == (int)Keys.F1)
                        {
                            HelpUtility.ProcessHelpRequest(Control.FromHandle(m.HWnd));
                            return true;
                        }
                        break;
                }
                return false;
            }
            #endregion
        }

        public static class HelpUtility
        {
            public static void ProcessHelpRequest(Control ctrContext)
            {
                ShowContextHelp(ctrContext);
            }
        }

        // Process a request to display help
        // for the context specified by ctrContext.
        public static void ShowContextHelp(Control ctrContext)
        {
            int i=0;
            Control ctr = ctrContext;
            Form form = null;
            while (i < 100 && null == form)
            {
                ctr = ctr.Parent;
                form = ctr as Form;
                ++i;
            }
            if (null != form)
            {
                string sHTMLHelp = form.GetType().Name + ".html";
                Help.ShowHelp(ctrContext
                    , Path.ChangeExtension(Application.ExecutablePath, "chm")
                    , HelpNavigator.Topic
                    , sHTMLHelp);
            }
        }
        #endregion

        #region File association
        private static void RegisterFileType()
        {
            try
            {
                FileAssociation FA = new FileAssociation();
                FA.Extension = "stb";
                FA.ContentType = "application/xml";
                FA.FullName = "TreeDim StackBuilder Data Files";
                FA.ProperName = "StackBuilder File";
                FA.AddCommand("open", Assembly.GetExecutingAssembly().Location + " \"%1\"");
                FA.IconPath = Assembly.GetExecutingAssembly().Location;
                FA.IconIndex = 0;
                FA.Create();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("File association failed! Exception : {0}", ex.Message));
            }
        }
        #endregion
    }
}