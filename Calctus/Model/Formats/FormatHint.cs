using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    class FormatHint {
        public static readonly FormatHint Default = new FormatHint(FormatStyle.Default, Radix.Decimal);
        public static readonly FormatHint Hexadecimal = new FormatHint(FormatStyle.Default, Radix.Hexadecimal);
        public static readonly FormatHint Octal = new FormatHint(FormatStyle.Default, Radix.Octal);
        public static readonly FormatHint Binary = new FormatHint(FormatStyle.Default, Radix.Binary);
        public static readonly FormatHint Character = new FormatHint(FormatStyle.Character, Radix.Hexadecimal);
        public static readonly FormatHint SiPrefixed = new FormatHint(FormatStyle.SiPrefixed);
        public static readonly FormatHint BinaryPrefixed = new FormatHint(FormatStyle.BinaryPrefixed);
        public static readonly FormatHint DayOfWeek = new FormatHint(FormatStyle.DayOfWeek);
        public static readonly FormatHint DateTime = new FormatHint(FormatStyle.DateTime); // todo 廃止 FormatHint.DateTime
        public static readonly FormatHint TimeSpan = new FormatHint(FormatStyle.TimeSpan); // todo 廃止? FormatHint.TimeSpan
        public static readonly FormatHint WebColor = new FormatHint(FormatStyle.WebColor, Radix.Hexadecimal);

        private static readonly Dictionary<string, FormatHint> _instances = new Dictionary<string, FormatHint>();
        public static FormatHint From(FormatStyle style = FormatStyle.Default, Radix radix = Radix.Decimal, FormatOption options = FormatOption.None) {
            var key = (int)style + "," + (int)radix + "," + (int)options;
            FormatHint ret;
            if (!_instances.TryGetValue(key, out ret)) {
                var field = typeof(FormatHint).GetFields()
                    .FirstOrDefault(p => p.IsStatic && p.FieldType.Equals(typeof(FormatHint)));
                if (field != null) {
                    var cand = (FormatHint)field.GetValue(null);
                    if (cand.Style == style && cand.Radix == radix && cand.Options == options) {
                        ret = cand;
                    }
                }
                if (ret == null) {
                    ret = new FormatHint(style, radix, options);
                }
                _instances[key] = ret;
            }
            return ret;
        }

        public readonly FormatStyle Style;
        public readonly Radix Radix;
        public readonly FormatOption Options;

        private FormatHint(FormatStyle style, Radix radix = Radix.Decimal, FormatOption options = FormatOption.None) {
            this.Style = style;
            this.Radix = radix;
            this.Options = options;
        }

        public override int GetHashCode() => ((int)Style * 0x1000000) + ((int)Radix * 0x10000) + (int)Options;

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public bool Equals(FormatHint other) =>
            (this.Style == other.Style) && (this.Radix == other.Radix) && (this.Options == other.Options);

        public override string ToString() {
            var ret = Style.ToString();
            if (Radix != Radix.Decimal) ret += ", " + Radix;
            if (Options != FormatOption.None) ret += ", " + Options;
            return "{" + ret + "}";
        }
    }

    static class FormatHintExtensions {
        public static FormatHint OrDefault(this FormatHint fmt, FormatHint defaultFmt = null) {
            if (defaultFmt == null) defaultFmt = FormatHint.Default;
            return fmt != null ? fmt : defaultFmt;
        }
    }
}
