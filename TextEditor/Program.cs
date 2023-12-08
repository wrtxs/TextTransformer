using System.Globalization;
using ActiproSoftware.Products;

namespace TextEditor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // The following code must be executed before application startup.
            var culture = CultureInfo.CreateSpecificCulture("ru-RU");
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            // Sets the German culture as the default culture for all threads in the application. 
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //Utils.UpdateRegistry();
            //ActiproLicenseManager.RegisterLicense("BOARD4ALL", "WIN211-8PYU2-Y6C23-KVVE2-DFCG");
            ActiproLicenseManager.RegisterLicense("Montaraz www.board4all.biz", "WIN231-J2WGE-NLJRU-UVVE2-UFGG");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
            //Application.Run(new Form1());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
        }
    }
}
