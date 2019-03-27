using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;
/*
 * 
 *               KEREM ÇALIŞKAN
 * 
*/
/*
 *Binance Auto Coin Tracker
 * Still working on
*/

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        //Basic variables
        static WebClient client = new WebClient();
        static XmlDocument xml = new XmlDocument();
        static string json;
        static XNode node;
        static XmlNodeList list;
        static string symbol;
        static string price;
        static int sayac = 0, btcsayac, ethsayac, bnbsayac, usdtsayac, btcsize, ethsize, bnbsize, usdtsize;
        static int arraysize;
        static string[] symbols;
        static string[] btcsymbols;
        static string[] ethsymbols;
        static string[] bnbsymbols;
        static string[] usdtsymbols;
        static float[] prices;
        static float[] btcprices;
        static float[] ethprices;
        static float[] bnbprices;
        static float[] usdtprices;
        static Thread thread1 = new Thread(new ThreadStart(Download));
        static Thread thread2 = new Thread(new ThreadStart(ArraySpliter));
        static bool downloadworkDone, getworkDone, printworkDone, labelchanger;
        static int bnbbtc, bnbusdt, bnbeth, btcusdt, ethusdt;



        static public void ArraySize()//Each day numbers of coins changes so it should be checked. This function counts how many coin exist.
        {
            arraysize = 0;
            foreach (XmlNode item in list)
            {
                arraysize++;
            }
            Array.Resize(ref symbols, arraysize);
            Array.Resize(ref prices, arraysize);
        }

        static public void ArraySize2()//This one counts coins units.
        {
            btcsize = 0;
            ethsize = 0;
            bnbsize = 0;
            usdtsize = 0;
            for (int i = 0; i < arraysize; i++)
            {
                if (symbols[i].Contains("BTC"))
                {
                    btcsize++;
                }
                if (symbols[i].Contains("ETH"))
                {
                    ethsize++;
                }
                if (symbols[i].Contains("BNB"))
                {
                    bnbsize++;
                }
                if (symbols[i].Contains("USDT"))
                {
                    usdtsize++;
                }
            }
            Array.Resize(ref btcsymbols, btcsize);
            Array.Resize(ref btcprices, btcsize);
            Array.Resize(ref ethsymbols, ethsize);
            Array.Resize(ref ethprices, ethsize);
            Array.Resize(ref bnbsymbols, bnbsize);
            Array.Resize(ref bnbprices, bnbsize);
            Array.Resize(ref usdtsymbols, usdtsize);
            Array.Resize(ref usdtprices, usdtsize);
        }

        static public void Download()// Get data from link, but its format is not suitable for list. So applied some replacement progress.
        {
            // Just tried something...
            //var apiClient = new ApiClient("x2tS7m81JmFai5kdo7eNDI7cmhSEmt83PQ6SYLSV53Snq4iLXIQQoBtxLOtOTdhI", "vyBEJaDD8kAsJ2tKwtxUbhbUB4CPSgIEaemR6rLgCM9MpXFxxkDjOT99vNjD3wKP");
            //var binanceClient = new BinanceClient(apiClient);
            for (; ; )
            {

                json = client.DownloadString("https://www.binance.com/api/v3/ticker/price"); // Gets data from binance api.
                json = json.Replace("{", "'Prices':{");
                json = json.Replace("[", "{'Ticker':{");
                json = json.Replace("]", "}");
                node = Newtonsoft.Json.JsonConvert.DeserializeXNode(json); // Converts json to node.
                xml.LoadXml(node.ToString());
                list = xml.SelectNodes("Ticker/Prices");
                ArraySize();
                foreach (XmlNode item in list) // Splits elements.
                {
                    symbol = item["symbol"].InnerText;
                    price = item["price"].InnerText;
                    symbols[sayac] = symbol;
                    prices[sayac] = float.Parse(price, CultureInfo.InvariantCulture.NumberFormat);
                    sayac++;
                }
                ArraySize2();
                sayac = 0;
                downloadworkDone = true;
            }
        }

        static public void ArraySpliter() //This one separates coins according to the its units.
        {
            for (; ; )
            {
                if (downloadworkDone)
                {
                    btcsayac = 0;
                    ethsayac = 0;
                    bnbsayac = 0;
                    usdtsayac = 0;
                    for (int i = 0; i < arraysize; i++)
                    {
                        if (symbols[i].Contains("BTC"))
                        {
                            btcsymbols[btcsayac] = symbols[i];
                            btcprices[btcsayac] = prices[i];
                            btcsayac++;
                        }
                        if (symbols[i].Contains("ETH"))
                        {
                            ethsymbols[ethsayac] = symbols[i];
                            ethprices[ethsayac] = prices[i];
                            ethsayac++;
                        }
                        if (symbols[i].Contains("BNB"))
                        {
                            bnbsymbols[bnbsayac] = symbols[i];
                            bnbprices[bnbsayac] = prices[i];
                            bnbsayac++;
                        }
                        if (symbols[i].Contains("USDT"))
                        {
                            usdtsymbols[usdtsayac] = symbols[i];
                            usdtprices[usdtsayac] = prices[i];
                            usdtsayac++;
                        }
                    }
                }
            }
        }

        public void Get() // Gets processed data.
        {
            for (; ; )
            {
                if (downloadworkDone)
                {
                    btcusdt = Array.IndexOf(symbols, "BTCUSDT");
                    ethusdt = Array.IndexOf(symbols, "ETHUSDT");
                    bnbusdt = Array.IndexOf(symbols, "BNBUSDT");
                    //ProfitCalculator1();
                    //ProfitCalculator2();
                }
            }
        }

        public void ProfitCalculator1()// not working as i want :(
        {
            for (int i = 0; i < btcprices.Length; i++)
            {
                for (int j = 0; j < ethprices.Length; j++)
                {
                    if (btcsymbols[i].Replace("BTC", "") == ethsymbols[j].Replace("ETH", ""))
                    {
                        /*if (((btcprices[i] * prices[btcusdt]) - (ethprices[j] * prices[ethusdt])) > 0.08 || ((btcprices[i] * prices[btcusdt]) - (ethprices[j] * prices[ethusdt])) < -0.08)
                        {
                            MessageBox.Show("price1 : " + (btcprices[i] * prices[btcusdt]) + " " + btcsymbols[i] + "\nprice2 : " + (ethprices[j] * prices[ethusdt]) + " " + ethsymbols[j] + "\nfark : " + ((btcprices[i] * prices[btcusdt]) - (ethprices[j] * prices[ethusdt])));
                        }*/
                        if ((btcprices[i] * prices[btcusdt]) - (ethprices[j] * prices[ethusdt]) > 0.08)
                        {
                            MessageBox.Show("Kârlı Coin : " + ethsymbols[j] + "\nFiyat : " + ethprices[j] + "\nKâr : " + ((btcprices[i] * prices[btcusdt]) - (ethprices[j] * prices[ethusdt])));
                        }
                        if ((btcprices[i] * prices[btcusdt]) - (ethprices[j] * prices[ethusdt]) < -0.08)
                        {
                            MessageBox.Show("Kârlı Coin : " + btcsymbols[i] + "\nFiyat : " + btcprices[i] + "\nKâr : " + ((ethprices[j] * prices[ethusdt]) - (btcprices[i] * prices[btcusdt])));
                        }
                    }
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

            thread1.Start();
            thread2.Start();
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
            button1.Enabled = false;
        }

        public void comboBoxItems()
        {
            for (; ; )
            {
                if (downloadworkDone)
                {
                    for (int i = 0; i < symbols.Length; i++)
                    {
                        comboBox1.Items.Add(symbols[i]);
                        comboBox2.Items.Add(symbols[i]);
                        comboBox3.Items.Add(symbols[i]);
                        comboBox4.Items.Add(symbols[i]);
                        comboBox5.Items.Add(symbols[i]);
                    }
                    break;
                }
            }
            button1.Enabled = true;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Get();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            comboBoxItems();
            labelController();
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelChanger();
        }

        public void labelChanger()
        {
            label1.Text = comboBox1.Text;
            label2.Text = comboBox2.Text;
            label3.Text = comboBox3.Text;
            label4.Text = comboBox4.Text;
            label5.Text = comboBox5.Text;
            labelchanger = true;
        }

        public void labelController()
        {
            for (; ; )
            {
                if (labelchanger)
                {
                    label6.Text = prices[Array.IndexOf(symbols, label1.Text)].ToString();
                    label7.Text = prices[Array.IndexOf(symbols, label2.Text)].ToString();
                    label8.Text = prices[Array.IndexOf(symbols, label3.Text)].ToString();
                    label9.Text = prices[Array.IndexOf(symbols, label4.Text)].ToString();
                    label10.Text = prices[Array.IndexOf(symbols, label5.Text)].ToString();
                    label11.Text = "Up to date";
                    Thread.Sleep(2000);
                }
            }
        }
    }
}