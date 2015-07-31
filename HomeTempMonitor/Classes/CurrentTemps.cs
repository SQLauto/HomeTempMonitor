using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

using HomeTempMonitor.Models;

namespace HomeTempMonitor.Classes
{
    public class CurrentTemps
    {
        public static async Task<Temps> GetTempsAsync()
        {
            Temps temps;
            try
            {
                WebClient client = new WebClient();
                var clientTemps = client.DownloadStringTaskAsync("http://airpi.bean.local/current/temps");

                temps = JsonConvert.DeserializeObject<Temps>(await clientTemps);
            }
            catch
            {
                temps = new Temps()
                {
                    Inside = -1,
                    Outside = -1
                };
            }

            return temps;
        }

        public static Temps GetTemps()
        {
            Temps temps;
            try
            {
                WebClient client = new WebClient();
                var clientTemps = client.DownloadString("http://airpi.bean.local/current/temps");

                temps = JsonConvert.DeserializeObject<Temps>(clientTemps);
            }
            catch
            {
                temps = new Temps()
                {
                    Inside = -1,
                    Outside = -1
                };
            }

            return temps;
        }
    }
}