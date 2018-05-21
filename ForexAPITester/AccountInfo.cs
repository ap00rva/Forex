using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class AccountInfo
    {
        public double balance { get; set; }
        public double deposit { get; set; }
        public double profitLoss { get; set; }
        public double available { get; set; }
    }

    public class Account
    {
        public string accountId { get; set; }
        public string accountName { get; set; }
        public bool preferred { get; set; }
        public string accountType { get; set; }
    }

    public class AccountMain
    {
        public string accountType { get; set; }
        public AccountInfo accountInfo { get; set; }
        public string currencyIsoCode { get; set; }
        public string currencySymbol { get; set; }
        public string currentAccountId { get; set; }
        public string lightstreamerEndpoint { get; set; }
        public List<Account> accounts { get; set; }
        public string clientId { get; set; }
        public int timezoneOffset { get; set; }
        public bool hasActiveDemoAccounts { get; set; }
        public bool hasActiveLiveAccounts { get; set; }
        public bool trailingStopsEnabled { get; set; }
        public object reroutingEnvironment { get; set; }
        public bool dealingEnabled { get; set; }
    }
}
