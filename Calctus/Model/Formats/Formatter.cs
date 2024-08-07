using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Shapoco.Maths;
using Shapoco.Texts;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    static class Formatter {
        public static string ObjectToString(object val, ToStringArgs args) {
            if (val is decimal decVal) {
                return DecimalToString(decVal, args);
            }
            else if (val is string strVal) {
                return StringToCStyleLiteral(strVal, args);
            }
            else if (val is char charVal) {
                return CharToCStyleLiteralString(charVal, args.Usage);
            }
            else if (val is bool boolVal) {
                return ToLiteralString(boolVal);
            }
            else if (val is apfixed apfixedVal) {
                return ApFixedToString(apfixedVal, args);
            }
            else if (val is Frac fracVal) {
                return FracToString(fracVal);
            }
            else if (val is Val valVal) {
                return valVal.ToString(args);
            }
            else if (val is ICollectionVal collectionVal) {
                return ListToString(collectionVal.ToValArray(), args);
            }
            else if (val is Array arrayVal) {
                return ListToString(arrayVal, args);
            }
            else if (val == null) {
                return Keyword.Null.String;
            }
            else {
                return val.ToString();
            }
        }

        public static string DecimalToString(decimal val, ToStringArgs args) {
            var isInteger = val.IsInteger();
            var fmt = args.Format;

            if (fmt.Style == FormatStyle.WebColor && isInteger && 0 <= val && val <= 0xffffff) {
                return ColorSpace.ToHtmlStyle((int)val);
            }
            else if (fmt.Style == FormatStyle.Character && isInteger && char.MinValue <= val && val <= char.MaxValue) {
                return CharToCStyleLiteralString((char)val, args.Usage);
            }
            else if (fmt.Style == FormatStyle.SiPrefixed) {
                return SiPrefix.ToString(val, args);
            }
            else if (fmt.Style == FormatStyle.BinaryPrefixed) {
                return BinaryPrefix.ToString(val, args);
            }
            else if ((fmt.Radix == Radix.Hexadecimal || fmt.Radix == Radix.Binary || fmt.Radix == Radix.Octal) && isInteger) {
                return DecimalToCStyleBinaryLiteral(val, args, true);
            }
            else if (fmt.Style == FormatStyle.DateTime) {
                return DateTimeFormat.FormatAsStringLiteral(val, args.Usage.HasFlag(StringUsage.DateTimeQuotationFlag));
            }
            else if (fmt.Style == FormatStyle.TimeSpan) {
                return TimeSpanFormat.FormatAsStringLiteral(val, args.Usage.HasFlag(StringUsage.DateTimeQuotationFlag));
            }
            else if (fmt.Style == FormatStyle.DayOfWeek && isInteger && 0 <= val && val <= 6) {
                return DecimalToDayOfWeekString(val, args);
            }
            else {
                return DecimalToCStyleDecimalLiteral(val, args, true);
            }
        }

        public static string DecimalToCStyleDecimalLiteral(decimal val, ToStringArgs args, bool allowENotation) {
            if (val == 0) return "0";

            var sbDecFormat = new StringBuilder("0.");
            for (int i = 0; i < args.Settings.DecimalLengthToDisplay; i++) {
                sbDecFormat.Append('#');
            }
            var decFormat = sbDecFormat.ToString();

            int exp = MathEx.FLog10Abs(val);
            if (allowENotation && args.Settings.ENotationEnabled && exp >= args.Settings.ENotationExpPositiveMin) {
                if (args.Settings.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val / MathEx.Pow10(exp);
                return frac.ToString(decFormat) + "e" + exp;
            }
            else if (allowENotation && args.Settings.ENotationEnabled && exp <= args.Settings.ENotationExpNegativeMax) {
                if (args.Settings.ENotationAlignment) {
                    exp = (int)Math.Floor((double)exp / 3) * 3;
                }
                var frac = val * MathEx.Pow10(-exp);
                return frac.ToString(decFormat) + "e" + exp;
            }
            else {
                return val.ToString(decFormat);
            }
        }

        public static string DecimalToCStyleBinaryLiteral(decimal fval , ToStringArgs args, bool allowENotation) {
            var radix = args.Format.Radix;
            int radixBase = radix.ToBaseNumber();

            var ival = Math.Truncate(fval);

            // 10進表記、かつ指数表記対象に該当する場合はデフォルトの数値表現を使う
            int exp = MathEx.FLog10Abs(fval);
            bool enotation =
                radix == Radix.Decimal && args.Settings.ENotationEnabled &&
                (exp >= args.Settings.ENotationExpPositiveMin || exp <= args.Settings.ENotationExpNegativeMax);

            if (fval != ival || ival < long.MinValue || long.MaxValue < ival || enotation) {
                // デフォルトの数値表現
                return DecimalToCStyleDecimalLiteral(fval, args, allowENotation);
            }
            else {
                switch(radix) {
                    case Radix.Decimal:
                        // 10進表現
                        var abs64val = ival >= 0 ? (decimal)ival : -(decimal)ival;
                        var ret = Convert.ToString(abs64val);
                        if (ival < 0) ret = "-" + ret;
                        return ret;
                    case Radix.Hexadecimal:
                    case Radix.Binary:
                    case Radix.Octal:
                        return CStyleBinary.GetPrefix(radix) + Convert.ToString((Int64)ival, radixBase);
                    default: throw new NotSupportedException();
                }
            }
        }

        public static string DecimalToDayOfWeekString(decimal val, ToStringArgs args) {
            if (val.IsInteger() && 0 <= val && val <= 6) {
                switch ((DayOfWeek)(int)val) {
                    case DayOfWeek.Sunday: return BuiltInConstants.Sunday.Name.Text;
                    case DayOfWeek.Monday: return BuiltInConstants.Monday.Name.Text;
                    case DayOfWeek.Tuesday: return BuiltInConstants.Tuesday.Name.Text;
                    case DayOfWeek.Wednesday: return BuiltInConstants.Wednesday.Name.Text;
                    case DayOfWeek.Thursday: return BuiltInConstants.Thursday.Name.Text;
                    case DayOfWeek.Friday: return BuiltInConstants.Friday.Name.Text;
                    case DayOfWeek.Saturday: return BuiltInConstants.Saturday.Name.Text;
                    default: throw new InvalidOperationException();
                }
            }
            else {
                return DecimalToCStyleDecimalLiteral(val, args, true);
            }
        }

        // todo 暫定実装 Formatter.ApFixedToString
        public static string ApFixedToString(apfixed val, ToStringArgs args) {
            var radix = args.Format.Radix;
            if (radix == Radix.Decimal) {
                // todo Formatter.ApFixedToString() 10進のときの多ビット対応
                return DecimalToCStyleDecimalLiteral((decimal)val, ToStringArgs.ForLiteral(), true);
            }
            else {
                var sb = new StringBuilder(CStyleBinary.GetPrefix(radix));
                var radixBase = radix.ToBaseNumber();
                if (args.Format.Options.HasFlag(FormatOption.ApFixedWithPoint)) {
                    val.ToBinaryStringWithPoint(radixBase, sb);
                }
                else {
                    val.ToRawBinaryString(radixBase, sb);
                }
                sb.Append(FixedPointFormatToString(val.Format));
                return sb.ToString();
            }
        }

        public static string FixedPointFormatToString(FixedPointFormat fpfmt) {
            if (fpfmt.FracWidth > 0) {
                return (fpfmt.Signed ? 's' : 'u') + fpfmt.IntWidth.ToString() + '.' + fpfmt.FracWidth.ToString();
            }
            else {
                return (fpfmt.Signed ? 's' : 'u') + fpfmt.IntWidth.ToString();
            }
        }

        public static string FracToString(Frac val) {
            var args = ToStringArgs.ForLiteral();
            return 
                DecimalToCStyleDecimalLiteral(val.Nume, args, false) + OpCodes.Frac.GetSymbol() +
                DecimalToCStyleDecimalLiteral(val.Deno, args, false);
        }

        public static string StringToCStyleLiteral(string val, ToStringArgs args) {
            var quotation = args.Usage.HasFlag(StringUsage.StringQuotationFlag);
            var sb = new StringBuilder();
            if (quotation) sb.Append('"');
            foreach (var c in val) {
                sb.Append(EscapeChar(c, true, args.Usage));
            }
            if (quotation) sb.Append('"');
            return sb.ToString();
        }

        public static string CharToCStyleLiteralString(char val, StringUsage flags) {
            if (flags.HasFlag(StringUsage.StringQuotationFlag)) {
                var sb = new StringBuilder();
                sb.Append("'");
                sb.Append(EscapeChar(val, false, flags));
                sb.Append("'");
                return sb.ToString();
            }
            else {
                return EscapeChar(val, false, flags);
            }
        }

        public static string EscapeChar(char c, bool stringMode, StringUsage flag) {
            if (flag.HasFlag(StringUsage.CharEscapingFlag)) {
                return CStyleEscaping.Escape(c);
            }
            else {
                return c.ToString();
            }
        }

        public static string ToLiteralString(bool val) {
            return val ? Keyword.True.String : Keyword.False.String;
        }

        public static string ListToString(IEnumerable collection, ToStringArgs args) {
            var sb = new StringBuilder();
            bool following = false;
            sb.Append("[");
            foreach (var val in collection) {
                if (following) sb.Append(", ");
                sb.Append(ObjectToString(val, new ToStringArgs(args, StringUsage.ForLiteral)));
                following = true;
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string ToDateTimeLiteralString(decimal t, StringUsage flags) {
            var minus = t < 0;
            if (minus) t = -t;
            var ts = TimeSpan.FromSeconds((double)t);
            var days = t / (24 * 60 * 60);
            var daysOnly = days.IsInteger();

            var quotation = flags.HasFlag(StringUsage.DateTimeQuotationFlag);
            var sb = new StringBuilder();
            if (quotation) sb.Append('#');
            sb.Append(minus ? '-' : '+');
            if (daysOnly) {
                sb.Append(days.ToString("0"));
            }
            else {
                string fmt = @"h\:mm\:ss";
                if (t >= 24 * 60 * 60) fmt = @"d\." + fmt;
                if (t.IsInteger()) fmt = fmt + @"\.fff";
                sb.Append(ts.ToString(fmt, CultureInfo.InvariantCulture));
            }
            if (quotation) sb.Append('#');
            return sb.ToString();
        }
    }
}
