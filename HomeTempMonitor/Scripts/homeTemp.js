var dtFormat = "m/d/yyyy h:MM TT";
var dataSet;
var minTemp;
var maxTemp;
var avgTemp;
var lineThickness;

function getLineThickness() {
    var selectedValue = parseInt($("#lstDataRange").val());
    if (navigator.userAgent.match(/iPhone/i)) {
        if (selectedValue <= 48)
            lineThickness = 1;
        else
            lineThickness = 0.5;
    }
    else if (navigator.userAgent.match(/iPad/i)) {
        if (selectedValue <= 168)
            lineThickness = 1;
        else
            lineThickness = 0.5;
    }
    else
        lineThickness = 1;
}

function loadCurrentTemps() {
    $.getJSON("/CurrentTemps", function (data) {
        $.each(data, function (key, val) {
            if (key == "Inside") {
                $("#insideTemp").text(val.toFixed(2));
            }
        });
    });
}
setInterval("loadCurrentTemps()", 60000);

function loadChartTempData() {
    var endDate = new Date();
    var startDate = new Date(endDate);
    var hours = parseInt($("#lstDataRange").val());
    startDate.setTime(startDate.getTime() - hours * 3600000);
    var dateRangeUrl = "/Temps/DateRange?startDateString=" + startDate.toGMTString() + "&endDateString=" + endDate.toGMTString();

    $.getJSON(dateRangeUrl, function (data) {
        dataSet = [];
        var totalTemp = 0;

        for (var x = 0; x < data.length; x++) {
            var newDataPoint = {};
            newDataPoint["x"] = new Date(data[x]["Recorded"]);
            newDataPoint["y"] = data[x]["Temperature"];
            dataSet.push(newDataPoint);

            totalTemp += data[x]["Temperature"];

            if (data[x] == data[0]) {
                minTemp = data[x];
                maxTemp = data[x];
            } else {
                if (data[x]["Temperature"] <= minTemp["Temperature"]) {
                    minTemp = data[x];
                } else if (data[x]["Temperature"] >= maxTemp["Temperature"]) {
                    maxTemp = data[x];
                }
            }
        }
        avgTemp = totalTemp / dataSet.length;

        var tblMinTempBody = $("#minTempTable tbody");
        tblMinTempBody.html("");
        var newMinTempRow = $("<tr>");
        $("<td>").text(dateFormat(new Date(minTemp["Recorded"]), dtFormat)).appendTo(newMinTempRow);
        $("<td>").text(minTemp["Temperature"].toFixed(2) + " °F").appendTo(newMinTempRow);
        tblMinTempBody.append(newMinTempRow);

        var tblMaxTempBody = $("#maxTempTable tbody");
        tblMaxTempBody.html("");
        var newMaxTempRow = $("<tr>");
        $("<td>").text(dateFormat(new Date(maxTemp["Recorded"]), dtFormat)).appendTo(newMaxTempRow);
        $("<td>").text(maxTemp["Temperature"].toFixed(2) + " °F").appendTo(newMaxTempRow);
        tblMaxTempBody.append(newMaxTempRow);

        var tblAvgTempBody = $("#avgTempTable tbody");
        tblAvgTempBody.html("");
        var newAvgTempRow = $("<tr>");
        $("<td>").text(avgTemp.toFixed(2) + " °F").appendTo(newAvgTempRow);
        tblAvgTempBody.append(newAvgTempRow);

        renderChart();
    });
}

function loadLastHour() {
    $.getJSON("/Temps/LastHour", function (data) {
        var tblTempBody = $("#tblLastTemps tbody");
        tblTempBody.html("");
        for (var x = 0; x < data.length; x++) {
            var newRow = $("<tr>");
            $("<td>").text(dateFormat(new Date(data[x]["Recorded"]), dtFormat)).appendTo(newRow);
            $("<td>").text(data[x]["Temperature"].toFixed(2) + " °F").appendTo(newRow);
            tblTempBody.append(newRow);
        }
    });
}

setInterval("loadTempData()", 900000);

function loadTempData() {
    loadChartTempData();
    loadLastHour();
    $("#updatedTime").text(dateFormat(Date.now(), "m/d/yyyy h:MM:ss TT"));
}

function renderChart() {
    $("#chart_div").html("");
    var chart = new CanvasJS.Chart("chart_div", {
        data: [
            {
                type: "line",
                markerType: "none",
                color: "blue",
                lineThickness: lineThickness,
                dataPoints: dataSet
            }
        ],
        axisY: {
            title: "Temperature (°F)",
            titleFontSize: 18,
            titleFontStyle: "italic",
            labelFontSize: 12,
            includeZero: false
        },
        axisX: {
            labelAngle: -45,
            labelFontSize: 12,
            valueFormatString: "M/D/YYYY h:mm TT"
        },
        toolTip: {
            content: "Recorded: {x}<br/>Temperature: {y} °F"
        }
    });

    chart.render();
}

function setStatsClasses() {
    var minMaxAvg = $("#minMaxAvg");
    var lastHour = $("#lastHour");

    minMaxAvg.removeClass();
    lastHour.removeClass();

    if (window.innerWidth < 768 && (window.orientation == 90 || window.orientation == -90)) {
        minMaxAvg.addClass("col-lg-6 col-md-6 col-sm-6 col-xs-6");
        lastHour.addClass("col-lg-6 col-md-6 col-sm-6 col-xs-6");
    } else {
        minMaxAvg.addClass("col-lg-6 col-md-6 col-sm-6 col-xs-12");
        lastHour.addClass("col-lg-6 col-md-6 col-sm-6 col-xs-12");
    }
}

window.onload = function () {
    $(window).on("orientationchange", function (event) {
        setStatsClasses();
    });

    setStatsClasses();
    loadCurrentTemps();
    getLineThickness();
    loadTempData();
}