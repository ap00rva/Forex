using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class OpenPrice
    {
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public object lastTraded { get; set; }
    }

    public class ClosePrice
    {
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public object lastTraded { get; set; }
    }

    public class HighPrice
    {
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public object lastTraded { get; set; }
    }

    public class LowPrice
    {
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public object lastTraded { get; set; }
    }

    public class Price
    {
        public string snapshotTime { get; set; }
        public DateTime snapshotTimeUTC { get; set; }
        public OpenPrice openPrice { get; set; }
        public ClosePrice closePrice { get; set; }
        public HighPrice highPrice { get; set; }
        public LowPrice lowPrice { get; set; }
        public int lastTradedVolume { get; set; }
    }

    public class Allowance
    {
        public int remainingAllowance { get; set; }
        public int totalAllowance { get; set; }
        public int allowanceExpiry { get; set; }
    }

    public class PageData
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public int totalPages { get; set; }
    }

    public class Metadata
    {
        public Allowance allowance { get; set; }
        public int size { get; set; }
        public PageData pageData { get; set; }
    }

    public class PriceData
    {
        public List<Price> prices { get; set; }
        public string instrumentType { get; set; }
        public Metadata metadata { get; set; }
    }
}
