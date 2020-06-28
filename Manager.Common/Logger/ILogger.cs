using System;
using System.Collections.Generic;
using System.Text;

namespace Manager.Common.Logger
{
    public  interface ILogger
    {
        void Print(LogType logType, string message);
    }
}
