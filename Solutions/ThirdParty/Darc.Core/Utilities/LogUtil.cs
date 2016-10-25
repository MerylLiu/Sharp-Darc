namespace Darc.Core.Utilities
{
    using System;
    using System.IO;
    using log4net;
    using log4net.Config;

    public static class LogUtil
    {
        static LogUtil()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

        public static ILog Log<T>()
        {
            return LogManager.GetLogger(typeof (T));
        }
    }
}