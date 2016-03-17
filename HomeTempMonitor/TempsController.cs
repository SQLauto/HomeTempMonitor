using System;
using System.Linq;
using System.Web.Http;

using HomeTempMonitor.Models;

namespace HomeTempMonitor
{
    [RoutePrefix(@"Temps")]
    public class TempsController : ApiController
    {
        [Route(@"DateRange")]
        [HttpGet]
        public Temp[] GetDateRange([FromUri] string startDateString, [FromUri] string endDateString)
        {
            Temp[] temps = null;
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);

            try {
                using (templogEntities context = new templogEntities())
                {
                    temps = (from t in context.templogs
                             where t.Recorded >= startDate
                             && t.Recorded <= endDate
                             orderby t.Recorded ascending
                             select new Temp
                             {
                                 Temperature = t.Temperature,
                                 Recorded = t.Recorded
                             }).ToArray();
                }
            }
            catch
            {

            }

            return temps;
        }

        [Route(@"LastHour")]
        [HttpGet]
        public Temp[] GetLastHour()
        {
            Temp[] temps = null;
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddHours(-1);

            try
            {
                using (templogEntities context = new templogEntities())
                {
                    temps = (from t in context.templogs
                             where t.Recorded >= startDate
                             && t.Recorded <= endDate
                             orderby t.Recorded ascending
                             select new Temp
                             {
                                 Temperature = t.Temperature,
                                 Recorded = t.Recorded
                             }).ToArray();
                }
            }
            catch
            {

            }


            return temps;
        }
    }
}