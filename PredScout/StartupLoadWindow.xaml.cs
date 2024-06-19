using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace PredScout
{
    public partial class StartupLoadWindow : Window
    {
        public StartupLoadWindow()
        {
            InitializeComponent();
        }

        public async Task<bool> CheckServerStatus()
        {
            try
            {
#if DEBUG
                // In debug mode, simulate a delay and then return true
                await Task.Delay(1000);
                return true;
#else
                using (HttpClient client = new HttpClient())
                {
                    await Task.Delay(1500);
                    HttpResponseMessage response = await client.GetAsync("http://yourbackend.com/api/check-status");
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    return result.status == "ok";
                }
#endif
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
