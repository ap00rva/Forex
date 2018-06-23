using Newtonsoft.Json;
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
        private static string tradeXToken;
        private static string tradeCstToken;
        private static AccountMain acctTrade;
        static TradeHelper()
        {
            APICredentials dataCredentials = new APICredentials();
            Library.ReadCredentials(ref dataCredentials, ref apiCredentials);
            if (APIHelper.LoginToAccount("https://demo-api.ig.com/gateway/deal/session", apiCredentials.APIKey, new Login()
            {
                identifier = apiCredentials.UserName,
                password = apiCredentials.Password,
                encryptedPassword = null
            }, out tradeXToken, out tradeCstToken, out acctTrade))
            {

            }
            else
            {
                throw new Exception("Trade account login failed");
            }
        }
        public static string PlaceTrade(string APIUrl, string Epic, Library.TradeDirection Direction, double Size, int StopDistance, int LimitDistance)
        {
            bool place = false;
            if (isPositionOpen(APIUrl))
            {
                //if position is open and the direction of trade is same, then dont do anything
                //else close current position (take a loss) and put new trade in the direction sent
                if (Direction != lastTradeDirection)
                {
                    CloseTrade(APIUrl);
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
                var tradeResponse = APIHelper.MakeTradeRequest(APIUrl, apiCredentials.APIKey, tradeXToken, tradeCstToken, tradeReq);
                if (tradeResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var dealRef = JsonConvert.DeserializeObject<TradeResponse>(tradeResponse.Content);
                    tradeRef = dealRef.dealReference;
                }
            }
            return tradeRef;

        }
        private static bool isPositionOpen(string APIUrl)
        {
            bool retVal = true;
            try
            {
                var positionsResponse = APIHelper.Positions($"{APIUrl}/deal/positions", apiCredentials.APIKey, tradeXToken, tradeCstToken);
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
            return retVal;
        }
        public static void CloseTrade(string APIUrl)
        {
            try
            {
                var positionsResponse = APIHelper.Positions($"{APIUrl}/deal/positions", apiCredentials.APIKey, tradeXToken, tradeCstToken);
                if (positionsResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var position = JsonConvert.DeserializeObject<TradePosition>(positionsResponse.Content);
                    var positionToCloseDeal = position.positions.Where(p => p.position.dealReference == tradeRef).FirstOrDefault();
                    var dealIdToClose = positionToCloseDeal.position.dealId;
                    var responseClose = APIHelper.CloseTradeRequest($"{APIUrl}/deal/positions/otc", apiCredentials.APIKey, tradeXToken, tradeCstToken, new CloseTradeRequest()
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
    }
}
