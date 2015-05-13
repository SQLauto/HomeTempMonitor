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

namespace HomeTempMonitor.Mobile
{
    public partial class Default : System.Web.UI.Page
    {
        private Temps _currentTemps = new Temps();
        private IQueryable<templog> _temps;
        private double _chartWidthFactor = 0.95;
        private double _chartAspect = 1.5;

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(LoadTempsAsync));
            RegisterAsyncTask(new PageAsyncTask(LoadTempDataAsync));

            Int32 chartWidth = Convert.ToInt32(Math.Ceiling(Request.Browser.ScreenPixelsWidth * _chartWidthFactor * 0.95));
            Int32 chartHeight = Convert.ToInt32(Math.Ceiling(chartWidth / _chartAspect));

            if (Request.Browser.MobileDeviceModel.ToLower() == "iphone")
            {
                chartWidth = Request.Browser.ScreenPixelsWidth / 2;
                chartHeight = chartWidth;
                lstDiv.Attributes["style"] = "width:90%;margin:0 auto;";
            }
            else
            {
                lstDiv.Attributes["style"] = "width:320px;margin:0 auto;";
            }

            chart_div.Width = Unit.Percentage(95);
            chart_div.Height = chartWidth;
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

                foreach (templog temp in lastHour)
                {
                    TableCell dateCell = new TableCell();
                    TableCell atCell = new TableCell();
                    TableCell tempCell = new TableCell();

                    dateCell.Text = GetFormattedDateString(temp.Recorded);
                    atCell.Text = "@";
                    tempCell.Text = temp.Temperature.ToString("f2") + " °F";

                    TableRow tableRow = new TableRow();
                    tableRow.Cells.Add(dateCell);
                    tableRow.Cells.Add(atCell);
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

        protected string GetChartOptions()
        {
            Int32 hourRange = Convert.ToInt32(lstDataRange.SelectedValue);

            if (Request.Browser.MobileDeviceModel.ToLower() == "iphone")
            {
                if (hourRange <= 48)
                {
                    return @"{ vAxis: { title: 'Temperature (°F)' }, lineWidth: 1, legend: 'none', chartArea: { left: 85, top: 10, width: '100%', height: '70%' }, backgroundColor: '#f9f9f9' }";
                }
                else
                {
                    return @"{ vAxis: { title: 'Temperature (°F)' }, lineWidth: 0.5, legend: 'none', chartArea: { left: 85, top: 10, width: '100%', height: '70%' }, backgroundColor: '#f9f9f9' }";
                }

            }
            else
            {
                if (hourRange <= 168)
                {
                    return @"{ vAxis: { title: 'Temperature (°F)' }, lineWidth: 1, legend: 'none', chartArea: { left: 120, top: 10, width: '100%', height: '80%' }, backgroundColor: '#f9f9f9' }";
                }
                else
                {
                    return @"{ vAxis: { title: 'Temperature (°F)' }, lineWidth: 0.5, legend: 'none', chartArea: { left: 120, top: 10, width: '100%', height: '80%' }, backgroundColor: '#f9f9f9' }";
                }
            }
        }

        protected string GetCurrentInsideTemp()
        {
            return _currentTemps.inside.ToString("f2");
        }

        protected string GetCurrentOutsideTemp()
        {
            return _currentTemps.outside.ToString("f2");
        }

        protected string GetMinTemp()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature < t2.Temperature ? t1 : t2);
            return temp.Temperature.ToString("f2");
        }

        protected string GetMinDateTime()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature < t2.Temperature ? t1 : t2);
            return GetFormattedDateString(temp.Recorded);
        }

        protected string GetMaxTemp()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature > t2.Temperature ? t1 : t2);
            return temp.Temperature.ToString("f2");
        }

        protected string GetMaxDateTime()
        {
            templog temp = _temps.Aggregate((t1, t2) => t1.Temperature > t2.Temperature ? t1 : t2);
            return GetFormattedDateString(temp.Recorded);
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