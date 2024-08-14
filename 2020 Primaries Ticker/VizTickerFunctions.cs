using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace _2020_Primaries_Ticker
{

    class VizTickerFunctions
    {
        public static string crlf = "\r\n";
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        
        public void InitTicker(string carouselName, string seqStr)
        {
            //Add the specified ticker (carousel) to the service
            string cmd = $"{seqStr} AddTicker " + carouselName;
            StringToVizTickerService(cmd);

        }
        public void InitProtocol(string seqStr)
        {
            //Add the specified ticker (carousel) to the service
            string cmd = $"{seqStr} protocol";
            StringToVizTickerService(cmd);
        }
        public void InitScene()
        {
            //Add the specified ticker (carousel) to the service
            string cmd = "initScene=1";
            VizDataPoolCmd(cmd);
        }

        public void ClearTickerEntries(string carouselName, string seqStr)
        {
            //Add the specified ticker (carousel) to the service
            string cmd = $"{seqStr} on_ticker {carouselName} ClearAll";
            StringToVizTickerService(cmd);
        }
        public void ClearFlush(string carouselName, string seqStr)
        {
            //Add the specified ticker (carousel) to the service

            string cmd = $"{seqStr} on_ticker {carouselName} Flush";
            StringToVizTickerService(cmd);

        }


       
        public void VizLoadSceneRenderer(string sceneName)
        {
            StringToViz("0 RENDERER*MAIN_LAYER SET_OBJECT SCENE*" + sceneName);
        }

        public void VizSetCamera()
        {
            StringToViz("0 RENDERER SET_CAMERA 1");
        }

        public void StringToViz(string s)
        {
            s += "\0";
            string st = ">> to cPort 7478: " + s;
            //Form1.Globals.VizMessages.Add(st);

            //Form1.VizControlSendCommand(s, 7478);
            //Form1.VizControlSendCommand(s, 6100);
        }
        public void StringToViz2(string shmKey, string value, string method, int SceneType)
        {
            //string s = $"{shmKey}|{value}\0";
            string s = $"SMMSystem_SetValue|{shmKey}|{value}\0";

            //string st = ">> to cPort 6100: " + s;
            //Form1.Globals.VizMessages.Add(st);
            Election_Ticker_2020.VizControlSendCommand2(s, Convert.ToInt32(Election_Ticker_2020.AppInfo.commPort));
            //Form1.VizControlSendCommand(s, 6100);

            File.AppendAllText("c:\\logs\\FWX_DATA_" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", $"[{DateTime.Now}] " +  s  + Environment.NewLine);
        }
        public void StringToVizTickerService(string s)
        {
            s += Environment.NewLine;
            string st = ">> to tsPort 6301: " + s;
            //Form1.Globals.VizMessages.Add(st);
            
            //Form1.VizTickerSendCommand(s, 6301);
        }

        public void StringToTDF(string s)
        {
            s += Environment.NewLine;
            string st = $">> to tsPort 7750: " + s;
            //Form1.Globals.VizMessages.Add(st);
            //Form1.FinanacialSendCommand(s, 7750);
        }
        public void VizDataPoolCmd(string cmd)
        {
            StringToViz("0 RENDERER*FUNCTION*DataPool*Data SET " + cmd);
        }

        public void DeleteSpecifiedEntry(string carousel, int index, string seqStr)
        {
            string cmd = $"{seqStr} on_ticker {carousel} DeleteGroupWhenInactive {index}";
            StringToVizTickerService(cmd);
        }

        public void DeleteTicker(string carousel, string seqStr)
        {
            string cmd = $"{seqStr} DeleteTicker {carousel}";
            StringToVizTickerService(cmd);
        }

        public void ListTickers(string seqStr)
        {
            string cmd = $"{seqStr} ListTickers";
            StringToVizTickerService(cmd);
        }

        public void LoopMode(string carousel, bool loop, string seqStr)
        {
            string loopMode = "no";
            if (loop)
                loopMode = "yes";

            string cmd = $"{seqStr} on_ticker {carousel} set_loop {loopMode}";
            StringToVizTickerService(cmd);
        }

        public void PauseTicker(string carousel, string seqStr)
        {
            string cmd = $"{seqStr} on_ticker {carousel} pause_ticker";
            StringToVizTickerService(cmd);
        }

        public void ContinueTicker(string carousel, string seqStr)
        {
            string cmd = $"{seqStr} on_ticker {carousel} continue_ticker";
            StringToVizTickerService(cmd);
        }


    }
}
