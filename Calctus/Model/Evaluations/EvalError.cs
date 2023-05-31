using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalError : CalctusError {
        public readonly EvalContext Context;
        public readonly Token Token;
        public EvalError(EvalContext ctx, Token tok, string msg) : base(msg) {
            this.Context = ctx;
            this.Token = tok;
        }
    }
}
