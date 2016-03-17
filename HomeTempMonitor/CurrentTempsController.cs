using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using HomeTempMonitor.Classes;
using HomeTempMonitor.Models;

namespace HomeTempMonitor
{
    public class CurrentTempsController : ApiController
    {
        public Temps Get()
        {
            return CurrentTemps.GetTemps();
        }
    }
}