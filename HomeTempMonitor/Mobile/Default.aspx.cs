using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using HomeTempMonitor.Classes;

namespace HomeTempMonitor.Mobile
{
    public partial class Default : System.Web.UI.Page
    {
        private Temps temps = new Temps();
        private DateTime minDate;
        private double minimum;
        private DateTime maxDate;
        private double maximum;
        private double average;
        private List<TempReading> tempReadings = new List<TempReading>();
        private double chartWidthFactor = 0.95;
        private double chartAspect = 1.5;

        protected async void Page_Load(object sender, EventArgs e)
        {
            temps = await CurrentTemps.GetTempsAsync();

            Int32 chartWidth = Convert.ToInt32(Math.Ceiling(Request.Browser.ScreenPixelsWidth * chartWidthFactor * 0.95));
            Int32 chartHeight = Convert.ToInt32(Math.Ceiling(chartWidth / chartAspect));

            if (Request.Browser.MobileDeviceModel.ToLower() == "iphone")
            {
                chartWidth = Request.Browser.ScreenPixelsWidth / 2;
                chartHeight = chartWidth;
            }

            chart_div.Width = Unit.Percentage(95);
            chart_div.Height = chartWidth;

            using (DataView dataView = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty))
            {
                DataRow firstRow = dataView.Table.Rows[0];

                minDate = new DateTime();
                maxDate = new DateTime();
                minimum = (double)firstRow["Temperature"];
                maximum = (double)firstRow["Temperature"];
                average = 0;

                double total = 0;

                foreach (DataRow row in dataView.Table.Rows)
                {
                    total += (double)row["Temperature"];

                    if ((double)row["Temperature"] <= minimum)
                    {
                        minimum = (double)row["Temperature"];
                        minDate = (DateTime)row["Recorded"];
                    }
                    else if ((double)row["Temperature"] >= maximum)
                    {
                        maximum = (double)row["Temperature"];
                        maxDate = (DateTime)row["Recorded"];
                    }

                    DateTime rowDate = (DateTime)row["Recorded"];
                    if (rowDate > DateTime.Now.AddHours(-1))
                    {
                        TempReading newReading = new TempReading();
                        newReading.Recorded = rowDate;
                        newReading.Temperature = (double)row["Temperature"];

                        tempReadings.Add(newReading);
                    }
                }

                average = total / dataView.Table.Rows.Count;
            }
        }

        protected string GetChartScript()
        {
            string value = "";
            StringBuilder strScript = new StringBuilder();

            using (DataView dataView = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty))
            {
                try
                {
                    strScript.Append(@"function drawVisualization() {         
                    var data = google.visualization.arrayToDataTable([  
                    ['Time', 'Temperature'],");

                    foreach (DataRow row in dataView.Table.Rows)
                    {
                        DateTime recorded = (DateTime)row["Recorded"];
                        strScript.Append("['" + recorded.ToString("M/d/yyyy h:mm tt") + "'," + row["Temperature"].ToString() + "],");
                    }
                    strScript.Remove(strScript.Length - 1, 1);
                    strScript.Append("]);");

                    Int32 hourRange = Convert.ToInt32(lstDataRange.SelectedValue);

                    if (Request.Browser.MobileDeviceModel.ToLower() == "iphone")
                    {
                        if (hourRange <= 48)
                        {
                            strScript.Append(@"var options = { vAxis: { title: 'Temperature (°F)' }, lineWidth: 1, legend: 'none', chartArea: { left: 85, top: 10, width: '100%', height: '70%' }, backgroundColor: '#f9f9f9' };");
                        }
                        else 
                        {
                            strScript.Append(@"var options = { vAxis: { title: 'Temperature (°F)' }, lineWidth: 0.5, legend: 'none', chartArea: { left: 85, top: 10, width: '100%', height: '70%' }, backgroundColor: '#f9f9f9' };");
                        }
                        
                    }
                    else
                    {
                        if (hourRange <= 168)
                        {
                            strScript.Append(@"var options = { vAxis: { title: 'Temperature (°F)' }, lineWidth: 1, legend: 'none', chartArea: { left: 120, top: 10, width: '100%', height: '80%' }, backgroundColor: '#f9f9f9' };");
                        }
                        else
                        {
                            strScript.Append(@"var options = { vAxis: { title: 'Temperature (°F)' }, lineWidth: 0.5, legend: 'none', chartArea: { left: 120, top: 10, width: '100%', height: '80%' }, backgroundColor: '#f9f9f9' };");
                        }
                    }

                    strScript.Append(" var chart = new google.visualization.LineChart(document.getElementById('chart_div'));  chart.draw(data, options); } google.setOnLoadCallback(drawVisualization);");

                    value = strScript.ToString();
                }
                finally
                {
                    strScript.Clear();
                }
                return value;
            }
        }

        protected string GetInsideTemp()
        {
            return temps.inside.ToString("f2") + " °F";
        }

        protected string GetOutsideTemp()
        {
            return temps.outside.ToString("f2") + " °F";
        }

        protected string GetMinTemp()
        {
            return minimum.ToString("f2") + " °F";
        }

        protected string GetMinDateTime()
        {
            return minDate.ToShortDateString() + "  " + minDate.ToShortTimeString();
        }

        protected string GetMaxTemp()
        {
            return maximum.ToString("f2") + " °F";
        }

        protected string GetMaxDateTime()
        {
            return maxDate.ToShortDateString() + "  " + maxDate.ToShortTimeString();
        }

        protected string GetAvgTemp()
        {
            return average.ToString("f2") + " °F";
        }

        protected string GetLatestTemps()
        {
            StringBuilder strLatestTemps = new StringBuilder();

            foreach (TempReading tempReading in tempReadings)
            {
                strLatestTemps.Append("<tr>");
                strLatestTemps.Append("<td>" + tempReading.Temperature.ToString("f2") + " °F</td>");
                strLatestTemps.Append("<td>@</td>");
                strLatestTemps.Append("<td>" + tempReading.Recorded.ToShortDateString() + "  " + tempReading.Recorded.ToShortTimeString() + "</td>");
                strLatestTemps.Append("</tr>");
            }

            return strLatestTemps.ToString();
        }
    }
}