using System;
using System.Linq;
using System.Web.Http;

using HomeTempMonitor.Models;

namespace HomeTempMonitor
{
    [RoutePrefix(@"Temps")]
    public class TempsController : ApiController
    {
        // GET api/<controller>/5
        [Route(@"FullData/{id}")]
        [HttpGet]
        public TempData GetFullData([FromUri] int id)
        {
            TempData tempData = new TempData();

            using (var context = new templogEntities())
            {
                DateTime range = DateTime.Now.AddHours(-Convert.ToInt32(id));
                tempData.DataSet = (from t in context.templogs
                                    where t.Recorded >= range
                                    orderby t.Recorded ascending
                                    select new Temp
                                    {
                                        Temperature = t.Temperature,
                                        Recorded = t.Recorded
                                    }).ToArray();
                IQueryable<Temp> templogData = tempData.DataSet.AsQueryable();

                tempData.LastHour = templogData.Where(t => t.Recorded >= DateTime.UtcNow.AddHours(-1)).OrderBy(t => t.Recorded).ToArray();
                tempData.MaximumTemp = templogData.Aggregate((t1, t2) => t1.Temperature > t2.Temperature ? t1 : t2);
                tempData.MinimumTemp = templogData.Aggregate((t1, t2) => t1.Temperature < t2.Temperature ? t1 : t2);
                tempData.AverageTemp = templogData.Average(t => t.Temperature);
            }

            return tempData;
        }

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
    }
}