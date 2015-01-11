﻿using System;
using System.Diagnostics;
using MonoTouch.Foundation;

namespace Quobject.EngineIoClientDotNet.Modules
{
    public class LogManager
    {
        private const string myFileName = "XunitTrace.txt";
        private string MyType;
        private static readonly LogManager EmptyLogger = new LogManager(null);

        private static System.IO.StreamWriter file;

        public static bool Enabled = false;

        #region Statics

        public static void SetupLogManager()
        {
          

        }

        public static LogManager GetLogger(string type)
        {
            var result = new LogManager(type);
            return result;
        }

        public static LogManager GetLogger(Type type)
        {
            return GetLogger(type.ToString());
        }

        public static LogManager GetLogger(System.Reflection.MethodBase methodBase)
        {
#if DEBUG
            var type = methodBase.DeclaringType == null ? "" : methodBase.DeclaringType.ToString();
            var type1 = string.Format("{0}#{1}", type, methodBase.Name);
            return GetLogger(type1);
#else
            return EmptyLogger;
#endif
        }

        #endregion

        public LogManager(string type)
        {
            this.MyType = type;
        }

        [Conditional("DEBUG")]
        public void Info(string msg)
        {
            //Trace.WriteLine(string.Format("{0} [{3}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), MyType, msg, System.Threading.Thread.CurrentThread.ManagedThreadId));
            if (!Enabled)
            {
                return;
            }

            if (LogManager.file == null)
            {
                var path = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0];
                LogManager.file = new System.IO.StreamWriter(System.IO.Path.Combine(path, myFileName), true);
                LogManager.file.AutoFlush = true;
            }

            msg = Global.StripInvalidUnicodeCharacters(msg);
            var msg1 = string.Format("{0} [{3}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), MyType,
                msg,
                System.Threading.Thread.CurrentThread.ManagedThreadId);
            LogManager.file.WriteLine(msg1);

        }

        [Conditional("DEBUG")]
        public void Error(string p, Exception exception)
        {
            this.Info(string.Format("ERROR {0} {1} {2}", p, exception.Message, exception.StackTrace));
            if (exception.InnerException != null)
            {
                this.Info(string.Format("ERROR exception.InnerException {0} {1} {2}", p, exception.InnerException.Message, exception.InnerException.StackTrace));                
            }
        
        }


        [Conditional("DEBUG")]
        internal void Error(Exception e)
        {
            this.Error("", e);
        }
    }
}