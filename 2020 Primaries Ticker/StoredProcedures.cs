using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _2020_Primaries_Ticker
{
    public class PromoData
    {
        public string NetworkName;
        public string TickerType;
        public string SortOrder;
        public string PromoImage;
        public string PromoTime;
        public string EndTime;
        public string isEnabled;
    }

    public class DelegateCounts
    {
        public string CandidateLastName;
        public string CandidateCount;
    }

    public class ECVotes
    {
        public string Abbv;
        public string EVotes;
    }

    public class Races
    {
        public string RaceID;
        public string RaceDesc;
        public string RaceDetail;
        public string RaceSortOrder;
        public string StateName;
        public string StateAbbv;
        public string PollClosingTime;
        public string PollClosingDateTime;
        public string UsAPcall;
        public string StateID;
        public string RaceOfficeCode;
        public string RaceElectionType;
        public string RaceDistrict;
        public string RaceHasData;
    }

    public class RaceFacts
    {
        public string Fact;
        public string StartTime;
        public string EndTime;
        public string isEnabled;
    }

    public class Delegates
    {
        public string DelegatesAtStake;
    }

    public class PollClosings
    {
        public string ClosingTime;
    }

    public class RaceData
    {
        public string OfficeName;
        public string Jname;
        public string StateAbbv;
        public string CountyName;
        public string Jcode;
        public string TotPcts;
        public string PctsReporting;
        public string PctExpVote;
        public string Cid;
        public string CandLastName;
        public string MajorPtyId;
        public string Cstat;
        public string EdListFlg;
        public string InIncPtyFlg;
        public string IsIncFlg;
        public string OutputOrder;
        public string Cvote;
        public string Avote;
        public string VoteSum;
        public string RaceWinnerCalled;
        public string RaceWinnderCalledTime;
        public string EstTS;
        public string RaceTooCloseToCall;
        public string RaceWinnerCanID;
        public string RacePollClosingTime;
        public string UseAPraceCall;
        public string IgnoreGain;
        public string FoxID;
        public string headShotPath;
    }

    public class Flags
    {
        public bool LockoutEnabled;
        public bool ZeroPrecinctsLockoutEnabled;
        public bool UseExpectedVoteIn;
        public bool UseSimTime;
        public bool UseTenthPercent;
        public int tickerCanNum;
        public int tickerDwellTime;
        public string sceneName;
        public string showHeadshots;
        public int bugInsertPollTime;
        public int bugRemovePollTime;
        public bool usePollCloseBug;
    }

    public class VoterAnalysisData
    {
        public string Response;
        public string StateName;
        public string VA_Title;
        public string VA_Data_Id;
        public string r_type;
        public string Updated;
        public string new_order;
        public string VA_Percent;
        public string StateAbbv;
    }

    public class HouseDistricts
    {
        public string StateID;
        public string HouseDistrict;
    }

    public class SidePanelData
    {
        public string ImagePath;
        public int PanelType;
        public bool IsEnabled;
        public int PanelFlag;
        public int Network;
    }

    public class CurrentBOP
    {
        public House house { get; set; }
        public Senate senate { get; set; }
    }

    public class House
    {
        public int repNum { get; set; }
        public int demNum { get; set; }
        public int indNum { get; set; }
    }

    public class HouseChng
    {
        public int repNetChange { get; set; }
        public int demNetChange { get; set; }
        public int indNetChange { get; set; }
    }

    public class HouseNum
    {
        public int repNum { get; set; }
        public int demNum { get; set; }
        public int indNum { get; set; }
    }

    public class NewBOP
    {
        public HouseNum houseNum { get; set; }
        public HouseChng houseChng { get; set; }
        public SenateNum senateNum { get; set; }
        public SenateChng senateChng { get; set; }
    }

    public class Root
    {
        public CurrentBOP currentBOP { get; set; }
        public NewBOP newBOP { get; set; }
    }

    public class Senate
    {
        public int repNum { get; set; }
        public int demNum { get; set; }
        public int indNum { get; set; }
    }

    public class SenateChng
    {
        public int repNetChange { get; set; }
        public int demNetChange { get; set; }
        public int indNetChange { get; set; }
    }

    public class SenateNum
    {
        public int repNum { get; set; }
        public int demNum { get; set; }
        public int indNum { get; set; }
    }

    public class electVotes
    {
        public evData electVotesData { get; set; }
    }

    public class evData
    {
        public string demCanName { get; set; }
        public string ecVoteDem { get; set; }
        public string repCanName { get; set; }
        public string ecVoteRep { get; set; }
    }

    public class StoredProcedures
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<PromoData> Promos(int TickerType, DateTime time)
        {
            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            List<PromoData> PromoList = new List<PromoData>();
            //Add code to check promo run and end times before adding to list
            try
            { 
                conn.Open();

                
                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetPromos"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@NetworkName", Election_Ticker_2020.AppInfo.ApplicationNetwork));
                cmd.Parameters.Add(new SqlParameter("@TickerType", TickerType));

                SqlDataReader PromoInfo = cmd.ExecuteReader();

                while (PromoInfo.Read())
                {
                    PromoData foo = new PromoData
                    {
                        NetworkName = PromoInfo["NetworkName"].ToString().Trim(),
                        TickerType = PromoInfo["TickerType"].ToString().Trim(),
                        SortOrder = PromoInfo["SortOrder"].ToString().Trim(),
                        PromoImage = PromoInfo["PromoImage"].ToString().Trim(),
                        PromoTime = PromoInfo["PromoTime"].ToString().Trim(),
                        EndTime = PromoInfo["EndTime"].ToString().Trim(),
                        isEnabled = PromoInfo["isEnabled"].ToString().Trim()
                    };

                    DateTime StartTime = Convert.ToDateTime(foo.PromoTime);
                    DateTime EndTime = Convert.ToDateTime(foo.EndTime);

                    if (time.Hour >= StartTime.Hour && time.Hour < EndTime.Hour)
                    {
                        if (Convert.ToBoolean(foo.isEnabled))
                        {
                            PromoList.Add(foo);
                        }
                    }
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Promos from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Promos from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if(conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return PromoList;
        }

        public static List<Races> RaceList(string tickerSide, DateTime time, bool DataOnly)
        {
            List<Races> ListOfRaces = new List<Races>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetRaces"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                //cmd.Parameters.Add(new SqlParameter("@Party", PartyName));

                SqlDataReader RaceInfo = cmd.ExecuteReader();

                while (RaceInfo.Read())
                {
                    Races foo = new Races
                    {
                        RaceID = RaceInfo["Race_ID"].ToString().Trim(),
                        RaceDesc = RaceInfo["Race_Description"].ToString().Trim(),
                        RaceDetail = RaceInfo["Race_Detail"].ToString().Trim(),
                        RaceSortOrder = RaceInfo["Race_OfficeSortOrder"].ToString().Trim(),
                        StateName = RaceInfo["State_Name"].ToString().Trim(),
                        StateAbbv = RaceInfo["State_Abbv"].ToString().Trim(),
                        PollClosingTime = RaceInfo["Race_PollClosingTime"].ToString().Trim(),
                        PollClosingDateTime = RaceInfo["Race_PollClosingTime_DateTime"].ToString().Trim(),
                        UsAPcall = RaceInfo["Use_AP_Race_Call"].ToString().Trim(),
                        StateID = RaceInfo["State_ID"].ToString().Trim(),
                        RaceOfficeCode = RaceInfo["Race_OfficeCode"].ToString().Trim(),
                        RaceElectionType = RaceInfo["Race_ElectionType"].ToString().Trim(),
                        RaceDistrict = RaceInfo["Race_District"].ToString().Trim(),
                        RaceHasData = RaceInfo["RaceHasData"].ToString().Trim()
                    };

                    if (foo.StateName != "UNITED STATES")
                    {
                        if (!DataOnly)
                        {
                            ListOfRaces.Add(foo);
                        }
                        else
                        {
                            if (tickerSide == "left")
                            {
                                if (RaceInfo["Race_OfficeCode"].ToString().Contains("S") || RaceInfo["Race_OfficeCode"].ToString().Contains("G"))
                                //if (RaceInfo["Race_ElectionType"].ToString().Contains("P"))
                                {
                                    //if (foo.RaceHasData == "1" && foo.RaceID != "1")
                                    if (Convert.ToBoolean(foo.RaceHasData))
                                    {
                                        ListOfRaces.Add(foo);
                                    }
                                }
                            }
                            else if (tickerSide == "right")
                            {
                                if (RaceInfo["Race_OfficeCode"].ToString().Contains("H"))
                                //if (RaceInfo["Race_ElectionType"].ToString().Contains("P"))
                                {
                                    //if (foo.RaceHasData == "1" && foo.RaceID != "1")
                                    if (Convert.ToBoolean(foo.RaceHasData) && foo.RaceID != "-1")
                                    {
                                        ListOfRaces.Add(foo);
                                    }
                                }
                            }
                            else if(tickerSide == "single")
                            {
                                if (Convert.ToBoolean(foo.RaceHasData) && foo.RaceID != " -1")
                                {
                                    
                                    ListOfRaces.Add(foo);
                                }
                            }
                        }
                    }
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Races from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Races from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return ListOfRaces;
        }

        public static List<RaceFacts> GetRaceFacts(string StateAbbv, int TickerType)
        {
            List<RaceFacts> SFacts = new List<RaceFacts>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetStateFacts"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@StateMnemonic", StateAbbv));
                cmd.Parameters.Add(new SqlParameter("@NetworkName", Election_Ticker_2020.AppInfo.ApplicationNetwork));
                cmd.Parameters.Add(new SqlParameter("@TickerType", TickerType));

                SqlDataReader StateFacts = cmd.ExecuteReader();

                while (StateFacts.Read())
                {
                    RaceFacts foo = new RaceFacts
                    {
                        Fact = StateFacts["FactText"].ToString().Trim(),
                        StartTime = StateFacts["StartTime"].ToString().Trim(),
                        EndTime = StateFacts["EndTime"].ToString().Trim(),
                        isEnabled = StateFacts["isEnabled"].ToString().Trim()
                    };

                    if (Convert.ToBoolean(foo.isEnabled))
                    {
                        SFacts.Add(foo);
                    }
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Race Facts from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Race Facts from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return SFacts;
        }

        public static List<Delegates> GetDelegates(string StateAbbv, string PartyName)
        {
            List<Delegates> StateDelegates = new List<Delegates>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetStateDelegates"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@StateMnemonic", StateAbbv));
                cmd.Parameters.Add(new SqlParameter("@Party", PartyName));

                SqlDataReader DelegateNumbers = cmd.ExecuteReader();

                while (DelegateNumbers.Read())
                {
                    Delegates foo = new Delegates
                    {
                        DelegatesAtStake = DelegateNumbers["DelegatesAtStake"].ToString().Trim()
                    };

                    StateDelegates.Add(foo);
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Delegates from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Delegates from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return StateDelegates;
        }

        public static List<PollClosings> GetClosingTime(string StateAbbv)
        {
            List<PollClosings> StateClosingTimes = new List<PollClosings>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetStateClosings"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@StateMnemonic", StateAbbv));

                SqlDataReader Times = cmd.ExecuteReader();

                while (Times.Read())
                {
                    PollClosings foo = new PollClosings
                    {
                        ClosingTime = Times["PollClosingTime"].ToString().Trim()
                    };
                    foo.ClosingTime = Convert.ToDateTime(foo.ClosingTime).ToString("HH:mm");
                    StateClosingTimes.Add(foo);
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Closing Times from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Closing Times from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return StateClosingTimes;
        }

        public static Queue<DateTime> PollClosingsQueue(bool ForSide, DateTime time)
        {
            Queue<DateTime> Times = new Queue<DateTime>();
            SqlCommand cmd;

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            conn.Open();

            cmd = new SqlCommand("getVDSPollClosingTimesDistinct", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlDataReader foo = cmd.ExecuteReader();

            while (foo.Read())
            {
                if (ForSide)
                {
                    /*
                    if (Convert.ToDateTime(foo[0]).ToString("HH:mm") == "19:00" || Convert.ToDateTime(foo[0]).ToString("HH:mm") == "20:00" || Convert.ToDateTime(foo[0]).ToString("HH:mm") == "20:30" || Convert.ToDateTime(foo[0]).ToString("HH:mm") == "22:00")
                    {
                        Times.Enqueue(Convert.ToDateTime(foo[0]));
                    }
                    */

                    if (time < Convert.ToDateTime(foo[0]))
                    {
                        Times.Enqueue(Convert.ToDateTime(foo[0]));
                    }
                }

                if (time < Convert.ToDateTime(foo[0]))
                {

                    if (!ForSide)
                    {
                        Times.Enqueue(Convert.ToDateTime(foo[0]));
                    }
                }
            }

            conn.Close();
            return Times;
        }

        public static List<ECVotes> GetECVotes()
        {
            List<ECVotes> EList = new List<ECVotes>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM VDS_ElectionStateInfo", conn)
            {
                CommandType = CommandType.Text
            };

            SqlDataReader foo = cmd.ExecuteReader();

            while (foo.Read())
            {
                if(foo["State_Abbv"].ToString().Trim() != "US")
                {
                    ECVotes foo2 = new ECVotes
                    {
                        Abbv = foo["State_Abbv"].ToString().Trim(),
                        EVotes = foo["State_EC_Votes_Available"].ToString().Trim()
                    };

                    EList.Add(foo2);
                }
            }

            conn.Close();

            return EList;
        }

        public static string getElectoralVotes(string st)
        {
            string eVote = "";

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM VDS_ElectionStateInfo WHERE State_Abbv='" + st + "'", conn)
            {
                CommandType = CommandType.Text
            };

            SqlDataReader foo = cmd.ExecuteReader();

            while (foo.Read())
            {
                if (foo["State_Abbv"].ToString().Trim() != "US")
                {
                    eVote = foo["State_EC_Votes_Available"].ToString().Trim();
                }
            }

            conn.Close();

            return eVote;
        }

        public static List<RaceData> GetRaceData(string st, string ofc, string jCde, string eType, string Party)
        {
            List<RaceData> RaceD = new List<RaceData>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetRaceDataByState"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if(eType == "D")
                {
                    Party = "Dem";
                }
                else
                {
                    Party = "Rep";
                }

                cmd.Parameters.Add(new SqlParameter("@st", st));
                cmd.Parameters.Add(new SqlParameter("@ofc", ofc));
                cmd.Parameters.Add(new SqlParameter("@jCde", jCde));
                cmd.Parameters.Add(new SqlParameter("@eType", eType));
                //cmd.Parameters.Add(new SqlParameter("@eType", "G"));
                //cmd.Parameters.Add(new SqlParameter("@Party", Party));
                cmd.Parameters.Add(new SqlParameter("@primaryApplicationCode", "T"));

                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    //string fID = data["FoxID"].ToString().Trim() == "" ? "USGOV999999" : data["FoxID"].ToString().Trim();
                    string fID = data["cID"].ToString().Trim() == "" ? "USGOV999999" : data["cID"].ToString().Trim();

                    RaceData foo = new RaceData
                    {
                        OfficeName = data["ofcName"].ToString().Trim(),
                        Jname = data["jName"].ToString().Trim(),
                        StateAbbv = data["stateAbbv"].ToString().Trim(),
                        CountyName = data["cntyName"].ToString().Trim(),
                        Jcode = data["jCde"].ToString().Trim(),
                        TotPcts = data["totPcts"].ToString().Trim(),
                        PctsReporting = data["pctsRep"].ToString().Trim(),
                        PctExpVote = data["pctExpVote"].ToString().Trim(),
                        Cid = data["cID"].ToString().Trim(),
                        FoxID = fID,
                        CandLastName = data["candLastName"].ToString().Trim(),
                        MajorPtyId = data["majorPtyId"].ToString().Trim(),
                        Cstat = data["cStat"].ToString().Trim(),
                        EdListFlg = data["edListFlg"].ToString().Trim(),
                        InIncPtyFlg = data["inIncPtyFlg"].ToString().Trim(),
                        IsIncFlg = data["isIncFlg"].ToString().Trim(),
                        OutputOrder = data["outputOrder"].ToString().Trim(),
                        Cvote = data["cVote"].ToString().Trim(),
                        Avote = data["aVote"].ToString().Trim(),
                        VoteSum = data["voteSum"].ToString().Trim(),
                        RaceWinnerCalled = data["Race_WinnerCalled"].ToString().Trim(),
                        RaceWinnderCalledTime = data["Race_WinnerCallTime"].ToString().Trim(),
                        EstTS = data["estTS"].ToString().Trim(),
                        RaceTooCloseToCall = data["Race_TooCloseToCall"].ToString().Trim(),
                        RaceWinnerCanID = data["Race_WinnerCandidateID"].ToString().Trim(),
                        RacePollClosingTime = data["Race_PollClosingTime"].ToString().Trim(),
                        UseAPraceCall = data["Use_AP_Race_Call"].ToString().Trim(),
                        IgnoreGain = data["IgnoreGain"].ToString().Trim(),
                        headShotPath = data["HeadshotPath"].ToString().Trim()
                    };
                   
                    RaceD.Add(foo);
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Race Data from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Race Data from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return RaceD;
        }

        public static Flags CheckFlags()
        {
            Flags flag = new Flags();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                //Console.WriteLine(config.AppSettings.Settings["ApplicationID"].Value.ToString());

                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM " + config.AppSettings.Settings["flagsTable"].Value, conn)
                {
                    CommandType = CommandType.Text
                };

                SqlDataReader flagsData = cmd.ExecuteReader();

                while (flagsData.Read())
                {
                    flag.LockoutEnabled = Convert.ToBoolean(flagsData["PollClosingLockoutEnable"].ToString().Trim());
                    flag.ZeroPrecinctsLockoutEnabled = Convert.ToBoolean(flagsData["ZeroPrecinctsLockoutEnable"].ToString().Trim());
                    flag.UseExpectedVoteIn = Convert.ToBoolean(flagsData["UseExpectedVoteIn"].ToString().Trim());
                    flag.UseSimTime = Convert.ToBoolean(flagsData["UseSimulatedElectionDayTime"].ToString().Trim());
                    flag.UseTenthPercent = Convert.ToBoolean(flagsData["UseTenthsOfPercent"].ToString().Trim());
                    flag.usePollCloseBug = Convert.ToBoolean(flagsData["usePollBug"].ToString().Trim());
                    flag.bugInsertPollTime = Convert.ToInt32(flagsData["pollBugMinsB4"].ToString().Trim());
                    flag.bugRemovePollTime = Convert.ToInt32(flagsData["pollBugRunMins"].ToString().Trim());
                }


                conn.Close();

                conn.Open();

                cmd = new SqlCommand("SELECT * FROM BMT_Ticker_Config WHERE AppIndex = " + config.AppSettings.Settings["ApplicationID"].Value, conn)
                {
                    CommandType = CommandType.Text
                };

                SqlDataReader tickerData = cmd.ExecuteReader();

                while(tickerData.Read())
                {
                    flag.tickerDwellTime = (Convert.ToInt32(tickerData["TickerDwellTime"].ToString().Trim()) * 1000);
                    flag.tickerCanNum = Convert.ToInt32(tickerData["TickerShowCanNum"].ToString().Trim());
                    flag.sceneName = tickerData["ScenePath"].ToString().Trim();

                    if (Convert.ToBoolean(tickerData["ShowHeadshots"].ToString().Trim()))
                    {
                        flag.showHeadshots = "1";
                    }
                    else
                    {
                        flag.showHeadshots = "0";
                    }
                }


                conn.Close();
            }
            catch(Exception ex)
            {
                log.Debug("\n\nError Getting Flags from DB\n\n" + ex + "\n");
                log.Error("\n\nError Getting Flags from DB\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return flag;
        }

        public static DateTime GetSimulatedTime()
        {
            string returnedDate = "";
            string returnedYear = "";
            string returnedMonth = "";
            string returnedDay = "";
            string returnedHour = "";
            string returnedMinute = "";
            string returnedSecond = "";

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP (1) value FROM " + config.AppSettings.Settings["simulatedTimeTable"].Value, conn)

                {
                    CommandType = CommandType.Text
                };

                SqlDataReader timeData = cmd.ExecuteReader();

                while (timeData.Read())
                {
                    returnedDate = timeData["value"].ToString().Trim();
                }

                conn.Close();

                returnedYear = returnedDate.Substring(0, 4);
                returnedMonth = returnedDate.Substring(4, 2);
                returnedDay = returnedDate.Substring(6, 2);
                returnedHour = returnedDate.Substring(8, 2);
                returnedMinute = returnedDate.Substring(10, 2);
                returnedSecond = returnedDate.Substring(12, 2);


            }
            catch (Exception ex)
            {
                log.Debug("\n\nERROR GETTING SIMULATED TIME\n\n" + ex + "\n");
                log.Debug("\n\nERROR GETTING SIMULATED TIME\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                conn.Close();
            }

            return Convert.ToDateTime(returnedMonth + "/" + returnedDay + "/" + returnedYear + " " + returnedHour + ":" + returnedMinute + ":" + returnedSecond);
        }

        public static void Post2ApplicationLog(string application_Name,
                            string application_Description,
                            string hostPC_Name,
                            string hostPC_IP_Address,
                            Boolean engine_Enabled_1,
                            string engine_IP_Address_1,
                            Boolean engine_Enabled_2,
                            string engine_IP_Address_2,
                            string entry_Text,
                            string application_Version,
                            int application_ID,
                            string comments,
                            DateTime currentSystemTime)
        {
            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                
                // Instantiate the connection
                conn.Open();

                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        SqlTransaction transaction;
                        // Start a local transaction.
                        transaction = conn.BeginTransaction("Post Application Log Entry");

                        // Must assign both transaction object and connection 
                        // to Command object for a pending local transaction
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;

                        try
                        {
                            //Specify base command
                            cmd.CommandText = "setFGEApplicationLogEntry " +
                                                                    "@Application_Name, " +
                                                                    "@Application_Description, " +
                                                                    "@HostPC_Name, " +
                                                                    "@HostPC_IP_Address, " +
                                                                    "@Engine_Enabled_1, " +
                                                                    "@Engine_IP_Address_1, " +
                                                                    "@Engine_Enabled_2, " +
                                                                    "@Engine_IP_Address_2, " +
                                                                    "@Entry_Text, " +
                                                                    "@Application_Version, " +
                                                                    "@Application_ID, " +
                                                                    "@Comments, " +
                                                                    "@CurrentSystemTime";

                            //Set parameters
                            cmd.Parameters.Add("@Application_Name", SqlDbType.NVarChar).Value = application_Name;
                            cmd.Parameters.Add("@Application_Description", SqlDbType.NVarChar).Value = application_Description;
                            cmd.Parameters.Add("@HostPC_Name", SqlDbType.NVarChar).Value = hostPC_Name;
                            cmd.Parameters.Add("@HostPC_IP_Address", SqlDbType.NVarChar).Value = hostPC_IP_Address;
                            cmd.Parameters.Add("@Engine_Enabled_1", SqlDbType.Bit).Value = engine_Enabled_1;
                            cmd.Parameters.Add("Engine_IP_Address_1", SqlDbType.NVarChar).Value = engine_IP_Address_2;
                            cmd.Parameters.Add("@Engine_Enabled_2", SqlDbType.Bit).Value = engine_Enabled_1;
                            cmd.Parameters.Add("Engine_IP_Address_2", SqlDbType.NVarChar).Value = engine_IP_Address_2;
                            cmd.Parameters.Add("Entry_Text", SqlDbType.NVarChar).Value = entry_Text;
                            cmd.Parameters.Add("@Application_Version", SqlDbType.NVarChar).Value = application_Version;
                            cmd.Parameters.Add("@Application_ID", SqlDbType.Int).Value = application_ID;
                            cmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = comments;
                            cmd.Parameters.Add("@CurrentSystemTime", SqlDbType.DateTime).Value = currentSystemTime;

                            sqlDataAdapter.SelectCommand = cmd;
                            sqlDataAdapter.SelectCommand.Connection = conn;
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;

                            // Execute stored proc to store top-level metadata
                            sqlDataAdapter.SelectCommand.ExecuteNonQuery();

                            //Attempt to commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("\n\nPost2ApplicationLog while posting application log entry\n\n" + ex + "\n");
                log.Debug("\n\nPost2ApplicationLog while posting application log entry\n\n" + ex + "\n");

                Election_Ticker_2020.ErrorFlag = true;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        public static List<VoterAnalysisData> VoterAnalysisData()
        {
            //Gets all the voter analysis data
            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);
            SqlConnection conn2 = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString2);

            List<string> stackData1 = new List<string>();

            List<VoterAnalysisData> finalVAdata = new List<VoterAnalysisData>();

            try
            {
                conn2.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM " + config.AppSettings.Settings["stackElementTable"].Value + " WHERE fkey_StackID = " + config.AppSettings.Settings["TickerStackID"].Value, conn2);

                cmd.CommandType = CommandType.Text;

                SqlDataReader stackDataVA1 = cmd.ExecuteReader();

                //Creates a list of all the ticker stacks
                while (stackDataVA1.Read())
                {
                    stackData1.Add(stackDataVA1[config.AppSettings.Settings["stackElementTableC1"].Value].ToString() + "|" + stackDataVA1[config.AppSettings.Settings["stackElementTableC2"].Value].ToString());
                }
                conn2.Close();

                //Loops through each stack
                foreach (string a in stackData1)
                {
                    conn.Open();

                    SqlCommand cmd2 = new SqlCommand(config.AppSettings.Settings["GetVoterAnalysisData"].Value, conn);

                    cmd2.CommandType = CommandType.StoredProcedure;
                    //Console.WriteLine(a.Split((char)124)[0] + " | " + a.Split((char)124)[1]);

                    cmd2.Parameters.Add(new SqlParameter("@VA_Data_Id", a.Split((char)124)[0]));
                    cmd2.Parameters.Add(new SqlParameter("@r_type", a.Split((char)124)[1]));

                    SqlDataReader questionData = cmd2.ExecuteReader();

                    while (questionData.Read())
                    {
                        VoterAnalysisData foo = new VoterAnalysisData
                        {
                            VA_Data_Id = questionData["VA_Data_Id"].ToString().Trim(),
                            r_type = questionData["r_type"].ToString().Trim(),
                            StateName = questionData["State"].ToString().Trim(),
                            VA_Title = questionData["Title"].ToString().Trim(),
                            Response = questionData["Response"].ToString().Trim(),
                            VA_Percent = questionData["Percent"].ToString().Trim(),
                            StateAbbv = questionData["st"].ToString().Trim()
                        };
                        finalVAdata.Add(foo);
                        //Console.WriteLine(foo.Response);
                    }

                    conn.Close();                    
                }
            }
            catch (Exception ex)
            {
                //Closes whatever connection is currently open when the error occured
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                if (conn2.State == ConnectionState.Open)
                {
                    conn2.Close();
                }

                log.Debug("ERROR GETTING VOTER ANALYSIS: " + ex);
                log.Debug("ERROR GETTING VOTER ANALYSIS: " + ex);
            }

            return finalVAdata;
        }

        public static List<DelegateCounts> GetDelegateCounts(string PartyName)
        {
            List<DelegateCounts> Counts = new List<DelegateCounts>();

            SqlConnection conn3 = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString3);

            try
            {
                conn3.Open();

                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetDelegateCounts"].Value, conn3)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@Party", PartyName));

                SqlDataReader CandidateTotals = cmd.ExecuteReader();

                while(CandidateTotals.Read())
                {
                    DelegateCounts foo = new DelegateCounts
                    {
                        CandidateLastName = CandidateTotals["CandidateLastName"].ToString().Trim(),
                        CandidateCount = CandidateTotals["CandidateDelegates"].ToString().Trim()
                    };

                    if(foo.CandidateLastName == "BIDEN" || foo.CandidateLastName == "SANDERS")
                    {
                        Counts.Add(foo);
                    }
                }
            }
            catch(Exception ex)
            {
                //Closes whatever connection is currently open when the error occured
                if (conn3.State == ConnectionState.Open)
                {
                    conn3.Close();
                }

                log.Debug("ERROR GETTING DELEGATE COUNTS: " + ex);
                log.Debug("ERROR GETTING DELEGATE COUNTS: " + ex);
            }

            return Counts;
        }

        public static string getDelegate(string rtype, string st)
        {
            string delcount = "0";
            string cName = "";

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                if (rtype == "D")
                {
                    cName = "DelegatesDem";
                }
                else
                {
                    cName = "DelegatesRep";
                }
                conn.Open();

                SqlCommand cmd = new SqlCommand($"SELECT {cName} FROM VDS_DelegatesByState WHERE StateAbbv='{st.Trim()}'", conn);

                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    delcount = data[0].ToString();
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return delcount;
        }

        public static List<int> GetHouseStates()
        {
            //Gets all the house states from the race list
            List<int> houseStateCodes = new List<int>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetRaces"].Value, conn);
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader houseCodes = cmd.ExecuteReader();

                while (houseCodes.Read())
                {
                    if (houseCodes["Race_OfficeCode"].ToString() == "H")
                    {
                        houseStateCodes.Add(Convert.ToInt32(houseCodes["State_ID"].ToString()));
                    }
                }

                conn.Close();
            }
            catch (Exception e)
            {
                log.Debug("ERROR GETTING HOUSESTATES: " + e);
                log.Debug("ERROR GETTING HOUSESTATES: " + e);

                conn.Close();
            }

            return houseStateCodes;
        }

        public static List<HouseDistricts> GetHouseDistricts()
        {
            //Gets all the house districts for each state
            List<HouseDistricts> houseDistricts = new List<HouseDistricts>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            {
                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetRaces"].Value, conn);
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader houseCodes = cmd.ExecuteReader();

                while (houseCodes.Read())
                {
                    if (houseCodes["Race_OfficeCode"].ToString() == "H")
                    {
                        HouseDistricts foo = new HouseDistricts
                        {
                            StateID = houseCodes["State_ID"].ToString(),
                            HouseDistrict = houseCodes["Race_District"].ToString()
                        };

                        houseDistricts.Add(foo);
                    }
                }

                conn.Close();
            }
            catch (Exception e)
            {
                log.Debug("ERROR GETTING HOUSEDISTRICTS: " + e);
                log.Debug("ERROR GETTING HOUSEDISTRICTS: " + e);

                conn.Close();
            }

            return houseDistricts;
        }

        public static List<int> GetRacesByOfficeCode(string ofeCde)
        {
            //Gets all the races for the specified office code
            
            List<int> stCode = new List<int>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            try
            { 
                SqlCommand cmd = new SqlCommand(config.AppSettings.Settings["GetRaces"].Value, conn);
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader raceCodes = cmd.ExecuteReader();

                while (raceCodes.Read())
                {
                    if (raceCodes["Race_OfficeCode"].ToString().Contains(ofeCde))
                    {
                        stCode.Add(Convert.ToInt32(raceCodes["State_ID"].ToString()));
                    }
                }

                conn.Close();
            }
            catch (Exception e)
            {
                log.Debug("ERROR GETING RACE DATA(GetRacesByOfficeCode) FOR " + ofeCde + ": " + e);
                log.Error("ERROR GETING RACE DATA(GetRacesByOfficeCode) FOR " + ofeCde + ": " + e);

                conn.Close();
            }
            return stCode;
        }

        public static Queue<SidePanelData> GetSidePanelData(int type, int network)
        {
            Queue<SidePanelData> foo = new Queue<SidePanelData>();

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            SqlCommand cmd = new SqlCommand($"SELECT * FROM {config.AppSettings.Settings["SidePanelTable"].Value} WHERE PanelType={type} AND Enabled=1 AND Network={network}", conn);
            conn.Open();
            cmd.CommandType = CommandType.Text;

            SqlDataReader Sdata = cmd.ExecuteReader();

            while(Sdata.Read())
            {
                SidePanelData foo1 = new SidePanelData
                {
                    ImagePath = Sdata["ImagePath"].ToString(),
                    PanelType = Convert.ToInt32(Sdata["PanelType"].ToString()),
                    Network = Convert.ToInt32(Sdata["Network"].ToString()),
                    PanelFlag = Convert.ToInt32(Sdata["Flag"].ToString()),
                    IsEnabled = Convert.ToBoolean(Sdata["Enabled"].ToString())
                };

                foo.Enqueue(foo1);
            }

            conn.Close();

            return foo;
        }

        public static string GetRightSidedata(int cde, int type, DateTime DT)
        {
            string Sproc = "";
            string data = "";
            string ClName = "";

            int rwCount = 0;
            int tempCount = 0;
            string GainText = "";
            string PartyWinner = "";

            int CurrentDemCount = 0;
            int CurrentRepCount = 0;
            int CurrentIndCount = 0;

            SqlCommand cmd;

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            if (cde == 0 || cde == 3)
            {
                string BopType = type == 0 ? "S" : "H";

                conn.Open();
                cmd = new SqlCommand(config.AppSettings.Settings["GetNetGainText"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@OfficeCode", BopType));

                SqlDataReader NetGain = cmd.ExecuteReader();
                
                while (NetGain.Read())
                {
                    GainText = NetGain["ControlString"].ToString();
                    PartyWinner = NetGain["PresidentWinningPartyValue"].ToString();
                }
                conn.Close();


                //===========
                conn.Open();
                cmd = new SqlCommand(config.AppSettings.Settings["GetBop"].Value, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@type", BopType));
                cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", DT.ToString()));
                cmd.Parameters.Add(new SqlParameter("@atStake", "1"));

                SqlDataReader CurrentNumbers = cmd.ExecuteReader();

                while (CurrentNumbers.Read())
                {
                    CurrentDemCount = Convert.ToInt32(CurrentNumbers["DEM_COUNT"].ToString());
                    CurrentRepCount = Convert.ToInt32(CurrentNumbers["GOP_COUNT"].ToString());
                    CurrentIndCount = Convert.ToInt32(CurrentNumbers["IND_COUNT"].ToString());
                }
                conn.Close();
                //==================
                string[] temp = GainText.Split(' ');

                GainText = temp[0] + Environment.NewLine + temp[1] + " " + temp[2];


                Sproc = config.AppSettings.Settings["GetBop"].Value;
                rwCount = 0;

                cmd = new SqlCommand(Sproc, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@type", BopType));
                cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", DT.ToString()));

                if(cde == 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@atStake", "1"));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@atStake", "0"));
                }
                
            }
            else if(cde == 1)
            {
                Sproc = config.AppSettings.Settings["GetRaceDataByState"].Value;
                rwCount = 1;
                ClName = "cVote";

                cmd = new SqlCommand(Sproc, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@st", "0"));
                cmd.Parameters.Add(new SqlParameter("@ofc", "P"));
                cmd.Parameters.Add(new SqlParameter("@jCde", "0"));
                cmd.Parameters.Add(new SqlParameter("@etype", "G"));
            }
            else
            {
                Sproc = config.AppSettings.Settings["GetElectVotes"].Value;
                rwCount = 1;
                ClName = "ECVotes";

                cmd = new SqlCommand(Sproc, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@currentDT", DT.ToString()));
                cmd.Parameters.Add(new SqlParameter("@rollupNonMajorPartyCandidates", "0"));
            }

            conn.Open();

            SqlDataReader DataString = cmd.ExecuteReader();

            while(DataString.Read())
            {
                if(cde == 0 || cde == 3)
                {
                    if (cde == 0)
                    {
                        data = $"{DataString["DEM_COUNT"].ToString()}^{DataString["GOP_COUNT"].ToString()}^{DataString["IND_COUNT"].ToString()}";
                    }
                    else
                    {
                        int DemDelta = Convert.ToInt32(DataString["DEM_DELTA"].ToString());
                        int RepDelta = Convert.ToInt32(DataString["GOP_DELTA"].ToString());
                        int IndDelta = Convert.ToInt32(DataString["IND_DELTA"].ToString());
                        int DemCount = CurrentDemCount;
                        int RepCount = CurrentRepCount;
                        //Calc need here

                        if (type == 0)
                        {
                            if(DemCount == 50 && RepCount == 50 && PartyWinner != "0")
                            {
                                PartyWinner = "1";
                            }
                            else if(DemCount >= 51 || RepCount >= 51)
                            {
                                PartyWinner = "1";
                            }
                            else
                            {
                                PartyWinner = "0";
                            }
                        }
                        else
                        {
                            if(DemCount >= 218 || RepCount >= 218)
                            {
                                PartyWinner = "1";
                            }
                            else
                            {
                                PartyWinner = "0";
                            }
                        }

                        if(DemDelta == 0 && RepDelta == 0)
                        {
                            if(type == 0)
                            {
                                data = $"0^{RepDelta}^{GainText}^{PartyWinner}";
                            }
                            else
                            {
                                data = $"1^{DemDelta}^{GainText}^{PartyWinner}";
                            }
                        }
                        else if (DemDelta > RepDelta && DemDelta > IndDelta)
                        {
                            data = $"1^{DemDelta}^{GainText}^{PartyWinner}";
                        }
                        else if(RepDelta > DemDelta && RepDelta > IndDelta)
                        {
                            data = $"0^{RepDelta}^{GainText}^{PartyWinner}";
                        }
                        else if(IndDelta > DemDelta & IndDelta > RepDelta)
                        {
                            data = $"2^{RepDelta}^{GainText}^{PartyWinner}";
                        }
                    }
                }
                else
                {
                    data += DataString[ClName].ToString() + "^";
                }

                if(rwCount == tempCount)
                {
                    break;
                }
                else
                {
                    tempCount++;
                }
            }

            conn.Close();
            
            data = cde == 1 || cde == 2 ? data.TrimEnd('^') : data;

            return data;
        }

        public static string GetNextPollTime(string PollTime)
        {
            SqlCommand cmd;
            string NextTime = "";

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            conn.Open();

            cmd = new SqlCommand(config.AppSettings.Settings["GetNextPollTime"].Value, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@UseDate", 1));
            cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", PollTime));

            SqlDataReader NextPTime = cmd.ExecuteReader();

            while (NextPTime.Read())
            {
                NextTime = NextPTime[0].ToString().Trim();
            }

            conn.Close();

            return NextTime;
        }

        public static Queue<string> GetPollStates(string PollTime)
        {
            Queue<string> states = new Queue<string>();
            SqlCommand cmd;

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            conn.Open();

            cmd = new SqlCommand(config.AppSettings.Settings["GetPollByTime"].Value, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@PollClosingTime", PollTime));

            SqlDataReader Times = cmd.ExecuteReader();

            while (Times.Read())
            {
                states.Enqueue(Times["State_Name"].ToString().Trim());
            }

            conn.Close();
            return states;
        }

        public static string getTotalEV(string dt)
        {
            SqlDataReader data;
            SqlCommand cmd;

            SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

            evData foo = new evData();

            conn.Open();

            cmd = new SqlCommand("getFGEElectoralCollegeVotes", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@currentDT", dt));
            cmd.Parameters.Add(new SqlParameter("@rollupNonMajorPartyCandidates", "0"));

            data = cmd.ExecuteReader();

            while (data.Read())
            {
                if (data["PartyID"].ToString().ToLower() == "dem")
                {
                    foo.demCanName = data["LastName"].ToString().Trim();
                    foo.ecVoteDem = data["ECVotes"].ToString().Trim();
                }
                else if (data["PartyID"].ToString().ToLower() == "rep")
                {
                    foo.repCanName = data["LastName"].ToString().Trim();
                    foo.ecVoteRep = data["ECVotes"].ToString().Trim();
                }
            }

            /*
            evData foo = new evData()
            {
                demCanName = "xxx",
                ecVoteDem = "55",
                repCanName = "sss",
                ecVoteRep = "33"
            };
            */

            electVotes a = new electVotes()
            {
                electVotesData = foo
            };

            conn.Close();

            return JsonConvert.SerializeObject(a, Formatting.Indented);
        }

        public static string BopString(string dt)
        {
            try
            {
                SqlDataReader data;
                SqlCommand cmd;

                SqlConnection conn = new SqlConnection(Election_Ticker_2020.AppInfo.ConnectionString1);

                CurrentBOP currentbop = new CurrentBOP();
                NewBOP newbop = new NewBOP();

                conn.Open();

                cmd = new SqlCommand("getVDSBalanceOfPowerAuto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@type", "H"));
                cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", dt));
                cmd.Parameters.Add(new SqlParameter("@atStake", "0"));

                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    currentbop.house = new House()
                    {
                        repNum = Convert.ToInt32(data["GOP_COUNT"].ToString()),
                        demNum = Convert.ToInt32(data["DEM_COUNT"].ToString()),
                        indNum = Convert.ToInt32(data["IND_COUNT"].ToString())
                    };
                }

                conn.Close();

                conn.Open();

                cmd = new SqlCommand("getVDSBalanceOfPowerAuto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@type", "S"));
                cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", dt));
                cmd.Parameters.Add(new SqlParameter("@atStake", "0"));

                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    currentbop.senate = new Senate()
                    {
                        repNum = Convert.ToInt32(data["GOP_COUNT"].ToString()),
                        demNum = Convert.ToInt32(data["DEM_COUNT"].ToString()),
                        indNum = Convert.ToInt32(data["IND_COUNT"].ToString())
                    };
                }

                conn.Close();

                //GET THE LIVE BOP DATA

                conn.Open();

                cmd = new SqlCommand("getVDSBalanceOfPowerAuto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@type", "H"));
                cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", dt));
                cmd.Parameters.Add(new SqlParameter("@atStake", "1"));

                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    newbop.houseNum = new HouseNum()
                    {
                        repNum = Convert.ToInt32(data["GOP_COUNT"].ToString()),
                        demNum = Convert.ToInt32(data["DEM_COUNT"].ToString()),
                        indNum = Convert.ToInt32(data["IND_COUNT"].ToString()),
                    };

                    newbop.houseChng = new HouseChng()
                    {
                        repNetChange = Convert.ToInt32(data["GOP_DELTA"].ToString()),
                        demNetChange = Convert.ToInt32(data["DEM_DELTA"].ToString()),
                        indNetChange = Convert.ToInt32(data["IND_DELTA"].ToString())
                    };
                }

                conn.Close();

                conn.Open();

                cmd = new SqlCommand("getVDSBalanceOfPowerAuto", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@type", "S"));
                cmd.Parameters.Add(new SqlParameter("@CurrentDateTime", dt));
                cmd.Parameters.Add(new SqlParameter("@atStake", "1"));

                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    newbop.senateNum = new SenateNum()
                    {
                        repNum = Convert.ToInt32(data["GOP_COUNT"].ToString()),
                        demNum = Convert.ToInt32(data["DEM_COUNT"].ToString()),
                        indNum = Convert.ToInt32(data["IND_COUNT"].ToString()),
                    };
                    newbop.senateChng = new SenateChng()
                    {
                        repNetChange = Convert.ToInt32(data["GOP_DELTA"].ToString()),
                        demNetChange = Convert.ToInt32(data["DEM_DELTA"].ToString()),
                        indNetChange = Convert.ToInt32(data["IND_DELTA"].ToString())
                    };

                }

                conn.Close();

                Root root = new Root()
                {
                    currentBOP = currentbop,
                    newBOP = newbop
                };

                string finalBopString = JsonConvert.SerializeObject(root,
                                Newtonsoft.Json.Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                //Console.WriteLine(finalBopString);
                return finalBopString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error getting bop data");
                return "";
            }

        }
    }
}
