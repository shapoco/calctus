using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.UI {
    public enum RadixMode {
        Auto, Dec, Hex, Bin, Oct
    }

    [Flags]
    public enum PasteOption {
        Default = 0, 
        WithoutCommas = 1,
    }
}
