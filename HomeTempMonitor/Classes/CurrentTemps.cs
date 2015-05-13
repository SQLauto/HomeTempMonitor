using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

using HomeTempMonitor.Models;

namespace HomeTempMonitor.Classes
{
    public class CurrentTemps
    {
        public static async Task<Temps> GetTemps()
        {
            WebClient client = new WebClient();
            var clientTemps = client.DownloadStringTaskAsync("http://airpi.bean.local/current/temps");

            Temps temps = JsonConvert.DeserializeObject<Temps>(await clientTemps);

            return temps;
        }
    }
}