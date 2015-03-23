using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace HomeTempMonitor
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Redirect mobile users to the mobile home page
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Browser.IsMobileDevice)
            {
                string path = httpRequest.Url.PathAndQuery;
                bool isOnMobilePage = path.StartsWith("/Mobile/",
                                       StringComparison.OrdinalIgnoreCase);
                if (!isOnMobilePage)
                {
                    string redirectTo = "~/Mobile/";

                    // Could also add special logic to redirect from certain 
                    // recognized pages to the mobile equivalents of those 
                    // pages (where they exist). For example,
                    // if (HttpContext.Current.Handler is UserRegistration)
                    //     redirectTo = "~/Mobile/Register.aspx";

                    HttpContext.Current.Response.Redirect(redirectTo);
                }
            }
        }

        public void Application_BeginRequest(object src, EventArgs e)
        {
            Context.Items["loadstarttime"] = DateTime.Now;
        }

        public void Application_EndRequest(object src, EventArgs e)
        {
            DateTime end = (DateTime)Context.Items["loadstarttime"];
            TimeSpan loadtime = DateTime.Now - end;
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Browser.IsMobileDevice)
            {
                Response.Write("<table style=\"margin-left:10px;margin-bottom:10px\"><tr><td style=\"width:100px\">Load Time:</td><td>" + loadtime.Milliseconds + "ms</td>");
            }
            else
            {
                Response.Write("<table><tr><td style=\"width:100px\">Load Time:</td><td>" + loadtime.Milliseconds + "ms</td>");
            }
        }
    }
}