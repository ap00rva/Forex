using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using Telerik.WinControls.UI;
using Telerik.Charting;
using System.Threading;
using static ForexAPITester.Library;

namespace ForexAPITester
{
    public partial class Form1 : Form
    {
        private string cstToken;
        private string xToken;

        private string tradeCstToken;
        private string tradeXToken;
        private AccountMain acctTrade;
        FileStream userData;
        List<PriceView> priceViews = new List<PriceView>();
        List<PriceView> allProbes = new List<PriceView>();
        List<PriceView> allCounterProbes = new List<PriceView>();
        List<PriceView> switchViews = new List<PriceView>();
        string apiKey = ""; // "27fc35faa6cc6ce0a7875e27afc5c713d77fa4a3";
        string tradeApiKey = ""; //"eeb7ac8e4ceee6a724bbd21214d880d1d7e9a549";
        int totalRequests = 0;
        int minutesToCheck = 1;
        string minutesToCheckString = "";
        int signallingWindowMinutes = -60;
        int probeWindowMinutes = 20;
        int filterWindowMinutes = -25;
        string logFile;
        string userName = ""; //"apoorvadixit";
        string password = ""; //"Jaisai64!";
        string tradeUserName = "";
        string tradePassword = "";
        Relations relationToCheck = Relations.Both;
        DateTime? probeTradeTime = null;
        TradeDirection probeTradeDirection = TradeDirection.Buy;
        decimal probeAmount = 0;
        bool switchHasBeenReset = false;
        public Form1()
        {
            InitializeComponent();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                string loginMessage = string.Empty;
                AccountMain acctMain;
                if (APIHelper.LoginToAccount("https://api.ig.com/gateway/deal/session", apiKey, new Login()
                {
                    identifier = userName,
                    password = password,
                    encryptedPassword = null
                }, out xToken, out cstToken, out acctMain, out loginMessage))
                {
                    txtLog.Text = "Login successfull";
                    userData = new FileStream(Application.UserAppDataPath + "\\appdata.txt", FileMode.OpenOrCreate);
                    userData.Position = 0;
                    using (StreamWriter userWriter = new StreamWriter(userData))
                    {
                        userWriter.WriteLine(cstToken);
                        userWriter.WriteLine(xToken);
                        userWriter.Flush();
                        userWriter.Close();
                    }

                }
            }
            else
            {
                MessageBox.Show("Please setup login credentials first", "Cannot login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            //var client = new RestClient();
            //client.BaseUrl = new Uri("https://api.ig.com/gateway/deal/session");
            //var request = new RestRequest();
            //request.Method = Method.POST;
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("X-IG-API-KEY", "27fc35faa6cc6ce0a7875e27afc5c713d77fa4a3");
            //request.AddHeader("Version", "2");
            //request.AddJsonBody(new Login() { identifier = "apoorvadixit", password = "Jaisai64!", encryptedPassword = null });

            //var response = client.Execute(request);

            //var responseHeaders = response.Headers;
            //var responseBody = response.Content;
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    cstToken = responseHeaders.First(c => c.Name == "CST")?.Value.ToString() ?? String.Empty;
            //    xToken = responseHeaders.First(c => c.Name == "X-SECURITY-TOKEN")?.Value.ToString() ?? String.Empty;
            //    acctMain = JsonConvert.DeserializeObject<AccountMain>(responseBody);
            //    txtLog.Text = "Login successfull";
            //    userData = new FileStream(Application.UserAppDataPath + "\\appdata.txt", FileMode.OpenOrCreate);
            //    userData.Position = 0;
            //    using (StreamWriter userWriter = new StreamWriter(userData))
            //    {
            //        userWriter.WriteLine(cstToken);
            //        userWriter.WriteLine(xToken);
            //        userWriter.Flush();
            //        userWriter.Close();
            //    }
            //}
            //else
            //{
            //    txtLog.Text = $"Error in login:{response.StatusDescription}";
            //}
            //Debug.WriteLine(response.ToString());

        }

        private void btnMarketData_Click(object sender, EventArgs e)
        {
            var response = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/markets/{txtEpic.Text}", apiKey, xToken, cstToken, Method.GET);
            var responseHeaders = response.Headers;
            var responseBody = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var marketInfo = JsonConvert.DeserializeObject<MarketInfo>(responseBody);
                lblName.Text = marketInfo.instrument.name;
                lblBid.Text = marketInfo.snapshot.bid.ToString();
                lblHigh.Text = marketInfo.snapshot.high.ToString();
                lblLow.Text = marketInfo.snapshot.low.ToString();
                lblOffer.Text = marketInfo.snapshot.offer.ToString();
                lblNetChange.Text = marketInfo.snapshot.netChange.ToString();
                lblPercChange.Text = marketInfo.snapshot.percentageChange.ToString();
                lblStatus.Text = marketInfo.snapshot.marketStatus;

            }
            else
            {
                txtLog.Text = $"Error in login:{response.StatusDescription}";
            }
        }

        private void btnHistoricData_Click(object sender, EventArgs e)
        {
            loadData(timeStart.Value, timeEnd.Value);
            btnFindDetailRelated_Click(sender, e);

            //loadData(timeStart.Value, timeEnd.Value);
            //var response = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=MINUTE{minutesToCheckString}&from={timeStart.Value.ToString("yyyy-MM-ddTHH:mm")}&to={timeEnd.Value.ToString("yyyy-MM-ddTHH:mm")}", apiKey, xToken, cstToken, Method.GET);
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var prices = JsonConvert.DeserializeObject<PriceData>(response.Content);
            //    if (prices.metadata.pageData.totalPages > 1)
            //    {
            //        for (int i = 2; i <= prices.metadata.pageData.totalPages; i++)
            //        {
            //            var responsePage = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=MINUTE{minutesToCheckString}&from={timeStart.Value.ToString("yyyy-MM-ddTHH:mm")}&to={timeEnd.Value.ToString("yyyy-MM-ddTHH:mm")}&pageNumber={i}", apiKey, xToken, cstToken, Method.GET);
            //            if (responsePage.StatusCode == System.Net.HttpStatusCode.OK)
            //            {
            //                var pricesPage = JsonConvert.DeserializeObject<PriceData>(responsePage.Content);
            //                prices.prices.AddRange(pricesPage.prices);
            //            }
            //            else
            //            {
            //                txtLog.Text += $"Non 200 response:{responsePage.StatusDescription}";
            //            }
            //        }
            //    }
            //    //priceViews = new List<PriceView>();
            //    PriceView previousPrice = new PriceView() { SnapshotTime = String.Empty };
            //    foreach (var price in prices.prices)
            //    {
            //        var pview = new PriceView()
            //        {
            //            AskClose = price.closePrice.ask,
            //            AskHigh = price.highPrice.ask,
            //            AskLow = price.lowPrice.ask,
            //            AskOpen = price.openPrice.ask,
            //            //BidClose = price.closePrice.bid,
            //            //BidHigh = price.highPrice.bid,
            //            //BidLow = price.lowPrice.bid,
            //            //BidOpen = price.openPrice.bid,
            //            SnapshotTime = price.snapshotTime
            //        };
            //        if (String.IsNullOrWhiteSpace(previousPrice.SnapshotTime))
            //        {
            //            pview.Ask2BarReversal = false;
            //            //pview.Bid2BarReversal = false;
            //        }
            //        else
            //        {
            //            pview.Ask2BarReversal = previousPrice.AskUpDown != pview.AskUpDown;
            //            //pview.Bid2BarReversal = previousPrice.BidUpDown != pview.BidUpDown;
            //        }
            //        priceViews.Add(pview);
            //        previousPrice = pview;
            //    }
            //    OhlcSeries oSeries = new OhlcSeries();

            //    foreach (var pview in priceViews)
            //    {

            //        oSeries.DataPoints.Add(new OhlcDataPoint(pview.AskOpen, pview.AskHigh, pview.AskLow, pview.AskClose, pview.TimeCategory));
            //    }
            //    LinearAxis linearAxis1 = (LinearAxis)radChartView1.Axes[1];
            //    linearAxis1.Minimum = priceViews.Min(p => p.AskLow) - 2;
            //    linearAxis1.Maximum = priceViews.Max(p => p.AskHigh) + 2;

            //    radChartView1.Series.Clear();
            //    radChartView1.Series.Add(oSeries);
            //    radChartView1.Refresh();
            //    BindingSource source = new BindingSource();
            //    source.DataSource = priceViews;
            //    dataGridView1.DataSource = source;

            //}
            //else
            //{
            //    txtLog.Text += Environment.NewLine + $"non-200 response {response.StatusDescription}";
            //}
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
#if !DEBUG
                btnFindRelated.Visible = false;
                btnFillIntervals.Visible = false;
                //btnHistoricData.Visible = false;
                btnStartAuto.Visible = false;
                btnFindDetailRelated.Visible = false;
                rdoUp.Visible = false;
                rdoDown.Visible = false;
                //tabControl1.TabPages.RemoveAt(3);

#endif
                //timeStart.MinDate = DateTime.UtcNow.AddHours(-8);
                cboFilter.Value = 25;
                logFile = $"log{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}.txt";
                timer1.Interval = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                //timer1.Start();
                // Create a file that the application will store user specific data in.
                readTokens();
                readCredentials();
                if (!String.IsNullOrWhiteSpace(cstToken) && !String.IsNullOrWhiteSpace(xToken))
                {
                    txtLog.Text = "Tokens found";
                }
                timeStart1.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 6, 0, 0);
                timeStart.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 6, 0, 0);
                timeEnd.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 19, 0, 0);
            }
            catch (IOException ex)
            {
                // Inform the user that an error occurred.
                MessageBox.Show("An error occurred while attempting to show the application." + "The error is:" + ex.ToString());
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            userData.Close();
            Logger.Close();
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells[10].Value) == true)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
            }
            if (Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells[11].Value) == true)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
            }

        }

        private void btnFindRelated_Click(object sender, EventArgs e)
        {
            if (rdoUp.Checked)
            {
                SignalHelper.FindRelated(Library.UpBar, priceViews);
            }
            else
            {
                SignalHelper.FindRelated(Library.DownBar, priceViews);
            }
        }
        #region Old Code
        //private void findRelated(string reversalType, List<PriceView> pViews)
        //{
        //    List<PriceView> relatedBars = new List<PriceView>();
        //    var probe = pViews.Last();
        //    bool isProbeSuccessfull = false;
        //    bool foundAll = false;
        //    string probeLog = string.Empty;
        //    for (int i = pViews.Count - 2; i >= 1; i--)
        //    {
        //        foundAll = false;
        //        var p1 = pViews[i];
        //        var p2 = pViews[i - 1];
        //        if (p1.Ask2BarReversal && p1.AskUpDown == reversalType)
        //        {
        //            //found first reversal, now first check if there is a probe and if so then find another pair
        //            //else no use in working on it

        //            if (reversalType == Library.DownBar)
        //            {
        //                ////for Down reversal pairs Up and Down
        //                ////p1 is down and p2 is up
        //                //successful Down probe
        //                isProbeSuccessfull = (probe.AskUpDown == Library.UpBar && probe.AskHigh >= p1.AskOpen && probe.AskClose <= Math.Max(p1.AskHigh, p2.AskHigh)); //&& probe.AskLow <= p1.AskOpen && probe.AskClose >= Math.Min(p1.AskLow, p2.AskLow));
        //                if (isProbeSuccessfull)
        //                {
        //                    probeLog = $"{probe.TimeCategory}-{p1.TimeCategory}-Probe sucess:{probe.AskUpDown}-High:{probe.AskHigh}-Open:{p1.AskOpen}-Close:{probe.AskClose}-{Math.Max(p1.AskHigh, p2.AskHigh)}";
        //                }
        //            }
        //            else
        //            {
        //                //for Up reversal pairs Down and Up
        //                //p1 is up and p2 is down
        //                //successful Up probe
        //                isProbeSuccessfull = (probe.AskUpDown == Library.DownBar && probe.AskLow <= p1.AskOpen && probe.AskClose >= Math.Min(p1.AskLow, p2.AskLow)); //&& probe.AskHigh >= p2.AskOpen && probe.AskClose <= Math.Max(p1.AskHigh, p2.AskHigh));
        //                if (isProbeSuccessfull)
        //                {
        //                    probeLog = $"{probe.TimeCategory}-{p1.TimeCategory}-Probe sucess:{probe.AskUpDown}-Low:{probe.AskLow}-Open:{p1.AskOpen}-Close:{probe.AskClose}-{Math.Min(p1.AskLow, p2.AskLow)}";
        //                }
        //            }

        //            if (isProbeSuccessfull)
        //            {
        //                for (int j = i - 2; j >= 1; j--)
        //                {
        //                    var p3 = pViews[j];
        //                    var p4 = pViews[j - 1];

        //                    ////for Down reversal pairs Up and Down
        //                    ////p1 is down and p2 is up
        //                    ////p3 is down and p4 is up

        //                    //for Up reversal pairs Down and Up
        //                    //p1 is up and p2 is down
        //                    //p3 is up and p4 is down
        //                    if (reversalType == Library.UpBar)
        //                    {
        //                        if (p3.Ask2BarReversal && p3.AskUpDown == reversalType &&
        //                            (isUpBarConditionMet(new List<PriceView>() { p1, p2 }, new List<PriceView>() { p3, p4 })))
        //                        {
        //                            string up1 = $"{p1.TimeCategory}-{p3.TimeCategory}-UpBarCondition1/1 met:Low:{p1.AskLow}-Open:{p4.AskOpen}-Close:{p1.AskClose}-Low:{Math.Min(p3.AskLow, p4.AskLow)}";
        //                            string up2 = $"{p2.TimeCategory}-{p4.TimeCategory}-UpBarCondition2/1 met:Low:{p2.AskLow}-Open:{Math.Min(p3.AskLow, p4.AskLow)}-Close:{p2.AskClose}-Low:{Math.Min(p3.AskLow, p4.AskLow)}";
        //                            //now look for a third with the same conditions
        //                            for (int k = j - 2; k >= 1; k--)
        //                            {
        //                                var p5 = pViews[k];
        //                                var p6 = pViews[k - 1];
        //                                if (p5.Ask2BarReversal && p5.AskUpDown == reversalType &&
        //                            (isUpBarConditionMet(new List<PriceView>() { p3, p4 }, new List<PriceView>() { p5, p6 }, true)))
        //                                {
        //                                    Logger.WriteLog(probeLog);
        //                                    Logger.WriteLog(up1);
        //                                    Logger.WriteLog(up2);
        //                                    Logger.WriteLog($"{p3.TimeCategory}-{p5.TimeCategory}-UpBarCondition1/2 met:Low:{p3.AskLow}-Low:{Math.Min(p5.AskLow, p6.AskLow)}-Close:{p3.AskClose}-Low:{Math.Min(p5.AskLow, p6.AskLow)}");
        //                                    Logger.WriteLog($"{p4.TimeCategory}-{p6.TimeCategory}-UpBarCondition2/2 met:Low:{p4.AskLow}-Low:{p6.AskOpen}-Close:{p4.AskClose}-Low:{Math.Min(p5.AskLow, p6.AskLow)}");
        //                                    //found all pairs
        //                                    p1.IsUpRelated = true;
        //                                    p2.IsUpRelated = true;
        //                                    p3.IsUpRelated = true;
        //                                    p4.IsUpRelated = true;
        //                                    p5.IsUpRelated = true;
        //                                    p6.IsUpRelated = true;
        //                                    probe.IsDownProbe = true;
        //                                    probe.ProbeRelations = $"{p1.TimeCategory},{p3.TimeCategory},{p5.TimeCategory}";
        //                                    //set them as green and exit
        //                                    //colourGridRows(relatedBars, Color.LightGreen);
        //                                    foundAll = true;
        //                                    break;
        //                                }
        //                            }
        //                            if (foundAll)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else if (reversalType == Library.DownBar)
        //                    {
        //                        if (p3.Ask2BarReversal && p3.AskUpDown == reversalType &&
        //                            (isDownBarConditionMet(new List<PriceView>() { p1, p2 }, new List<PriceView>() { p3, p4 })))
        //                        {
        //                            string down1 = $"{p1.TimeCategory}-{p3.TimeCategory}-DownBarCondition1/1 met:High:{p1.AskHigh}-Open:{p4.AskOpen}-Close:{p1.AskClose}-High:{Math.Max(p3.AskHigh, p4.AskHigh)}";
        //                            string down2 = $"{p2.TimeCategory}-{p4.TimeCategory}-DownBarCondition2/1 met:High:{p2.AskHigh}-Open:{p4.AskOpen}-Close:{p2.AskClose}-High:{Math.Max(p3.AskHigh, p4.AskHigh)}";

        //                            //now look for a third with the same conditions
        //                            for (int k = j - 2; k >= 1; k--)
        //                            {
        //                                var p5 = pViews[k];
        //                                var p6 = pViews[k - 1];
        //                                if (p5.Ask2BarReversal && p5.AskUpDown == reversalType &&
        //                            (isDownBarConditionMet(new List<PriceView>() { p3, p4 }, new List<PriceView>() { p5, p6 }, true)))
        //                                {
        //                                    Logger.WriteLog(probeLog);
        //                                    Logger.WriteLog(down1);
        //                                    Logger.WriteLog(down2);
        //                                    Logger.WriteLog($"{p3.TimeCategory}-{p5.TimeCategory}-DownBarCondition1/2 met:High:{p3.AskHigh}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}-Close:{p3.AskClose}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}");
        //                                    Logger.WriteLog($"{p4.TimeCategory}-{p6.TimeCategory}-DownBarCondition2/2 met:High:{p4.AskHigh}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}-Close:{p4.AskClose}-High:{Math.Max(p5.AskHigh, p6.AskHigh)}");

        //                                    //found all pairs
        //                                    p1.IsDownRelated = true;
        //                                    p2.IsDownRelated = true;
        //                                    p3.IsDownRelated = true;
        //                                    p4.IsDownRelated = true;
        //                                    p5.IsDownRelated = true;
        //                                    p6.IsDownRelated = true;
        //                                    probe.IsUpProbe = true;
        //                                    probe.ProbeRelations = $"{p1.TimeCategory},{p3.TimeCategory},{p5.TimeCategory}";
        //                                    //set them as green and exit
        //                                    //colourGridRows(relatedBars, Color.Orange);
        //                                    foundAll = true;
        //                                    break;
        //                                }
        //                            }
        //                            if (foundAll)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                if (foundAll)
        //                {
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                //break;
        //            }
        //            if (foundAll)
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            //break;
        //        }
        //    }
        //    //if (foundAll)
        //    //{
        //    //    MessageBox.Show("relation found");
        //    //}


        //}
        //private bool isUpBarConditionMet(List<PriceView> currentPair, List<PriceView> previousPair, bool isThird = false)
        //{
        //    //current pair 0 is Down
        //    //current pair 1 is Up
        //    //previous pair 0 is Down
        //    //previous pair 1 is Up
        //    if (currentPair.Count != 2 || previousPair.Count != 2)
        //    {
        //        return false;
        //    }
        //    if (!isThird)
        //    {
        //        return (currentPair[0].AskLow <= previousPair[1].AskOpen && currentPair[0].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow))
        //            || (currentPair[1].AskLow <= previousPair[1].AskOpen && currentPair[1].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow));
        //    }
        //    else
        //    {
        //        return (currentPair[0].AskLow < Math.Min(previousPair[0].AskLow, previousPair[1].AskLow) && (currentPair[0].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow))
        //            || currentPair[1].AskLow < Math.Min(previousPair[0].AskLow, previousPair[1].AskLow)) && (currentPair[1].AskClose >= Math.Min(previousPair[0].AskLow, previousPair[1].AskLow));
        //    }

        //}
        //private bool isDownBarConditionMet(List<PriceView> currentPair, List<PriceView> previousPair, bool isThird = false)
        //{
        //    //current pair 0 is Up
        //    //current pair 1 is Down
        //    //previous pair 0 is Up
        //    //previous pair 1 is Down

        //    if (currentPair.Count != 2 || previousPair.Count != 2)
        //    {
        //        return false;
        //    }
        //    if (!isThird)
        //    {
        //        return (currentPair[0].AskHigh >= previousPair[1].AskOpen && currentPair[0].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh))
        //            || (currentPair[1].AskHigh >= previousPair[1].AskOpen && currentPair[1].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh));
        //    }
        //    else
        //    {
        //        return (currentPair[0].AskHigh > Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh) && (currentPair[0].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh))
        //            || currentPair[1].AskHigh > Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh)) && (currentPair[1].AskClose <= Math.Max(previousPair[0].AskHigh, previousPair[1].AskHigh));
        //    }
        //}
        #endregion

        private bool isRowTobeColoured(List<PriceView> priceViews, DataGridViewRow row)
        {
            return priceViews.Any(p => p.SnapshotTime == row.Cells[0].Value.ToString());
        }
        private void colourGridRows(List<PriceView> relatedBars, Color colour)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (isRowTobeColoured(relatedBars, row))
                {
                    row.DefaultCellStyle.BackColor = colour;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime current = timeStart.Value;
            if (current.Minute == 0 || current.Minute % minutesToCheck == 0)
            {
                //then we can request data for current
                totalRequests += 1;
                if (totalRequests > 1000)
                {
                    timer1.Stop();
                    MessageBox.Show("Stopping automatic data collection.", "Message");
                }
                //txtLog.AppendText($"{Environment.NewLine} Called at:{DateTime.Now}");
                //call from time start to start+5mins

                //store only 6 hours worth of data
                reduceDataSize();
                //loadData(timeStart.Value, timeStart.Value.AddMinutes(minutesToCheck - 1));
                loadData(current.AddMinutes(minutesToCheck * (-1)), current, true);
                //List<PriceView> pViews = new List<PriceView>();
                DateTime start1;
                if (priceViews.Any())
                {

                    var last = priceViews.OrderBy(p => p.SnapshotDateTime).Last();
                    #region  Switch related processing
                    if (SwitchHelper.IsSwitchSearchInProgress())
                    {
                        if (SwitchHelper.IsSwitchConditionMet(last))
                        {
                            SetSwitch();
                        }
                    }
                    else
                    {
                        if (!switchHasBeenReset)
                        {
                            ResetSwitch();
                        }
                    }
                    if (probeTradeTime != null)
                    {
                        //future place trade has been set
                        if (IsTimeForProbeTrade(last))
                        {
                            if (IsConditionMetForProbeTrade(last))
                            {
                                TradeHelper.PlaceTrade(txtEpic.Text, probeTradeDirection, double.Parse(txtSize.Text), int.Parse(txtStopDistance.Text), int.Parse(txtLimitDistance.Text), !rdoBoth.Checked, (int)txtMaxTrades.Value);
                            }
                            probeTradeTime = null;
                        }
                    }
                    #endregion
                    start1 = last.SnapshotDateTime.AddMinutes(signallingWindowMinutes);
                    var pViews = priceViews.Where(p => p.SnapshotDateTime >= start1).OrderBy(p => p.SnapshotDateTime).ToList();
                    var probe = new PriceView();
                    var counterProbe = new PriceView();
                    if (pViews.Any())
                    {
                        //this is probe, check if this is up or down. accordingly, check for 1 type of reversal only
                        //if probe is up, check for pair or down reversals else opposite
                        if (last.AskUpDown == Library.UpBar && (relationToCheck == Relations.Both || relationToCheck == Relations.UpOnly))
                        {
                            //mean probe closed up
                            probe = SignalHelper.FindRelated(Library.DownBar, pViews, probeWindowMinutes);
                        }
                        else if (last.AskUpDown == Library.DownBar && (relationToCheck == Relations.Both || relationToCheck == Relations.DownOnly))
                        {
                            probe = SignalHelper.FindRelated(Library.UpBar, pViews, probeWindowMinutes);
                        }
                        if (probe.IsUpProbe || probe.IsDownProbe)
                        {
                            checkProbeQualification(probe, priceViews.OrderBy(p => p.SnapshotDateTime), filterWindowMinutes);
                            allProbes.Add(probe);
                        }
                        if (relationToCheck == Relations.UpOnly || relationToCheck == Relations.DownOnly)
                        {
                            //means look for counter probe too
                            if (last.AskUpDown == Library.UpBar)
                            {
                                counterProbe = SignalHelper.FindRelated(Library.DownBar, pViews, probeWindowMinutes);
                            }
                            else if (last.AskUpDown == Library.DownBar)
                            {
                                counterProbe = SignalHelper.FindRelated(Library.UpBar, pViews, probeWindowMinutes);
                            }
                            if (counterProbe.IsUpProbe || counterProbe.IsDownProbe)
                            {
                                checkProbeQualification(counterProbe, priceViews.OrderBy(p => p.SnapshotDateTime), filterWindowMinutes);
                                allCounterProbes.Add(counterProbe);
                            }
                        }
                    }
                    if (probe.ProbeQualified && chkTrade.Checked)
                    {
                        TradeHelper.PlaceTrade(txtEpic.Text, probe.AskUpDown == Library.UpBar ? Library.TradeDirection.Sell : Library.TradeDirection.Buy, double.Parse(txtSize.Text), int.Parse(txtStopDistance.Text), int.Parse(txtLimitDistance.Text), !rdoBoth.Checked, (int)txtMaxTrades.Value);
                    }
                    //allProbes.AddRange(pViews.Where(p => p.IsDownProbe || p.IsUpProbe));
                    pViews.Clear();
                    saveProbesToLog(allProbes.Distinct(new PriceViewComparer()).OrderBy(p => p.SnapshotDateTime).ToList());
                    rebindProbes();
                }
            }
            timeStart.Value = timeStart.Value.AddMinutes(1);
        }

        private void btnStartAuto_Click(object sender, EventArgs e)
        {
            //List<PriceView> priceViews = new List<PriceView>();
            //DateTime startTime = timeStart.Value;
            //DateTime endTime = startTime.AddHours(2);
            //DateTime start1 = startTime;
            //DateTime end1 = start1.AddHours(1);
            //while (start1 <= endTime)
            //{
            //    loadData(start1, end1);
            //    findRelated(Library.DownBar);
            //    findRelated(Library.UpBar);
            //    start1 = end1.AddMinutes(5);
            //    end1 = end1.AddHours(1);
            //}
            //timeStart.Value = end1.AddHours(-1);
            //findRelated(Library.UpBar);
            //findRelated(Library.DownBar);
            //MessageBox.Show($"All data imported:{end1}");

        }

        private void loadData(DateTime start1, DateTime end1, bool isLive = false)
        {
            start1 = start1.AddMinutes(-1);
            end1 = end1.AddMinutes(-1);
            if (start1.Minute == 0 || start1.Minute % minutesToCheck == 0)
            {
                //avoid checks at 3min intervals
                //start1 = start1.AddMinutes(1);
                end1 = end1.AddMinutes(1);
            }

            //Logger.WriteLog($"Load Start:{start1.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss")} End:{end1.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss")}");
            IRestResponse response;
            if (isLive)
            {
                response = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=MINUTE{minutesToCheckString}", apiKey, xToken, cstToken, Method.GET);
            }
            else
            {
                response = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=MINUTE{minutesToCheckString}&from={start1.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00")}&to={end1.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00")}", apiKey, xToken, cstToken, Method.GET);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var prices = JsonConvert.DeserializeObject<PriceData>(response.Content);
                if (prices.metadata.pageData.totalPages > 1)
                {
                    for (int i = 2; i <= prices.metadata.pageData.totalPages; i++)
                    {
                        var responsePage = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=MINUTE{minutesToCheckString}&from={start1.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00")}&to={end1.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00")}&pageNumber={i}", apiKey, xToken, cstToken, Method.GET);
                        if (responsePage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var pricesPage = JsonConvert.DeserializeObject<PriceData>(responsePage.Content);
                            prices.prices.AddRange(pricesPage.prices);
                        }
                        else
                        {
                            txtLog.Text += $"Non 200 response:{responsePage.StatusDescription}";
                        }
                    }
                }
                PriceView previousPrice = priceViews.LastOrDefault() ?? new PriceView() { SnapshotTime = String.Empty };
                if (isLive)
                {
                    //log all prices
                    foreach (var p in prices.prices)
                    {
                        Logger.WriteData(JsonConvert.SerializeObject(p), true);
                    }
                    Price priceFirst;
                    if (minutesToCheck == 1)
                    {
                        priceFirst = prices.prices.Skip(Math.Max(0, prices.prices.Count() - 2)).First();
                    }
                    else
                    {
                        priceFirst = prices.prices.Last();
                    }
                    processPriceData(priceFirst, ref previousPrice);

                }
                else
                {
                    foreach (var price in prices.prices)
                    {
                        processPriceData(price, ref previousPrice);
                        ////if (firstOnly)
                        ////{
                        ////    //firstOnly = false;
                        ////    //continue;
                        ////}
                        ////else
                        //{
                        //    var pview = new PriceView()
                        //    {
                        //        AskClose = price.closePrice.ask,
                        //        AskHigh = price.highPrice.ask,
                        //        AskLow = price.lowPrice.ask,
                        //        AskOpen = price.openPrice.ask,
                        //        //BidClose = price.closePrice.bid,
                        //        //BidHigh = price.highPrice.bid,
                        //        //BidLow = price.lowPrice.bid,
                        //        //BidOpen = price.openPrice.bid,
                        //        SnapshotTime = price.snapshotTime
                        //    };
                        //    if (String.IsNullOrWhiteSpace(previousPrice.SnapshotTime))
                        //    {
                        //        pview.Ask2BarReversal = false;
                        //        //pview.Bid2BarReversal = false;
                        //    }
                        //    else
                        //    {
                        //        pview.Ask2BarReversal = ((previousPrice.AskUpDown != pview.AskUpDown) && (previousPrice.AskUpDown != Library.None && pview.AskUpDown != Library.None));
                        //        //pview.Bid2BarReversal = previousPrice.BidUpDown != pview.BidUpDown;
                        //    }
                        //    if (priceViews.Exists(p => DateTime.Compare(p.SnapshotDateTime, pview.SnapshotDateTime) == 0))
                        //    {
                        //        var pv = priceViews.FirstOrDefault(p => p.SnapshotDateTime == pview.SnapshotDateTime);
                        //        Logger.WriteLog($"Duplicate priceview:Time (n):{pview.SnapshotDateTime} - High:{pview.AskHigh} Low:{pview.AskLow} Open:{pview.AskOpen} Close:{pview.AskClose}");
                        //        Logger.WriteLog($"Duplicate priceview:Time (o):{pv.SnapshotDateTime} - High:{pv.AskHigh} Low:{pv.AskLow} Open:{pv.AskOpen} Close:{pv.AskClose}");
                        //        priceViews.RemoveAll(p => DateTime.Compare(p.SnapshotDateTime, pview.SnapshotDateTime) == 0);
                        //    }
                        //    Logger.WriteData(JsonConvert.SerializeObject(pview));
                        //    priceViews.Add(pview);
                        //    previousPrice = pview;
                        //}

                    }
                }
                radChartView1.Refresh();

                OhlcSeries oSeries = new OhlcSeries();

                foreach (var pview in priceViews)
                {
                    oSeries.DataPoints.Add(new OhlcDataPoint((double)pview.AskOpen, (double)pview.AskHigh, (double)pview.AskLow, (double)pview.AskClose, pview.TimeCategory));
                }
                if (priceViews.Any())
                {
                    LinearAxis linearAxis1 = (LinearAxis)radChartView1.Axes[1];
                    linearAxis1.Minimum = (double)priceViews.Min(p => p.AskLow) - 2;
                    linearAxis1.Maximum = (double)priceViews.Max(p => p.AskHigh) + 2;
                    linearAxis1.Refresh();
                }

                radChartView1.Series.Clear();
                radChartView1.Series.Add(oSeries);
                BindingSource source = new BindingSource();
                source.DataSource = priceViews.OrderByDescending(p => p.SnapshotDateTime);
                dataGridView1.DataSource = source;

            }
            else
            {
                txtLog.Text += Environment.NewLine + $"non-200 response {response.StatusDescription} - {start1}";
            }

        }

        private void processPriceData(Price price, ref PriceView previousPrice)
        {
            var pview = new PriceView()
            {
                AskClose = price.closePrice.ask,
                AskHigh = price.highPrice.ask,
                AskLow = price.lowPrice.ask,
                AskOpen = price.openPrice.ask,
                //BidClose = price.closePrice.bid,
                //BidHigh = price.highPrice.bid,
                //BidLow = price.lowPrice.bid,
                //BidOpen = price.openPrice.bid,
                SnapshotTime = price.snapshotTime
            };
            if (String.IsNullOrWhiteSpace(previousPrice.SnapshotTime))
            {
                pview.Ask2BarReversal = false;
                //pview.Bid2BarReversal = false;
            }
            else
            {
                pview.Ask2BarReversal = ((previousPrice.AskUpDown != pview.AskUpDown) && (previousPrice.AskUpDown != Library.None && pview.AskUpDown != Library.None));
                //pview.Bid2BarReversal = previousPrice.BidUpDown != pview.BidUpDown;
            }
            if (priceViews.Exists(p => DateTime.Compare(p.SnapshotDateTime, pview.SnapshotDateTime) == 0))
            {
                var pv = priceViews.FirstOrDefault(p => p.SnapshotDateTime == pview.SnapshotDateTime);
                Logger.WriteLog($"Duplicate priceview:Time (n):{pview.SnapshotDateTime} - High:{pview.AskHigh} Low:{pview.AskLow} Open:{pview.AskOpen} Close:{pview.AskClose}");
                Logger.WriteLog($"Duplicate priceview:Time (o):{pv.SnapshotDateTime} - High:{pv.AskHigh} Low:{pv.AskLow} Open:{pv.AskOpen} Close:{pv.AskClose}");
                priceViews.RemoveAll(p => DateTime.Compare(p.SnapshotDateTime, pview.SnapshotDateTime) == 0);
            }
            Logger.WriteData(JsonConvert.SerializeObject(pview));
            priceViews.Add(pview);
            previousPrice = pview;

        }
        private void btnFillIntervals_Click(object sender, EventArgs e)
        {

            //DateTime startTime = timeStart.Value;
            //DateTime endTime = startTime.AddHours(10);
            //DateTime start1 = startTime.AddMinutes(5);
            //DateTime end1 = start1;
            //while (start1 < endTime)
            //{
            //    loadData(start1, end1);
            //    findRelated(Library.UpBar, true);
            //    findRelated(Library.DownBar, true);
            //    start1 = start1.AddMinutes(5);
            //    end1 = start1;
            //    Thread.Sleep(10000);
            //}
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            priceViews.Clear();
            allProbes.Clear();
            allCounterProbes.Clear();
            radChartView1.Series.Clear();
            BindingSource source = new BindingSource();
            source.DataSource = priceViews;
            dataGridView1.DataSource = source;

            BindingSource source2 = new BindingSource();
            source2.DataSource = allProbes.Distinct(new PriceViewComparer()).Select(p => new { Time = p.TimeCategory, Up = p.IsUpProbe, Down = p.IsDownProbe, Relations = p.ProbeRelations });
            dataGridView2.DataSource = source2;

        }

        private void btnFindDetailRelated_Click(object sender, EventArgs e)
        {
            //start from the initial 2 hour window working backwards
            //then find related after every 5mins
            if (priceViews.Any())
            {
                DateTime start1 = DateTime.Parse((priceViews[0]).SnapshotTime);
                DateTime end1 = start1.AddMinutes(signallingWindowMinutes * (-1));
                DateTime completedTime = DateTime.Parse((priceViews[priceViews.Count - 1]).SnapshotTime);
                //var pViews = priceViews.Where(p => DateTime.Parse(p.SnapshotTime) >= start1 && DateTime.Parse(p.SnapshotTime) <= end1).ToList();
                //findRelated(Library.UpBar, pViews);
                //findRelated(Library.DownBar, pViews);
                allProbes = new List<PriceView>();
                allCounterProbes = new List<PriceView>();
                var probe = new PriceView();
                var counterProbe = new PriceView();
                while (end1 <= completedTime)
                {
                    end1 = end1.AddMinutes(minutesToCheck);
                    start1 = end1.AddMinutes(signallingWindowMinutes);
                    var pViews = priceViews.Where(p => DateTime.Parse(p.SnapshotTime) >= start1 && DateTime.Parse(p.SnapshotTime) <= end1).OrderBy(p => p.SnapshotDateTime).ToList();
                    if (pViews.Any())
                    {
                        var last = pViews.Last();
                        //this is probe, check if this is up or down. accordingly, check for 1 type of reversal only
                        //if probe is up, check for pair or down reversals else opposite
                        if (last.AskUpDown == Library.UpBar && (relationToCheck == Relations.Both || relationToCheck == Relations.UpOnly))
                        {
                            //mean probe closed up
                            probe = SignalHelper.FindRelated(Library.DownBar, pViews, probeWindowMinutes);
                        }
                        else if (last.AskUpDown == Library.DownBar && (relationToCheck == Relations.Both || relationToCheck == Relations.DownOnly))
                        {
                            probe = SignalHelper.FindRelated(Library.UpBar, pViews, probeWindowMinutes);
                        }
                        if (probe.IsDownProbe || probe.IsUpProbe)
                        {
                            checkProbeQualification(probe, priceViews.OrderBy(p => p.SnapshotDateTime), filterWindowMinutes);
                            allProbes.Add(probe);
                        }
                        if (relationToCheck == Relations.UpOnly || relationToCheck == Relations.DownOnly)
                        {
                            //do counter probes
                            if (last.AskUpDown == Library.UpBar)
                            {
                                counterProbe = SignalHelper.FindRelated(Library.DownBar, pViews, probeWindowMinutes);
                            }
                            else if (last.AskUpDown == Library.DownBar)
                            {
                                counterProbe = SignalHelper.FindRelated(Library.UpBar, pViews, probeWindowMinutes);
                            }
                            if (counterProbe.IsDownProbe || counterProbe.IsUpProbe)
                            {
                                checkProbeQualification(counterProbe, priceViews.OrderBy(p => p.SnapshotDateTime), filterWindowMinutes);
                                allCounterProbes.Add(counterProbe);
                            }

                        }
                        //allProbes.AddRange(pViews.Where(p => p.IsDownProbe || p.IsUpProbe));
                    }

                    pViews.Clear();
                }
                saveProbesToLog(allProbes.Distinct(new PriceViewComparer()).ToList());
                rebindProbes();
            }

        }

        private void dataGridView2_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                if (dataGridView2.RowCount > 0)
                {
                    if (dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString() == "U")
                    {
                        dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
                    }
                    else
                    {
                        dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
            }
            catch
            {

            }

        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            //timeStart.Value = timeStart1.Value.ToUniversalTime();
            //if (timeStart.Value < DateTime.UtcNow.AddMinutes(-1))
            //{
            //    //means collect some previous data until now then carry on with current collection
            //    timeEnd.Value = DateTime.UtcNow.AddMinutes(minutesToCheck * (-1));
            //    btnHistoricData_Click(sender, e);
            //    btnFindDetailRelated_Click(sender, e);
            //    timeStart.Value = DateTime.UtcNow; //.AddMinutes(minutesToCheck * (-1));
            //}
            timeStart.Value = timeStart1.Value;
            timeEnd.Value = DateTime.Now.AddMinutes(-1);
            if (timeStart.Value < DateTime.Now.AddMinutes(-1))
            {
                //means collect some previous data until now then carry on with current collection
                //timeEnd.Value = DateTime.Now.AddMinutes(minutesToCheck * (-1));
                //btnHistoricData_Click(sender, e);
                timeStart.Value = timeStart.Value.AddMinutes(1);
                //timeEnd.Value = timeEnd.Value.AddMinutes(1);
                loadData(timeStart.Value, timeEnd.Value);
                btnFindDetailRelated_Click(sender, e);
                timeStart.Value = DateTime.Now; //.AddMinutes(minutesToCheck * (-1));
            }

            timer1.Start();
        }

        private void btnStopTimer_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void btnResetInterval_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            minutesToCheck = int.Parse(cboInterval.SelectedItem.ToString());
            minutesToCheckString = $"_{cboInterval.SelectedItem.ToString()}";
            if (minutesToCheck == 1)
            {
                minutesToCheckString = "";
            }
            //timer1.Interval = (int)TimeSpan.FromMinutes(minutesToCheck).TotalMilliseconds;
            btnClear_Click(this, new EventArgs());
            if (!(sender is ComboBox))
            {
                timer1.Start();
            }

        }

        private void cboInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnResetInterval_Click(cboInterval, new EventArgs());
            sendStatus();
        }

        private void btnSetSwitch_Click(object sender, EventArgs e)
        {
            switchViews = new List<PriceView>();
            //get correct time's hour
            //if odd, then -1
            int hour = timeStart.Value.Hour;
            if (hour % 2 != 0)
            {
                hour -= 1;
            }
            DateTime endSwitch = new DateTime(timeStart.Value.Year, timeStart.Value.Month, timeStart.Value.Day, hour, 0, 0);
            DateTime startSwitch = endSwitch.AddHours(-36);
            var response = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=HOUR_2&from={startSwitch.ToString("yyyy-MM-ddTHH:mm")}&to={endSwitch.ToString("yyyy-MM-ddTHH:mm")}", apiKey, xToken, cstToken, Method.GET);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var switchPrices = JsonConvert.DeserializeObject<PriceData>(response.Content);
                if (switchPrices.metadata.pageData.totalPages > 1)
                {
                    for (int i = 2; i <= switchPrices.metadata.pageData.totalPages; i++)
                    {
                        var responsePage = APIHelper.makeRequest($"https://api.ig.com/gateway/deal/prices/{txtEpic.Text}?resolution=HOUR_2&from={startSwitch.ToString("yyyy-MM-ddTHH:mm")}&to={endSwitch.ToString("yyyy-MM-ddTHH:mm")}&pageNumber={i}", apiKey, xToken, cstToken, Method.GET);
                        if (responsePage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var pricesPage = JsonConvert.DeserializeObject<PriceData>(responsePage.Content);
                            switchPrices.prices.AddRange(pricesPage.prices);
                        }
                        else
                        {
                            txtLog.Text += $"Non 200 response:{responsePage.StatusDescription}";
                        }
                    }
                }
                PriceView previousPrice = new PriceView() { SnapshotTime = String.Empty };
                foreach (var price in switchPrices.prices)
                {
                    var pview = new PriceView()
                    {
                        AskClose = price.closePrice.ask,
                        AskHigh = price.highPrice.ask,
                        AskLow = price.lowPrice.ask,
                        AskOpen = price.openPrice.ask,
                        //BidClose = price.closePrice.bid,
                        //BidHigh = price.highPrice.bid,
                        //BidLow = price.lowPrice.bid,
                        //BidOpen = price.openPrice.bid,
                        SnapshotTime = price.snapshotTime
                    };
                    if (String.IsNullOrWhiteSpace(previousPrice.SnapshotTime))
                    {
                        pview.Ask2BarReversal = false;
                        //pview.Bid2BarReversal = false;
                    }
                    else
                    {
                        pview.Ask2BarReversal = (previousPrice.AskUpDown != pview.AskUpDown && (previousPrice.AskUpDown != Library.None && pview.AskUpDown != Library.None));
                        //pview.Bid2BarReversal = previousPrice.BidUpDown != pview.BidUpDown;
                    }
                    switchViews.Add(pview);
                    previousPrice = pview;
                }

            }
            bool isProbeSuccessfull = false;
            var probe = switchViews.Last();
            var indicator = switchViews.Count - 2;
            var p1 = switchViews[indicator];
            var p2 = switchViews[indicator - 1];
            if (p1.Ask2BarReversal && p1.AskUpDown == Library.UpBar)
            {
                isProbeSuccessfull = (probe.AskUpDown == Library.DownBar && probe.AskLow <= p2.AskOpen);
            }
            if (p1.Ask2BarReversal && p1.AskUpDown == Library.DownBar)
            {
                isProbeSuccessfull = (probe.AskUpDown == Library.UpBar && probe.AskHigh >= p1.AskOpen);
            }
            if (isProbeSuccessfull)
            {

            }
            if (probe.AskClose > probe.AskOpen)
            {
                if (isProbeSuccessfull)
                {
                    rdoSwitchUp.Checked = true;
                }
                else
                {
                    rdoSwitchDown.Checked = true;
                }
            }
            else
            {
                if (isProbeSuccessfull)
                {
                    rdoSwitchDown.Checked = true;
                }
                else
                {
                    rdoSwitchUp.Checked = true;
                }
            }
            OhlcSeries oSeries = new OhlcSeries();

            foreach (var pview in switchViews)
            {

                oSeries.DataPoints.Add(new OhlcDataPoint((double)pview.AskOpen, (double)pview.AskHigh, (double)pview.AskLow, (double)pview.AskClose, pview.TimeCategory));
            }
            //LinearAxis linearAxis1 = (LinearAxis)radChartView2.Axes[1];
            //linearAxis1.Minimum = (double)switchViews.Min(p => p.AskLow) - 2;
            //linearAxis1.Maximum = (double)switchViews.Max(p => p.AskHigh) + 2;

            //radChartView2.Series.Clear();
            //radChartView2.Series.Add(oSeries);

        }

        private void saveProbesToLog(List<PriceView> probes)
        {
            var fs = File.CreateText(logFile);
            foreach (var p in probes)
            {
                fs.WriteLine($"{p.TimeCategory}-{p.IsDownProbe}-{p.IsUpProbe}-{p.ProbeRelations}");
            }
            fs.Flush();
            fs.Close();
        }

        private void reduceDataSize()
        {
            if (priceViews.Any())
            {
                var pLast = priceViews.Last();
                DateTime earliestTime = pLast.SnapshotDateTime.AddHours(-6);
                priceViews.RemoveAll(p => p.SnapshotDateTime < earliestTime);
            }
        }

        private void btnLoginTrade_Click(object sender, EventArgs e)
        {
            string loginMessage = string.Empty;
            var tradeAccountInfo = TradeHelper.GetTradingAccount();
            lblBalance.Text = tradeAccountInfo.accounts.Where(a => a.accountType == "SPREADBET").FirstOrDefault()?.balance?.balance.ToString();

        }

        private void btnPlaceTrade_Click(object sender, EventArgs e)
        {
            txtDealReference.Text = TradeHelper.PlaceTrade(txtEpic.Text, cboDirection.SelectedItem.ToString() == "BUY" ? TradeDirection.Buy : TradeDirection.Sell, float.Parse(txtSize.Text), int.Parse(txtStopDistance.Text), int.Parse(txtLimitDistance.Text), true, 1);
            //var tradePositionUrl = "https://demo-api.ig.com/gateway/deal/positions/otc";
            //TradeRequest tradeReq = new TradeRequest()
            //{
            //    epic = txtEpic.Text,
            //    direction = cboDirection.SelectedItem.ToString(),
            //    size = float.Parse(txtSize.Text),
            //    stopDistance = int.Parse(txtStopDistance.Text),
            //    limitDistance = int.Parse(txtLimitDistance.Text)
            //};
            //var tradeResponse = APIHelper.MakeTradeRequest(tradePositionUrl, tradeApiKey, tradeXToken, tradeCstToken, tradeReq);
            //if (tradeResponse.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var dealRef = JsonConvert.DeserializeObject<TradeResponse>(tradeResponse.Content);
            //    txtDealReference.Text = dealRef.dealReference;
            //}
            //else
            //{
            //    txtLog.Text = tradeResponse.Content;
            //}

        }

        private void btnCloseDeal_Click(object sender, EventArgs e)
        {
            //string url = "https://demo-api.ig.com/gateway/deal/positions";
            //var response = APIHelper.makeRequest(url, tradeApiKey, tradeXToken, tradeCstToken, Method.GET, "2");
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    var position = JsonConvert.DeserializeObject<TradePosition>(response.Content);
            //    var positionToCloseDeal = position.positions.Where(p => p.position.dealReference == txtDealReference.Text).FirstOrDefault();
            //    var dealIdToClose = positionToCloseDeal.position.dealId;
            //    var responseClose = APIHelper.CloseTradeRequest("https://demo-api.ig.com/gateway/deal/positions/otc", tradeApiKey, tradeXToken, tradeCstToken, new CloseTradeRequest()
            //    {
            //        dealId = dealIdToClose,
            //        direction = cboDirection.SelectedItem.ToString() == "BUY" ? "SELL" : "BUY",
            //        size = float.Parse(txtSize.Text)
            //    });
            //}

            TradeHelper.CloseTrade(txtDealReference.Text);
        }

        private void rebindProbes()
        {

            BindingSource source = new BindingSource();
            source.DataSource = allProbes.Distinct(new PriceViewComparer()).Select(p => new { Time = p.TimeCategory, UpDown = p.IsUpProbe ? "U" : "D", Relations = p.ProbeRelations, Qualified = p.ProbeQualified, Qualifications = p.ProbeQualifications }).OrderByDescending(p => p.Time);
            dataGridView2.Refresh();
            dataGridView2.DataSource = source;

            if (allProbes.Count > 0)
            {
                dataGridView2.Columns[0].Width = 40;
                dataGridView2.Columns[1].Width = 30;
                dataGridView2.Columns[2].Width = 90;
                dataGridView2.Columns[3].Width = 30;
                dataGridView2.Columns[4].Width = 60;
            }
            BindingSource sourceQualified = new BindingSource();
            sourceQualified.DataSource = allProbes.Where(p => p.ProbeQualified).Distinct(new PriceViewComparer()).Select(p => new { Time = p.TimeCategory, UpDown = p.IsUpProbe ? "U" : "D", Qualifications = p.ProbeQualifications, Close = p.AskClose }).OrderByDescending(p => p.Time);
            dataGridView3.Refresh();
            dataGridView3.DataSource = sourceQualified;

            if (dataGridView3.RowCount > 0)
            {
                dataGridView3.Columns[0].Width = 40;
                dataGridView3.Columns[1].Width = 50;
                dataGridView3.Columns[2].Width = 150;
                dataGridView3.Columns[3].Width = 50;

            }

            BindingSource sourceCounterQualified = new BindingSource();
            sourceCounterQualified.DataSource = allCounterProbes.Where(p => p.ProbeQualified && (relationToCheck == Relations.DownOnly ? p.IsUpProbe : p.IsDownProbe)).Distinct(new PriceViewComparer()).Select(p => new { Time = p.TimeCategory, UpDown = p.IsUpProbe ? "U" : "D", Qualifications = p.ProbeQualifications, Close = p.AskClose }).OrderByDescending(p => p.Time);
            dataGridView4.Refresh();
            dataGridView4.DataSource = sourceCounterQualified;

            if (dataGridView4.RowCount > 0)
            {
                dataGridView4.Columns[0].Width = 40;
                dataGridView4.Columns[1].Width = 50;
                dataGridView4.Columns[2].Width = 150;
                dataGridView4.Columns[3].Width = 50;

            }

        }

        private void cboSignallingMinutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            signallingWindowMinutes = int.Parse(cboSignallingMinutes.SelectedItem.ToString()) * (-1);
            sendStatus();
        }

        private void cboProbeWindowMinutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            probeWindowMinutes = int.Parse(cboProbeWindowMinutes.SelectedItem.ToString());
            sendStatus();
        }

        private void rdoBoth_CheckedChanged(object sender, EventArgs e)
        {
            relationToCheck = Relations.Both;
            if (rdoBoth.Checked)
            {
                sendStatus();
            }

        }

        private void rdoDownOnly_CheckedChanged(object sender, EventArgs e)
        {
            relationToCheck = Relations.DownOnly;
            TradeDirection currentDirection;
            if (chkTrade.Checked && TradeHelper.numberOfOpenPositions(out currentDirection) > 0)
            {
                if (currentDirection == TradeDirection.Sell)
                {
                    //if sells are open, close them
                    TradeHelper.CloseExistingTrades = true;
                    MessageBox.Show("Will close all open trades");
                }
            }
            if (rdoDownOnly.Checked)
            {
                sendStatus();
            }

        }

        private void rdoUpOnly_CheckedChanged(object sender, EventArgs e)
        {
            relationToCheck = Relations.UpOnly;
            TradeDirection currentDirection;
            if (chkTrade.Checked && TradeHelper.numberOfOpenPositions(out currentDirection) > 0)
            {
                if (currentDirection == TradeDirection.Buy)
                {
                    TradeHelper.CloseExistingTrades = true;
                    MessageBox.Show("Will close all open trades");
                }
            }
            if (rdoUpOnly.Checked)
            {
                sendStatus();
            }

        }

        private void checkProbeQualification(PriceView probe, IEnumerable<PriceView> pViews, int filterWindowMins)
        {
            var relationTimes = probe.ProbeRelations.Split(',');
            if (relationTimes.Length == 3)
            {
                var priceViewPair = new PriceViewPair(pViews.First(p => p.TimeCategory == relationTimes[0]), pViews.SkipWhile(p => p.TimeCategory != relationTimes[0]).Skip(1).First());
                //check of qualification, if succeeds, then good else look in the remaining pairs
                probe = SignalHelper.CheckQualification(priceViewPair, probe, pViews, filterWindowMins);
                if (!probe.ProbeQualified)
                {
                    priceViewPair = new PriceViewPair(pViews.First(p => p.TimeCategory == relationTimes[1]), pViews.SkipWhile(p => p.TimeCategory != relationTimes[1]).Skip(1).First());
                    probe = SignalHelper.CheckQualification(priceViewPair, probe, pViews, filterWindowMins);
                    if (!probe.ProbeQualified)
                    {
                        priceViewPair = new PriceViewPair(pViews.First(p => p.TimeCategory == relationTimes[2]), pViews.SkipWhile(p => p.TimeCategory != relationTimes[2]).Skip(1).First());
                        probe = SignalHelper.CheckQualification(priceViewPair, probe, pViews, filterWindowMins);
                    }
                }
            }

        }


        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterWindowMinutes = int.Parse(cboFilter.Value.ToString()) * (-1);
            sendStatus();
        }

        private void btnLoginCreds_Click(object sender, EventArgs e)
        {
            LoginCreds loginForm = new LoginCreds();
            loginForm.ShowDialog();
            readCredentials();
        }
        private void readCredentials()
        {
            APICredentials dataCredentials = new APICredentials();
            APICredentials tradeCredentials = new APICredentials();
            Library.ReadCredentials(ref dataCredentials, ref tradeCredentials);
            userName = dataCredentials.UserName;
            password = dataCredentials.Password;
            apiKey = dataCredentials.APIKey;
            tradeUserName = tradeCredentials.UserName;
            tradePassword = tradeCredentials.Password;
            tradeApiKey = tradeCredentials.APIKey;

        }
        private void readTokens()
        {
            userData = new FileStream(Application.UserAppDataPath + "\\appdata.txt", FileMode.OpenOrCreate);
            string sessionData = String.Empty;
            using (StreamReader userReader = new StreamReader(userData))
            {
                sessionData = userReader.ReadLine();
                if (!String.IsNullOrWhiteSpace(sessionData))
                {
                    cstToken = sessionData;
                }
                sessionData = userReader.ReadLine();
                if (!String.IsNullOrWhiteSpace(sessionData))
                {
                    xToken = sessionData;
                }
            }

        }

        private void dataGridView3_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                if (dataGridView3.RowCount > 0)
                {
                    if (dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString() == "U")
                    {
                        dataGridView3.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
                    }
                    else
                    {
                        dataGridView3.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
            }
            catch
            {

            }

        }

        private void dataGridView4_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                if (dataGridView4.RowCount > 0)
                {
                    if (dataGridView4.Rows[e.RowIndex].Cells[1].Value.ToString() == "U")
                    {
                        dataGridView4.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
                    }
                    else
                    {
                        dataGridView4.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
            }
            catch
            {

            }

        }

        private void chkSAR_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoSARCloseLater_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoSARCloseLater.Checked)
            {
                TradeHelper.CloseOptions = SARCloseOptions.CloseLater;
            }
        }

        private void rdoSARCloseNow_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoSARCloseNow.Checked)
            {
                TradeHelper.CloseOptions = SARCloseOptions.CloseNow;
            }
        }

        private void sendStatus()
        {
#if !DEBUG
            if (chkTrade.Checked)
            {
                TradeState state = new TradeState()
                {
                    Interval = cboInterval.Text,
                    SignallingWindow = cboSignallingMinutes.Text,
                    ProbeWindow = cboProbeWindowMinutes.Text,
                    FilterWindow = cboFilter.Value.ToString(),
                    Relations = rdoBoth.Checked ? "Both" : (rdoUp.Checked ? "Up" : "Down"),
                    LimitDistance = int.Parse(txtLimitDistance.Text),
                    StopDistance = int.Parse(txtStopDistance.Text),
                    Size = decimal.Parse(txtSize.Text),
                    Epic = txtEpic.Text
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;

                Task.Run(() => Library.SendStatusEmail(Newtonsoft.Json.JsonConvert.SerializeObject(state, settings)));

            }
#endif
        }

        private void chkUseLiveAccount_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseLiveAccount.Checked)
            {
                LiveCredentials liveCreds = new LiveCredentials();
                liveCreds.ShowDialog();
                if (!liveCreds.IsConfirmed)
                {
                    chkUseLiveAccount.Checked = false;
                    TradeHelper.TradeUrl = Library.DemoTradingUrl;
                }
                else
                {
                    TradeHelper.TradeUrl = LiveTradingUrl;

                }
            }
            else
            {
                TradeHelper.TradeUrl = Library.DemoTradingUrl;

            }
        }

        private void txtAmountToCheck_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void txtAmountToCheck_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnStartSwitchSearch_Click(object sender, EventArgs e)
        {
            //once the switch search has started, set the form level option that switch search is on
            //on timer, look if the switch params are breached
            //if they are then turn on the automated trade, set the directional switch (should close existing trades, if needed)
            //then also check if a flash trade needs to be put, if so, then put a trade in the direction chosen and look for the next hour
            //if the hourly trade also breaches the conditions, then put the probe trade as well
            //all this within the expiry period
            //once action has taken place, set the label, turn off options, clear the textbox and radio(optional), ie perform reset
            decimal amountToCheck = 0;
            if (!decimal.TryParse(txtAmountToCheck.Text, out amountToCheck))
            {
                MessageBox.Show("Incrrect price to check", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                label30.Text = "Switch search is in progress...";
                SwitchHelper.PriceToCheck = amountToCheck;
                SwitchHelper.SwitchSearchStartTime = DateTime.Now;
                SwitchHelper.SwitchSearchEndTime = SwitchHelper.SwitchSearchStartTime.AddMinutes(double.Parse(cboSwitchCheckValidity.Text));
                SwitchHelper.SwitchDirection = (rdoSwitchUp.Checked ? TradeDirection.Sell : TradeDirection.Buy);
                SwitchHelper.SwitchStarted = true;
                txtAmountToCheck.Enabled = false;
                rdoSwitchUp.Enabled = false;
                rdoSwitchDown.Enabled = false;
                chkPerformFlashTrade.Enabled = false;
                chkPerformProbeTrade.Enabled = false;
                cboSwitchCheckValidity.Enabled = false;
            }
        }
        private void ResetSwitch()
        {
            txtAmountToCheck.Text = string.Empty;
            rdoSwitchUp.Checked = false;
            rdoSwitchDown.Checked = false;
            chkPerformFlashTrade.Checked = false;
            chkPerformProbeTrade.Checked = false;
            label30.Text = string.Empty;
            SwitchHelper.PriceToCheck = 0;
            SwitchHelper.SwitchSearchStartTime = DateTime.Today.AddDays(100);
            SwitchHelper.SwitchSearchEndTime = DateTime.Today.AddDays(100);
            switchHasBeenReset = true;
            txtAmountToCheck.Enabled = true;
            rdoSwitchUp.Enabled = true;
            rdoSwitchDown.Enabled = true;
            chkPerformFlashTrade.Enabled = true;
            chkPerformProbeTrade.Enabled = true;
            cboSwitchCheckValidity.Enabled = true;
        }
        private void SetSwitch()
        {
            chkTrade.Checked = true;
            if (SwitchHelper.SwitchDirection == TradeDirection.Sell)
            {
                rdoUpOnly.Checked = true;
                //rdoUpOnly_CheckedChanged(this, new EventArgs());
            }
            else if (SwitchHelper.SwitchDirection == TradeDirection.Buy)
            {
                rdoDownOnly.Checked = true;
                //rdoDownOnly_CheckedChanged(this, new EventArgs());
            }
            if (chkPerformFlashTrade.Checked)
            {
                TradeHelper.PlaceTrade(txtEpic.Text, SwitchHelper.SwitchDirection, double.Parse(txtSize.Text), int.Parse(txtStopDistance.Text), int.Parse(txtLimitDistance.Text), !rdoBoth.Checked, (int)txtMaxTrades.Value);
            }
            if (chkPerformProbeTrade.Checked)
            {
                //set trade to take place on the next hour
                probeTradeTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, DateTime.Now.AddHours(1).Hour, 0, 0);
                probeTradeDirection = SwitchHelper.SwitchDirection;
                probeAmount = SwitchHelper.PriceToCheck;
            }
            switchHasBeenReset = true;
            ResetSwitch();
        }
        private bool IsTimeForProbeTrade(PriceView CurrentPriceData)
        {
            DateTime probeTrade = DateTime.Now.AddYears(1);
            if (probeTradeTime != null)
            {
                probeTrade = (DateTime)probeTradeTime;
            }
            return probeTrade.Year == DateTime.Now.Year && probeTrade.Month == DateTime.Now.Month && probeTrade.Day == DateTime.Now.Day && probeTrade.Hour == DateTime.Now.Hour && CurrentPriceData.SnapshotDateTime.Minute == 0;
        }

        private bool IsConditionMetForProbeTrade(PriceView CurrentPriceData)
        {
            if (probeTradeDirection == TradeDirection.Buy)
            {
                return CurrentPriceData.AskClose >= probeAmount;
            }
            else if (probeTradeDirection == TradeDirection.Sell)
            {
                return CurrentPriceData.AskClose <= probeAmount;
            }
            return false;
        }
        private void btnCancelSwitchSearch_Click(object sender, EventArgs e)
        {
            ResetSwitch();
        }
    }
}
