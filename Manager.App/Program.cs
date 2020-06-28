using CommandLine;
using Manager.App;
using Manager.App.CommandLine;
using ManagerBL;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ManagerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();

            var options = new Options();

            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(opt =>
            {
                options = opt;
            });

            if (string.IsNullOrEmpty(options.Token))
                options.Token = Path.Combine(Environment.CurrentDirectory, "token.txt");

            if (string.IsNullOrEmpty(options.WhiteListPath))
                options.WhiteListPath = Path.Combine(Environment.CurrentDirectory, "white.txt");

            var token = string.Empty;
            if (File.Exists(options.Token))
                token = File.ReadAllText(options.Token, Encoding.UTF8);
            else
                token = options.Token;

            string[] userWhiteList = new string[0];
            
            if (File.Exists(options.WhiteListPath))
                userWhiteList = File.ReadAllLines(options.WhiteListPath);

            var tBot = new TBot(token, logger, userWhiteList);
            tBot.BotStop += TBot_BotStop; 
            tBot.StartAsync();

            Console.ReadKey();
        }

        private static void TBot_BotStop(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
