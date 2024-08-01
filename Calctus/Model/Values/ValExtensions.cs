using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;


namespace Shapoco.Calctus.Model.Values {
    static class ValExtensions {
        public static BoolVal ToVal(this bool val) => BoolVal.From(val);
        public static RealVal ToVal(this decimal val, FormatFlags fmt = FormatFlags.Default) => new RealVal(val, fmt);
        public static Val ToVal(this frac val) => FracVal.Normalize(val);
        public static StrVal ToVal(this string val) => new StrVal(val);
        public static RealVal ToVal(this DayOfWeek val) => (RealVal)DateTimeVal.DayOfWeekList[(int)val];

        public static ListVal ToVal(this Val[] val) => new ListVal(val);
        public static ListVal ToVal(this decimal[] val, FormatFlags fmt = FormatFlags.Default) => new ListVal(val.ToValArray(fmt));
        public static ListVal ToVal(this byte[] val, FormatFlags fmt = FormatFlags.Hexadecimal) => new ListVal(val.ToValArray(fmt));
        public static ListVal ToVal(this int[] val, FormatFlags fmt = FormatFlags.Default) => new ListVal(val.ToValArray(fmt));
        public static ListVal ToVal(this long[] val, FormatFlags fmt = FormatFlags.Default) => new ListVal(val.ToValArray(fmt));
        public static ListVal ToVal(this string[] val) => new ListVal(val.ToValArray());

        public static RealVal ToRealVal(this int val, FormatFlags fmt = FormatFlags.Default) => new RealVal(val, fmt);
        public static RealVal ToRealVal(this long val, FormatFlags fmt = FormatFlags.Default) => new RealVal(val, fmt);
        public static RealVal ToRealVal(this char val, FormatFlags fmt = FormatFlags.Character) => new RealVal(val, fmt);

        // todo 廃止: ToIntVal
        public static RealVal ToIntVal(this decimal val) => new RealVal(val, FormatFlags.Decimal);
        public static RealVal ToIntVal(this int val) => new RealVal(val, FormatFlags.Decimal);
        public static RealVal ToIntVal(this long val) => new RealVal(val, FormatFlags.Decimal);
        public static RealVal ToIntVal(this char val) => new RealVal(val, FormatFlags.Decimal);

        // todo 廃止: ToHexVal
        public static RealVal ToHexVal(this decimal val) => new RealVal(val, FormatFlags.Hexadecimal);
        public static RealVal ToHexVal(this int val) => new RealVal(val, FormatFlags.Hexadecimal);
        public static RealVal ToHexVal(this long val) => new RealVal(val, FormatFlags.Hexadecimal);
        public static RealVal ToHexVal(this char val) => new RealVal(val, FormatFlags.Character);

        // todo 廃止: ToCharVal
        public static RealVal ToCharVal(this decimal val) => new RealVal(val, FormatFlags.Character);
        public static RealVal ToCharVal(this int val) => new RealVal(val, FormatFlags.Character);
        public static RealVal ToCharVal(this long val) => new RealVal(val, FormatFlags.Character);
        public static RealVal ToCharVal(this char val) => new RealVal(val, FormatFlags.Character);

        public static RealVal ToDateTimeVal(this decimal val) => new RealVal(val, FormatFlags.DateTime);
        public static RealVal ToDateTimeVal(this int val) => new RealVal(val, FormatFlags.DateTime);
        public static RealVal ToDateTimeVal(this long val) => new RealVal(val, FormatFlags.DateTime);

        public static RealVal ToTimeSpanVal(this decimal val) => new RealVal(val, FormatFlags.TimeSpan);
        public static RealVal ToTimeSpanVal(this int val) => new RealVal(val, FormatFlags.TimeSpan);
        public static RealVal ToTimeSpanVal(this long val) => new RealVal(val, FormatFlags.TimeSpan);

        // todo 廃止: ToColorVal
        public static RealVal ToColorVal(this decimal val) => new RealVal(val, FormatFlags.WebColor);
        public static RealVal ToColorVal(this int val) => new RealVal(val, FormatFlags.WebColor);
        public static RealVal ToColorVal(this long val) => new RealVal(val, FormatFlags.WebColor);

        public static Val[] ToValArray(this decimal[] val, FormatFlags fmt = FormatFlags.Default) {
            ListVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for(int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fmt);
            }
            return array;
        }

        public static Val[] ToValArray(this char[] val, FormatFlags fmt = FormatFlags.Character) {
            ListVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fmt);
            }
            return array;
        }

        public static Val[] ToValArray(this byte[] val, FormatFlags fmt = FormatFlags.Hexadecimal) {
            ListVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fmt);
            }
            return array;
        }

        public static Val[] ToValArray(this int[] val, FormatFlags fmt = FormatFlags.Default) {
            ListVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fmt);
            }
            return array;
        }

        public static Val[] ToValArray(this long[] val, FormatFlags fmt = FormatFlags.Default) {
            ListVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fmt);
            }
            return array;
        }

        public static Val[] ToValArray(this string[] val) {
            ListVal.CheckArrayLength(val.Length);
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new StrVal(val[i]);
            }
            return array;
        }

        public static decimal[] ToDecimalArray(this Val[] val) {
            var array = new decimal[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = val[i].AsDecimal;
            }
            return array;
        }

        public static decimal[] ToDecimalArray(this string val) {
            var array = new decimal[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = val[i];
            }
            return array;
        }
    }
}
