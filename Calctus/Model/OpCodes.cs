using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model {
    enum OpCodes {
        Plus,
        ArithInv,
        LogicNot,
        BitNot,
        Frac,
        Pow,
        Mul,
        Div,
        IDiv,
        Mod,
        Add,
        Sub,
        LogicShiftL,
        LogicShiftR,
        ArithShiftL,
        ArithShiftR,
        Grater,
        GraterEqual,
        Less,
        LessEqual,
        Equal,
        NotEqual,
        BitAnd,
        BitXor,
        BitOr,
        LogicAnd,
        LogicOr,
        ExclusiveRange,
        InclusiveRange,
        Arrow,
        Assign,
    }

    static class OpExtension {
        public static OpInfo Info(this OpCodes op) => OpInfo.Items[(int)op];
        public static string GetSymbol(this OpCodes op) => OpInfo.Items[(int)op].Symbol;
        public static int Priority(this OpCodes op) => OpInfo.Items[(int)op].Priority;
        public static OpType Type(this OpCodes op) => OpInfo.Items[(int)op].Type;
        public static OpPriorityDir ComparePriority(this OpCodes op, OpCodes right)
            => OpInfo.Items[(int)op].ComparePriority(right);
    }

    class OpInfo {
        public readonly OpCodes Code;
        public readonly OpType Type;
        public readonly int Priority;
        public readonly string Symbol;
        public readonly OpPriorityDir PriorirtyDir;

        public OpInfo(OpType type, OpCodes Code, int priority, string synbol, OpPriorityDir dir = OpPriorityDir.Left) {
            this.Type = type;
            this.Code = Code;
            this.Priority = priority;
            this.Symbol = synbol;
            this.PriorirtyDir = dir;
        }

        public OpPriorityDir ComparePriority(OpCodes rightOp) {
            var left = this;
            var right = Items[(int)rightOp];
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

        public static OpInfo From(OpType type, Token tok) {
            OpInfo near = null;
            foreach(var op in Items) {
                if (op.Symbol == tok.Text) {
                    if (op.Type == type) return op;
                    near = op;
                }
            }
            if (near == null) {
                throw new ParserError(tok, "Operator is expected.");
            }
            else {
                throw new ParserError(tok, tok + " is not " + type.ToString());
            }
        }

        public static readonly OpInfo[] Items = enumOpCodes()
            .Select(p => getInfo(p))
            .ToArray();

        private static IEnumerable<OpCodes> enumOpCodes() {
            foreach (var op in Enum.GetValues(typeof(OpCodes))) {
                yield return (OpCodes)op;
            }
        }
 
        private static OpInfo getInfo(OpCodes op) {
            switch (op) {
                case OpCodes.Plus: return new OpInfo(OpType.Unary, op, 90, "+");
                case OpCodes.ArithInv: return new OpInfo(OpType.Unary, op, 90, "-");
                case OpCodes.LogicNot: return new OpInfo(OpType.Unary, op, 90, "!");
                case OpCodes.BitNot: return new OpInfo(OpType.Unary, op, 90, "~");
                case OpCodes.Frac: return new OpInfo(OpType.Binary, op, 70, ":");
                case OpCodes.Pow: return new OpInfo(OpType.Binary, op, 62, "^");
                case OpCodes.Mul: return new OpInfo(OpType.Binary, op, 61, "*");
                case OpCodes.Div: return new OpInfo(OpType.Binary, op, 61, "/");
                case OpCodes.IDiv: return new OpInfo(OpType.Binary, op, 61, "//");
                case OpCodes.Mod: return new OpInfo(OpType.Binary, op, 61, "%");
                case OpCodes.Add: return new OpInfo(OpType.Binary, op, 60, "+");
                case OpCodes.Sub: return new OpInfo(OpType.Binary, op, 60, "-");
                case OpCodes.LogicShiftL: return new OpInfo(OpType.Binary, op, 50, "<<");
                case OpCodes.LogicShiftR: return new OpInfo(OpType.Binary, op, 50, ">>");
                case OpCodes.ArithShiftL: return new OpInfo(OpType.Binary, op, 50, "<<<");
                case OpCodes.ArithShiftR: return new OpInfo(OpType.Binary, op, 50, ">>>");
                case OpCodes.Grater: return new OpInfo(OpType.Binary, op, 41, ">");
                case OpCodes.GraterEqual: return new OpInfo(OpType.Binary, op, 41, ">=");
                case OpCodes.Less: return new OpInfo(OpType.Binary, op, 41, "<");
                case OpCodes.LessEqual: return new OpInfo(OpType.Binary, op, 41, "<=");
                case OpCodes.Equal: return new OpInfo(OpType.Binary, op, 40, "==");
                case OpCodes.NotEqual: return new OpInfo(OpType.Binary, op, 40, "!=");
                case OpCodes.BitAnd: return new OpInfo(OpType.Binary, op, 34, "&");
                case OpCodes.BitXor: return new OpInfo(OpType.Binary, op, 33, "+|");
                case OpCodes.BitOr: return new OpInfo(OpType.Binary, op, 32, "|");
                case OpCodes.LogicAnd: return new OpInfo(OpType.Binary, op, 31, "&&");
                case OpCodes.LogicOr: return new OpInfo(OpType.Binary, op, 30, "||");
                case OpCodes.ExclusiveRange: return new OpInfo(OpType.Binary, op, 20, "..");
                case OpCodes.InclusiveRange: return new OpInfo(OpType.Binary, op, 20, "..=");
                case OpCodes.Arrow: return new OpInfo(OpType.Binary, op, 10, "=>");
                case OpCodes.Assign: return new OpInfo(OpType.Binary, op, 0, "=", OpPriorityDir.Right);
                default: throw new NotImplementedException();
            }
        }
    }

}
