using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class PriceViewPair
    {
        public List<PriceView> PricePair;
        public PriceViewPair()
        {
            PricePair = new List<PriceView>();
        }
        public PriceViewPair(PriceView p1, PriceView p2) : this()
        {
            PricePair.Add(p1);
            PricePair.Add(p2);
        }
        public decimal HighestHigh()
        {
            return PricePair.Max(p => p.AskHigh);
        }
        public decimal LowestLow()
        {
            return PricePair.Min(p => p.AskLow);
        }

    }
}
