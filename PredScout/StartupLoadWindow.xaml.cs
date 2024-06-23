using System;
using System.Diagnostics;
using System.IO.Packaging;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PredScout
{
    public partial class StartupLoadWindow : Window
    {

        readonly string newSignature = SystemSignature.GenerateSystemSignature();

        //readonly string oldSignature = GetOldSignature();

        public StartupLoadWindow()
        {
            InitializeComponent();
        }

        public async Task<bool> CheckStartupAuthorized()
        {
            //bool isValidSignature = SystemSignature.ValidateSystemSignature(oldSignature, newSignature);

            string version = GetApplicationVersion();
            Console.WriteLine($"Application Version: {version}");

            version = "2.1.1";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    await Task.Delay(1500);
                    var url = $"https://localhost:32786/api/Launch/CheckLaunch?signature={newSignature}&version={version}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    Console.WriteLine("Response: " + response);

                    if (response.IsSuccessStatusCode) { return true; }

                    if (response.StatusCode == System.Net.HttpStatusCode.UpgradeRequired)
                    {                         
                        MessageBox.Show("An update is required to continue using PredScout.", "Update Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                        //Process.Start(); // Start the update process
                        return false;
                    }

                    // If the response is not successful, return false
                    return false;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private string GetApplicationVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            string fullVersion = informationalVersionAttribute?.InformationalVersion ?? "Version not found";

            // Regular expression to match the version number up to the last digit
            var match = Regex.Match(fullVersion, @"^\d+\.\d+\.\d+");
            return match.Value;
        }
    }
}
