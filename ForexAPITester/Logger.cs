using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public static class Logger
    {
        static StreamWriter fs;
        static StreamWriter fsData;
        static Logger()
        {
            fs = File.CreateText($"ProbeDetails{DateTime.Now.ToString("yyyyMMddHHmm")}.log");
            fsData = File.CreateText($"Data{DateTime.Now.ToString("yyyyMMddHHmm")}.txt");
            fsData.Write("[{}");
        }

        public static void WriteLog(string logInfo)
        {
            fs.WriteLine(logInfo);
        }
        public static void WriteData(string dataInfo)
        {
            fsData.Write(",");
            fsData.Write(dataInfo);
        }
        public static void Close()
        {
            fs.Flush();
            fs.Close();

            fsData.Write("]");
            fsData.Flush();
            fsData.Close();

        }
    }
}
