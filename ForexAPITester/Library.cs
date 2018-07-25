using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForexAPITester
{
    public class Library
    {
        public const string UpBar = "Up Bar";
        public const string DownBar = "Down Bar";
        public const string None = "None";

        public const string ENCRYPTSALT = "7746934bfaa44d72836df556d47770b9";
        public enum Relations
        {
            Both = 0,
            UpOnly = 1,
            DownOnly = 2
        }
        public enum TradeDirection
        {
            Buy = 1,
            Sell = 2
        }
        public enum SARCloseOptions
        {
            CloseNow = 1,
            CloseLater = 2
        }
        public static void ReadCredentials(ref APICredentials DataCredentials, ref APICredentials TradeCredentials)
        {
            var userData = new FileStream(Application.UserAppDataPath + "\\appcreds.txt", FileMode.OpenOrCreate);
            SimpleAES simpleAES = new SimpleAES();
            using (StreamReader userReader = new StreamReader(userData))
            {
                while (!userReader.EndOfStream)
                {
                    var dataLine = userReader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(dataLine))
                    {
                        var credData = dataLine.Split('|');
                        switch (credData[0])
                        {
                            case "UserName":
                                DataCredentials.UserName = simpleAES.Decrypt(simpleAES.StrToByteArray(credData[1])).Replace(Library.ENCRYPTSALT, string.Empty);
                                break;
                            case "Password":
                                DataCredentials.Password = simpleAES.Decrypt(simpleAES.StrToByteArray(credData[1])).Replace(Library.ENCRYPTSALT, string.Empty);
                                break;
                            case "DataAPIKey":
                                DataCredentials.APIKey = credData[1];
                                break;
                            case "TradeUserName":
                                TradeCredentials.UserName = simpleAES.Decrypt(simpleAES.StrToByteArray(credData[1])).Replace(Library.ENCRYPTSALT, string.Empty);
                                break;
                            case "TradePassword":
                                TradeCredentials.Password = simpleAES.Decrypt(simpleAES.StrToByteArray(credData[1])).Replace(Library.ENCRYPTSALT, string.Empty);
                                break;
                            case "TradeAPIKey":
                                TradeCredentials.APIKey = credData[1];
                                break;
                            default:
                                break;

                        }

                    }
                }
            }
        }

    }

}
