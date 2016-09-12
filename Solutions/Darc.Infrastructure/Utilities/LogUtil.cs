namespace Darc.Infrastructure.Utilities
{
    using System;
    using System.IO;
    using log4net;
    using log4net.Config;

    public class LogUtil
    {
        static LogUtil()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

        public static void Error(Type t, Exception ex)
        {
            var log = LogManager.GetLogger(t);
            log.Error("Error", ex);
        }

        public static void Error(Type t, string message)
        {
            var log = LogManager.GetLogger(t);
            log.Error(message);
        }
    }
}