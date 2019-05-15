using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Util
{
    public static class Logger
    {
        public static void Log(string message,string logPath)
        {
            try
            {
                File.AppendAllText(logPath, message + "\r\n");
            }
            catch (Exception)
            {

            }
           
        }
    }
}
