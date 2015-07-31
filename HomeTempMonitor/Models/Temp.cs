using System;
using System.Runtime.Serialization;

namespace HomeTempMonitor.Models
{
    [DataContract]
    public class Temp
    {
        private DateTime _recorded;
        [DataMember]
        public double Temperature { get; set; }
        [DataMember]
        public DateTime Recorded
        {
            get { return _recorded.ToUniversalTime().Subtract(new TimeSpan(0, 0, 0, _recorded.Second, _recorded.Millisecond)); }
            set { _recorded = value; }
        }
    }
}