using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace HomeTempMonitor.Classes
{
    public class CurrentTemps
    {
        public static async Task<Temps> GetTempsAsync()
        {
            Temps temps = new Temps();

            using (HttpClient httpClient = new HttpClient())
            {   
                string responseBody;
                Uri address = new Uri("http://airpi.bean.local/current/temps");

                HttpResponseMessage responseMessage = await httpClient.GetAsync(address);
                responseMessage.EnsureSuccessStatusCode();
                responseBody = await responseMessage.Content.ReadAsStringAsync();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                temps = serializer.Deserialize<Temps>(responseBody);
            }

            return temps;
        }
    }
}