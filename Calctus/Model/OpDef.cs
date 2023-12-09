using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.Model {

    /// <summary>演算子の定義</summary> 
    class OpDef {
        public readonly OpType Type;
        public readonly OpPriorityDir PriorirtyDir;
        public readonly int Priority;
        public readonly string Symbol;
        public OpDef(OpType typ, OpPriorityDir pdir, int p, string sym) {
            this.Type = typ;
            this.PriorirtyDir = pdir;
            this.Priority = p;
            this.Symbol = sym;
        }

        public OpDef(OpType typ,  int p, string sym) : this(typ, OpPriorityDir.Left, p, sym) { }

        // ネイティブ演算子の定義
        public static OpDef Plus = new OpDef(OpType.Unary, 90, "+");
        public static OpDef ArithInv = new OpDef(OpType.Unary, 90, "-");
        public static OpDef LogicNot = new OpDef(OpType.Unary, 90, "!");
        public static OpDef BitNot = new OpDef(OpType.Unary, 90, "~");

        public static OpDef Frac = new OpDef(OpType.Binary, 70, "$");

        public static OpDef Pow = new OpDef(OpType.Binary, 62, "^");
        public static OpDef Mul = new OpDef(OpType.Binary, 61, "*");
        public static OpDef Div = new OpDef(OpType.Binary, 61, "/");
        public static OpDef IDiv = new OpDef(OpType.Binary, 61, "//");
        public static OpDef Mod = new OpDef(OpType.Binary, 61, "%");
        public static OpDef Add = new OpDef(OpType.Binary, 60, "+");
        public static OpDef Sub = new OpDef(OpType.Binary, 60, "-");

        public static OpDef LogicShiftL = new OpDef(OpType.Binary, 50, "<<");
        public static OpDef LogicShiftR = new OpDef(OpType.Binary, 50, ">>");
        public static OpDef ArithShiftL = new OpDef(OpType.Binary, 50, "<<<");
        public static OpDef ArithShiftR = new OpDef(OpType.Binary, 50, ">>>");

        public static OpDef Grater = new OpDef(OpType.Binary, 41, ">");
        public static OpDef GraterEqual = new OpDef(OpType.Binary, 41, ">=");
        public static OpDef Less = new OpDef(OpType.Binary, 41, "<");
        public static OpDef LessEqual = new OpDef(OpType.Binary, 41, "<=");
        public static OpDef Equal = new OpDef(OpType.Binary, 40, "==");
        public static OpDef NotEqual = new OpDef(OpType.Binary, 40, "!=");

        public static OpDef BitAnd = new OpDef(OpType.Binary, 34, "&");
        public static OpDef BitXor = new OpDef(OpType.Binary, 33, "+|");
        public static OpDef BitOr = new OpDef(OpType.Binary, 32, "|");

        public static OpDef LogicAnd = new OpDef(OpType.Binary, 31, "&&");
        public static OpDef LogicOr = new OpDef(OpType.Binary, 30, "||");

        public static OpDef ExclusiveRange = new OpDef(OpType.Binary, 20, "..");
        public static OpDef InclusiveRange = new OpDef(OpType.Binary, 20, "..=");

        // ここに 3項演算子
        public static OpDef Assign = new OpDef(OpType.Binary, OpPriorityDir.Right, 0, "=");

        /// <summary>ネイティブ演算子の一覧</summary>
        public static OpDef[] NativeOperators = EnumOperators().ToArray();
        private static IEnumerable<OpDef> EnumOperators() {
            return
                typeof(OpDef)
                .GetFields()
                .Where(p => p.IsStatic && (p.FieldType == typeof(OpDef)))
                .Select(p => (OpDef)p.GetValue(null));
        }

        public OpPriorityDir ComparePriority(OpDef right) {
            var left = this;
            if (left.Priority > right.Priority) {
                return OpPriorityDir.Left;
            }
            else if (left.Priority < right.Priority) {
                return OpPriorityDir.Right;
            }
            else {
                return left.PriorirtyDir;
            }
        }

        /// <summary>演算子の一覧</summary>
        public static IEnumerable<OpDef> AllOperators => NativeOperators;

        /// <summary>演算子記号の一覧</summary>
        public static string[] AllOperatorSymbols = NativeOperators.Select(p=>p.Symbol).Distinct().ToArray();

        /// <summary>指定された条件にマッチする演算子定義を返す</summary>
        public static bool Match(OpType typ, string s, out OpDef op) {
            op = AllOperators.FirstOrDefault(p => p.Type == typ && p.Symbol == s);
            return op != null;
        }

        /// <summary>指定された条件にマッチする演算子定義を返す</summary>
        public static OpDef Match(OpType typ, Token tok) {
            var ops = AllOperators.Where(p=>p.Symbol == tok.Text).ToArray();
            if (ops.Length == 0) {
                throw new LexerError(tok.Position, tok + " is not operator");
            }
            var op = ops.FirstOrDefault(p => p.Type == typ);
            if (op == null) {
                throw new LexerError(tok.Position, tok + " is not " + typ.ToString());
            }
            return op;
        }
    }
}
