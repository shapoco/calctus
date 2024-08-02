using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Functions {
    enum FuncMatchLevel {
        NameUnmatched = 0,
        ArgumentsUnmatched = 1,
        Matched = 2,
    }

    class FuncMatch {
        public readonly FuncDef Func;
        public readonly FuncMatchLevel MatchLevel;
        public readonly string Message;

        public FuncMatch(FuncDef func, FuncMatchLevel level, string message = "") {
            Func = func;
            MatchLevel = level;
            Message = message;
        }
    }
}
