using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    class ToStringArgs {
        public readonly FormatFlags Flags;
        public readonly FormatSettings Settings;
        public readonly StringUsage Usage;

        public static ToStringArgs ForLiteral()
            => new ToStringArgs(FormatFlags.Default, new FormatSettings(), StringUsage.ForLiteral);

        public static ToStringArgs ForDisplay()
            => new ToStringArgs(FormatFlags.Default, new FormatSettings(), StringUsage.ForDisplay);

        public static ToStringArgs ForValue(EvalContext e, FormatFlags flags)
            => new ToStringArgs(flags, e.FormatSettings, StringUsage.ForValue);

        public ToStringArgs(ToStringArgs src, FormatFlags flags) : this(flags, src.Settings, src.Usage) { }
        public ToStringArgs(ToStringArgs src, StringUsage usage) : this(src.Flags, src.Settings, usage) { }
        public ToStringArgs(FormatFlags flags, FormatSettings settings, StringUsage usage) {
            this.Flags = flags;
            this.Settings = settings;
            this.Usage = usage;
        }
    }
}
