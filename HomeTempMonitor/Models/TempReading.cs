using System;

namespace HomeTempMonitor.Models
{
    public class TempReading
    {
        public DateTime Recorded { get; set; }
        public double Temperature { get; set; }
    }
}