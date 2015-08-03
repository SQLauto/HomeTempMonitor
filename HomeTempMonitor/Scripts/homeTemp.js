var dtFormat = "m/d/yyyy h:MM TT";
var dataSet;
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

function loadTempData() {
    $.getJSON("/Temps/FullData/" + $("#lstDataRange").val(), function (data) {
        $.each(data, function (key, val) {
            if (key == "AverageTemp") {
                $("#averageTemp").text(val.toFixed(2) + " °F");
            }
            else if (key == "MaximumTemp") {
                $("#maxTemp").text(val["Temperature"].toFixed(2) + " °F");
                $("#maxTempDate").text(dateFormat(new Date(val["Recorded"]), dtFormat));
            }
            else if (key == "MinimumTemp") {
                $("#minTemp").text(val["Temperature"].toFixed(2) + " °F");
                $("#minTempDate").text(dateFormat(new Date(val["Recorded"]), dtFormat));
            }
            else if (key == "LastHour") {
                var tblTempBody = $("#tblLastTemps tbody");
                tblTempBody.html("");
                $.each(val, function (k, v) {
                    var newRow = $("<tr>");
                    $("<td>").text(dateFormat(new Date(v["Recorded"]), dtFormat)).appendTo(newRow);
                    $("<td>").text(v["Temperature"].toFixed(2) + " °F").appendTo(newRow);
                    tblTempBody.append(newRow);
                });
            }
            else if (key == "DataSet") {
                dataSet = [];
                $.each(val, function (k, v) {
                    var newDataPoint = {};
                    newDataPoint["x"] = new Date(v["Recorded"]);
                    newDataPoint["y"] = v["Temperature"];
                    dataSet.push(newDataPoint);
                });
                renderChart();
            }
        });
    });
    $("#updatedTime").text(dateFormat(Date.now(), "m/d/yyyy h:MM:ss TT"));
}
setInterval("loadTempData()", 900000);

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