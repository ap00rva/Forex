using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class TradeClose
    {
        public string dealId { get; set; }
        public string epic { get; set; }
        public string expiry { get; set; }
        public string direction { get; set; }
        public string size { get; set; }
        public string level { get; set; }
        public string orderType { get; set; } = "MARKET";
        public string timeInForce { get; set; }
        public string quoteId { get; set; }
    }
}
