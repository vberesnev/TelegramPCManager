using Manager.Common.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Manager.BL
{
    static class PCManager
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;



        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);


        internal static bool ShutDown(ILogger logger)
        {
            try
            {
                var psi = new ProcessStartInfo("shutdown", "/s /f /t 3");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                logger.Print(LogType.ERROR, $"Shut down PC error: {ex.Message}");
                return false;
            }
        }

        internal static bool Sleep(ILogger logger)
        {
            try
            {
                SetSuspendState(false, true, true);
                return true;
            }
            catch (Exception ex)
            {
                logger.Print(LogType.ERROR, $"Sleep down PC error: {ex.Message}");
                return false;
            }
        }

        internal static bool Mute(ILogger logger)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, "NirCMD", "nircmd.exe");
                if (File.Exists(path))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = path,
                            Arguments = "mutesysvolume 2"
                        }
                    };
                    process.Start();
                    return true;
                }
                else
                    throw new Exception("No nircmd.exe for volume control");
            }
            catch (Exception ex)
            {
                logger.Print(LogType.ERROR, $"Muting PC error: {ex.Message}");
                return false;
            }
        }

        internal static bool VolumeUp(ILogger logger)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, "NirCMD", "nircmd.exe");
                if (File.Exists(path))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = path,
                            Arguments = "changesysvolume 3276"
                        }
                    };
                    process.Start();
                    return true;
                }
                else
                    throw new Exception("No nircmd.exe for volume control");
            }
            catch (Exception ex)
            {
                logger.Print(LogType.ERROR, $"Volume Up PC error: {ex.Message}");
                return false;
            }
        }

        internal static bool VolumeDown(ILogger logger)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, "NirCMD", "nircmd.exe");
                if (File.Exists(path))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = path,
                            Arguments = "changesysvolume -3276"
                        }
                    };
                    process.Start();
                    return true;
                }
                else
                    throw new Exception("No nircmd.exe for volume control");
            }
            catch (Exception ex)
            {
                logger.Print(LogType.ERROR, $"Volume Up PC error: {ex.Message}");
                return false;
            }
        }

        internal static bool Music(ILogger logger)
        {
            try
            {
                Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "https://radio.yandex.ru/user/vberesnev.job");
                return true;
            }
            catch (Exception ex)
            {
                logger.Print(LogType.ERROR, $"Opening Yandex radio error: {ex.Message}");
                return false;
            }
        }

        
    }
}
