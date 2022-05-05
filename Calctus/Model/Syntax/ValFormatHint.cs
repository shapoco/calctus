using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Syntax {
    /// <summary>Val の表現に関するヒントを格納するクラス</summary>
    class ValFormatHint {
        public static readonly ValFormatHint Default = new ValFormatHint(NumberFormatter.CStyleReal);

        public readonly NumberFormatter Formatter;

        public ValFormatHint(NumberFormatter f) {
            this.Formatter = f;
        }

        // todo: delete
        /*
        public string Format(object val) {
            if (Formatter.Type == NumberFormatType.Time)
                return FormatTime((double)val);
            else if (val is double || val is float)
                return Format((double)val);
            else if (val is char || val is byte || val is int || val is uint || val is short || val is ushort || val is long || val is ulong)
                return Format((double)val);
            else
                return val.ToString();
        }

        public string Format(double val) {
            if (Formatter.Type == NumberFormatType.Time) 
                return FormatTime((double)val);
            else if ((long)val == val) 
                return Format((long)val);
            else
                return val.ToString();
        }

        public string Format(long val) {
            if (Formatter.Type == NumberFormatType.Time)
                return FormatTime((double)val);
            else
                switch (Formatter.Type) {
                    case NumberFormatType.Real: return val.ToString();
                    case NumberFormatType.Integer: return val.ToString();
                    case NumberFormatType.CStyleHex: return "0x" + Convert.ToString(val & 0xffffffffL, 16);
                    case NumberFormatType.CStyleBin: return "0b" + Convert.ToString(val & 0xffffffffL, 2);
                    case NumberFormatType.CStyleOct: return "0" + Convert.ToString(val & 0xffffffffL, 8);
                    default: throw new NotImplementedException();
                }
        }

        private static string FormatTime(double val) {
            int sign = val >= 0 ? 1 : -1;
            val *= sign;
            long h = (long)Math.Truncate(val / 3600); val -= h * 3600;
            long m = (long)Math.Truncate(val / 60); val -= m * 60;
            double s = val;
            var sb = new StringBuilder();
            if (sign < 0) sb.Append('-');
            if (h > 0) sb.Append(h).Append('h');
            if (m > 0) sb.Append(m).Append('m');
            // m だけだとメートルと区別がつかないので h == 0 の場合は s も付ける
            if (s > 0 || h == 0) sb.Append(s).Append('s');
            return sb.ToString();
        }
        */
    }
}
