using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class CloseTradeRequest
    {
        public string dealId { get; set; }
        public string epic { get; set; }
        public string expiry { get; set; }
        public string direction { get; set; }
        public float size { get; set; }
        public string level { get; set; }
        public string orderType { get; set; } = "MARKET";
        public object timeInForce { get; set; }
        public object quoteId { get; set; }
    }
}
