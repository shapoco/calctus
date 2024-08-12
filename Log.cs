using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Shapoco.Texts;

namespace Shapoco {
    static class Log {
        public delegate MethodBase GetCurrentMethodFunc();
        public static readonly GetCurrentMethodFunc Here = MethodBase.GetCurrentMethod;

        // todo Log.OutputPath 絶対パスにする
        public static readonly string OutputPath =
            Path.Combine(AppDataManager.RoamingUserDataPath, Application.ProductName + ".log");
        public static StreamWriter Writer { get; private set; }

        private static long _lastTickTime = Environment.TickCount;

        private static LogLevel _logLevel = LogLevel.Error;
        public static LogLevel LogLevel {
            get => _logLevel;
            set {
                if (value == _logLevel) return;
                WriteLine("Log level changed from " + _logLevel + " to " + value);
                _logLevel = value;
            }
        }

        public static void StartLogging() {
            try {
                Writer = new StreamWriter(OutputPath);
                Here().I("Log file opened: '" + OutputPath + "'");
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
        public static object F(string scope, object msgObj) { WriteLine(LogLevel.Fatal, scope, msgObj); return msgObj; }

        public static Exception T(string scope, Exception ex) { WriteLine(LogLevel.Trace, scope, DescribeException(ex)); return ex; }
        public static Exception I(string scope, Exception ex) { WriteLine(LogLevel.Info, scope, DescribeException(ex)); return ex; }
        public static Exception W(string scope, Exception ex) { WriteLine(LogLevel.Warning, scope, DescribeException(ex)); return ex; }
        public static Exception E(string scope, Exception ex) { WriteLine(LogLevel.Error, scope, DescribeException(ex)); return ex; }
        public static Exception F(string scope, Exception ex) { WriteLine(LogLevel.Fatal, scope, DescribeException(ex)); return ex; }

        public static string DescribeException(Exception ex)
            => ex.GetType().Name + "\r\n" + ex.ToString();

        public static void WriteLine(LogLevel lv, string scope, object msg) {
            if (lv < _logLevel) return;
            var t = Environment.TickCount;
            var deltaT = t - _lastTickTime;
            _lastTickTime = t;
            var msgStr = '\t' + scope + '\t';
            if (deltaT > 0) msgStr = '+' + deltaT.ToString() + msgStr;
            msgStr += (msg == null) ? "(null)" : msg.ToString();
            switch (lv) {
                case LogLevel.Trace: WriteLine("*trace\t" + msgStr, flush: false); break;
                case LogLevel.Info: WriteLine("*info\t" + msgStr, flush: false); break;
                case LogLevel.Warning: WriteLine("*Warn\t" + msgStr, flush: false); break;
                case LogLevel.Error: WriteLine("*ERROR\t" + msgStr, flush: true); break;
                case LogLevel.Fatal: WriteLine("*FATAL\t" + msgStr, flush:true); break;
                default: throw new NotImplementedException();
            }
        }

        public static void WriteLine(object msg, bool flush = false) {
            if (msg == null) return;
            var msgStr = msg.ToString();
            msgStr = msgStr.Replace("\n", "\n\t\t");
            Console.Error.WriteLine(msgStr);
            Writer?.WriteLine(msgStr);
            if (flush) {
                Console.Error.Flush();
                Writer?.Flush();
            }
        }

        public static string LastTestMessage { get; private set; }

        public static bool NotEqHex(this object a, object b) => NotEq(a, b, Radix.Hex);
        public static bool NotEq(this object a, object b, Radix radix = Radix.Decimal) {
            if ((a == null && b == null) || (a != null && a.Equals(b))) {
                return false;
            }
            else {
                LastTestMessage =a.ToStringForDisplay(radix) + " != " + b.ToStringForDisplay(radix);
                return true;
            }
        }

        public static bool NotEqHex<T>(this T[] a, T[] b) => NotEq(a, b, Radix.Hex);
        public static bool NotEq<T>(this T[] a, T[] b, Radix radix = Radix.Decimal) {
            if (a.Length != b.Length) {
                LastTestMessage = "Array length differ: " + a.Length + " != " + b.Length;
                return true;
            }

            for (int i = 0; i < a.Length; i++) {
                if (!a[i].Equals(b[i])) {
                    LastTestMessage = "Array element differ at index=" + i + " : " +
                        a[i].ToStringForDisplay(radix) + " != " + b[i].ToStringForDisplay(radix);
                    return true;
                }
            }

            return false;
        }

        public static bool NotLessEq<T>(this T a, T b, Radix radix = Radix.Decimal) where T: IComparable<T> {
            if (a.CompareTo(b) <= 0) {
                return false;
            }
            else {
                LastTestMessage = a.ToStringForDisplay(radix) + " > " + b.ToStringForDisplay(radix);
                return true;
            }
        }

        public static string ToStringForDisplay(this object obj,Radix radix = Radix.Decimal) {
            var sb = new StringBuilder();
            ToStringForDisplay(obj, sb, radix);
            return sb.ToString();
        }

        public static void ToStringForDisplay(this object obj, StringBuilder sb, Radix radix = Radix.Decimal) {
            if (obj == null) { sb.Append("null"); return; }
            if (radix != Radix.Decimal) {
                var baseNum = radix.ToBaseNumber();
                string digits = null;
                if (obj is char cval) digits = Convert.ToString(cval, baseNum);
                else if (obj is byte bval) digits = Convert.ToString(bval, baseNum);
                else if (obj is short sval) digits = Convert.ToString(sval, baseNum);
                else if (obj is ushort usval) digits = Convert.ToString(usval, baseNum);
                else if (obj is int ival) digits = Convert.ToString(ival, baseNum);
                else if (obj is uint uival) digits = Convert.ToString(uival, baseNum);
                else if (obj is long lval) digits = Convert.ToString(lval, baseNum);
                else if (obj is ulong ulval) digits = Convert.ToString((long)ulval, baseNum);
                if (!string.IsNullOrEmpty(digits)) {
                    sb.Append(radix.ToRustStylePrefix());
                    sb.Append(digits);
                    return;
                }
            }
            if (obj is char c) { sb.Append(CStyleEscaping.EscapeAndQuote(c)); return; }
            if (obj is string s) { sb.Append(CStyleEscaping.EscapeAndQuote(s)); return; }
            if (obj is Array a) {
                bool following = false;
                sb.Append('{');
                foreach (var elm in a) {
                    if (following) sb.Append(", ");
                    ToStringForDisplay(elm, sb, radix);
                    following = true;
                }
                sb.Append('}');
                return;
            }
            sb.Append(obj.ToString());
        }
    }

    static class LogExtension {
        public static object T(this MethodBase mb, object msgObj = null) => Log.T(PositionStringFrom(mb), msgObj);
        public static object I(this MethodBase mb, object msgObj = null) => Log.I(PositionStringFrom(mb), msgObj);
        public static object W(this MethodBase mb, object msgObj = null) => Log.W(PositionStringFrom(mb), msgObj);
        public static object E(this MethodBase mb, object msgObj = null) => Log.E(PositionStringFrom(mb), msgObj);
        public static object F(this MethodBase mb, object msgObj = null) => Log.E(PositionStringFrom(mb), msgObj);
        public static Exception T(this MethodBase mb, Exception ex) => Log.T(PositionStringFrom(mb), ex);
        public static Exception I(this MethodBase mb, Exception ex) => Log.I(PositionStringFrom(mb), ex);
        public static Exception W(this MethodBase mb, Exception ex) => Log.W(PositionStringFrom(mb), ex);
        public static Exception E(this MethodBase mb, Exception ex) => Log.E(PositionStringFrom(mb), ex);
        public static Exception F(this MethodBase mb, Exception ex) => Log.F(PositionStringFrom(mb), ex);

        public static Exception ArgException(this MethodBase mb, string argName, string msg = null) {
            var scope = PositionStringFrom(mb);
            return Log.E(scope, new ArgumentException(msg, argName + " for " + scope));
        }

        public static Exception TestFailException(this MethodBase mb, string msg = null) {
            var exMsg = Log.LastTestMessage;
            if (!string.IsNullOrEmpty(msg)) exMsg += " (" + msg + ")";
            return Log.E(PositionStringFrom(mb), new Exception(exMsg));
        }

        public static string PositionStringFrom(MethodBase mb) => mb.DeclaringType.Name + "." + mb.Name + "()";
    }

    enum LogLevel {
        Quiet,
        Trace,
        Info,
        Warning,
        Error,
        Fatal,
    }
}
