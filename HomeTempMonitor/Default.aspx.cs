using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

using HomeTempMonitor.Classes;

namespace HomeTempMonitor
{
    public partial class Default : System.Web.UI.Page
    {
        private Temps temps = new Temps();

        protected async void Page_Load(object sender, EventArgs e)
        {
            temps = await CurrentTemps.GetTempsAsync();

            lblCurrentTemp.Text = temps.inside.ToString("f2") + " °F";

            using (DataView dataView = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty))
            {
                DataRow firstRow = dataView.Table.Rows[0];

                DateTime minDate = new DateTime();
                DateTime maxDate = new DateTime();
                double minimum = (double)firstRow["Temperature"];
                double maximum = (double)firstRow["Temperature"];
                double average = 0;

                List<TempReading> tempReadings = new List<TempReading>();

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

                StringBuilder strScript = new StringBuilder();

                try
                {
                    strScript.Append(@"<script type='text/javascript'>  
                    google.load('visualization', '1', {packages: ['corechart']});</script>  
  
                    <script type='text/javascript'>  
                    function drawVisualization() {         
                    var data = google.visualization.arrayToDataTable([  
                    ['Time', 'Temperature (°F)'],");

                    foreach (DataRow row in dataView.Table.Rows)
                    {
                        DateTime recorded = (DateTime)row["Recorded"];
                        strScript.Append("['" + recorded.ToString("M/d/yyyy h:mm tt") + "'," + row["Temperature"].ToString() + "],");
                    }
                    strScript.Remove(strScript.Length - 1, 1);
                    strScript.Append("]);");

                    Int32 hourRange = Convert.ToInt32(lstDataRange.SelectedValue);

                    if (hourRange <= 48)
                        strScript.Append(@"var options = { vAxis: { title: 'Temperature (°F)' }, legend: 'none', chartArea: { left: 150, top: 10, width: '100%', height: '80%' } };");
                    else
                        strScript.Append(@"var options = { vAxis: { title: 'Temperature (°F)' }, lineWidth: 1, legend: 'none', chartArea: { left: 150, top: 10, width: '100%', height: '80%' } };");

                    strScript.Append(" var chart = new google.visualization.LineChart(document.getElementById('chart_div'));  chart.draw(data, options); } google.setOnLoadCallback(drawVisualization);");
                    strScript.Append(" </script>");

                    chartScripts.Text = strScript.ToString();
                }
                finally
                {
                    strScript.Clear();
                }

                lblMinDate.Text = minDate.ToShortDateString() + "  " + minDate.ToShortTimeString();
                lblMinVal.Text = minimum.ToString("f2");

                lblMaxDate.Text = maxDate.ToShortDateString() + "  " + maxDate.ToShortTimeString();
                lblMaxVal.Text = maximum.ToString("f2");

                lblAvgVal.Text = average.ToString("f2");

                foreach (TempReading tempReading in tempReadings)
                {
                    TableCell dateCell = new TableCell();
                    TableCell tempCell = new TableCell();

                    dateCell.Text = tempReading.Recorded.ToShortDateString() + "  " + tempReading.Recorded.ToShortTimeString();
                    dateCell.CssClass = "temp";
                    tempCell.Text = tempReading.Temperature.ToString("f2") + " °F";
                    tempCell.CssClass = "temp";

                    TableRow tableRow = new TableRow();
                    tableRow.Cells.Add(dateCell);
                    tableRow.Cells.Add(tempCell);

                    tblTemps.Rows.Add(tableRow);
                }

                lblGenDate.Text = DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString();
            }
        }
    }
}