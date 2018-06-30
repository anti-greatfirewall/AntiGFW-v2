using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AntiGFW.Json {
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class JsonConfig {
        [Serializable]
        public class Autorun {
            public int versionIndex;
            public Autorun() {
                versionIndex = 0;
            }
        }

        [Serializable]
        public class ShadowsocksConfig {
            public bool global, enabled, shareOverLan, autoCheckUpdate;
            public int index;

            public ShadowsocksConfig() {
                index = 0;
                global = false;
                enabled = true;
                shareOverLan = false;
                autoCheckUpdate = true;
            }
        }

        [Serializable]
        public class Config {
            public string spiderName;
            public dynamic spiderConfig;

            public Config() {
                spiderName = "";
            }
        }

        //[Serializable]
        //public class PacUrl {
        //    [Serializable]
        //    public class DynamicPac {
        //        public string url, pacprefix;
        //        public int length;

        //        public DynamicPac() {
        //            url = pacprefix = null;
        //            length = 0;
        //        }
        //    }

        //    public bool enabled;
        //    public bool staticUrl;
        //    public string pacUrl;
        //    public DynamicPac dynamicPac;

        //    public PacUrl() {
        //        enabled = false;
        //    }

        //    public string GetUrl() {
        //        if (!enabled) {
        //            return null;
        //        }
        //        if (staticUrl) {
        //            return pacUrl;
        //        }
        //        string html = Utils.DownloadString(dynamicPac.url);
        //        int pos = html.IndexOf(dynamicPac.pacprefix, StringComparison.Ordinal);
        //        return html.Substring(pos + dynamicPac.length, dynamicPac.length);
        //        //"text">
        //    }
        //}

        public List<Config> config;
        public List<string> versions;
        public Autorun autorun;
        public ShadowsocksConfig shadowsocksConfig;
        //public PacUrl pacUrl;

        public string shadowsocksPath;
        public bool autorunEnabled;
        public bool hourlyStartup;
        public bool autoStartup;

        public JsonConfig() {
            config = new List<Config>();
            versions = new List<string>();
            autorun = new Autorun();
            shadowsocksConfig = new ShadowsocksConfig();

            shadowsocksPath = null;
            autorunEnabled = false;
            autoStartup = false;
            //pacUrl = new PacUrl();
        }
    }
}
