using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using HomeTempMonitor.Models;

namespace HomeTempMonitor
{
    public class TempsController : ApiController
    {
        // GET api/<controller>/5
        public TempData Get(int id)
        {
            TempData tempData = new TempData();

            using (var context = new templogEntities())
            {
                DateTime range = DateTime.Now.AddHours(-Convert.ToInt32(id));
                tempData.DataSet = (from t in context.templogs
                                    where t.Recorded >= range
                                    orderby t.Recorded ascending
                                    select t).ToArray();
                IQueryable<templog> templogData = tempData.DataSet.AsQueryable();

                tempData.LastHour = templogData.Where(t => t.Recorded >= DateTime.Now.AddHours(-1)).OrderBy(t => t.Recorded).ToArray();
                tempData.MaximumTemp = templogData.Aggregate((t1, t2) => t1.Temperature > t2.Temperature ? t1 : t2);
                tempData.MinimumTemp = templogData.Aggregate((t1, t2) => t1.Temperature < t2.Temperature ? t1 : t2);
                tempData.AverageTemp = templogData.Average(t => t.Temperature);
            }

            return tempData;
        }
    }
}