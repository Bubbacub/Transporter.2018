using System;
using log4net;

namespace Imd.Transporter.Viewer.Logging
{
    public static class Logger
    {
        private static ILog Log { get; }
        static Logger()
        {
            Log = LogManager.GetLogger(typeof(Logger));
        }

        public static void Error(object msg)
        {
            Log.Error(msg);
        }

        public static void Error(object msg, Exception ex)
        {
            Log.Error(msg, ex);
        }

        public static void Error(Exception ex)
        {
            Log.Error(ex.Message, ex);
        }

        public static void Info(object msg)
        {
            Log.Info(msg);
        }

        public static void InfoFormat(string userName, object msg)
        {
            Log.InfoFormat("User:{0}, {1}", userName, msg);
        }

        public static void Debug(object message)
        {
            Log.Debug(message);
        }
    }
}