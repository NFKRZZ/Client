using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace Client
{
    public class Logger
    {
        static List<string> log = new List<string>();
        static string filePath;
        static TextWriter tw;
        public static async void Initialize()
        {
            
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            string path = Directory.GetCurrentDirectory();
            Console.WriteLine("\n File Path: "+path);
            filePath = path+@"\log.txt";
            try
            {
                FileStream fs = File.Create(filePath);
                
            }
            catch
            {
                Initialize();
            }
        }
        public static void Log(string statement)
        {
            log.Add(statement);
            byte[] logs = new UTF8Encoding(true).GetBytes(statement);
             tw = new StreamWriter(filePath, true);
            tw.Write(log);
            
        }
        static void OnProcessExit(object sender, EventArgs e)
        {
            Thread.Sleep(10000);
            foreach(string a in log)
            {
                byte[] logs = new UTF8Encoding(true).GetBytes(a);
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    fs.Write(logs, 0, logs.Length);
                }
            }
            Console.WriteLine(ClientBot.Client.getTime()+"I'm out of here");
        }

    }
}
