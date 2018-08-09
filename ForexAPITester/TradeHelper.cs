using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForexAPITester.Library;

namespace ForexAPITester
{
    public static class TradeHelper
    {
        private static TradeDirection lastTradeDirection;
        private static string tradeRef;
        private static double size;
        private static string tradeUrl;
        private static APICredentials apiCredentials = new APICredentials();
        private static APICredentials dataCredentials = new APICredentials();
        private static string tradeXToken;
        private static string tradeCstToken;
        private static AccountMain acctTrade;
        private static int currentTrades;
        private static bool closeExistingTrades = false;

        public static SARCloseOptions CloseOptions { get; set; } = SARCloseOptions.CloseNow;
        public static string TradeUrl
        {
            get
            {
                return tradeUrl;
            }
            set
            {
                if (tradeUrl != value)
                {
                    tradeUrl = value;
                    LoginToTradingAccount();
                }
            }
        }
        public static bool CloseExistingTrades
        {
            get
            {
                return closeExistingTrades;
            }
            set
            {
                closeExistingTrades = value;
                if (value && CloseOptions == SARCloseOptions.CloseNow)
                {
                    CloseAllTrades();
                }
            }
        }
        static TradeHelper()
        {

            tradeUrl = "https://demo-api.ig.com/gateway";
            currentTrades = 0;
            LoginToTradingAccount();
        }

        public static void LoginToTradingAccount()
        {
            string loginMessage = string.Empty;
            Library.ReadCredentials(ref dataCredentials, ref apiCredentials);
            Login tradeLogin = new Login() { encryptedPassword = null };
            string apiKey;
            if (tradeUrl == Library.DemoTradingUrl)
            {
                tradeLogin.identifier = apiCredentials.UserName;
                tradeLogin.password = apiCredentials.Password;
                apiKey = apiCredentials.APIKey;
            }
            else
            {
                tradeLogin.identifier = dataCredentials.UserName;
                tradeLogin.password = dataCredentials.Password;
                apiKey = dataCredentials.APIKey;
            }
            if (APIHelper.LoginToAccount($"{tradeUrl}/deal/session", apiKey, tradeLogin, out tradeXToken, out tradeCstToken, out acctTrade, out loginMessage))
            {

            }
            else
            {
                throw new Exception($"Trade account login failed:{loginMessage}");
            }

        }
        public static string PlaceTrade(string Epic, Library.TradeDirection Direction, double Size, int StopDistance, int LimitDistance, bool IsSingleDirectional, int MaxNumberOfTrades)
        {
            bool place = false;
            TradeDirection currentDirection;
            if (closeExistingTrades && CloseOptions == SARCloseOptions.CloseLater && lastTradeDirection != Direction)
            {
                closeExistingTrades = false;
                CloseAllTrades();
            }
            if (IsSingleDirectional) //&& currentTrades < MaxNumberOfTrades)
            {
                place = true;
                if (numberOfOpenPositions(out currentDirection) >= MaxNumberOfTrades)
                {
                    place = false;
                }
            }
            else if (isPositionOpen())
            {
                //if position is open and the direction of trade is same, then dont do anything
                //else close current position (take a loss) and put new trade in the direction sent
                if (Direction != lastTradeDirection)
                {
                    CloseTrade(tradeRef);
                    place = true;
                }
            }
            else
            {
                //no position is open just now
                place = true;
            }
            if (place)
            {
                lastTradeDirection = Direction;
                size = Size;
                TradeRequest tradeReq = new TradeRequest()
                {
                    epic = Epic,
                    direction = Direction == TradeDirection.Buy ? "BUY" : "SELL",
                    size = Size,
                    stopDistance = StopDistance,
                    limitDistance = LimitDistance
                };
                var tradeResponse = APIHelper.MakeTradeRequest($"{tradeUrl}/deal/positions/otc", (tradeUrl == Library.DemoTradingUrl ? apiCredentials.APIKey : dataCredentials.APIKey), tradeXToken, tradeCstToken, tradeReq);
                if (tradeResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var dealRef = JsonConvert.DeserializeObject<TradeResponse>(tradeResponse.Content);
                    tradeRef = dealRef.dealReference;
                    lastTradeDirection = Direction;
                    currentTrades += 1;
                }
            }
            return tradeRef;

        }
        private static bool isPositionOpen()
        {
            bool retVal = false;
            if (!string.IsNullOrWhiteSpace(tradeRef))
            {
                try
                {
                    var positionsResponse = APIHelper.Positions($"{tradeUrl}/deal/positions", apiCredentials.APIKey, tradeXToken, tradeCstToken);
                    if (positionsResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var position = JsonConvert.DeserializeObject<TradePosition>(positionsResponse.Content);
                        var positionToCloseDeal = position.positions.Where(p => p.position.dealReference == tradeRef).FirstOrDefault();
                        retVal = positionToCloseDeal != null;
                    }
                }
                catch
                {

                }
            }
            return retVal;
        }
        public static int numberOfOpenPositions(out TradeDirection Direction)
        {
            Direction = TradeDirection.Buy;
            int positions = 0;
            try
            {
                var positionsResponse = APIHelper.Positions($"{tradeUrl}/deal/positions", apiCredentials.APIKey, tradeXToken, tradeCstToken);
                if (positionsResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var position = JsonConvert.DeserializeObject<TradePosition>(positionsResponse.Content);
                    positions = position.positions.Count;
                    Direction = (position.positions.FirstOrDefault()?.position?.direction) == "BUY" ? TradeDirection.Buy : TradeDirection.Sell;
                }
            }
            catch
            {

            }
            return positions;

        }
        public static void CloseTrade(string tradeRefToClose)
        {
            try
            {
                var positionsResponse = APIHelper.Positions($"{tradeUrl}/deal/positions", (tradeUrl == Library.DemoTradingUrl ? apiCredentials.APIKey : dataCredentials.APIKey), tradeXToken, tradeCstToken);
                if (positionsResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var position = JsonConvert.DeserializeObject<TradePosition>(positionsResponse.Content);
                    var positionToCloseDeal = position.positions.Where(p => p.position.dealReference == tradeRefToClose).FirstOrDefault();
                    var dealIdToClose = positionToCloseDeal.position.dealId;
                    var responseClose = APIHelper.CloseTradeRequest($"{tradeUrl}/deal/positions/otc", (tradeUrl == Library.DemoTradingUrl ? apiCredentials.APIKey : dataCredentials.APIKey), tradeXToken, tradeCstToken, new CloseTradeRequest()
                    {
                        dealId = dealIdToClose,
                        direction = lastTradeDirection == TradeDirection.Buy ? "SELL" : "BUY",
                        size = size
                    });
                }
            }
            catch
            {

            }

        }
        public static void CloseAllTrades()
        {
            var positionsResponse = APIHelper.Positions($"{tradeUrl}/deal/positions", apiCredentials.APIKey, tradeXToken, tradeCstToken);
            if (positionsResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var position = JsonConvert.DeserializeObject<TradePosition>(positionsResponse.Content);
                foreach (var positionToCloseDeal in position.positions)
                {
                    var responseClose = APIHelper.CloseTradeRequest($"{tradeUrl}/deal/positions/otc", (tradeUrl == Library.DemoTradingUrl ? apiCredentials.APIKey : dataCredentials.APIKey), tradeXToken, tradeCstToken, new CloseTradeRequest()
                    {
                        dealId = positionToCloseDeal.position.dealId,
                        direction = positionToCloseDeal.position.direction == "BUY" ? "SELL" : "BUY",
                        size = positionToCloseDeal.position.size
                    });
                }
            }

        }

        public static TradingAccounts GetTradingAccount()
        {
            Library.ReadCredentials(ref dataCredentials, ref apiCredentials);

            var accountDetails = APIHelper.makeRequest($"{tradeUrl}/deal/accounts", (tradeUrl == Library.DemoTradingUrl ? apiCredentials.APIKey : dataCredentials.APIKey), tradeXToken, tradeCstToken, Method.GET, "1");
            var tradeAccountInfo = JsonConvert.DeserializeObject<TradingAccounts>(accountDetails.Content);
            return tradeAccountInfo;
        }
    }
}
