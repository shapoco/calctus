using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Values {
    interface ICollectionVal {
        Array ToRawArray();
        Val[] ToValArray();
        ListVal ToListVal();
        int Length { get; }
    }
}
