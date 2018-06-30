using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// ReSharper disable InconsistentNaming

namespace AntiGFW.Json {
    [Serializable]
    public class LogViewerConfig {
        public bool topMost;
        public bool wrapText;
        public bool toolbarShown;

        public Font Font { get; set; } = new Font("Consolas", 8F);

        public Color BackgroundColor { get; set; } = Color.Black;

        public Color TextColor { get; set; } = Color.White;

        public LogViewerConfig() {
            topMost = false;
            wrapText = false;
            toolbarShown = false;
        }
    }

    [Serializable]
    public class HotkeyConfig {
        public string switchSystemProxy;
        public string switchSystemProxyMode;
        public string switchAllowLan;
        public string showLogs;
        public string serverMoveUp;
        public string serverMoveDown;

        public HotkeyConfig() {
            switchSystemProxy = "";
            switchSystemProxyMode = "";
            switchAllowLan = "";
            showLogs = "";
            serverMoveUp = "";
            serverMoveDown = "";
        }
    }

    [Serializable]
    public class ProxyConfig {
        public const int ProxySocks5 = 0;
        public const int ProxyHttp = 1;

        public const int MaxProxyTimeoutSec = 10;
        private const int DefaultProxyTimeoutSec = 3;

        public bool useProxy;
        public int proxyType;
        public string proxyServer;
        public int proxyPort;
        public int proxyTimeout;

        public ProxyConfig() {
            useProxy = false;
            proxyType = ProxySocks5;
            proxyServer = "";
            proxyPort = 0;
            proxyTimeout = DefaultProxyTimeoutSec;
        }
    }

    [Serializable]
    public class Configuration {
        public List<Server> configs = new List<Server>();

        public string strategy;
        public int index;
        public bool global;
        public bool enabled;
        public bool shareOverLan;
        public bool isDefault;
        public int localPort;
        public string pacUrl;
        public bool useOnlinePac;
        public bool secureLocalPac = true;
        public bool availabilityStatistics;
        public bool autoCheckUpdate;
        public bool checkPreRelease;
        public bool isVerboseLogging;

        public LogViewerConfig logViewer;
        public ProxyConfig proxy;
        public HotkeyConfig hotkey;
    }

    [Serializable]
    public class Server {
        public static readonly Regex
            UrlFinder = new Regex(@"ss://(?<base64>[A-Za-z0-9+-/=_]+)(?:#(?<tag>\S+))?", RegexOptions.IgnoreCase),
            DetailsParser = new Regex(@"^((?<method>.+?):(?<password>.*)@(?<hostname>.+?):(?<port>\d+?))$", RegexOptions.IgnoreCase);

        private const int DefaultServerTimeoutSec = 5;
        public const int MaxServerTimeoutSec = 20;

        public string server;
        public int server_port;
        public string password;
        public string method;
        public string plugin;
        public string plugin_opts;
        public string remarks;
        public int timeout;

        public Server() {
            server = "";
            server_port = 8388;
            method = "aes-256-cfb";
            plugin = "";
            plugin_opts = "";
            password = "";
            remarks = "";
            timeout = DefaultServerTimeoutSec;
        }

        public static List<Server> GetServers(string ssUrl) {
            MatchCollection matches = UrlFinder.Matches(ssUrl);
            if (matches.Count <= 0) return null;
            List<Server> servers = new List<Server>();
            foreach (Match match in matches) {
                Server tmp = new Server();
                string base64 = match.Groups["base64"].Value;
                string tag = match.Groups["tag"].Value;
                if (!string.IsNullOrEmpty(tag)) {
                    tmp.remarks = HttpUtility.UrlDecode(tag, Encoding.UTF8);
                }
                Match details = DetailsParser.Match(Encoding.UTF8.GetString(Convert.FromBase64String(
                    base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='))));
                if (!details.Success)
                    continue;
                tmp.method = details.Groups["method"].Value;
                tmp.password = details.Groups["password"].Value;
                tmp.server = details.Groups["hostname"].Value;
                tmp.server_port = int.Parse(details.Groups["port"].Value);

                servers.Add(tmp);
            }
            return servers;
        }
    }
}