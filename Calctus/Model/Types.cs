﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    enum VariadicMode {
        None,
        Array,
        Flatten,
    }

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
