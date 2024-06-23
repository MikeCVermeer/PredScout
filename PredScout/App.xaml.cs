using System;
using System.Windows;

namespace PredScout
{
    public partial class App : Application
    {
#if DEBUG
        bool LoadingWindowEnabled = false;
#else
        bool LoadingWindowEnabled = true;
#endif

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // If the LoadingWindowEnabled flag is set to false, skip the loading window and go straight to the main window
            if (!LoadingWindowEnabled)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                return;
            }

            StartupLoadWindow loadingWindow = new StartupLoadWindow();
            loadingWindow.Show();

            bool isAuthorized = await loadingWindow.CheckStartupAuthorized();

            if (isAuthorized)
            {
                MainWindow mainWindow = new MainWindow();
                loadingWindow.Close();
                mainWindow.Show();
            }
            else
            {
                //MessageBox.Show("Could not establish a connection with PredScout servers.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //Shutdown();
            }
        }
    }
}
