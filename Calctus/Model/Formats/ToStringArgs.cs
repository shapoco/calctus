using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class ToStringArgs {
        public readonly FormatHint Format;
        public readonly FormatSettings Settings;
        public readonly StringUsage Usage;

        public static ToStringArgs ForLiteral(FormatHint fmt = null)
            => new ToStringArgs(fmt.OrDefault(), new FormatSettings(), StringUsage.ForLiteral);

        public static ToStringArgs ForDisplay()
            => new ToStringArgs(FormatHint.Default, new FormatSettings(), StringUsage.ForDisplay);

        public static ToStringArgs ForValue(EvalContext e, FormatHint fmt)
            => new ToStringArgs(fmt, e.FormatSettings, StringUsage.ForValue);

        public ToStringArgs(ToStringArgs src, FormatHint fmt) : this(fmt, src.Settings, src.Usage) { }
        public ToStringArgs(ToStringArgs src, StringUsage usage) : this(src.Format, src.Settings, usage) { }
        public ToStringArgs(FormatHint fmt, FormatSettings settings, StringUsage usage) {
            this.Format = fmt;
            this.Settings = settings;
            this.Usage = usage;
        }
    }
}
