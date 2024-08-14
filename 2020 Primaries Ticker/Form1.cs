using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Sockets;
using System.Net;
using log4net;
using log4net.Appender;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
//using System.Threading;


namespace _2020_Primaries_Ticker
{
    public partial class Election_Ticker_2020 : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static TcpClient client = new TcpClient();
        public static TcpListener server;
        public static NetworkStream stream;
        public string trioString = "";
        public string activeCR = "";
        public int e_bug_mode = 0;
       

        public int bugMode = 0;
        public string bugKey = "";
        public bool pollBugIn = false;
        public int pollCounter = 0;

        public int TickerType;
        public int PromoCounter = 0;
        public int FactCounter = 0;
        public int FactCounter2 = 0;
        public int AnalysisCounter = 0;
        public int ActiveStateCounter = 0;
        public int RaceCounter = 0;
        public int OneLineRaceCounter = 0;
        public int PrePromoCounter = 0;

        public int CanScrollCount = 3;
        public int DataPreDataCounter = 0;

        public int leftSideCounter = 0;
        public int rightSideCounter = 0;

        public string CurrentPreDataOption = "";
        public string KeyValue = "ONE_LINE";
        public string DataMode = "NULL";
        public string hostIP = "";
        public string currentRaceState = "";

        public bool StartTicker = false;
        public bool PollLockoutEnabled = false;
        public bool ZeroPctsLockoutEnabled = true;
        public bool UseExpectedVoteIn = true;
        public bool UseSimTime = false;
        public bool UseTenthsPercent = true;
        public bool isVizConnected = false;
        public bool LeftCup = false;
        public bool RightCup = false;
        public bool AreRaces = false;
        public bool IsDataMode = false;
        public bool InsertClosing = false;
        public bool PromoIn = true;
        public bool LEmger = false;
        public bool REmger = false;
        public bool annOn = false;

        public static bool ErrorFlag = false;

        public static string AppName = "";

        public List<Races> ActiveRaces = new List<Races>();

        public List<Races> LeftSideRace = new List<Races>();
        public List<Races> RightSideRace = new List<Races>();
        public List<Races> SingleSideRace = new List<Races>();

        public List<string> PreDataStates = new List<string>();
        public List<string> PreDataStatesFull = new List<string>();

        public Queue<string> PreDataSchedule = new Queue<string>();
        public Queue<string> NextClosings = new Queue<string>();

        public static Flags flag = new Flags();

        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static DateTime SimTime = new DateTime();

        public static Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public string RightSideString2 = "";
        public string RightSideType = "";
        public bool UseDiffRightSide = false;
        public int BopCounter;
        public int GainCounter;
        public int NetworkID;
        public static int RightSideTime = 1;

        public class RightSideOptions
        {
            public string OptionName;
            public string OptionType;
            public int OptionTime;
            public bool OptionEnabled;
            public NumericUpDown RSTime;
            public CheckBox RSCb;
        }

        List<RightSideOptions> RightSideList = new List<RightSideOptions>();

        public class ApplicationInfo
        {
            public string ApplicationID = "";
            public string ApplicationName = "";
            public string ConnectionString1 = "";
            public string ConnectionString2 = "";
            public string ConnectionString3 = "";
            public string ApplicationNetwork = "";
            public string EngineIP = "";
            public string EnginePort = "";
            public string EngineIP2 = "";
            public string EnginePort2 = "";
            public string commPort = "";
            public string ApplicationListeningPort = "";
            public string controllerIP = "";
        }

        public class foxId
        {
            public string id = "";
        }
        public class raceData
        {
            public List<candidates> candidates = new List<candidates>();
            public string state = "";
            public string cd = "";
            public string precincts = "";
            public string office = "";
            public string raceMode = "";
            public string delegates = "";
            public string evotes = "";
        }

        public class candidates
        {
            public string name { get; set; }
            public string party { get; set; }
            public string incum { get; set; }
            public string vote { get; set; }
            public string percent { get; set; }
            public string check { get; set; }
            public string gain { get; set; }
            public string imagePath { get; set; }
        }

        public class fullData
        {
            public List<foxId> foxId = new List<foxId>();
            public raceData raceData = new raceData();
        }
        public static ApplicationInfo AppInfo;

        public List<int> GetGovStates = new List<int>();
        public List<int> GetSenateStates = new List<int>();
        public List<int> GetHouseStates = new List<int>();

        public Queue<SidePanelData> PromoQueue = new Queue<SidePanelData>();
        public Queue<SidePanelData> BopQueue = new Queue<SidePanelData>();
        public Queue<SidePanelData> CensusQueue = new Queue<SidePanelData>();
        public Queue<SidePanelData> FactsQueue = new Queue<SidePanelData>();
        public Queue<SidePanelData> EVQueue = new Queue<SidePanelData>();
        public Queue<SidePanelData> R2WQueue = new Queue<SidePanelData>();

        public Queue<string> QIds = new Queue<string>();

        public Queue<DateTime> CloseTimes = new Queue<DateTime>();
        public Queue<DateTime> CloseTimes2 = new Queue<DateTime>();

        public List<VoterAnalysisData> AllVaStatesData = new List<VoterAnalysisData>();

        public List<HouseDistricts> houseInitList = new List<HouseDistricts>();

        public List<ECVotes> CollegeVotes = new List<ECVotes>();

        public int DisplayCandidates = 1;
        public int candStartIndex = 0;
        public bool nextRace = true;
        public int canCont = 1;

        public static TcpClient FoxPlus;
        public static NetworkStream FoxPlusStream;

        public static _2020_Primaries_Ticker.ClientSocket VizControl;

        public string showHS;

        public Election_Ticker_2020()
        {
            
            try
            {
                InitializeComponent();

                try
                {
                    JObject a = JObject.Parse(File.ReadAllText(@"C:\Windows\BMTAppData\Election_Config.json"));
                    
                    var engineList = a.SelectTokens("Engine[*]");

                    AppName = a.SelectToken("AppName").ToString();

                    numericUpDown1.Value = Convert.ToDecimal(config.AppSettings.Settings["BopTime"].Value);
                    numericUpDown2.Value = Convert.ToDecimal(config.AppSettings.Settings["PopTime"].Value);
                    numericUpDown3.Value = Convert.ToDecimal(config.AppSettings.Settings["ElecTime"].Value);
                    numericUpDown4.Value = Convert.ToDecimal(config.AppSettings.Settings["NetTime"].Value);
                    TickerType = Convert.ToInt32(config.AppSettings.Settings["tickerLineType"].Value);

                    RSBop.Checked = Convert.ToBoolean(config.AppSettings.Settings["BopCheck"].Value);
                    RSPopVote.Checked = Convert.ToBoolean(config.AppSettings.Settings["PopCheck"].Value);
                    RSElectVote.Checked = Convert.ToBoolean(config.AppSettings.Settings["ElecCheck"].Value);
                    RSNetGain.Checked = Convert.ToBoolean(config.AppSettings.Settings["NetCheck"].Value);
                    RSPollClose.Checked = Convert.ToBoolean(config.AppSettings.Settings["PollClose"].Value);
                    EnabledRightSide.Checked = Convert.ToBoolean(config.AppSettings.Settings["UseRight"].Value);


                    if (config.AppSettings.Settings["BopOption"].Value == "Both")
                    {
                        BopCounter = 0;
                        radioButton1.Checked = true;
                    }
                    else if(config.AppSettings.Settings["BopOption"].Value == "House")
                    {
                        BopCounter = 1;
                        radioButton3.Checked = true;
                    }
                    else
                    {
                        BopCounter = 0;
                        radioButton2.Checked = true;
                    }

                    foreach (CheckBox x in groupBox3.Controls.OfType<CheckBox>())
                    {
                        RightSideOptions foo = new RightSideOptions
                        {
                            RSCb = x,
                            OptionName = x.Text
                        };

                        if(foo.OptionName.Contains("POWER"))
                        {
                            foo.RSTime = numericUpDown1;
                            foo.OptionType = "BALANCE_OF_POWER";
                        }
                        else if(foo.OptionName.Contains("POPULAR"))
                        {
                            foo.RSTime = numericUpDown2;
                            foo.OptionType = "POPULAR_VOTE";
                        }
                        else if (foo.OptionName.Contains("ELECTORAL"))
                        {
                            foo.RSTime = numericUpDown3;
                            foo.OptionType = "DELEGATE_TRACKER";
                        }
                        else if (foo.OptionName.Contains("NET GAIN"))
                        {
                            foo.RSTime = numericUpDown4;
                            foo.OptionType = "NETGAIN";
                        }
                        else if(foo.OptionName.Contains("POLL CLOSING"))
                        {
                            foo.RSTime = null;
                            foo.OptionType = "DOORS_CLOSE";
                        }

                        RightSideList.Add(foo);
                    }

                    foreach (JObject b in engineList)
                    {
                        if (b.SelectToken("id").ToString() == config.AppSettings.Settings["ApplicationID"].Value)
                        {
                            AppInfo = new ApplicationInfo()
                            {
                                ApplicationName = b.SelectToken("name").ToString(),
                                ConnectionString1 = b.SelectToken("connString1").ToString(),
                                ConnectionString2 = b.SelectToken("connString2").ToString(),
                                ConnectionString3 = b.SelectToken("connString3").ToString(),
                                ApplicationNetwork = b.SelectToken("network").ToString(),
                                EngineIP = b.SelectToken("engineIP").ToString(),
                                EnginePort = b.SelectToken("enginePort").ToString(),
                                EngineIP2 = b.SelectToken("engineIP2").ToString(),
                                EnginePort2 = b.SelectToken("enginePort2").ToString(),
                                commPort = b.SelectToken("commPort").ToString(),
                                ApplicationListeningPort = b.SelectToken("listeningPort").ToString(),
                                controllerIP = b.SelectToken("controllerIP").ToString()
                            };

                            break;
                        }
                    }

                    var host = Dns.GetHostEntry(Dns.GetHostName());

                    string ipAddr = "";

                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddr = ip.ToString();
                            break;
                        }
                    }

                    server = new TcpListener(IPAddress.Parse(ipAddr), 11000);
                    server.Start();

                    var trioCommand = new BackgroundWorker();
                    trioCommand.DoWork += TrioCommand_DoWork;
                    trioCommand.RunWorkerAsync();

                    NetworkID = AppInfo.ApplicationNetwork == "FNC" ? 0 : 1;

                    if (NetworkID != 1)
                    {
                        FoxPlus = new TcpClient(AppInfo.controllerIP, 11000);
                        FoxPlusStream = FoxPlus.GetStream();
                    }
                }
                catch(Exception ex)
                {
                    ClockTimer.Enabled = false;

                    MessageBox.Show("Error reading/finding Election_Config.json!!");

                    log.Debug("\n\nINIT - Application Load Error!!\n\n" + ex + "\n");
                    log.Error("\n\nINIT - Application Load Error!!\n\n" + ex + "\n");

                    Process.GetCurrentProcess().Kill();
                }

                GetGovStates = StoredProcedures.GetRacesByOfficeCode("G");
                GetSenateStates = StoredProcedures.GetRacesByOfficeCode("S");
                GetHouseStates = StoredProcedures.GetHouseStates();
                houseInitList = StoredProcedures.GetHouseDistricts();
                CollegeVotes = StoredProcedures.GetECVotes();
                
                flag = StoredProcedures.CheckFlags();
                
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;
                
                this.Text = $"{AppName} V{version} {AppInfo.ApplicationName} Host:{HostIPNameFunctions.GetLocalIPAddress()} {AppInfo.ConnectionString1.Split(';')[0]}";
                this.Height = 555;

                VizControl = new _2020_Primaries_Ticker.ClientSocket(IPAddress.Parse(AppInfo.EngineIP), Convert.ToInt32(AppInfo.commPort), false);
                VizControl.Connect();

                log.Debug($"{this.Text} Started");
                log.Error($"{this.Text} Started");

                hostIP = HostIPNameFunctions.GetLocalIPAddress();

                StoredProcedures.Post2ApplicationLog(this.Name, this.Text, HostIPNameFunctions.GetHostName(hostIP), hostIP, true, AppInfo.EngineIP, false, "N/A", "Launched application", config.AppSettings.Settings["AppVersion"].Value, Convert.ToInt32(config.AppSettings.Settings["ApplicationID"].Value), "N/A", DateTime.Now);
                loadedScenePath.Text = "SCENE PATH: " + flag.sceneName;
                loadedScenePath.BackColor = Color.Green;
                isVizConnected = VizCommands.SendVizMessage("", flag.sceneName, "load", 0);                        
                //isVizConnected = VizCommands.SendVizMessage("", config.AppSettings.Settings["SidePanelPath"].Value, "load", 1);

                WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                RaceDataWheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["RaceDataDwell"].Value);

                foreach (ToolStripMenuItem x in raceDwellTimesecondsToolStripMenuItem.DropDownItems)
                {
                    if (Convert.ToString(Convert.ToInt32(x.Text) * 1000) == config.AppSettings.Settings["RaceDataDwell"].Value)
                    {
                        x.Checked = true;
                    }
                }

                foreach (ToolStripMenuItem x in preDataWheelDwellTimesecondsToolStripMenuItem.DropDownItems)
                {
                    if (Convert.ToString(Convert.ToInt32(x.Text) * 1000) == config.AppSettings.Settings["PreDataDwell"].Value)
                    {
                        x.Checked = true;
                    }
                }

                if(Convert.ToBoolean(config.AppSettings.Settings["DataMode"].Value))
                {
                    PreDataTicker.Checked = false;
                    DataTicker.Checked = true;
                }
                else
                {
                    PreDataTicker.Checked = true;
                    DataTicker.Checked = false;
                }

                if (config.AppSettings.Settings["OneLineCands"].Value == "3")
                {
                    toolStripMenuItem20.Checked = false;
                    toolStripMenuItem21.Checked = true;
                }
                else
                {
                    toolStripMenuItem20.Checked = true;
                    toolStripMenuItem21.Checked = false;
                }

                CheckRaceMode();
                CheckDisplay(config.AppSettings.Settings["OneLineCands"].Value);

                Init();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nINIT - Unable to connect to Viz Engine!!\n\n" + ex + "\n");
                log.Error("\n\nINIT - Unable to connect to Viz Engine!!\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }
        
        public static void VizControlSendCommand2(string outbuf, int port)

        {
            try
            {
                // Send the data; terminiate with CRLF
                VizControl.Send(outbuf);
                string s = ">> to cPort 6100: " + outbuf;
            }
            catch (Exception ex)
            {
                //a.LogError("ERROR SENDING MESSAGE TO VIZ ENGINE\n\n\n" + ex.ToString());
            }
        }

        public void Init()
        {
            try
            {
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey4"].Value, AppInfo.ApplicationNetwork, "sendData", 0);
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "NULL", "sendData", 0);
                VizCommands.SendVizMessage("BUG_DATA_MODES", "NULL", "sendData", 0);
                if (AppInfo.ApplicationNetwork == "FBN")
                {
                    button6.Visible = true;
                }
                else
                {
                    button6.Visible = false;
                }

                CandidateOptionChecker("ShowAll");
                TickerTypeSelectorCheck(TickerType);

                PromoCounter = 0;
                FactCounter = 0;
                ActiveStateCounter = 0;


                leftSideCounter = 0;
                rightSideCounter = 0;

                ActiveRaces.Clear();
                PreDataStates.Clear();
                PreDataStatesFull.Clear();

                PreDataSchedule.Clear();
                AllVaStatesData = StoredProcedures.VoterAnalysisData();
                

                foreach (VoterAnalysisData a in AllVaStatesData)
                {
                    if (!QIds.Contains(a.VA_Data_Id))
                    {
                        QIds.Enqueue(a.VA_Data_Id);
                    }
                }

                CheckFlags();
                GetRaces();
            }
            catch (Exception ex)
            {
                log.Debug("\n\nINIT - Error initializing ticker!!\n\n" + ex + "\n");
                log.Error("\n\nINIT - Error initializing ticker!!\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        private void TrioCommand_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                while (true)
                {
                    client = server.AcceptTcpClient();

                    System.Threading.ThreadPool.QueueUserWorkItem(ThreadProc, client);
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex);
                log.Error(ex);
            }
        }

        public void ThreadProc(object obj)
        {
            Byte[] bytes = new byte[8192];
            string data = "";

            try
            {
                data = null;
                stream = client.GetStream();

                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = Encoding.ASCII.GetString(bytes, 0, i).Replace((char)3, (char)32).Replace((char)4, (char)32).Trim();
                    Console.WriteLine(data);
                    string[] dataSplit = data.Split('|');

                    if(dataSplit.Length == 2)
                    {
                        if (this.IsHandleCreated)
                        {
                            Invoke(new Action(() => label15.Text = "ACTIVE CR: " + dataSplit[1]));
                        }
                        else
                        {
                            label15.Text = "ACTIVE CR: " + dataSplit[1];
                        }

                        activeCR = dataSplit[1];
                    }

                    string str = "";

                    if(dataSplit[0].Contains("TRIO"))
                    {
                        dataSplit[0] = dataSplit[0].Replace("TRIO", "");

                        Console.WriteLine($"{activeCR}|{dataSplit[0]}|{dataSplit.Length}");
                    }

                    if (trioString != data && dataSplit.Length > 2 && (activeCR == dataSplit[0] || dataSplit[0].Contains("DEV")))
                    {
                        if (dataSplit[1] == "0")
                        {
                            str = "BUG OUT CALLED";
                            bugKey = "NULL";

                            Invoke(new Action(() => e_bug.Enabled = false));
                            Invoke(new Action(() => e_bug.Stop()));
                        }
                        else
                        {
                            Invoke(new Action(() => e_bug.Enabled = false));
                            Invoke(new Action(() => e_bug.Stop()));

                            if (dataSplit[2] == "0")
                            {
                                str = "EV CALLED";
                                bugKey = "ELECTORAL_VOTES";
                                e_bug_mode = 1;
                                //Console.WriteLine(StoredProcedures.getTotalEV("2022-11-09 01:00:00.000"));

                            }
                            else if (dataSplit[2] == "1")
                            {
                                if (dataSplit[3] == "0")
                                {
                                    str = "HOUSE BOP CALLED";
                                    bugKey = "BALANCE_OF_POWER^HOUSE";
                                    e_bug_mode = 3;
                                }
                                else if (dataSplit[3] == "1")
                                {
                                    str = "SENATE BOP CALLED";
                                    bugKey = "BALANCE_OF_POWER^SENATE";
                                    e_bug_mode = 2;
                                }

                                //Console.WriteLine(StoredProcedures.BopString("2022-11-09 01:00:00.000"));
                            }
                            else if (dataSplit[2] == "2")
                            {
                                if (dataSplit[3] == "0")
                                {
                                    str = "HOUSE NET GAIN CALLED";
                                    bugKey = "NETGAIN^HOUSE";
                                    e_bug_mode = 5;
                                }
                                else if (dataSplit[3] == "1")
                                {
                                    str = "SENATE NET GAIN CALLED";
                                    bugKey = "NETGAIN^SENATE";
                                    e_bug_mode = 4;
                                }

                                //Console.WriteLine(StoredProcedures.BopString("2022-11-09 01:00:00.000"));
                            }

                            //Call bug in code after setting the data
                            Invoke(new Action(() => e_bug.Enabled = true));
                            Invoke(new Action(() => e_bug.Start()));
                        }

                        SendBugData(bugKey, e_bug_mode);

                        Invoke(new Action(() => TrioBox.Items.Add($"{DateTime.Now} - {str}")));
                        Invoke(new Action(() => TrioBox.SelectedIndex = TrioBox.Items.Count - 1));

                        trioString = data;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        
        }
        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine(SimTime);
                //ClockLabel.Text = UseSimTime ? StoredProcedures.GetSimulatedTime().ToString() : DateTime.Now.ToString();
                ClockLabel.Text = UseSimTime ? SimTime.ToString() : DateTime.Now.ToString();
                VizConnection.BackColor = isVizConnected ? Color.Green : Color.Red;

                ErrorLabel.BackColor = ErrorFlag ? Color.Red : Color.Green;
                ErrorLabel.Text = ErrorFlag ? "ERROR - CHECK LOGS!!" : "NO ERRORS";
               
                foreach (RightSideOptions a in RightSideList)
                {

                    if (a.RSTime != null)
                    {
                        a.OptionTime = Convert.ToInt32(a.RSTime.Value);
                    }

                    a.OptionEnabled = a.RSCb.Checked;
                }

                //if (RSPollClose.Checked && TickerType == 2 && StartTicker && CloseTimes2.Count != 0)
                if (TickerType == 3 && StartTicker && CloseTimes2.Count != 0 && flag.usePollCloseBug)
                {
                    DateTime NextPollTime = CloseTimes2.Peek();

                    int delta_start = flag.bugInsertPollTime;
                    int delta_end = flag.bugRemovePollTime;

                    DateTime startTime = NextPollTime.AddMinutes(-delta_start);
                    DateTime endTime = startTime.AddMinutes(delta_end);

                    bool inLimit = SimTime >= startTime && SimTime < endTime ? true : false;

                    if (inLimit)
                    {
                        if (pollCounter == 0)
                        {
                            e_bug.Enabled = false;
                            e_bug.Stop();

                            pollBugIn = true;

                            VizCommands.SendVizMessage("BUG_DATA_MODES", $"DOORS_CLOSE^{NextPollTime.ToString("HH:mm")}", "sendData", 0);

                            pollCounter++;
                        }
                    }
                    else
                    {
                        if (pollBugIn)
                        {
                            CloseTimes2.Dequeue();

                            if (e_bug_mode == 0)
                            {
                                Console.WriteLine("remove bug");
                                VizCommands.SendVizMessage("BUG_DATA_MODES", "NULL", "sendData", 0);
                            }
                            else
                            {
                                Console.WriteLine("start bug timer");
                                SendBugData(bugKey, e_bug_mode);
                                if (!e_bug.Enabled)
                                {
                                    e_bug.Enabled = true;
                                    e_bug.Start();
                                }
                                
                            }

                            pollBugIn = false;
                            pollCounter = 0;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nCLOCK TIMER ISSUE\n\n" + ex + "\n");
                log.Error("\n\nCLOCK TIMER ISSUE\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void CandidateOptions(object sender, EventArgs e)
        {
            CandidateOptionChecker(((RadioButton)sender).Name);
        }

        public void CandidateOptionChecker(string name)
        {
            if (name == "ShowTop3")
            {
                CanScrollCount = 3;
            }
            else if (name == "ShowTop6")
            {
                CanScrollCount = 6;
            }
            else
            {
                CanScrollCount = 0;
            }

            OneLineRaceCounter = 0;
        }

        public void DataCoverUp(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                if (((CheckBox)sender).Name.ToLower().Contains("left"))
                {
                    Console.WriteLine("left side coverup");
                    LeftCup = true;
                }
                else if (((CheckBox)sender).Name.ToLower().Contains("right"))
                {
                    Console.WriteLine("right side coverup");
                    RightCup = true;
                }
            }
        }

        public void RaceDataDwell(object sender, EventArgs e)
        {
            try
            {
                string SettingName = "";
                string ActiveMenu = ((ToolStripMenuItem)sender).OwnerItem.Name;

                ToolStripMenuItem DropDown;
                Timer WheelTime;

                if (ActiveMenu.Contains("raceDwell"))
                {
                    DropDown = raceDwellTimesecondsToolStripMenuItem;
                    SettingName = "RaceDataDwell";
                    WheelTime = RaceDataWheelTimer;
                }
                else
                {
                    DropDown = preDataWheelDwellTimesecondsToolStripMenuItem;
                    SettingName = "PreDataDwell";
                    WheelTime = RaceDataWheelTimer;
                }

                ((ToolStripMenuItem)sender).Checked = true;

                foreach (ToolStripMenuItem a in DropDown.DropDownItems)
                {
                    if (((ToolStripMenuItem)sender).Name != a.Name)
                    {
                        a.Checked = false;
                    }
                }

                config.AppSettings.Settings[SettingName].Value = Convert.ToString(Convert.ToInt32(((ToolStripMenuItem)sender).Text) * 1000);
                WheelTime.Interval = Convert.ToInt32(((ToolStripMenuItem)sender).Text) * 1000;
                config.Save(ConfigurationSaveMode.Full, true);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch(Exception ex)
            {

                log.Debug("\n\nRaceDataDwell Error\n\n" + ex + "\n");
                log.Error("\n\nRaceDataDwell Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        private void WheelTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PreDataTicker.Checked)
                {
                    PreDataWheeler();
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nWheelTimer error\n\n" + ex + "\n");
                log.Error("\n\nWheelTimer error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void PreDataWheeler()
        {
            try
            {
                if (PreDataSchedule.Count == 0)
                {
                    Checker();
                    ActiveStateCounter++;
                }

                if (ActiveStateCounter > PreDataStates.Count - 1)
                {
                    ActiveStateCounter = 0;
                }

                WheelTimer.Stop();
                
                switch (CurrentPreDataOption)
                {
                    case "PromoCB":
                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                        GetPromos();
                        break;
                    case "PollClosingCB":
                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                        GetClosingTime("");
                        break;
                    case "BopCB":
                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                        //GetDelegates();
                        GetBopImage();
                        break;
                    case "RFcb":
                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                        //GetDelegates();
                        GetFactsData();
                        break;
                    case "R2Wcb":
                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                        //GetDelegates();
                        GetR2Wdata();
                        break;
                    case "CensusCB":
                        //GetRaceFacts();
                        
                        GetCensusImage();
                        break;
                    case "VAcb":
                        GetStateVoterAnalysisData();
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nPreDataWheel Error\n\n" + ex + "\n");
                log.Error("\n\nPreDataWheel Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void GetBopImage()
        {
            if (BopQueue.Count == 0)
            {
                BopQueue = StoredProcedures.GetSidePanelData(2, NetworkID);
            }

            string BopString = $"IMAGE^{BopQueue.Peek().ImagePath}";

            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, BopString, "sendData", 1);
            PostToTB(BopString);


            PreDataSchedule.Dequeue();
            BopQueue.Dequeue();

            if (PreDataSchedule.Count != 0)
            {
                CurrentPreDataOption = PreDataSchedule.Peek();
            }

            WheelTimer.Start();
        }

        public void GetFactsData()
        {
            if (FactsQueue.Count == 0)
            {
                FactsQueue = StoredProcedures.GetSidePanelData(4, NetworkID);
            }

            string FactString = $"IMAGE^{FactsQueue.Peek().ImagePath}";

            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, FactString, "sendData", 1);
            PostToTB(FactString);


            PreDataSchedule.Dequeue();
            FactsQueue.Dequeue();

            if (PreDataSchedule.Count != 0)
            {
                CurrentPreDataOption = PreDataSchedule.Peek();
            }

            WheelTimer.Start();
        }

        public void GetR2Wdata()
        {
            if (R2WQueue.Count == 0)
            {
                R2WQueue = StoredProcedures.GetSidePanelData(5, NetworkID);
            }

            string R2WString = $"IMAGE^{R2WQueue.Peek().ImagePath}";

            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, R2WString, "sendData", 1);
            PostToTB(R2WString);


            PreDataSchedule.Dequeue();
            R2WQueue.Dequeue();

            if (PreDataSchedule.Count != 0)
            {
                CurrentPreDataOption = PreDataSchedule.Peek();
            }

            WheelTimer.Start();
        }


        public void GetCensusImage()
        {
            if(CensusQueue.Count == 0)
            {
                CensusQueue = StoredProcedures.GetSidePanelData(3, NetworkID);
            }

            string CensusString = $"IMAGE^{CensusQueue.Peek().ImagePath}";

            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, CensusString, "sendData", 1);
            PostToTB(CensusString);


            PreDataSchedule.Dequeue();
            CensusQueue.Dequeue();

            if (PreDataSchedule.Count != 0)
            {
                CurrentPreDataOption = PreDataSchedule.Peek();
            }

            WheelTimer.Start();
        }

        public void GetPromos()
        {
            try
            {
                string PromoString = "";

                //var a = StoredProcedures.Promos(TickerType, SimTime);

                if(PromoQueue.Count == 0)
                {
                    if (sidePanelToolStripMenuItem.Checked)
                    {
                        PromoQueue = StoredProcedures.GetSidePanelData(1, NetworkID);
                    }
                    else
                    {
                        PromoQueue = StoredProcedures.GetSidePanelData(99, NetworkID);
                    }
                }

                if(PromoQueue.Count > 0)
                {
                    /*
                    if (PromoCounter > a.Count - 1)
                    {
                        PromoCounter = 0;
                    }
                    */

                    int SType = 0;

                    if (TickerType == 1 || TickerType == 0)
                    {
                        if (sidePanelToolStripMenuItem.Checked)
                        {
                            PromoString = $"IMAGE^{PromoQueue.Peek().ImagePath}";
                            SType = 1;
                        }
                        else
                        {
                            PromoString = $"IMAGE^^{PromoQueue.Peek().ImagePath}";
                            SType = 0;
                        }
                    }
                    else if (TickerType == 2)
                    {
                        //isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE_BUCKET", "sendData", SType);
                        //PromoString = $"EMERGENCY|IMAGE^^{a[PromoCounter].PromoImage}";
                    }

                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, PromoString, "sendData", SType);
                    PostToTB(PromoString);

                    PromoCounter++;

                    PromoQueue.Dequeue();

                    if (sidePanelToolStripMenuItem.Checked)
                    {
                        PreDataSchedule.Dequeue();

                        if (PreDataSchedule.Count != 0)
                        {
                            CurrentPreDataOption = PreDataSchedule.Peek();
                        }

                        WheelTimer.Start();
                    }
                }
                else
                {
                    PostToTB("NO PROMOS!!!");

                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    PreDataWheeler();
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nGetPromos Error\n\n" + ex + "\n");
                log.Error("\n\nGetPromos Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void GetClosingTime(string DataText)
        {
            try
            {
                string ClosingTimeString = "";

                if (AreRaces)
                {
                    if (CloseTimes.Count == 0)
                    {
                        CloseTimes = StoredProcedures.PollClosingsQueue(true, SimTime);
                    }
                    int Stype = 0;
                    if (TickerType == 1 || TickerType == 0)
                    {
                        ClosingTimeString = $"DOORS_CLOSE^{CloseTimes.Peek().ToString("HH:mm")}";
                        CloseTimes.Dequeue();
                        Stype = 1;
                    }
                    else if(TickerType == 2)
                    {
                        Stype = 0;
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE_BUCKET", "sendData", Stype);
                        if (!IsDataMode)
                        {
                            //ClosingTimeString = $"EMERGENCY|DOORS_CLOSE^{PreDataStatesFull[ActiveStateCounter]}^{a[0].ClosingTime}";
                        }
                        else
                        {
                            //ClosingTimeString = $"DOORS_CLOSE^{PreDataStatesFull[ActiveStateCounter]}^{a[0].ClosingTime}|DATA^{DataText}";
                        }
                    }

                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, ClosingTimeString, "sendData", Stype);
                    PostToTB(ClosingTimeString);

                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    WheelTimer.Start();
                }
                else
                {
                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    PreDataWheeler();
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nGetClosingTime Error\n\n" + ex + "\n");
                log.Error("\n\nGetClosingTime Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void GetDelegates()
        {
            try
            {
                string DelegateString = "";

                if (AreRaces)
                {
                    var a = StoredProcedures.GetDelegates(PreDataStates[ActiveStateCounter], "Dem");
                    if (TickerType == 1)
                    {
                        DelegateString = $"DELEGATES^{PreDataStatesFull[ActiveStateCounter]}^{a[0].DelegatesAtStake}";
                    }
                    else if (TickerType == 2)
                    {
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE_BUCKET", "sendData", 0);
                        DelegateString = $"EMERGENCY|DELEGATES^{PreDataStatesFull[ActiveStateCounter]}^{a[0].DelegatesAtStake}";
                    }

                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, DelegateString, "sendData", 0);
                    PostToTB(DelegateString);

                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    WheelTimer.Start();
                }
                else
                {
                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    PreDataWheeler();
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nGetDelegates Error\n\n" + ex + "\n");
                log.Error("\n\nGetDelegates Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void GetDelegateCounts()
        {
            try
            {
                string DelegateCountString = "";

                if (AreRaces)
                {
                    var a = StoredProcedures.GetDelegateCounts("Dem");

                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE_BUCKET", "sendData", 0);
                    DelegateCountString = $"EMERGENCY|{config.AppSettings.Settings["shmKey5"].Value}^{a[0].CandidateLastName}^{a[0].CandidateCount}^{a[1].CandidateLastName}^{a[1].CandidateCount}";

                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, DelegateCountString, "sendData", 0);
                    PostToTB(DelegateCountString);

                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    WheelTimer.Start();
                }
                else
                {
                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    PreDataWheeler();
                }
            }
            catch (Exception ex)
            {
                log.Debug("\n\nGetDelegatesCount Error\n\n" + ex + "\n");
                log.Error("\n\nGetDelegatesCount Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void GetRaces()
        {
            try
            {
                ActiveRaces.Clear();
                PreDataStates.Clear();

                UseSimTime = flag.UseSimTime;
                SimTime = UseSimTime ? StoredProcedures.GetSimulatedTime() : DateTime.Now;

                ActiveRaces = StoredProcedures.RaceList("", SimTime, false);

                //LeftSideRace = StoredProcedures.RaceList("left", SimTime, true);
                //RightSideRace = StoredProcedures.RaceList("right", SimTime, true);
                SingleSideRace = StoredProcedures.RaceList("single", SimTime, true);
                label11.Text = "TOTAL RACES: " + ActiveRaces.Count + " RACES WITH DATA: " + SingleSideRace.Count;
                if (ActiveRaces.Count == 0)
                {
                    PostToTB("There are no races today!");

                    AreRaces = false;
                }
                else
                {
                    foreach (Races b in ActiveRaces)
                    {
                        if (!PreDataStates.Contains(b.StateAbbv))
                        {
                            PreDataStates.Add(b.StateAbbv);

                            PreDataStatesFull.Add(b.StateName);
                        }
                    }

                    AreRaces = true;
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nGetRaces Error\n\n" + ex + "\n");
                log.Error("\n\nGetRaces Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void GetRaceFacts()
        {
            try
            {
                string StateFactString = "";

                if (AreRaces)
                {
                    var a = StoredProcedures.GetRaceFacts(PreDataStates[ActiveStateCounter], TickerType);

                    if (a.Count != 0)
                    {
                        if (FactCounter == a.Count)
                        {
                            FactCounter = 0;
                        }

                        if (TickerType == 1)
                        {
                            StateFactString = $"FACTS^{PreDataStatesFull[ActiveStateCounter]}^{a[FactCounter].Fact.ToUpper()}";
                        }
                        else if (TickerType == 2)
                        {
                            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE_BUCKET", "sendData", 0);

                            if (a[FactCounter].Fact.Contains("~"))
                            {
                                a[FactCounter].Fact = a[FactCounter].Fact.Replace("~", Environment.NewLine);
                            }

                            StateFactString = $"EMERGENCY|FACTS^{PreDataStatesFull[ActiveStateCounter]}^{a[FactCounter].Fact.ToUpper()}";
                        }

                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, StateFactString, "sendData", 0);
                        PostToTB(StateFactString);

                        FactCounter++;
                        FactCounter2++;

                        WheelTimer.Start();

                        if (FactCounter2 == Convert.ToInt32(config.AppSettings.Settings["Facts2Show"].Value) && TickerType == 1)
                        {
                            if (FactCounter == a.Count)
                            {
                                FactCounter = 0;
                            }

                            FactCounter2 = 0;

                            PreDataSchedule.Dequeue();

                            if (PreDataSchedule.Count != 0)
                            {
                                WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                                CurrentPreDataOption = PreDataSchedule.Peek();
                            }
                        }
                        else if(FactCounter2 == a.Count && TickerType == 2)
                        {
                            if (FactCounter == a.Count)
                            {
                                FactCounter = 0;
                            }

                            FactCounter2 = 0;

                            PreDataSchedule.Dequeue();

                            if (PreDataSchedule.Count != 0)
                            {
                                WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                                CurrentPreDataOption = PreDataSchedule.Peek();
                            }
                        }

                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                    }
                    else
                    {
                        PostToTB($"NO FACTS FOR: {PreDataStatesFull[ActiveStateCounter]}");
                        FactCounter = 0;
                        FactCounter2 = 0;

                        PreDataSchedule.Dequeue();

                        if (PreDataSchedule.Count != 0)
                        {
                            CurrentPreDataOption = PreDataSchedule.Peek();
                        }

                        PreDataWheeler();
                    }
                }
                else
                {
                    FactCounter = 0;
                    FactCounter2 = 0;

                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    PreDataWheeler();
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nGetRaceFacts Error\n\n" + ex + "\n");
                log.Error("\n\nGetRaceFacts Error\n\n" + ex + "\n");

                ErrorFlag = true;
                CurrentPreDataOption = "";
                PreDataSchedule.Dequeue();
            }
        }

        public void GetStateVoterAnalysisData()
        {
            try
            {
                List<VoterAnalysisData> CurrentVaStateData = new List<VoterAnalysisData>();

                if(QIds.Count == 0)
                {
                    AllVaStatesData = StoredProcedures.VoterAnalysisData();
                    foreach (VoterAnalysisData a in AllVaStatesData)
                    {
                        if (!QIds.Contains(a.VA_Data_Id))
                        {
                            QIds.Enqueue(a.VA_Data_Id);
                        }
                    }
                }

                string CurrentID = QIds.Peek();

                if (AllVaStatesData.Count != 0 && AreRaces)
                {
                    foreach (VoterAnalysisData c in AllVaStatesData)
                    {
                        if (c.VA_Title.Contains("~"))
                        {
                            c.VA_Title = c.VA_Title.Replace("~", Environment.NewLine);
                        }

                        if (c.Response.Contains("~"))
                        {
                            c.Response = c.Response.Replace("~", Environment.NewLine);
                        }
                        
                        if (c.VA_Data_Id == CurrentID)
                        {
                            CurrentVaStateData.Add(c);
                        }
                    }

                    if (CurrentVaStateData.Count != 0)
                    {
                        string responseString = "";
                        string percentString = "";

                        foreach(VoterAnalysisData x in CurrentVaStateData)
                        {
                            responseString += x.Response + "~";
                            percentString += x.VA_Percent + "~";
                        }

                        string voteString = $"VOTER_ANALYSIS^{CurrentVaStateData.Count}^{CurrentVaStateData[0].VA_Title}^{CurrentVaStateData[0].VA_Title}^{CurrentVaStateData[0].StateName}^{responseString.TrimEnd('~')}^{percentString.TrimEnd('~')}";



                        //isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE", "sendData");
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, voteString.ToUpper(), "sendData", 1);
                        PostToTB(voteString);

                        //AnalysisCounter++;
                        WheelTimer.Start();

                        PreDataSchedule.Dequeue();
                        QIds.Dequeue();

                        if (PreDataSchedule.Count != 0)
                        {
                            WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                            CurrentPreDataOption = PreDataSchedule.Peek();
                        }

                        WheelTimer.Interval = Convert.ToInt32(config.AppSettings.Settings["PreDataDwell"].Value);
                    }
                    else
                    {
                        PostToTB("NO VOTER ANALYSIS DATA FOR " + PreDataStatesFull[ActiveStateCounter] + "!!");
                        AnalysisCounter = 0;

                        //PreDataSchedule.Dequeue();

                        if (PreDataSchedule.Count != 0)
                        {
                            //CurrentPreDataOption = PreDataSchedule.Peek();
                        }
                        ActiveStateCounter++;
                        PreDataWheeler();
                    }
                }
                else
                {
                    AnalysisCounter = 0;

                    PreDataSchedule.Dequeue();

                    if (PreDataSchedule.Count != 0)
                    {
                        CurrentPreDataOption = PreDataSchedule.Peek();
                    }

                    PreDataWheeler();
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nGetVoterAnalysis Error\n\n" + ex + "\n");
                log.Error("\n\nGetVoterAnalysis Error\n\n" + ex + "\n");

                ErrorFlag = true;
                CurrentPreDataOption = "";
                PreDataSchedule.Dequeue();
            }
        }

        private void StartStopTicker_Click(object sender, EventArgs e)
        {
            try
            {
                CloseTimes = StoredProcedures.PollClosingsQueue(true, SimTime);
                CloseTimes2 = StoredProcedures.PollClosingsQueue(false, SimTime);

                if (PreDataTicker.Checked || DataTicker.Checked)
                {
                    if (StartStopTicker.BackColor == Color.Green)
                    {
                        Init();
                        
                        StartStopTicker.Text = "STOP TICKER";
                        StartStopTicker.BackColor = Color.Red;
                        StartTicker = true;
                        if (PreDataTicker.Checked)
                        {
                            if(sidePanelToolStripMenuItem.Checked)
                            {
                                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, KeyValue, "sendData", 1);
                                Checker();
                                PreDataWheeler();
                                WheelTimer.Enabled = true;
                                PromoTimer2.Enabled = false;
                                PromoTimer2.Stop();
                            }
                            else
                            {
                                PromoTimer2.Enabled = true;
                                PromoTimer2.Start();
                            }

                            RaceDataWheelTimer.Enabled = false;
                        }
                        else
                        {
                            if (NetworkID != 1)
                            {
                                if (OneLineTicker.Checked)
                                {
                                    Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP1" + (char)4);
                                    FoxPlusStream.Write(data, 0, data.Length);
                                }
                                else
                                {
                                    Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP0" + (char)4);
                                    FoxPlusStream.Write(data, 0, data.Length);
                                }
                            }

                            System.Threading.Thread.Sleep(2000);
                            PromoTimer2.Enabled = false;
                            PromoTimer2.Stop();
                            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, KeyValue, "sendData", 0);
                            SendRaceDataToViz();
                            WheelTimer.Enabled = false;
                            RaceDataWheelTimer.Enabled = true;
                        }

                        tabControl1.SelectedTab = tabPage2;
                    }
                    else
                    {
                        StartStopTicker.Text = "START TICKER";
                        StartStopTicker.BackColor = Color.Green;
                        StartTicker = false;
                        WheelTimer.Enabled = false;
                        RaceDataWheelTimer.Enabled = false;
                        PromoTimer2.Stop();

                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "NULL", "sendData", 0);

                        Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP0" + (char)4);
                        FoxPlusStream.Write(data, 0, data.Length);
                    }
                }
                else
                {
                    StartTicker = false;
                    MessageBox.Show("Please select a ticker mode!!");
                }
            }
            catch(Exception ex)
            {
                log.Debug("\n\nStartStop Error\n\n" + ex + "\n");
                log.Error("\n\nStartStop Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void Checker()
        {
            try
            {
                var CheckBoxes = PreOptionsBox.Controls.OfType<CheckBox>();

                foreach (CheckBox rb in CheckBoxes)
                {
                    bool state = rb.Checked;
                    string name = rb.Name;
                    bool isEnabled = rb.Enabled;

                    if (state && isEnabled)
                    {
                        PreDataSchedule.Enqueue(name);
                    }
                }

                CurrentPreDataOption = PreDataSchedule.Peek();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nChecker Error\n\n" + ex + "\n");
                log.Error("\n\nChecker Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        private void TickerTypeSelector(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Name.ToLower().Contains("one"))
            {
                PromoQueue.Clear();
                TickerTypeSelectorCheck(1);
                //isVizConnected = VizCommands.SendVizMessage("", "", "unload", 1);
            }
            else if (((ToolStripMenuItem)sender).Name.ToLower().Contains("two") && ((ToolStripMenuItem)sender).Name.ToLower().Contains("full"))
            {
                TickerTypeSelectorCheck(3);
                //isVizConnected = VizCommands.SendVizMessage("", "", "unload", 1);
                //DisplayCandidates = 1;
            }
            else if (((ToolStripMenuItem)sender).Name.ToLower().Contains("two"))
            {
                TickerTypeSelectorCheck(2);
                //isVizConnected = VizCommands.SendVizMessage("", "", "unload", 1);
                DisplayCandidates = 1;
            }
            else
            {
                PromoQueue.Clear();
                TickerTypeSelectorCheck(0);
                isVizConnected = VizCommands.SendVizMessage("", config.AppSettings.Settings["SidePanelPath"].Value, "load", 1);
            }
        }

        private void TickerCandDisplaySelector(object sender, EventArgs e)
        {
            CheckDisplay(((ToolStripMenuItem)sender).Text);

            config.AppSettings.Settings["OneLineCands"].Value = Convert.ToString(((ToolStripMenuItem)sender).Text);
            config.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void CheckDisplay(string cmd)
        {
            if (!TwoLineTicker.Checked)
            {
                if (cmd == "2")
                {
                    toolStripMenuItem20.Checked = true;
                    toolStripMenuItem21.Checked = false;
                    //DisplayCandidates = 1;
                }
                else
                {
                    toolStripMenuItem20.Checked = false;
                    toolStripMenuItem21.Checked = true;
                    //DisplayCandidates = 2;
                }
            }
        }

        public void TickerTypeSelectorCheck(int name)
        {
            try
            {
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "NULL", "sendData", 0);

                int SType = 0;
                if (name == 1)
                {
                    OneLineTicker.Checked = true;
                    TwoLineTicker.Checked = false;
                    twoFull.Checked = false;
                    sidePanelToolStripMenuItem.Checked = false;
                    VAcb.Enabled = true;
                    ShowTop3.Text = "SHOW TOP 3";
                    SType = 0;
                    TickerType = 1;
                    KeyValue = "ONE_LINE";
                    //KeyValue = "TWO_LINE";
                }
                else if(name == 2)
                {
                    OneLineTicker.Checked = false;
                    TwoLineTicker.Checked = true;
                    twoFull.Checked = false;
                    sidePanelToolStripMenuItem.Checked = false;
                    VAcb.Enabled = true;
                    ShowTop3.Text = "SHOW TOP 4";
                    SType = 0;
                    TickerType = 2;
                    //TickerType = 1;
                    KeyValue = "TWO_LINE_BUCKET";
                    //KeyValue = "TWO_LINE";
                }
                else if(name == 3)
                {
                    OneLineTicker.Checked = false;
                    TwoLineTicker.Checked = false;
                    twoFull.Checked = true;
                    sidePanelToolStripMenuItem.Checked = false;
                    VAcb.Enabled = true;
                    ShowTop3.Text = "SHOW TOP 3";
                    SType = 0;
                    TickerType = 3;
                    //KeyValue = "ONE_LINE";
                    KeyValue = "TWO_LINE";
                }
                else
                {
                    OneLineTicker.Checked = false;
                    TwoLineTicker.Checked = false;
                    twoFull.Checked = false;
                    sidePanelToolStripMenuItem.Checked = true;
                    TickerType = 0;
                    KeyValue = "NULL";
                    SType = 1;
                }

                if (KeyValue != "NULL" && NetworkID != 1)
                {
                    Byte[] data = Encoding.ASCII.GetBytes(KeyValue);
                    FoxPlusStream.Write(data, 0, data.Length);
                }

                if (StartTicker)
                {
                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, KeyValue, "sendData", SType);
                }
            }
            catch (Exception ex)
            {
                log.Debug("\n\nTickerTypeSelector Error\n\n" + ex + "\n");
                log.Error("\n\nTickerTypeSelector Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        private void RaceDataWheelTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DataFetchTime.Text = DateTime.Now.ToString();
                SendRaceDataToViz();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nRaceDataWheelTimer\n\n" + ex + "\n");
                log.Error("\n\nRaceDataWheelTimer\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        public void SendRaceDataToViz()
        {
            ActiveRaces.Clear();
            ActiveRaces = StoredProcedures.RaceList("", SimTime, false);

            LeftSideRace = StoredProcedures.RaceList("left", SimTime, true).OrderBy(f => f.StateAbbv).ThenBy(f => f.RaceSortOrder).ThenBy(f => f.RaceDistrict).ToList();
            RightSideRace = StoredProcedures.RaceList("right", SimTime, true).OrderBy(f => f.StateAbbv).ThenBy(f => f.RaceSortOrder).ThenBy(f => f.RaceDistrict).ToList();
            //SingleSideRace = StoredProcedures.RaceList("single", SimTime, true).OrderBy(f => f.StateAbbv).ThenBy(f => f.RaceSortOrder).ThenBy(f => f.RaceDistrict).ToList();
            SingleSideRace = StoredProcedures.RaceList("single", SimTime, true);
            label11.Text = "TOTAL RACES: " + ActiveRaces.Count + " RACES WITH DATA: " + SingleSideRace.Count;
            if (1 == 2)
            {
                try
                {
                    bool isDone = false;

                    if (RaceCounter > ActiveRaces.Count - 1)
                    {
                        RaceCounter = 0;
                    }

                    List<RaceData> candidateRaceInfo = new List<RaceData>();
                    
                    if (AreRaces && ActiveRaces.Count != 0)
                    {
                        candidateRaceInfo = StoredProcedures.GetRaceData(ActiveRaces[RaceCounter].StateID, ActiveRaces[RaceCounter].RaceOfficeCode, ActiveRaces[RaceCounter].RaceDistrict, ActiveRaces[RaceCounter].RaceElectionType, "Dem");
                    }

                    if (AreRaces && candidateRaceInfo.Count > 0)
                    {
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "DATA", "sendData", 0);

                        RaceDataWheelTimer.Stop();

                        if (ShowAll.Checked)
                        {
                            CanScrollCount = candidateRaceInfo.Count - 1;
                        }

                        DateTime pollClosingTime;
                        DateTime CurrentDateTime = UseSimTime ? SimTime : DateTime.Now;

                        bool lockout = true;
                        bool HasData = false;

                        double candidatePercent = 0.0;
                        double dateDiff = 0.0;

                        string isIncumbent = "";
                        string isGain = "";
                        string isWinner = "";
                        string isWinner2 = "";
                        string candidatePercentStr = "";
                        string DataForViz = "";
                        string RaceIDs = "";
                        string RaceWinnerID = "";
                        string Cands = "";
                        string WinnerInfo = "";
                        string Races = "";
                        string preInPrecentFinal = "";

                        double preInPercent = 0;
                        int WinnerIndex = 0;
                        int isWinnerCounter = 0;

                        foreach (RaceData v in candidateRaceInfo)
                        {
                            pollClosingTime = Convert.ToDateTime(ActiveRaces[RaceCounter].PollClosingDateTime);

                            dateDiff = (CurrentDateTime - pollClosingTime).TotalSeconds;

                            lockout = PollLockoutEnabled ? (lockout = (dateDiff < 0.0) ? true : false) : false;

                            if (Convert.ToBoolean(v.UseAPraceCall))
                            {
                                isWinner2 = (v.Cstat == "W" && !lockout) ? "1" : "0";
                            }
                            else
                            {
                                isWinner2 = (v.Cid == v.RaceWinnerCanID && !lockout) ? "1" : "0";
                            }

                            if (isWinner2 == "1")
                            {
                                candidatePercent = (Convert.ToDouble(v.VoteSum) != 0.0) ? ((Convert.ToDouble(v.Cvote) / Convert.ToDouble(v.VoteSum)) * 100) : 0.0;
                                candidatePercentStr = UseTenthsPercent ? string.Format("{0:0.0}", candidatePercent) : string.Format("{0:0}", Math.Round(candidatePercent, MidpointRounding.AwayFromZero));
                                isIncumbent = (v.IsIncFlg.ToLower() == "y") ? "1" : "0";

                                if (UseExpectedVoteIn)
                                {
                                    preInPercent = Convert.ToDouble(v.PctExpVote);
                                }
                                else
                                {
                                    if (v.PctsReporting != "0")
                                    {
                                        var num1 = Convert.ToDouble(v.PctsReporting);
                                        var num2 = Convert.ToDouble(v.TotPcts);

                                        preInPercent = num1 * 100 / num2;

                                        preInPercent = preInPercent == 100.0 ? 99.0 : preInPercent;
                                    }
                                    else
                                    {
                                        preInPercent = 0;
                                    }
                                }
                                if (!v.OfficeName.Contains("House"))
                                {
                                    isGain = (v.InIncPtyFlg == "N" && isWinner == "1" && v.IgnoreGain == "1") ? "1" : "0";
                                }
                                else
                                {
                                    isGain = "0";
                                }

                                WinnerInfo = $"name={v.CandLastName};party={GetPartyCode(v.MajorPtyId)};incum={isIncumbent};vote={v.Cvote};percent={candidatePercentStr};check={isWinner2};gain={isGain};imagePath=^";
                                RaceWinnerID = v.FoxID + "^";
                                isWinnerCounter = 1;
                                break;
                            }

                            WinnerIndex++;
                        }

                        if (WinnerIndex != candidateRaceInfo.Count)
                        {
                            candidateRaceInfo.RemoveAt(WinnerIndex);
                        }

                        if (TickerType == 1)
                        {
                            for (int i = (0 + OneLineRaceCounter); i <= (2 + (OneLineRaceCounter - isWinnerCounter)); i++)
                            {
                                if (i > candidateRaceInfo.Count - 1)
                                {
                                    isDone = true;
                                    RaceIDs += "USGOV999999" + "^";
                                    Cands += $"name=;party=;incum=;vote=;percent=;check=;gain=;imagePath=^";
                                    continue;
                                }

                                RaceIDs += candidateRaceInfo[i].FoxID + "^";
                                pollClosingTime = Convert.ToDateTime(ActiveRaces[RaceCounter].PollClosingDateTime);
                                dateDiff = (CurrentDateTime - pollClosingTime).TotalSeconds;


                                lockout = PollLockoutEnabled ? (lockout = (dateDiff < 0.0) ? true : false) : false;

                                candidatePercent = (Convert.ToDouble(candidateRaceInfo[i].VoteSum) != 0.0) ? ((Convert.ToDouble(candidateRaceInfo[i].Cvote) / Convert.ToDouble(candidateRaceInfo[i].VoteSum)) * 100) : 0.0;
                                candidatePercentStr = UseTenthsPercent ? string.Format("{0:0.0}", candidatePercent) : string.Format("{0:0}", Math.Round(candidatePercent, MidpointRounding.AwayFromZero));

                                if (candidatePercent < 1.0)
                                {
                                    candidatePercentStr = "<1";
                                }

                                isIncumbent = (candidateRaceInfo[i].IsIncFlg.ToLower() == "y") ? "1" : "0";

                                if (Convert.ToBoolean(candidateRaceInfo[i].UseAPraceCall))
                                {
                                    isWinner = (candidateRaceInfo[i].Cstat == "W" && !lockout) ? "1" : "0";
                                }
                                else
                                {
                                    isWinner = (candidateRaceInfo[i].Cid == candidateRaceInfo[i].RaceWinnerCanID && !lockout) ? "1" : "0";
                                }

                                if (UseExpectedVoteIn)
                                {
                                    preInPercent = (float)Convert.ToDouble(candidateRaceInfo[i].PctExpVote);
                                }
                                else
                                {
                                    if (candidateRaceInfo[i].PctsReporting != "0")
                                    {
                                        var num1 = Convert.ToDouble(candidateRaceInfo[i].PctsReporting);
                                        var num2 = Convert.ToDouble(candidateRaceInfo[i].TotPcts);
                                        preInPercent = (num1 * 100) / num2;

                                        preInPercent = preInPercent == 100.0 ? 99.0 : preInPercent;

                                    }
                                    else
                                    {
                                        preInPercent = 0;
                                    }
                                }

                                Console.WriteLine(candidateRaceInfo[i].OfficeName);
                                isGain = (candidateRaceInfo[i].InIncPtyFlg == "N" && isWinner == "1" && candidateRaceInfo[i].IgnoreGain == "1") ? "1" : "0";

                                Cands += $"name={candidateRaceInfo[i].CandLastName};party={GetPartyCode(candidateRaceInfo[i].MajorPtyId)};incum={isIncumbent};vote={candidateRaceInfo[i].Cvote};percent={candidatePercentStr};check={isWinner};gain={isGain};imagePath=^";

                                if (i == candidateRaceInfo.Count - 1)
                                {
                                    isDone = true;
                                }
                            }

                            if (isWinner2 == "1")
                            {
                                Cands = WinnerInfo + Cands;
                                RaceIDs = RaceWinnerID + RaceIDs;
                            }

                            preInPrecentFinal = (preInPercent < 1.0) ? "<1" : Convert.ToInt32(preInPercent).ToString();

                            Races = $"state={candidateRaceInfo[0].Jname.ToUpper()}; race=;precincts={preInPrecentFinal};office={candidateRaceInfo[0].OfficeName};raceMode=;eVote=;atStake={StoredProcedures.GetDelegates(candidateRaceInfo[0].StateAbbv, candidateRaceInfo[0].MajorPtyId.ToUpper())[0].DelegatesAtStake}";

                            DataForViz = $"{RaceIDs.TrimEnd('^')}~{Races}~{Cands.TrimEnd('^')}";

                            HasData = preInPercent > 0 ? true : false;

                            string[] candIDs = RaceIDs.Split('^');

                            if (isWinner2 == "1" && candIDs[1] == "USGOV999999" && candIDs[2] == "USGOV999999")
                            {
                                HasData = false;
                            }

                            if (HasData)
                            {
                                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey1"].Value, DataForViz, "raceData", 0);
                                PostToTB(DataForViz);
                                RaceDataWheelTimer.Start();
                            }
                            else
                            {
                                RaceCounter++;
                                SendRaceDataToViz();
                            }

                            if (ShowTop3.Checked)
                            {
                                RaceCounter++;
                            }

                            if (ShowTop6.Checked)
                            {
                                if ((isWinnerCounter == 0 && OneLineRaceCounter != 3))
                                {
                                    OneLineRaceCounter = 3;
                                }
                                else if ((isWinnerCounter == 1 && OneLineRaceCounter < 2))
                                {
                                    OneLineRaceCounter = 3 - isWinnerCounter;
                                }
                                else
                                {
                                    OneLineRaceCounter = 0;
                                    RaceCounter++;
                                }
                            }

                            if (ShowAll.Checked)
                            {
                                if (!isDone)
                                {
                                    OneLineRaceCounter = (OneLineRaceCounter + 3) - isWinnerCounter;
                                }
                                else
                                {
                                    OneLineRaceCounter = 0;
                                    RaceCounter++;
                                }
                            }
                        }
                    }
                    else
                    {
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "EMERGENCY", "sendData", 0);
                        PostToTB("NO DATA IN FOR RACES!!");

                        if(ActiveRaces.Count > 1)
                        {
                            RaceCounter++;
                            SendRaceDataToViz();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Debug("\n\nSendDataToViz Error\n\n" + ex + "\n");
                    log.Error("\n\nSendDataToViz Error\n\n" + ex + "\n");

                    ErrorFlag = true;
                    RaceDataWheelTimer.Start();
                }
            }
            else
            {
                try
                {
                    if (AreRaces)
                    {
                        string leftSideData = "";
                        string rightSideData = "";
                        string DataType = "";

                        if (ActiveRaces.Count != -1)
                        {
                            if (TickerType == 2)
                            {
                                if (LEmger && !REmger)
                                {
                                    DataType = "EMERGENCY|DATA";
                                }
                                else if (REmger && !LEmger)
                                {
                                    DataType = "DATA|EMERGENCY";
                                }
                                else if (LEmger && REmger)
                                {
                                    DataType = "EMERGENCY^^|EMERGENCY^^";
                                }
                                else
                                {
                                    if (LeftSideRace.Count() != 0 && RightSideRace.Count() != 0)
                                    {
                                        DataType = "DATA|DATA";
                                        leftSideData = SetTwoLineData("0");
                                    }
                                    else if (LeftSideRace.Count() == 0 && RightSideRace.Count() != 0)
                                    {
                                        DataType = "EMERGENCY|DATA";
                                    }
                                    else if (LeftSideRace.Count() != 0 && RightSideRace.Count() == 0)
                                    {
                                        DataType = "DATA|EMERGENCY";
                                        leftSideData = SetTwoLineData("0");
                                    }
                                    else
                                    {
                                        DataType = "EMERGENCY^^|EMERGENCY^^";
                                        leftSideData = SetTwoLineData("0");
                                    }
                                }

                                if (!UseDiffRightSide)
                                {
                                    rightSideData = SetTwoLineData("1");
                                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, DataType, "sendData", 0);
                                    string DataForViz = "";

                                    if (DataType != "EMERGENCY|DATA" && DataType != "DATA|EMERGENCY" && leftSideData != "")
                                    {
                                        DataForViz = $"{leftSideData.Split('~')[0]}|{rightSideData.Split('~')[0]}|~{leftSideData.Split('~')[1]}|{rightSideData.Split('~')[1]}|~{leftSideData.Split('~')[2]}|{rightSideData.Split('~')[2]}|";
                                    }
                                    else if(DataType == "EMERGENCY|DATA")
                                    {
                                        DataForViz = $"|{rightSideData.Split('~')[0]}|~{rightSideData.Split('~')[1]}|~{rightSideData.Split('~')[2]}";
                                    }

                                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey1"].Value, DataForViz, "raceData", 0);

                                    if (DataForViz != "")
                                    {
                                        PostToTB(DataForViz);
                                    }
                                    else
                                    {
                                        PostToTB("NO DATA FOR ALL RACES");
                                    }
                                }
                                else if(UseDiffRightSide && !DataType.Contains("EMERGENCY"))
                                {
                                    if (!InsertClosing)
                                    {
                                        if ((RSBop.Checked && RightSideType == "BALANCE_OF_POWER") || (RSNetGain.Checked && RightSideType == "NETGAIN"))
                                        {
                                            RightSideString2 = GetRightSideData(RightSideType, BopCounter);

                                            if (RSBop.Checked && radioButton1.Checked)
                                            {
                                                if (BopCounter == 0)
                                                {
                                                    BopCounter++;
                                                }
                                                else
                                                {
                                                    BopCounter = 0;
                                                    UseDiffRightSide = false;
                                                }
                                            }
                                            else
                                            {
                                                UseDiffRightSide = false;
                                            }
                                        }
                                        else
                                        {
                                            UseDiffRightSide = false;
                                            RightSideString2 = GetRightSideData(RightSideType, 0);
                                        }
                                    }
                                    else
                                    {
                                        RightSideString2 = GetRightSideData("DOORS_CLOSE", 0);
                                        RightSideType = "DOORS_CLOSE";
                                    }

                                    DataType = "DATA|" + RightSideType + "^" + RightSideString2;
                                    
                                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, DataType, "sendData", 0);

                                    string DataForViz = $"{leftSideData.Split('~')[0]}|~{leftSideData.Split('~')[1]}|~{leftSideData.Split('~')[2]}|";
                                    PostToTB(DataForViz + RightSideType + "^" + RightSideString2);
                                    isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey1"].Value, DataForViz, "raceData", 0);

                                }
                                else
                                {
                                    UseDiffRightSide = false;
                                }
                            }
                            else
                            {
                                if (!LEmger)
                                {
                                    LeftSideRace = SingleSideRace;
                                    LeftSideRace = SingleSideRace.ToList();

                                    leftSideData = SetTwoLineData("0");

                                    if (leftSideData == "" || leftSideData == null)
                                    {
                                        //leftSideData = SetTwoLineData("0");
                                        //SendRaceDataToViz();
                                        //return;
                                    }
                                    
                                    //leftSideData = SetTwoLineData("1");

                                    if (!annOn)
                                    {
                                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "DATA", "sendData", 0);
                                    }

                                    if (leftSideData != "")
                                    {

                                        //string DataForViz = $"{leftSideData.Split('~')[0]}~{leftSideData.Split('~')[1]}~{leftSideData.Split('~')[2]}";
                                        string DataForViz = leftSideData;
                                        if (DataForViz != null && DataForViz != "")
                                        {
                                            PostToTB(DataForViz);
                                        }

                                        /*
                                        string comp = leftSideData.Split('~')[1].Split(';')[0].Split('=')[1];

                                        if (comp != currentRaceState)
                                        {
                                            if (annOn)
                                            {
                                                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, $"DATA^{comp.ToUpper().Trim()}^", "sendData", 0);
                                            }
                                            currentRaceState = comp;
                                        }

                                        */
                                        VizCommands.SendVizMessage("", flag.showHeadshots , "headshots", 0);
                                        //isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, $"DATA^NJ^", "sendData", 0);
                                        //isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey1"].Value, DataForViz, "raceData", 0);
                                        //VizCommands.SendVizMessage("", "", "forBA", 0);
                                        VizTickerFunctions VTF = new VizTickerFunctions();
                                        VTF.StringToViz2("TICKER_DATA_BROKER", DataForViz, "raceData", 0);
                                        System.Threading.Thread.Sleep(50);
                                        VizCommands.SendVizMessage("ElectDataParser", "TICKER_DATA_BROKER", "forBA", 0);
                                    }
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                        else
                        {
                            PostToTB("NO DATA IN FOR ANY RACES!!");
                            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "EMERGENCY|EMERGENCY", "sendData", 0);
                        }
                    }
                    else
                    {
                        PostToTB("NO DATA IN FOR ANY RACES!!");
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "EMERGENCY|EMERGENCY", "sendData", 0);
                    }
                }
                catch(Exception ex)
                {
                    log.Debug("\n\nSendDataToViz - TwoLine Error\n\n" + ex + "\n");
                    log.Error("\n\nSendDataToViz - TwoLine Error\n\n" + ex + "\n");

                    ErrorFlag = true;
                    RaceDataWheelTimer.Start();
                }
            }
        }

        public void BOPSelection(object sender, EventArgs e)
        {
            if(radioButton1.Checked || radioButton2.Checked)
            {
                BopCounter = 0;
            }
            else
            {
                BopCounter = 1;
            }
        }

        public string SetTwoLineData(string side)
        {
            fullData fullData = new fullData();
            List<foxId> ids = new List<foxId>();
            List<candidates> candidates = new List<candidates>();  
            raceData raceData = new raceData();

            int sideCounter = side == "0" ? leftSideCounter : rightSideCounter;

            List<Races> CurrentRace = side == "0" ? LeftSideRace : RightSideRace;
            //List<Races> CurrentRace = side == "0" ? SingleSideRace : RightSideRace;

            string DataForViz = "";

            if (sideCounter > CurrentRace.Count() - 1)
            {
                sideCounter = 0;

                if(side == "0")
                {
                    leftSideCounter = 0;
                }
                else
                {
                    rightSideCounter = 0;
                }
            }

            //Console.WriteLine(leftSideCounter);
            List<RaceData> candidateRaceInfo = new List<RaceData>();

            try
            {
                if (CurrentRace.Count == 0)
                {
                    if (side == "0")
                    {
                        PostToTB("NO DATA IN FOR LEFT SIDE RACES");
                    }
                    else
                    {
                        PostToTB("NO DATA IN FOR RIGHT SIDE RACES");
                    }

                }
                else
                {
                    if (AreRaces)
                    {
                        string testStr = "";
                        if (CurrentRace[sideCounter].RaceElectionType == "R")
                        {
                            testStr = "rep";
                        }
                        else
                        {
                            testStr = "dem";
                        }
                        //Console.WriteLine(sideCounter);
                        candidateRaceInfo = StoredProcedures.GetRaceData(CurrentRace[sideCounter].StateID, CurrentRace[sideCounter].RaceOfficeCode, CurrentRace[sideCounter].RaceDistrict, CurrentRace[sideCounter].RaceElectionType, testStr);
                    }

                    if (AreRaces && candidateRaceInfo.Count > 0)
                    {
                        //isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "DATA", "sendData", 0);

                        RaceDataWheelTimer.Stop();

                        //Console.WriteLine(candidateRaceInfo);
                        DateTime pollClosingTime;
                        DateTime CurrentDateTime = UseSimTime ? SimTime : DateTime.Now;

                        bool lockout = true;
                        bool HasData = false;

                        double candidatePercent = 0.0;
                        double dateDiff = 0.0;

                        string isIncumbent = "";
                        string isGain = "";
                        string isWinner = "";
                        //string isWinner2 = "";
                        string candidatePercentStr = "";

                        string RaceIDs = "";
                        string Cands = "";
                        string Races = "";
                        string preInPrecentFinal = "";

                        double preInPercent = 0;


                        if(flag.tickerCanNum != 0)
                        {
                            canCont = DisplayCandidates;
                            nextRace = true;
                        }
                        else
                        {
                            if (nextRace)
                            {
                                canCont = 2;
                                nextRace = false;
                            }
                        }

                        if (true)
                        {
                            //Change i to 1 for just two candidates
                            for (int i = candStartIndex; i <= canCont; i++)
                            {
                                //Console.WriteLine(i + "|" + canCont + "|" + (candidateRaceInfo.Count - 1).ToString());

                                if(i <= candidateRaceInfo.Count - 1)
                                {

                                    //candidates candidates1 = new candidates();

                                    RaceIDs += candidateRaceInfo[i].FoxID + "^";
                                    foxId fox = new foxId()
                                    {
                                        id = candidateRaceInfo[i].FoxID
                                    };
                                    ids.Add(fox);
                                    pollClosingTime = Convert.ToDateTime(CurrentRace[sideCounter].PollClosingDateTime);
                                    dateDiff = (CurrentDateTime - pollClosingTime).TotalSeconds;


                                    lockout = PollLockoutEnabled ? (lockout = (dateDiff < 0.0) ? true : false) : false;

                                    candidatePercent = (Convert.ToDouble(candidateRaceInfo[i].VoteSum) != 0.0) ? ((Convert.ToDouble(candidateRaceInfo[i].Cvote) / Convert.ToDouble(candidateRaceInfo[i].VoteSum)) * 100) : 0.0;
                                    candidatePercentStr = UseTenthsPercent ? string.Format("{0:0.0}", candidatePercent) : string.Format("{0:0}", Math.Round(candidatePercent, MidpointRounding.AwayFromZero));

                                    if (candidatePercent < 1.0)
                                    {
                                        candidatePercentStr = "<1";
                                    }

                                    isIncumbent = (candidateRaceInfo[i].IsIncFlg.ToLower() == "y") ? "1" : "0";

                                    if (Convert.ToBoolean(candidateRaceInfo[i].UseAPraceCall))
                                    {
                                        isWinner = (candidateRaceInfo[i].Cstat == "W" && !lockout) ? "1" : "0";
                                    }
                                    else
                                    {
                                        isWinner = (candidateRaceInfo[i].Cid == candidateRaceInfo[i].RaceWinnerCanID && !lockout) ? "1" : "0";
                                    }

                                    //if (UseExpectedVoteIn && !CurrentRace[sideCounter].RaceOfficeCode.Contains("H"))
                                    if (UseExpectedVoteIn)
                                    {
                                        preInPercent = (float)Convert.ToDouble(candidateRaceInfo[i].PctExpVote);
                                    }
                                    else
                                    {
                                        if (candidateRaceInfo[i].PctsReporting != "0")
                                        {
                                            var num1 = Convert.ToDouble(candidateRaceInfo[i].PctsReporting);
                                            var num2 = Convert.ToDouble(candidateRaceInfo[i].TotPcts);
                                            preInPercent = (num1 * 100) / num2;

                                            if (!UseExpectedVoteIn)
                                            {
                                                preInPercent = preInPercent > 100 ? 99 : preInPercent;
                                            }
                                        }
                                        else
                                        {
                                            preInPercent = 0;
                                        }
                                    }

                                    string inc = "";

                                    if (trueToolStripMenuItem.Checked)
                                    {
                                        inc = isIncumbent == "0" ? "" : "*";
                                    }
                                    else
                                    {
                                        inc = isIncumbent == "0" ? "" : "";
                                    }


                                    if (!candidateRaceInfo[i].OfficeName.Contains("House"))
                                    {
                                        isGain = (candidateRaceInfo[i].InIncPtyFlg == "N" && isWinner == "1" && candidateRaceInfo[i].IgnoreGain == "0" && (CurrentRace[sideCounter].RaceOfficeCode.Contains("S") || CurrentRace[sideCounter].RaceOfficeCode.Contains("H"))) ? "1" : "0";
                                    }
                                    else
                                    {
                                        isGain = "0";
                                    }

                                    //string fVoteCount = Convert.ToInt32(candidateRaceInfo[i].Cvote).ToString("N0");
                                    string fVoteCount = Convert.ToInt32(candidateRaceInfo[i].Cvote).ToString();

                                    //Console.WriteLine(Convert.ToInt32(candidateRaceInfo[i].Cvote).ToString("N0"));

                                    //Cands += $"name={candidateRaceInfo[i].CandLastName}{inc};party={GetPartyCode(candidateRaceInfo[i].MajorPtyId)};incum={isIncumbent};vote={candidateRaceInfo[i].Cvote};percent={candidatePercentStr};check={isWinner};gain={isGain};imagePath=^";
                                    Cands += $"name={candidateRaceInfo[i].CandLastName}{inc};party={GetPartyCode(candidateRaceInfo[i].MajorPtyId)};incum={isIncumbent};vote={fVoteCount};percent={candidatePercentStr};check={isWinner};gain={isGain};imagePath=^";

                                    string[] canSplit = candidateRaceInfo[i].headShotPath.Split('\\');

                                    candidates candidates1 = new candidates()
                                    {
                                        name = candidateRaceInfo[i].CandLastName + inc,
                                        party = GetPartyCode(candidateRaceInfo[i].MajorPtyId),
                                        incum = isIncumbent,
                                        vote = fVoteCount,
                                        percent = candidatePercentStr,
                                        check = isWinner,
                                        gain = isGain,
                                        imagePath = canSplit[canSplit.Length - 1].Split('.')[0].Trim()
                                    };


                                    candidates.Add(candidates1);
                                }
                                else
                                {
                                    RaceIDs += "USGOV999999";
                                    Cands += $"name=;party=;incum=;vote=;percent=;check=;gain=;imagePath=^";
                                    nextRace = true;
                                    /*
                                    
                                    candStartIndex = 0;
                                    canCont = 1;
                                    leftSideCounter++;
                                    SetTwoLineData(side);
                                    */

                                    //candidates candidates1 = new candidates();

                                    //candidates.Add(candidates1);
                                }
                            }

                            //Console.WriteLine(candidates.Count);

                            if(candidates.Count == 0)
                            {
                                nextRace = true;
                                leftSideCounter++;
                                candStartIndex = 0;
                                canCont = 2;
                                SendRaceDataToViz();
                                return null;
                            }

                            if(ids.Count == 1)
                            {
                                foxId fox = new foxId()
                                {
                                    id = "USGOV999999"
                                };

                                candidates candidates1 = new candidates()
                                {
                                    name = "",
                                    party = "",
                                    incum = "",
                                    vote = "",
                                    percent = "",
                                    check = "",
                                    gain = "",
                                    imagePath = ""
                                };

                                ids.Add(fox);
                                candidates.Add(candidates1);
                                Console.WriteLine("one");
                            }

                            if(preInPercent >= 100)
                            {
                                preInPercent = 99.0;
                            }

                            preInPrecentFinal = (preInPercent < 1.0) ? "<1" : Convert.ToInt32(preInPercent).ToString();

                            if (UseExpectedVoteIn)
                            {
                                preInPrecentFinal = preInPrecentFinal == "100" ? "99" : preInPrecentFinal;
                            }

                            //Races = $"state={candidateRaceInfo[0].Jname.ToUpper()}; race=;precincts={preInPrecentFinal};office={candidateRaceInfo[0].OfficeName};raceMode=;eVote=;atStake={StoredProcedures.GetDelegates(candidateRaceInfo[0].StateAbbv, candidateRaceInfo[0].MajorPtyId.ToUpper())[0].DelegatesAtStake}";

                            //string district = CurrentRace[sideCounter].RaceDistrict == "0" ? "" : "CD " + CurrentRace[sideCounter].RaceDistrict;

                            if(Convert.ToInt32(CurrentRace[sideCounter].RaceDistrict) > 0 && Convert.ToInt32(CurrentRace[sideCounter].RaceDistrict) < 10)
                            {
                                CurrentRace[sideCounter].RaceDistrict = "0" + CurrentRace[sideCounter].RaceDistrict;
                            }
                            string district = CurrentRace[sideCounter].RaceDistrict == "0" ? "" : "CD-" + CurrentRace[sideCounter].RaceDistrict;
                            string officeType = "";

                            //officeType = CurrentRace[sideCounter].RaceOfficeCode.Contains("P") ? "PRESIDENT" : CurrentRace[sideCounter].RaceOfficeCode.Contains("S") ? "SENATE" : CurrentRace[sideCounter].RaceOfficeCode.Contains("G") ? "GOVERNOR" : CurrentRace[sideCounter].RaceOfficeCode.Contains("H") ? "HOUSE" + district : "";
                            //officeType = CurrentRace[sideCounter].RaceOfficeCode.Contains("P") ? "PRESIDENT" : CurrentRace[sideCounter].RaceOfficeCode.Contains("S") ? "SENATE" : CurrentRace[sideCounter].RaceOfficeCode.Contains("G") ? "GOVERNOR" : CurrentRace[sideCounter].RaceOfficeCode.Contains("H") ? "" + district : "";
                            officeType = CurrentRace[sideCounter].RaceOfficeCode.Contains("P") ? "PRESIDENT" : CurrentRace[sideCounter].RaceOfficeCode.Contains("S") ? "SENATE" : CurrentRace[sideCounter].RaceOfficeCode.Contains("G") ? "GOVERNOR" : CurrentRace[sideCounter].RaceOfficeCode.Contains("H") ? "HOUSE" : "";

                            //Remove after the primaries are done
                            if (officeType == "GOVERNOR")
                            {
                                //officeType = "GOV";
                            }
                            else if(officeType == "HOUSE")
                            {
                                //officeType = "";
                            }

                            Races = $"state={CurrentRace[sideCounter].StateName.ToUpper()}; race={district};precincts={preInPrecentFinal};office={officeType};raceMode=;eVote=;atStake=10;evdel={CollegeVotes.Find(x => x.Abbv == CurrentRace[sideCounter].StateAbbv).EVotes}";

                            raceData.state = CurrentRace[sideCounter].StateName.ToUpper();
                            raceData.cd = district;
                            raceData.precincts = preInPrecentFinal;
                            raceData.office = officeType;
                            raceData.raceMode = "";
                            raceData.delegates = StoredProcedures.getDelegate(CurrentRace[sideCounter].RaceDetail, CurrentRace[sideCounter].StateAbbv);
                            raceData.evotes = CollegeVotes.Find(x => x.Abbv == CurrentRace[sideCounter].StateAbbv).EVotes;

                            DataForViz = $"{RaceIDs.TrimEnd('^')}~{Races}~{Cands.TrimEnd('^')}";

                            HasData = preInPercent > 0 ? true : false;

                            string[] candIDs = RaceIDs.Split('^');

                            fullData.raceData = raceData;
                            fullData.foxId = ids;
                            //fullData.candidateData.candidates = candidates;
                            raceData.candidates = candidates;

                            

                            List<fullData> root = new List<fullData>();
                            root.Add(fullData);
                            //Console.WriteLine(JsonConvert.SerializeObject(root, Formatting.Indented));
                            DataForViz = JsonConvert.SerializeObject(root, Formatting.Indented);

                            HasData = true;
                            if (HasData)
                            {
                                //PostToTB(DataForViz);
                                RaceDataWheelTimer.Start();
                            }
                            else
                            {

                                if (side == "0")
                                {
                                    leftSideCounter++;
                                }
                                else
                                {
                                    rightSideCounter++;
                                }

                                SetTwoLineData(side);

                            }
                        }
                    }
                    else
                    {
                        PostToTB("NO DATA IN FOR " + CurrentRace[sideCounter].StateName + " " + CurrentRace[sideCounter].RaceDesc);

                        if (side == "0")
                        {
                            leftSideCounter++;
                        }
                        else
                        {
                            rightSideCounter++;
                        }

                        SetTwoLineData(side);
                    }
                }
            }
            catch(Exception ex)
            {
                DataForViz = "ERROR!";

                if (side == "0")
                {
                    leftSideCounter++;
                }
                else
                {
                    rightSideCounter++;
                }

                log.Debug("\n\nSendDataToViz - TwoLine Error\n\n" + ex + "\n");
                log.Error("\n\nSendDataToViz - TwoLine Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }

            if(side == "0")
            {
                if (nextRace && flag.tickerCanNum == 0)
                {
                    leftSideCounter++;
                    candStartIndex = 0;
                    canCont = 2;
                }
                else if (nextRace && flag.tickerCanNum != 0)
                {
                    leftSideCounter++;
                    candStartIndex = 0;
                    canCont = DisplayCandidates;
                }
                else
                {
                    candStartIndex = candStartIndex + 3;
                    canCont = canCont + 3;
                }
            }
            else
            {
                rightSideCounter++;
            }
            return DataForViz;
        }

        private void RaceModeChange(object sender, EventArgs e)
        {
            CheckRaceMode();
        }

        public void CheckRaceMode()
        {
            try
            {
                if (PreDataTicker.Checked)
                {
                    groupBox3.Visible = false;
                    CanOptionsBox.Visible = false;
                    PreOptionsBox.Visible = true;
                    EnabledRightSide.Visible = false;
                    DataMode = "NULL";
                    IsDataMode = false;

                    if (StartTicker)
                    {
                        if (sidePanelToolStripMenuItem.Checked)
                        {
                            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, DataMode, "sendData", 1);
                        }
                        else
                        {
                            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, DataMode, "sendData", 0);
                        }


                        PreDataWheeler();

                        WheelTimer.Enabled = true;
                        RaceDataWheelTimer.Enabled = false;
                    }
                }
                else
                {
                    RSChecker();
                    EnabledRightSide.Visible = true;
                    PreOptionsBox.Visible = false;
                    IsDataMode = true;
                    if (StartTicker)
                    {
                        SendRaceDataToViz();
                        WheelTimer.Enabled = false;
                        RaceDataWheelTimer.Enabled = true;
                    }
                }


                config.AppSettings.Settings["DataMode"].Value = Convert.ToString(DataTicker.Checked);
                config.Save(ConfigurationSaveMode.Full, true);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                log.Debug("\n\nRaceModeChange Error\n\n" + ex + "\n");
                log.Error("\n\nRaceModeChange Error\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void ErrorLabel_Click(object sender, EventArgs e)
        {
            if(ErrorLabel.BackColor == Color.Red)
            {
                ErrorLabel.BackColor = Color.Green;
                ErrorFlag = false;
                ErrorLabel.Text = "NO ERRORS";
                try
                {
                    Process.Start("explorer.exe", @"c:\logs");
                }
                catch(Exception ex)
                {
                    log.Debug("\n\nError Opening Log Folder\n\n" + ex + "\n");
                    log.Error("\n\nError Opening Log Folder\n\n" + ex + "\n");
                }
            }
        }

        private void restartApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        public void PostToTB(string text)
        {
            //Console.WriteLine(text);
            try
            {
                if (RaceDataLB.Items.Count >= 200)
                {
                    Invoke(new Action(() => RaceDataLB.Items.Clear()));
                }

                RaceDataLB.Items.Add($"[{DateTime.Now}] - {text}");

                if(AutoScrollLB.Checked)
                {
                    RaceDataLB.SelectedIndex = RaceDataLB.Items.Count - 1;
                }

                log.Debug($"[{DateTime.Now}] - {text}");
            }
            catch (Exception ex)
            {
                log.Debug("\n\nError clearing listbox\n\n" + ex + "\n");
                log.Error("\n\nError clearing listbox\n\n" + ex + "\n");
            }
        }

        public string GetPartyCode(string PartyCode)
        {
            string code = "2";

            if(PartyCode.ToLower() == "dem")
            {
                code = "1";
            }
            else if(PartyCode.ToLower() == "rep")
            {
                code = "0";
            }
            else if(PartyCode.ToLower() == "ind")
            {
                code = "2";
            }

            return code;
        }

        private void UpdateWheel_Click(object sender, EventArgs e)
        {
            try
            {
                PreDataSchedule.Clear();

                Checker();
            }
            catch (Exception ex)
            {
                log.Debug("\n\nCheckPreDataSchedule\n\n" + ex + "\n");
                log.Error("\n\nCheckPreDataSchedule\n\n" + ex + "\n");

                ErrorFlag = true;
            }
        }

        private void RightSideTimer_Tick(object sender, EventArgs e)
        {
            int max = 0;

            if (EnabledRightSide.Checked)
            {
                foreach (RightSideOptions ax in RightSideList)
                {
                    if (ax.OptionEnabled)
                    {
                        if (ax.OptionTime > max)
                        {
                            max = ax.OptionTime;
                        }
                    }
                }

                foreach (RightSideOptions a in RightSideList)
                {
                    if (a.OptionEnabled)
                    {
                        if (a.OptionTime == RightSideTime)
                        {
                            UseDiffRightSide = true;
                            RightSideType = a.OptionType;

                            break;
                        }
                    }
                }

                if (RightSideTime < max)
                {
                    RightSideTime++;
                }
                else
                {
                    RightSideTime = 1;
                }
            }
        }

        public string GetRightSideData(string RType, int RCount)
        {
            string data = "";

            if (StartTicker && TickerType == 2 && DataTicker.Checked)
            {
                if (RType == "BALANCE_OF_POWER")
                {
                    if (RCount == 0)
                    {
                        data = "SENATE^NEW^" + StoredProcedures.GetRightSidedata(0, RCount, SimTime);
                    }
                    else
                    {
                        data = "HOUSE^NEW^" + StoredProcedures.GetRightSidedata(0, RCount, SimTime);
                    }
                }
                else if (RType == "POPULAR_VOTE")
                {
                    data = StoredProcedures.GetRightSidedata(1, 0, SimTime);
                    string[] dataAr = data.Split('^');
                    data = String.Format("{0:n0}", Convert.ToInt32(dataAr[0])) + "^" + String.Format("{0:n0}", Convert.ToInt32(dataAr[1]));
                }
                else if (RType == "DELEGATE_TRACKER")
                {
                    data = StoredProcedures.GetRightSidedata(2, 0, SimTime);
                }
                else if (RType == "NETGAIN")
                {
                    if (RCount == 0)
                    {
                        data = $"SENATE^{StoredProcedures.GetRightSidedata(3, RCount, SimTime)}";
                    }
                    else
                    {
                        data = $"HOUSE^{StoredProcedures.GetRightSidedata(3, RCount, SimTime)}";
                    }
                }
                else if (RType == "DOORS_CLOSE")
                {
                    string NextTime = StoredProcedures.GetNextPollTime(SimTime.ToString("yyyy-MM-dd HH:mm"));
                    
                    if (NextClosings.Count == 0)
                    {
                        NextClosings = StoredProcedures.GetPollStates(NextTime);
                    }
                    
                    data = NextClosings.Peek() + "^" + Convert.ToDateTime(NextTime).ToString("HH:mm");
                  
                    NextClosings.Dequeue();

                    if (NextClosings.Count == 0)
                    {
                        UseDiffRightSide = false;
                        InsertClosing = false;
                        CloseTimes2.Dequeue();
                    }
                }
            }
            else
            {
                UseDiffRightSide = false;
            }
            return data;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RSChecker();
        }

        public void RSChecker()
        {
            if (EnabledRightSide.Checked && DataTicker.Checked)
            {
                groupBox3.Visible = true;
                RightSideTimer.Enabled = true;
                RightSideTimer.Stop();
                RightSideTimer.Start();
            }
            else
            {
                groupBox3.Visible = false;
                RightSideTimer.Enabled = false;
                RightSideTimer.Stop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsertClosing = true;
            UseDiffRightSide = true;
        }

        private void PromoTimer2_Tick(object sender, EventArgs e)
        {
            PromoTimer2.Stop();
            PrePromoCounter++;
            if(PromoIn)
            {
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "ONE_LINE", "sendData", 0);

                if (PrePromoCounter > 3)
                {
                    PromoIn = false;
                }

                PromoTimer2.Interval = 5000;
                PromoTimer2.Start();

                GetPromos();

            }
            else
            {
                
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "NULL", "sendData", 0);
                PrePromoCounter = 0;
                PromoIn = true;
                PromoTimer2.Interval = 10000;
                PromoTimer2.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LEmger = LEmger ? false : true;

            if(!LEmger)
            {
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, $"DATA^^", "sendData", 0);
            }
            else
            {
                if (NetworkID != 1)
                {
                    if (OneLineTicker.Checked)
                    {
                        Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP1" + (char)4);
                        FoxPlusStream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP0" + (char)4);
                        FoxPlusStream.Write(data, 0, data.Length);
                    }
                }
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, KeyValue, "sendData", 0);
                isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "EMERGENCY^^", "sendData", 0);
            }

            button2.Text = LEmger ? "LEFT COVERUP - OUT" : "LEFT COVERUP - IN";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            REmger = REmger ? false : true;

            button3.Text = REmger ? "RIGHT COVERUP - OUT" : "RIGHT COVERUP - IN";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(annOn)
            {
                annOn = false;
                button4.Text = "Ann On";
            }
            else
            {
                annOn = true;
                button4.Text = "Ann off";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (PreDataTicker.Checked || DataTicker.Checked)
            {
                if (StartStopTicker.BackColor == Color.Green)
                {
                    Init();

                    StartStopTicker.Text = "STOP TICKER";
                    StartStopTicker.BackColor = Color.Red;
                    StartTicker = true;
                    if (PreDataTicker.Checked)
                    {
                        if (sidePanelToolStripMenuItem.Checked)
                        {
                            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, KeyValue, "sendData", 1);
                            Checker();
                            PreDataWheeler();
                            WheelTimer.Enabled = true;
                            PromoTimer2.Enabled = false;
                            PromoTimer2.Stop();
                        }
                        else
                        {
                            PromoTimer2.Enabled = true;
                            PromoTimer2.Start();
                        }

                        RaceDataWheelTimer.Enabled = false;
                    }
                    else
                    {
                        PromoTimer2.Enabled = false;
                        PromoTimer2.Stop();
                        isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, KeyValue, "sendData", 0);
                        SendRaceDataToViz();
                        WheelTimer.Enabled = false;
                        RaceDataWheelTimer.Enabled = true;
                    }

                    tabControl1.SelectedTab = tabPage2;
                }
                else
                {
                    StartStopTicker.Text = "START TICKER";
                    StartStopTicker.BackColor = Color.Green;
                    StartTicker = false;
                    WheelTimer.Enabled = false;
                    RaceDataWheelTimer.Enabled = false;
                    PromoTimer2.Stop();
                }
            }

            isVizConnected = VizCommands.SendVizMessage("", "", "unload", 0);
            isVizConnected = VizCommands.SendVizMessage("", "", "unload", 1);

            //Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP0" + (char)4);
            //FoxPlusStream.Write(data, 0, data.Length);

            this.Close();
        }

        private void button5_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, button1.ClientRectangle,
            SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset);
        }

        private void Election_Ticker_2020_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (NetworkID != 1)
                {
                    Byte[] data = Encoding.ASCII.GetBytes((char)1 + "BP0" + (char)4);
                    FoxPlusStream.Write(data, 0, data.Length);

                    data = Encoding.ASCII.GetBytes((char)1 + "BP3" + (char)4);
                    FoxPlusStream.Write(data, 0, data.Length);

                    FoxPlusStream.Close();
                    FoxPlus.Close();
                }
                isVizConnected = VizCommands.SendVizMessage("", "", "unload", 0);
                isVizConnected = VizCommands.SendVizMessage("", "", "unload", 1);

                StoredProcedures.Post2ApplicationLog(this.Name, this.Text, HostIPNameFunctions.GetHostName(hostIP), hostIP, true, AppInfo.EngineIP, false, "N/A", "Closed application", config.AppSettings.Settings["AppVersion"].Value, Convert.ToInt32(config.AppSettings.Settings["ApplicationID"].Value), "N /A", DateTime.Now);
            }
            catch (Exception ex)
            {
                log.Debug("\n\nError when closing application\n\n" + ex + "\n");
                log.Error("\n\nError when closing application\n\n" + ex + "\n");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey4"].Value, "FBN", "sendData", 0);
            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey2"].Value, "TWO_LINE", "sendData", 0);
            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "DATA", "sendData", 0);
            isVizConnected = VizCommands.SendVizMessage(config.AppSettings.Settings["shmKey3"].Value, "NULL", "sendData", 0);
        }

        private void RaceDataLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = RaceDataLB.SelectedItem.ToString();
            
        }

        private void checkFlags_Tick(object sender, EventArgs e)
        {
            CheckFlags();
        }

        public void CheckFlags()
        {
            flag = StoredProcedures.CheckFlags();

            PollLockoutEnabled = flag.LockoutEnabled;
            PollLock.BackColor = PollLockoutEnabled ? Color.Green : Color.Red;

            ZeroPctsLockoutEnabled = flag.ZeroPrecinctsLockoutEnabled;

            UseExpectedVoteIn = flag.UseExpectedVoteIn;
            UseExp.BackColor = UseExpectedVoteIn ? Color.Green : Color.Red;

            UseSimTime = flag.UseSimTime;
            UseSim.BackColor = UseSimTime ? Color.Green : Color.Red;
            SimTime = UseSimTime ? StoredProcedures.GetSimulatedTime() : DateTime.Now;

            UseTenthsPercent = flag.UseTenthPercent;
            UseTenth.BackColor = UseTenthsPercent ? Color.Green : Color.Red;

            label16.Text = flag.usePollCloseBug ? "USE POLL CLOSING BUG : TRUE" : "USE POLL CLOSING BUG: FASLE";
            label17.Text = $"RUN POLL CLOCK {flag.bugInsertPollTime} mins prior to closing";
            label18.Text = $"RUN POLL CLOCK FOR {flag.bugRemovePollTime} mins";

            if (flag.tickerDwellTime != 0)
            {
                RaceDataWheelTimer.Interval = flag.tickerDwellTime;
                label12.Text = "TICKER DWELL: " + (flag.tickerDwellTime/1000) + " SECONDS ";
            }
            else
            {
                RaceDataWheelTimer.Interval = 5000;
                label12.Text = "TICKER DWELL: 5 SECONDS ";
            }

            if(flag.showHeadshots == "0")
            {
                label14.Text = "NO HEADSHOTS";
            }
            else
            {
                label14.Text = "SHOW HEADSHOTS";
            }

            if(flag.tickerCanNum == 2)
            {
                DisplayCandidates = 1;
                label13.Text = "SHOWING 2 CANDIDATES MAX";
            }
            else if(flag.tickerCanNum == 3)
            {
                DisplayCandidates = 2;
                label13.Text = "SHOWING 3 CANDIDATES MAX";
            }
            else
            {
                label13.Text = "SHOWING ALL CANDIDATES";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            VizCommands.SendVizMessage("", "1", "headshots", 0);
        }

        private void trueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            falseToolStripMenuItem.Checked = false;
            trueToolStripMenuItem.Checked = true;
        }

        private void falseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            falseToolStripMenuItem.Checked = true;
            trueToolStripMenuItem.Checked = false;
        }

        private void e_bug_Tick(object sender, EventArgs e)
        {
            SendBugData(bugKey, e_bug_mode);
        }

        public void SendBugData(string bKey, int bMode)
        {
            string str = "";
            string key = "";

            if (e_bug_mode == 1)
            {
                //ev
                str = "EV UPDATE";
            }
            else if (e_bug_mode == 2)
            {
                //bop senate
                //Console.WriteLine(StoredProcedures.BopString(SimTime.ToString()));
                str = "BOP SENATE UPDATE";
            }
            else if (e_bug_mode == 3)
            {
                //bop house
                //Console.WriteLine(StoredProcedures.BopString(SimTime.ToString()));
                str = "BOP HOUSE UPDATE";
            }
            else if (e_bug_mode == 4)
            {
                //net senate
                //Console.WriteLine(StoredProcedures.BopString(SimTime.ToString()));
                str = "NET SENATE UPDATE";
            }
            else if (e_bug_mode == 5)
            {
                //net house
                //Console.WriteLine(StoredProcedures.BopString(SimTime.ToString()));
                str = "NET HOUSE UPDATE";
            }

           

            VizTickerFunctions VTF = new VizTickerFunctions();

            if(e_bug_mode == 1)
            {
                key = "EV_VOTES_6100";
                VTF.StringToViz2(key, StoredProcedures.getTotalEV(SimTime.ToString()), "", 0);
            }
            else
            {
                key = "BOP_DATA_6100";
                VTF.StringToViz2(key, StoredProcedures.BopString(SimTime.ToString()), "", 0);
            }


            System.Threading.Thread.Sleep(200);

            VizCommands.SendVizMessage("ElectDataParser",key , "forBA", 0);

            VizCommands.SendVizMessage("BUG_DATA_MODES", bugKey, "sendData", 0);

            Invoke(new Action(() => TrioBox.Items.Add($"{DateTime.Now} - {str}")));
            Invoke(new Action(() => TrioBox.SelectedIndex = TrioBox.Items.Count - 1));

            if (TrioBox.Items.Count > 100)
            {
                TrioBox.Items.Clear();
            }
        }
    }
}