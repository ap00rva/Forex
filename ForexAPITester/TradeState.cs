using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class TradeState
    {
        public string Epic { get; set; }
        public string Interval { get; set; }
        public string SignallingWindow { get; set; }
        public string ProbeWindow { get; set; }
        public string FilterWindow { get; set; }
        public string Relations { get; set; }
        public decimal Size { get; set; }
        public int StopDistance { get; set; }
        public int LimitDistance { get; set; }

    }
}
