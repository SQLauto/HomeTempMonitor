using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using HomeTempMonitor.Classes;
using HomeTempMonitor.Models;

namespace HomeTempMonitor
{
    public partial class Default : System.Web.UI.Page
    {
        private Temps _currentTemps;
        private IQueryable<templog> _temps;

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(LoadTempsAsync));
            RegisterAsyncTask(new PageAsyncTask(LoadTempDataAsync));
        }

        private async Task LoadTempsAsync()
        {
            Temps temps = await CurrentTemps.GetTemps();
            _currentTemps = temps;
        }

        private async Task LoadTempDataAsync()
        {
            using (var context = new templogEntities())
            {
                DateTime range = DateTime.Now.AddHours(-Convert.ToInt32(lstDataRange.SelectedValue));
                List<templog> temps = await (from t in context.templogs
                                             where t.Recorded >= range
                                             orderby t.Recorded ascending
                                             select t).ToListAsync();
                _temps = temps.AsQueryable();

                List<templog> lastHour = _temps.Where(t => t.Recorded >= DateTime.Now.AddHours(-1)).OrderBy(t => t.Recorded).ToList();

                foreach(templog temp in lastHour)
                {
                    TableCell dateCell = new TableCell();
                    TableCell tempCell = new TableCell();

                    dateCell.Text = GetFormattedDateString(temp.Recorded);
                    dateCell.CssClass = "temp";
                    tempCell.Text = temp.Temperature.ToString("f2") + " °F";
                    tempCell.CssClass = "temp";

                    TableRow tableRow = new TableRow();
                    tableRow.Cells.Add(dateCell);
                    tableRow.Cells.Add(tempCell);

                    tblTemps.Rows.Add(tableRow);
                }
            }
        }

        protected string GetTempData()
        {
            StringBuilder tempData = new StringBuilder();

            foreach (templog temp in _temps)
            {
                DateTime recorded = temp.Recorded;
                tempData.Append(",['" + recorded.ToString("M/d/yyyy h:mm tt") + "'," + temp.Temperature.ToString() + "]");
            }

            return tempData.ToString();
        }

        protected string GetLineWidth()
        {
            if (Convert.ToInt32(lstDataRange.SelectedValue) > 48)
                return "lineWidth: 1, ";
            else
                return "";
        }

        protected string GetCurrentInsideTemp()
        {
            return _currentTemps.inside.ToString("f2");
        }

        protected string GetCurrentOutsideTemp()
        {
            return _currentTemps.outside.ToString("f2");
        }

        protected string GetMinDate()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature < t2.Temperature ? t1 : t2);
            return GetFormattedDateString(temp.Recorded);
        }

        protected string GetMinTemp()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature < t2.Temperature ? t1 : t2);
            return temp.Temperature.ToString("f2");
        }

        protected string GetMaxDate()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature > t2.Temperature ? t1 : t2);
            return GetFormattedDateString(temp.Recorded);
        }

        protected string GetMaxTemp()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature > t2.Temperature ? t1 : t2);
            return temp.Temperature.ToString("f2");
        }

        protected string GetAvgTemp()
        {
            return _temps.Average(t => t.Temperature).ToString("f2");
        }

        protected string GetFormattedDateString(DateTime dateTime)
        {
            return dateTime.ToShortDateString() + "  " + dateTime.ToShortTimeString();
        }
    }
}