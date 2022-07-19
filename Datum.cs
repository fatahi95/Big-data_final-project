using System;
using System.Collections.Generic;

namespace WebApplication2.Model
{
    public partial class Datum
    {
        public DateTime DateTime { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Base { get; set; } = null!;
    }
}
