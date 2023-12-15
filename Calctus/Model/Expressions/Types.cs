using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Expressions {
    enum OpType {
        None,
        Unary,
        Binary,
    }

    enum OpPriorityDir {
        Left,
        Right
    }
}
