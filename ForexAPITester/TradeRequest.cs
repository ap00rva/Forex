using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class TradeRequest
    {
        public string epic { get; set; }
        public string expiry { get; set; } = "DFB";
        public string direction { get; set; }
        public double size { get; set; }
        public string orderType { get; set; } = "MARKET";
        public string timeInForce { get; set; } = "EXECUTE_AND_ELIMINATE";
        public string level { get; set; }
        public bool guaranteedStop { get; set; } = true;
        public string stopLevel { get; set; }
        public int stopDistance { get; set; }
        public bool trailingStop { get; set; } = false;
        public string trailingStopIncrement { get; set; }
        public bool forceOpen { get; set; } = true;
        public string limitLevel { get; set; }
        public int limitDistance { get; set; }
        public string quoteId { get; set; }
        public string currencyCode { get; set; } = "GBP";
    }

    public enum Direction
    {
        BUY,
        SELL
    }
}
