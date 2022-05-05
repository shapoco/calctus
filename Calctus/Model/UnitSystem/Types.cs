using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    [Flags]
    enum UnitEnumMode {
        Named = (1 << 0),
        Dimension = (1 << 1),
        Complete = (1 << 16)
    }
}
