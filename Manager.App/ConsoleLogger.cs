using Manager.Common.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manager.App
{
    class ConsoleLogger : ILogger
    {
        public void Print(LogType logType, string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss")} | { message}");
        }
    }
}
