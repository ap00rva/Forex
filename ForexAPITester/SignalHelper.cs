using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public static class SignalHelper
    {
        public static PriceView FindRelated(string reversalType, List<PriceView> pViews, int probeWindowMinutes = 30)
        {

            List<PriceView> relatedBars = new List<PriceView>();
            var probe = pViews.Last();
            Logger.WriteLog($"Probe starts with:{pViews.First()?.SnapshotDateTime} - {probe?.SnapshotDateTime} - {probe.AskUpDown} - {reversalType} High:{probe.AskHigh} Low:{probe.AskLow} Open:{probe.AskOpen} Close:{probe.AskClose}");
            bool isProbeSuccessfull = false;
            bool foundAll = false;
            string probeLog = string.Empty;
            for (int i = pViews.Count - 2; i >= 1; i--)
            {
                foundAll = false;
                var p1 = pViews[i];
                var p2 = pViews[i - 1];
                //if the 2br is too far behind the probe then no point
                if ((probe.SnapshotDateTime - p1.SnapshotDateTime).TotalMinutes > probeWindowMinutes)
                {
                    break;
                }
                if (p1.Ask2BarReversal && p1.AskUpDown == reversalType)
                {
                    //found first reversal, now first check if there is a probe and if so then find another pair
                    //else no use in working on it

                    if (reversalType == Library.DownBar)
                    {
                        ////for Down reversal pairs Up and Down
                        ////p1 is down and p2 is up
                        //successful Down probe
                        isProbeSuccessfull = (probe.AskUpDown == Library.UpBar && probe.AskHigh >= p1.AskOpen && probe.AskClose <= Math.Max(p1.AskHigh, p2.AskHigh)); //&& probe.AskLow <= p1.AskOpen && probe.AskClose >= Math.Min(p1.AskLow, p2.AskLow));
                        if (isProbeSuccessfull)
                        {
                            probeLog = $"{probe.TimeCategory}-{p1.TimeCategory}-Probe sucess:{probe.AskUpDown}-High:{probe.AskHigh}-Open:{p1.AskOpen}-Close:{probe.AskClose}-{Math.Max(p1.AskHigh, p2.AskHigh)}";
                        }
                    }
                    else
                    {
                        //for Up reversal pairs Down and Up
                        //p1 is up and p2 is down
                        //successful Up probe
                        isProbeSuccessfull = (probe.AskUpDown == Library.DownBar && probe.AskLow <= p1.AskOpen && probe.AskClose >= Math.Min(p1.AskLow, p2.AskLow)); //&& probe.AskHigh >= p2.AskOpen && probe.AskClose <= Math.Max(p1.AskHigh, p2.AskHigh));
                        if (isProbeSuccessfull)
                        {
                            probeLog = $"{probe.TimeCategory}-{p1.TimeCategory}-Probe sucess:{probe.AskUpDown}-Low:{probe.AskLow}-Open:{p1.AskOpen}-Close:{probe.AskClose}-{Math.Min(p1.AskLow, p2.AskLow)}";
                        }
                    }

                    if (isProbeSuccessfull)
                    {
                        for (int j = i - 2; j >= 1; j--)
                        {
                            var p3 = pViews[j];
                            var p4 = pViews[j - 1];

                            ////for Down reversal pairs Up and Down
                            ////p1 is down and p2 is up
                            ////p3 is down and p4 is up

                            //for Up reversal pairs Down and Up
                            //p1 is up and p2 is down
                            //p3 is up and p4 is down
                            if (reversalType == Library.UpBar)
                            {
                                if (p3.Ask2BarReversal && p3.AskUpDown == reversalType &&
                                    (isUpBarConditionMet(new List<PriceView>() { p1, p2 }, new List<PriceView>() { p3, p4 })))
                                {
                                    string up1 = $"{p1.TimeCategory}-{p3.TimeCategory}-UpBarCondition1/1 met:Low:{p1.AskLow}-Open:{p4.AskOpen}-Close:{p1.AskClose}-Low:{Math.Min(p3.AskLow, p4.AskLow)}";
                                    string up2 = $"{p2.TimeCategory}-{p4.TimeCategory}-UpBarCondition2/1 met:Low:{p2.AskLow}-Open:{Math.Min(p3.AskLow, p4.AskLow)}-Close:{p2.AskClose}-Low:{Math.Min(p3.AskLow, p4.AskLow)}";
                                    //now look for a third with the same conditions
                                    for (int k = j - 2; k >= 1; k--)
                                    {
                                        var p5 = pViews[k];
                                        var p6 = pViews[k - 1];
                                        if (p5.Ask2BarReversal && p5.AskUpDown == reversalType &&
                                    (isUpBarConditionMet(new List<PriceView>() { p3, p4 }, new List<PriceView>() { p5, p6 }, true)))
                                        {
                                            Logger.WriteLog(probeLog);
                                            Logger.WriteLog(up1);
                                            Logger.WriteLog(up2);
                                            Logger.WriteLog($"{p3.TimeCategory}-{p5.TimeCategory}-UpBarCondition1/2 met:Low:{p3.AskLow}-Low:{Math.Min(p5.AskLow, p6.AskLow)}-Close:{p3.AskClose}-Low:{Math.Min(p5.AskLow, p6.AskLow)}");
                                            Logger.WriteLog($"{p4.TimeCategory}-{p6.TimeCategory}-UpBarCondition2/2 met:Low:{p4.AskLow}-Low:{p6.AskOpen}-Close:{p4.AskClose}-Low:{Math.Min(p5.AskLow, p6.AskLow)}");
                                            //found all pairs
                                            probe.IsDownProbe = true;
                                            probe.ProbeRelations = $"{p2.TimeCategory},{p4.TimeCategory},{p6.TimeCategory}";
                                            //set them as green and exit
                                            //colourGridRows(relatedBars, Color.LightGreen);
                                            foundAll = true;
                                            break;
                                        }
                                    }
                                    if (foundAll)
                                    {
                                        break;
                                    }
                                }
                            }
                            else if (reversalType == Library.DownBar)
                            {
                                if (p3.Ask2BarReversal && p3.AskUpDown == reversalType &&
                                    (isDownBarConditionMet(new List<PriceView>() { p1, p2 }, new List<PriceView>() { p3, p4 })))
                                {
                                    string down1 = $"{p1.TimeCategory}-{p3.TimeCategory}-DownBarCondition1/1 met:High:{p1.AskHigh}-Open:{p4.AskOpen}-Close:{p1.AskClose}-High:{Math.Max(p3.AskHigh, p4.AskHigh)}";
                                    string down2 = $"{p2.TimeCategory}-{p4.TimeCategory}-DownBarCondition2/1 met:High:{p2.AskHigh}-Open:{p4.AskOpen}-Close:{p2.AskClose}-High:{Math.Max(p3.AskHigh, p4.AskHigh)}";

                                    //now look for a third with the same conditions
                                    for (int k = j - 2; k >= 1; k--)
                                    {
                                        var p5 = pViews[k];
                                        var p6 = pViews[k - 1];
                                        if (p5.Ask2BarReversal && p5.AskUpDown == reversalType &&
                                    (isDownBarConditionMet(new List<PriceView>() { p3, p4 }, new List<PriceView>() { p5, p6 }, true)))
                                        {
                                            Logger.WriteLog(probeLog);
                                            Logger.WriteLog(down1);
                                            Logger.WriteLog(down2);
                                            Logger.WriteLog($"{p3.TimeCategory}-{p5.TimeCategory}-DownBarCondition1/2 met:High:{p3.AskHigh}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}-Close:{p3.AskClose}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}");
                                            Logger.WriteLog($"{p4.TimeCategory}-{p6.TimeCategory}-DownBarCondition2/2 met:High:{p4.AskHigh}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}-Close:{p4.AskClose}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}");

                                            //found all pairs
                                            probe.IsUpProbe = true;
                                            probe.ProbeRelations = $"{p2.TimeCategory},{p4.TimeCategory},{p6.TimeCategory}";
                                            //set them as green and exit
                                            //colourGridRows(relatedBars, Color.Orange);
                                            foundAll = true;
                                            break;
                                        }
                                    }
                                    if (foundAll)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (foundAll)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //probe was not successfull with this 2BR, so keep on looking for another 2BR
                        //break;
                    }
                    if (foundAll)
                    {
                        break;
                    }
                }
                else
                {
                    //2BR was not found, so keep on looking back
                    //break;
                }
            }
            //if (foundAll)
            //{
            //    MessageBox.Show("relation found");
            //}
            return probe;

        }
        private static bool isUpBarConditionMet(List<PriceView> currentPair, List<PriceView> previousPair, bool isThird = false)
        {
            //current pair 0 is Down
            //current pair 1 is Up
            //previous pair 0 is Down
            //previous pair 1 is Up
            if (currentPair.Count != 2 || previousPair.Count != 2)
            {
                return false;
            }
            if (!isThird)
            {
                return (currentPair[0].AskLow <= previousPair[0].AskOpen && currentPair[0].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow))
                    || (currentPair[1].AskLow <= previousPair[0].AskOpen && currentPair[1].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow));
            }
            else
            {
                return (currentPair[0].AskLow < Math.Min(previousPair[0].AskLow, previousPair[1].AskLow) && (currentPair[0].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow))
                    || currentPair[1].AskLow < Math.Min(previousPair[0].AskLow, previousPair[1].AskLow)) && (currentPair[1].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow));
            }

        }
        private static bool isDownBarConditionMet(List<PriceView> currentPair, List<PriceView> previousPair, bool isThird = false)
        {
            //current pair 0 is Up
            //current pair 1 is Down
            //previous pair 0 is Up
            //previous pair 1 is Down

            if (currentPair.Count != 2 || previousPair.Count != 2)
            {
                return false;
            }
            if (!isThird)
            {
                return (currentPair[0].AskHigh >= previousPair[0].AskOpen && currentPair[0].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh))
                    || (currentPair[1].AskHigh >= previousPair[0].AskOpen && currentPair[1].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh));
            }
            else
            {
                return (currentPair[0].AskHigh > Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh) && (currentPair[0].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh))
                    || currentPair[1].AskHigh > Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh)) && (currentPair[1].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh));
            }
        }

        public static PriceView CheckQualification(PriceViewPair pricePair, PriceView probe, IEnumerable<PriceView> pViews, int filterWindowMins)
        {

            DateTime endTime = pricePair.PricePair[0].SnapshotDateTime;
            DateTime lookBackTime = endTime.AddMinutes(filterWindowMins);
            var filterDataToCheck = pViews.Where(p => p.SnapshotDateTime >= lookBackTime && p.SnapshotDateTime <= endTime).OrderBy(p => p.SnapshotDateTime);
            if (probe.IsUpProbe)
            {
                //find highesthigh
                var filterHigh = filterDataToCheck.Max(p => p.AskHigh);
                if (pricePair.HighestHigh() >= filterHigh)
                {
                    probe.ProbeQualified = true;
                    probe.ProbeQualifications = pricePair.PricePair[0].TimeCategory;
                }
            }
            else if (probe.IsDownProbe)
            {
                //find lowestlow
                var filterLow = filterDataToCheck.Min(p => p.AskLow);
                if (pricePair.LowestLow() <= filterLow)
                {
                    probe.ProbeQualified = true;
                    probe.ProbeQualifications = pricePair.PricePair[0].TimeCategory;
                }
            }
            return probe;
        }

    }
}
