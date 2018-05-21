using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class Currency
    {
        public string code { get; set; }
        public string symbol { get; set; }
        public double baseExchangeRate { get; set; }
        public double exchangeRate { get; set; }
        public bool isDefault { get; set; }
    }

    public class MarginDepositBand
    {
        public double min { get; set; }
        public double? max { get; set; }
        public double margin { get; set; }
        public string currency { get; set; }
    }

    public class SlippageFactor
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class LimitedRiskPremium
    {
        public double value { get; set; }
        public string unit { get; set; }
    }

    public class ExpiryDetails
    {
        public string lastDealingDate { get; set; }
        public string settlementInfo { get; set; }
    }

    public class Instrument
    {
        public string epic { get; set; }
        public string expiry { get; set; }
        public string name { get; set; }
        public bool forceOpenAllowed { get; set; }
        public bool stopsLimitsAllowed { get; set; }
        public double lotSize { get; set; }
        public string unit { get; set; }
        public string type { get; set; }
        public bool controlledRiskAllowed { get; set; }
        public bool streamingPricesAvailable { get; set; }
        public string marketId { get; set; }
        public List<Currency> currencies { get; set; }
        public object sprintMarketsMinimumExpiryTime { get; set; }
        public object sprintMarketsMaximumExpiryTime { get; set; }
        public List<MarginDepositBand> marginDepositBands { get; set; }
        public double marginFactor { get; set; }
        public string marginFactorUnit { get; set; }
        public SlippageFactor slippageFactor { get; set; }
        public LimitedRiskPremium limitedRiskPremium { get; set; }
        public object openingHours { get; set; }
        public ExpiryDetails expiryDetails { get; set; }
        public object rolloverDetails { get; set; }
        public string newsCode { get; set; }
        public string chartCode { get; set; }
        public object country { get; set; }
        public object valueOfOnePip { get; set; }
        public object onePipMeans { get; set; }
        public object contractSize { get; set; }
        public object specialInfo { get; set; }
    }

    public class MinStepDistance
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class MinDealSize
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class MinControlledRiskStopDistance
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class MinNormalStopOrLimitDistance
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class MaxStopOrLimitDistance
    {
        public string unit { get; set; }
        public double value { get; set; }
    }

    public class DealingRules
    {
        public MinStepDistance minStepDistance { get; set; }
        public MinDealSize minDealSize { get; set; }
        public MinControlledRiskStopDistance minControlledRiskStopDistance { get; set; }
        public MinNormalStopOrLimitDistance minNormalStopOrLimitDistance { get; set; }
        public MaxStopOrLimitDistance maxStopOrLimitDistance { get; set; }
        public string marketOrderPreference { get; set; }
        public string trailingStopsPreference { get; set; }
    }

    public class Snapshot
    {
        public string marketStatus { get; set; }
        public double netChange { get; set; }
        public double percentageChange { get; set; }
        public string updateTime { get; set; }
        public int delayTime { get; set; }
        public double bid { get; set; }
        public double offer { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public object binaryOdds { get; set; }
        public int decimalPlacesFactor { get; set; }
        public int scalingFactor { get; set; }
        public int controlledRiskExtraSpread { get; set; }
    }

    public class MarketInfo
    {
        public Instrument instrument { get; set; }
        public DealingRules dealingRules { get; set; }
        public Snapshot snapshot { get; set; }
    }
}
