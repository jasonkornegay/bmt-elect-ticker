using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO.Ports;
using System.Configuration;
using System.Net;using log4net;

namespace _2020_Primaries_Ticker
{

    public class VizCommands
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool SendVizMessage(string shmKey, string value, string method, int SceneType)
        {
            //Console.WriteLine($"{shmKey}|{value}|{method}");
            bool isSuccessful = false;

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            byte[] msg = null;

            try
            {
                byte[] bytes = new byte[4096];

                if (method == "load")
                {
                    msg = Encoding.UTF8.GetBytes($"0 RENDERER*FRONT_LAYER SET_OBJECT SCENE*{value}");
                }
                else if(method == "unload")
                {
                    msg = Encoding.UTF8.GetBytes($"0 RENDERER*FRONT_LAYER SET_OBJECT ");
                }
                else if(method == "sendData")
                {
                    msg = Encoding.UTF8.GetBytes($"send FRONT_SCENE*MAP SET_STRING_ELEMENT \"{shmKey}\" {value}");
                    //msg = Encoding.UTF8.GetBytes($"{shmKey}|{value}");
                }
                else if(method == "raceData")
                {
                    //msg = Encoding.UTF8.GetBytes($"send VIZ_COMMUNICATION*MAP SET_STRING_ELEMENT \"{shmKey}\" {value}");
                    msg = Encoding.UTF8.GetBytes($"{shmKey}|{value}\0");
                }
                else if(method == "headshots")
                {
                    msg = Encoding.UTF8.GetBytes($"send FRONT_SCENE*MAP SET_STRING_ELEMENT \"HEADSHOT_MODES\" {value}");
                }
                else if(method == "forBA")
                {
                    msg = Encoding.UTF8.GetBytes($"send FRONT_SCENE*TREE*$FOX_TICKER_CONTROLLER*FUNCTION*{shmKey}*updateData SET {value}");
                }
                string ip = SceneType == 0 ? Election_Ticker_2020.AppInfo.EngineIP : Election_Ticker_2020.AppInfo.EngineIP2;
                int port = SceneType == 0 ? Convert.ToInt32(Election_Ticker_2020.AppInfo.EnginePort) : Convert.ToInt32(Election_Ticker_2020.AppInfo.EnginePort2);

                if (method == "raceData")
                {
                    port = Convert.ToInt32(Election_Ticker_2020.AppInfo.commPort);
                }

                if (ip != "")
                {
                    s.Connect(ip, port);

                    s.Send(msg, 0, msg.Length, SocketFlags.None);

                    if (method != "raceData")
                    {
                        s.Disconnect(true);
                    }
                    else
                    {
                        s.Disconnect(false);
                    }

                    isSuccessful = true;
                }

            }
            catch(Exception ex)
            {
                log.Debug("\n\nSOCKET PROBLEM(" + method + ")\n\n" + ex + "\n");
                log.Error("\n\nSOCKET PROBLEM(" + method + ")\n\n" + ex + "\n");

                isSuccessful = false;

                Election_Ticker_2020.ErrorFlag = true;

                if (s.Connected)
                {
                    s.Disconnect(true);
                }
            }

            return isSuccessful;

        }
    }
}
