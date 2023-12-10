using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Expressions {
    class ExtendExpr : Expr {
        public readonly Expr SeedArray;
        public readonly Token ArrayName;
        public readonly Expr Generator;
        public readonly Expr Count;

        public ExtendExpr(Token keyword, Token arrayName, Expr seed, Expr generator, Expr count) : base(keyword) {
            this.ArrayName = arrayName;
            this.SeedArray = seed;
            this.Generator = generator;
            this.Count = count;
        }

        protected override Val OnEval(EvalContext e) {
            var seedVal = SeedArray.Eval(e);
            var list = ((Val[])seedVal.Raw).ToList();
            var countVal = Count.Eval(e).AsReal;
            if (!RMath.IsInteger(countVal)) throw new CalctusError("Count must be integer.");
            if (countVal < 0 || ArrayVal.MaxLength < countVal) throw new CalctusError("Count must be integer.");

            var scope = new EvalContext(e);
            for (int i = 0; i < countVal; i++) {
                scope.Ref(ArrayName.Text, true).Value = new ArrayVal(list.ToArray());
                list.Add(Generator.Eval(scope));
            }
            return new ArrayVal(list.ToArray(), seedVal.FormatHint);
        }
    }
}
