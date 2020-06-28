using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLine;

namespace Manager.App.CommandLine
{
    class Options
    {
        [Option('t', "token", Required = false, HelpText = "Your Telegram Bot token or path to .txt file with it")]
        public string Token { get; set; }

        [Option('w', "path to users white list", Required = false, HelpText = "Path to .txt file with your users white list")]
        public string WhiteListPath { get; set; }
    }
}
