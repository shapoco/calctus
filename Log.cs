using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Shapoco {
    static class Log {
        public delegate MethodBase GetCurrentMethodFunc();
        public static readonly GetCurrentMethodFunc Here = MethodBase.GetCurrentMethod;

        // todo Log.OutputPath 絶対パスにする
        public static readonly string OutputPath =
            Path.Combine(AppDataManager.RoamingUserDataPath, Application.ProductName + ".log");
        public static StreamWriter Writer { get; private set; }

        public static void StartLogging() {
            try {
                Writer = new StreamWriter(OutputPath);
            }
            catch (Exception ex) {
                Console.Error.WriteLine("Open log file failed: '" + OutputPath + "'");
                Console.Error.WriteLine(ex);
            }
        }

        public static void EndLogging() {
            if (Writer != null) {
                try { Writer.Close(); } catch { }
                Writer = null;
            }
        }

        public static void OpenOutputFileInApp() {
            try {
                Writer?.Flush();
                System.Diagnostics.Process.Start(OutputPath);
            }
            catch (Exception ex) {
                Console.Error.WriteLine("Open log file failed: '" + OutputPath + "'");
                Console.Error.WriteLine(ex);
            }
        }

        public static object T(string scope, object msgObj) { WriteLine(LogLevel.Trace, scope, msgObj); return msgObj; }
        public static object I(string scope, object msgObj) { WriteLine(LogLevel.Info, scope, msgObj); return msgObj; }
        public static object W(string scope, object msgObj) { WriteLine(LogLevel.Warning, scope, msgObj); return msgObj; }
        public static object E(string scope, object msgObj) { WriteLine(LogLevel.Error, scope, msgObj); return msgObj; }

        public static Exception T(string scope, Exception ex) { WriteLine(LogLevel.Trace, scope, DescribeException(ex)); return ex; }
        public static Exception I(string scope, Exception ex) { WriteLine(LogLevel.Info, scope, DescribeException(ex)); return ex; }
        public static Exception W(string scope, Exception ex) { WriteLine(LogLevel.Warning, scope, DescribeException(ex)); return ex; }
        public static Exception E(string scope, Exception ex) { WriteLine(LogLevel.Error, scope, DescribeException(ex)); return ex; }

        public static string DescribeException(Exception ex)
            => ex.GetType().Name + "\r\n" + ex.Message + "\r\n" + ex.StackTrace;

        private static void WriteLine(LogLevel lv, string scope, object msg) {
            var msgStr = scope + '\t';
            msgStr += (msg == null) ? "(null)" : msg.ToString();
            msgStr = msgStr.Replace("\n", "\n        ");
            switch (lv) {
                case LogLevel.Trace: WriteLine("*trace\t" + msgStr); break;
                case LogLevel.Info: WriteLine("*info\t" + msgStr); break;
                case LogLevel.Warning: WriteLine("*Warn\t" + msgStr); break;
                case LogLevel.Error: WriteLine("*ERROR\t" + msgStr); break;
                default: throw new NotImplementedException();
            }
        }

        private static void WriteLine(object msg) {
            if (msg == null) return;
            var msgStr = msg.ToString();
            Console.Error.WriteLine(msgStr);
            Writer?.WriteLine(msgStr);
        }
    }

    static class LogExtension {
        public static object T(this MethodBase mb, object msgObj = null) => Log.T(PositionStringFrom(mb), msgObj);
        public static object I(this MethodBase mb, object msgObj = null) => Log.I(PositionStringFrom(mb), msgObj);
        public static object W(this MethodBase mb, object msgObj = null) => Log.W(PositionStringFrom(mb), msgObj);
        public static object E(this MethodBase mb, object msgObj = null) => Log.E(PositionStringFrom(mb), msgObj);
        public static Exception T(this MethodBase mb, Exception ex) => Log.T(PositionStringFrom(mb), ex);
        public static Exception I(this MethodBase mb, Exception ex) => Log.I(PositionStringFrom(mb), ex);
        public static Exception W(this MethodBase mb, Exception ex) => Log.W(PositionStringFrom(mb), ex);
        public static Exception E(this MethodBase mb, Exception ex) => Log.E(PositionStringFrom(mb), ex);

        public static Exception ArgErr(this MethodBase mb, string argName, string msg = null) {
            var scope = PositionStringFrom(mb);
            return Log.E(scope, new ArgumentException(msg, argName + " for " + scope));
        }

        public static string PositionStringFrom(MethodBase mb) => mb.DeclaringType.Name + "." + mb.Name + "()";
    }

    enum LogLevel {
        Error,
        Warning,
        Info,
        Trace,
    }
}
