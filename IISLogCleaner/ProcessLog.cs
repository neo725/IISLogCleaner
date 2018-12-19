using HelpersForNet;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IISLogCleaner
{

    public class ProcessLog : IDisposable
    {
        static ILog Log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProcessLog()
        {

        }

        public static ProcessLog Instance => Singleton<ProcessLog>.Instance;

        public void Init(string configPath)
        {
            if (File.Exists(configPath) == false)
            {
                throw new Exception("log4net.config 不存在");
            }
            
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configPath));

            WriteLine($">> Runs @ {DateTime.Now.ToString()}");
        }

        public void WriteLine(string message)
        {
            Log.Info(message);
        }

        public void Error(Exception exception)
        {
            Log.Error(exception.Message, exception);
        }

        public void Debug(string message = "")
        {
            Console.WriteLine(message);
            if (Settings.Mode == "debug")
            {
                Log.Debug(message);
            }
        }

        public void Dispose()
        {
            WriteLine($"Dispose @ {DateTime.Now.ToString()}");
        }
    }
}
