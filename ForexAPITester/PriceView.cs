using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class PriceView
    {
        public string SnapshotTime { get; set; }
        public DateTime SnapshotDateTime
        {
            get
            {
                return DateTime.ParseExact(SnapshotTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }
        public string TimeCategory
        {
            get
            {
                return DateTime.Parse(SnapshotTime).ToString("HH:mm");
            }
        }
        //public double BidOpen { get; set; }
        //public double BidClose { get; set; }
        //public double BidHigh { get; set; }
        //public double BidLow { get; set; }
        //public string BidUpDown
        //{
        //    get
        //    {
        //        return BidClose>=BidOpen ? "Up Bar" : "Down Bar";
        //    }
        //}
        //public bool Bid2BarReversal { get; set; }
        public decimal AskOpen { get; set; }
        public decimal AskClose { get; set; }
        public decimal AskHigh { get; set; }
        public decimal AskLow { get; set; }
        public string AskUpDown
        {
            get
            {
                return AskClose > AskOpen ? "Up Bar" : (AskClose < AskOpen ? "Down Bar" : "None");
            }
        }
        public bool Ask2BarReversal { get; set; }
        public bool IsUpRelated { get; set; } = false;
        public bool IsDownRelated { get; set; } = false;
        public bool IsUpProbe { get; set; } = false;
        public bool IsDownProbe { get; set; } = false;
        public string ProbeRelations { get; set; }
        public bool ProbeQualified { get; set; } = false;
        public string ProbeQualifications { get; set; }
    }

    public class PriceViewComparer : IEqualityComparer<PriceView>
    {
        public bool Equals(PriceView x, PriceView y)
        {
            return DateTime.Compare(x.SnapshotDateTime, y.SnapshotDateTime) == 0;
        }

        public int GetHashCode(PriceView obj)
        {
            return obj.GetHashCode();
        }
    }
}
