using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class SwitchData
    {
        public string Market { get; set; }
        public DateTime SwitchDate { get; set; }
        public string SwitchDirection { get; set; }
        public decimal SwitchAmount { get; set; }
        public bool FlashTrade { get; set; }
        public bool ProbeTrade { get; set; }
        public DateTime? ProbeTradeDate { get; set; }
    }
}
