using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeTempMonitor.Classes
{
    public static class DateTimeToString
    {
        public static string FormattedDateString(DateTime dateTime)
        {
            return dateTime.ToShortDateString() + "  " + dateTime.ToShortTimeString();
        }

        public static string FormattedDateStringWithSeconds(DateTime dateTime)
        {
            return dateTime.ToShortDateString() + "  " + dateTime.ToLongTimeString();
        }
    }
}