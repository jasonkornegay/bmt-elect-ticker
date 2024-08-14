using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace _2020_Primaries_Ticker
{
    static class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                log4net.Config.XmlConfigurator.Configure();

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                // This code is used to support logging to the status bar - registers frmMain with IAppender interface from log4net
                var mainForm = new Election_Ticker_2020();
                Application.EnableVisualStyles();
                Application.Run(mainForm);

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    var ex = (Exception)e.ExceptionObject;
                // Log error
                log.Debug("Unhandled exception occurred", ex);
                    log.Error("Unhandled exception occurred: " + ex.Message);
                };
            }
            catch(Exception ex)
            {
                // Top-level error dialog if exception bubbles up
                MessageBox.Show("General error occurred with application. Please re-start to ensure proper operation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Log the error 
                log.Debug("General exception occurred at main program level", ex);
                log.Error("General exception occurred at main program level: " + ex.Message);
            }
        }
    }
}
