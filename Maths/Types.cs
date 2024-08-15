using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    enum ShiftMode {
        Auto,
        Logical,
        Arithmetic,
    }

    [Flags]
    enum CastOptions {
        Strict = 0,
        AllowDegrade = (1 << 0),
        AllowOverflow = (1 << 1),
        Floor = (1 << 2),
        Round = (1 << 3),
        Clip = (1 << 4),
        Force = AllowDegrade | AllowOverflow | Clip,
    }
}
