using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Win32;

namespace AntiGFW {

    public static class Extensions {
        public static bool IsNullOrEmpty(this string value) {
            return string.IsNullOrEmpty(value);
        }
    }

    public static class Utils {
        private class Dummy { }

        public static string ExePath => typeof(Dummy).Assembly.Location;

        public static string ExeDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static string ProgramName = "AntiGFW-v2";

        private static readonly string[] UAs = {
            // Windows 10 Chrome dev
            @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3213.3 Safari/537.36",
            // Windows 10 Chrome
            @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36",
            // Windows 10 Edge
            @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134",
            // Windows 10 Firefox
            @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:60.0) Gecko/20100101 Firefox/60.0"
        };

        // Windows 10; Chrome dev
        public static string UserAgent => UAs[new Random().Next() % UAs.Length];

        public static WebClient Wc =
        new WebClient {
            Encoding = Encoding.UTF8,
            Credentials = CredentialCache.DefaultCredentials
        };

        public static RegistryKey OpenRegKey(string name, bool writable, RegistryHive hive = RegistryHive.CurrentUser) {
            RegistryKey result;
            try {
                RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey(name, writable);
                result = registryKey;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                result = null;
            }
            return result;
        }

        public static void Initialize() {
            Wc.Headers.Set("User-Agent", UserAgent);
        }

        public static string DownloadString(string address) {
            return Wc.DownloadString(address);
        }

        public static byte[] DownloadData(string address) {
            return Wc.DownloadData(address);
        }

        public static void DownloadDataProgress(string srcUrl, string destFile) {
            DateTime start = DateTime.Now;
            int top = Console.CursorTop;
            Console.WriteLine("Process:");
            try {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(srcUrl);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                long totalBytes = webResponse.ContentLength;
                Stream webStream = webResponse.GetResponseStream();
                FileStream fileStream = new FileStream(destFile, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] data = new byte[1024];
                // ReSharper disable once PossibleNullReferenceException
                int osize = webStream.Read(data, 0, data.Length);
                while (osize > 0) {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    fileStream.Write(data, 0, osize);
                    osize = webStream.Read(data, 0, data.Length);
                    double percent = totalDownloadedByte / (double)totalBytes * 100;
                    DateTime point = DateTime.Now;
                    Console.SetCursorPosition(0, top);
                    Console.WriteLine($"{Math.Round(percent, 2)}% {totalDownloadedByte / (point - start).TotalSeconds / 1000}KB/s");
                    Console.WriteLine($"{totalDownloadedByte}B/{totalBytes}B");
                    for (int i = 0; i < percent / 10; i++) {
                        Console.Write("█");
                    }
                }
                fileStream.Close();
                webStream.Close();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Console.WriteLine("\nFinished!");
        }
    }

    public static class AutoStartup {
        public static readonly string ExecutablePath = Utils.ExePath;
        public static readonly string Key = Utils.ProgramName + "_" + Utils.ExeDirectory.GetHashCode();

        public static bool Enabled {
            set => Set(value);
        }

        public static void Set(bool enabled) {
            try {
                RegistryKey registryKey = Utils.OpenRegKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (enabled) {
                    registryKey.SetValue(Key, ExecutablePath);
                } else {
                    registryKey.DeleteValue(Key);
                }
            } catch {
                // ignored
            }
        }
    }

    public static class MessagePrinter {
        public enum MessageType {
            Info,
            Warn,
            Error,
            Fatal
        }

        public static void Print(string msg, MessageType type) {
            switch (type) {
                case MessageType.Info:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("INFO");
                    break;
                case MessageType.Warn:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("WARN");
                    break;
                case MessageType.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("ERROR");
                    break;
                case MessageType.Fatal:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FATAL");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + msg);
        }

        public static void Print(string msg, ConsoleColor backgraoundColor, ConsoleColor foregroundColor) {
            Console.BackgroundColor = backgraoundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(msg);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
