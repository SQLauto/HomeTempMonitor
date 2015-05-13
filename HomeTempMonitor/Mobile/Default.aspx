<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HomeTempMonitor.Mobile.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bean House Temperature Logger</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />
    <link rel="apple-touch-icon" href="touch-icon-iphone.png" />
    <link rel="apple-touch-icon" sizes="76x76" href="touch-icon-ipad.png" />
    <link rel="apple-touch-icon" sizes="120x120" href="touch-icon-iphone-retina.png" />
    <link rel="apple-touch-icon" sizes="152x152" href="touch-icon-ipad-retina.png" />
    <link rel="apple-touch-icon" sizes="180x180" href="touch-icon-iphone-retina-3x.png" />
    <link rel="stylesheet" href="//code.jquery.com/mobile/1.4.5/jquery.mobile-1.4.5.min.css" />
    <script src="//code.jquery.com/jquery-2.1.3.min.js"></script>
    <script src="//code.jquery.com/mobile/1.4.5/jquery.mobile-1.4.5.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type='text/javascript'>
        google.load('visualization', '1', { packages: ['corechart'] });
    </script>
    <script type="text/javascript">
        function drawVisualization() {
            var data = google.visualization.arrayToDataTable([
            ['Time', 'Temperature (°F)']<%= GetTempData() %>]);
            var options = <%= GetChartOptions() %>;
            var chart = new google.visualization.LineChart(document.getElementById('chart_div')); chart.draw(data, options);
        } google.setOnLoadCallback(drawVisualization);
    </script>
    <script type="text/javascript">
        $(window).resize(function () {
            drawVisualization();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style='margin:10px 10px 0px 10px; position:relative'>
        <div style="text-align:center;">
            <span style="font-weight:bold;font-size:20px;">Bean House Temperature Logger</span>
            <p style="font-size:16px;"><span style="font-weight:bold;">Current Temp: </span><%= GetCurrentInsideTemp() %> °F</p>
            <div id="lstDiv" runat="server">
                <asp:DropDownList ID="lstDataRange" runat="server" AutoPostBack="True" Width="100%">
                    <asp:ListItem Selected="True" Value="24">Last 24 Hours</asp:ListItem>
                    <asp:ListItem Value="48">Last 48 Hours</asp:ListItem>
                    <asp:ListItem Value="168">Last Week</asp:ListItem>
                    <asp:ListItem Value="720">Last Month</asp:ListItem>
                    <asp:ListItem Value="2160">Last 3 Months</asp:ListItem>
                    <asp:ListItem Value="4320">Last 6 Months</asp:ListItem>
                    <asp:ListItem Value="8760">Last Year</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div style="margin-top:15px;">
            <asp:Panel ID="chart_div" style="margin:0 auto;" runat="server"></asp:Panel>
        </div>
        <table>
            <tr>
                <td style="width:100px"><span style="font-weight:bold">Min Temp:</span></td>
                <td><%= GetMinTemp() %> °F</td>
                <td>@</td>
                <td><%= GetMinDateTime() %></td>
            </tr>
            <tr>
                <td style="width:100px"><span style="font-weight:bold">Max Temp:</span></td>
                <td><%= GetMaxTemp() %> °F</td>
                <td>@</td>
                <td><%= GetMaxDateTime() %></td>
            </tr>
            <tr>
                <td style="width:100px"><span style="font-weight:bold">Avg Temp:</span></td>
                <td><%= GetAvgTemp() %> °F</td>
            </tr>
        </table>
        <br />
        <span style="font-weight:bold">Last Hour</span>
        <asp:Table ID="tblTemps" runat="server">
        </asp:Table>
        <br />
        <table>
            <tr>
                <td style="width:100px">Generated:</td>
                <td><%= GetFormattedDateString(DateTime.Now) %></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
