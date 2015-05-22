using System;
using System.Runtime.Serialization;

namespace HomeTempMonitor.Models
{
    [DataContract]
    public class Temps
    {
        [DataMember]
        public double Inside { get; set; }
        [DataMember]
        public double Outside { get; set; }
    }
}