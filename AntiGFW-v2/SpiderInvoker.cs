using System;
using System.Collections.Generic;
using System.Reflection;
using AntiGFW.Json;

namespace AntiGFW {
    public static class SpiderInvoker {
        public static void Invoke(List<JsonConfig.Config> config) {
            foreach (JsonConfig.Config i in config) {
                try {
                    Assembly spider = Assembly.LoadFrom(Utils.ExeDirectory + i.spiderName);
                    Type type = spider.GetType("Spider");
                    object o = Activator.CreateInstance(type);
                    MethodInfo method = type.GetMethod("Fetch");
                    // ReSharper disable once PossibleNullReferenceException
                    method.Invoke(o, i.spiderConfig);
                } catch (Exception e) {
                    MessagePrinter.Print(e.ToString(), MessagePrinter.MessageType.Error);
                }
            }
        }
    }
}
