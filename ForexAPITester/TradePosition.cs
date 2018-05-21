using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class Position2
    {
        public double contractSize { get; set; }
        public string createdDate { get; set; }
        public DateTime createdDateUTC { get; set; }
        public string dealId { get; set; }
        public string dealReference { get; set; }
        public double size { get; set; }
        public string direction { get; set; }
        public double limitLevel { get; set; }
        public double level { get; set; }
        public string currency { get; set; }
        public bool controlledRisk { get; set; }
        public double stopLevel { get; set; }
        public object trailingStep { get; set; }
        public object trailingStopDistance { get; set; }
        public double limitedRiskPremium { get; set; }
    }

    public class Market
    {
        public string instrumentName { get; set; }
        public string expiry { get; set; }
        public string epic { get; set; }
        public string instrumentType { get; set; }
        public double lotSize { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double percentageChange { get; set; }
        public double netChange { get; set; }
        public double bid { get; set; }
        public double offer { get; set; }
        public string updateTime { get; set; }
        public string updateTimeUTC { get; set; }
        public int delayTime { get; set; }
        public bool streamingPricesAvailable { get; set; }
        public string marketStatus { get; set; }
        public int scalingFactor { get; set; }
    }

    public class Position
    {
        public Position2 position { get; set; }
        public Market market { get; set; }
    }

    public class TradePosition
    {
        public List<Position> positions { get; set; }
    }
}
