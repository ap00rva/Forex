using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForexAPITester.Library;

namespace ForexAPITester
{
    public static class SwitchHelper
    {
        public static DateTime SwitchSearchStartTime { get; set; }
        public static DateTime SwitchSearchEndTime { get; set; }
        public static bool SwitchStarted { get; set; }
        public static EventHandler ConditionMet = delegate { };
        public static decimal PriceToCheck { get; set; } = 0;
        static SwitchHelper()
        {
            SwitchSearchStartTime = DateTime.Today.AddDays(100);
            SwitchSearchEndTime = DateTime.Today.AddDays(100);
            SwitchStarted = false;
        }
        public static bool IsSwitchSearchInProgress()
        {
            return DateTime.Now >= SwitchSearchStartTime && DateTime.Now <= SwitchSearchEndTime;
        }
        public static TradeDirection SwitchDirection { get; set; }

        public static bool IsSwitchConditionMet(PriceView CurrentData)
        {
            if (IsSwitchSearchInProgress())
            {
                if (SwitchDirection == TradeDirection.Sell)
                {
                    return CurrentData.AskHigh >= PriceToCheck;
                }
                else if (SwitchDirection == TradeDirection.Buy)
                {
                    return CurrentData.AskLow <= PriceToCheck;
                }

            }
            return false;
        }
    }
}
