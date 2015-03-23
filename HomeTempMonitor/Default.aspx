<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HomeTempMonitor.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bean House Temperature Logger</title>
    <script src="//code.jquery.com/jquery-2.1.3.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <asp:Literal ID="chartScripts" runat="server"></asp:Literal>
    <script type="text/javascript">
        $(window).resize(function () {
            drawVisualization();
        });
    </script>
    <style>
        body {
            background-color:#f9f9f9;
        }

        table.temp {
            width:100%;
            margin-bottom:12px;
        }
        caption {
            font-weight:bold;
            font-size:24px;
        }
        td.temp {
            text-align:center;
        }

        div.chartarea {
            width:100%;
            border-radius:10px;
            border-style:solid;
            border-color:black;
            border-width:3px;
            /*padding-top:24px;*/
        }
    </style>
</head>
<body>
    <div style="width:100%;">
        <div style="text-align:center;"><h1>Bean House Temperature Logger</h1></div>
            <div class="chartarea">
                <form id="form1" runat="server">
                    <div style="width:100%;text-align:center;">
                        <p style="font-size:20px;"><strong>Current Temperature: </strong><asp:Label ID="lblCurrentTemp" runat="server"></asp:Label></p>
                        Time Range:
                        <asp:DropDownList ID="lstDataRange" runat="server" Height="19px" Width="209px" AutoPostBack="True">
                            <asp:ListItem Selected="True" Value="24">Last 24 Hours</asp:ListItem>
                            <asp:ListItem Value="48">Last 48 Hours</asp:ListItem>
                            <asp:ListItem Value="168">Last Week</asp:ListItem>
                            <asp:ListItem Value="720">Last Month</asp:ListItem>
                            <asp:ListItem Value="2160">Last 3 Months</asp:ListItem>
                            <asp:ListItem Value="4320">Last 6 Months</asp:ListItem>
                            <asp:ListItem Value="8760">Last Year</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </form>

                <div>
                    <h2 style="padding-left:2.5%">Temperature Chart</h2>
                    <div style="text-align:center;">
                        <div id="chart_div" style="margin:0 auto;width:95%;height:600px;"></div>
                    </div>
                </div>
            </div>
        
            <div style="margin-top:12px;">
                <table style="margin:0 auto;width:50%;">
                    <tr>
                        <td style="width:50%;">
                            <table class="temp">
                                <caption class="temp">Minimum Temperature</caption>
                                <tr>
                                    <td class="temp"><asp:Label ID="lblMinDate" runat="server"></asp:Label></td>
                                    <td class="temp"><asp:Label ID="lblMinVal" runat="server"></asp:Label> °F</td>
                                </tr>
                            </table>
                            <table class="temp">
                                <caption style="font-weight:bold;font-size:24px;">Maximum Temperature</caption>
                                <tr>
                                    <td class="temp"><asp:Label ID="lblMaxDate" runat="server"></asp:Label></td>
                                    <td class="temp"><asp:Label ID="lblMaxVal" runat="server"></asp:Label> °F</td>
                                </tr>
                            </table>
                            <table class="temp">
                                <caption class="temp">Average Temperature</caption>
                                <tr>
                                    <td class="temp"><asp:Label ID="lblAvgVal" runat="server"></asp:Label> °F</td>
                                </tr>
                            </table>
                        </td>
                        <td style="width:50%">
                            <asp:Table ID="tblTemps" Class="temp" Caption="Readings For Last Hour" runat="server">
                                <asp:TableHeaderRow><asp:TableHeaderCell>Date/Time</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Temperature</asp:TableHeaderCell></asp:TableHeaderRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
            </div>
        <hr style="color:black;background-color:black;height:3px;border:none;" />
        <table>
            <tr>
                <td style="width:100px">Generated:</td>
                <td><asp:Label ID="lblGenDate" runat="server"></asp:Label></td>
            </tr>
        </table>
    </div>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:templogConnectionString %>" SelectCommand="TempsForRange" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:ControlParameter ControlID="lstDataRange" DefaultValue="24" Name="hourRange" PropertyName="SelectedValue" DbType="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
</body>
</html>
