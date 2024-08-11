using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    struct IntFormat {
        public readonly bool Signed;
        public readonly int Width;

        public IntFormat(bool signed, int width) {
#if DEBUG
            if (width < 1) Log.Here().ArgException(nameof(width));
#endif
            this.Signed = signed;
            this.Width = width;
        }

        public override string ToString() {
            return (Signed ? "s" : "u") + Width;
        }
    }
}
