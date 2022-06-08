using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Parser
{
    class Parser
    {
        private Token _buff = null;
        private Lexer _lex;
        public Lexer Lexer => _lex;

        public static Expr Parse(string s) => new Parser(s).Pop(last: true);

        public Parser(string exprStr) {
            _lex = new Lexer(exprStr);
        }

        public Expr Pop(bool last = true) {
            return Expr(last);
        }

        public Expr Expr(bool last = false) {
            var vals = new Stack<Expr>();
            var ops = new Stack<BinaryOp>();

            vals.Push(OperandWithUnit());

            while (!EndOfExpr) {
                var rightOp = new BinaryOp(Read(), null, null);
                //while (ops.Count > 0 && ops.Peek().Method.Priority >= rightOp.Method.Priority) {
                while (ops.Count > 0 && ops.Peek().Method.ComparePriority(rightOp.Method) == OpPriorityDir.Left) {
                    var b = vals.Pop();
                    var a = vals.Pop();
                    vals.Push(new BinaryOp(ops.Pop().Token, a, b));
                }
                ops.Push(rightOp);
                vals.Push(OperandWithUnit());
            }

            while (ops.Count > 0) {
                var b = vals.Pop();
                var a = vals.Pop();
                vals.Push(new BinaryOp(ops.Pop().Token, a, b));
            }

            if (vals.Count != 1) {
                throw new SyntaxError(_lex.Position, "Internal Error: stack broken");
            }

            var expr = vals.Pop();
            if (last && !Eos) {
                throw new SyntaxError(_lex.Position, "operator missing");
            }
            return expr;
        }

        public Expr OperandWithUnit() {
            var operand = Operand();
            if (ReadIf("[", out Token tok)) {
                var ret = UnitExpr();
                Expect("]");
                return new UnitifyOp(tok, operand, ret);
            }
            else {
                return operand;
            }
        }

        public Expr Operand() {
            Token tok;
            if (ReadIf("(")) {
                var ret = Expr();
                Expect(")");
                return ret;
            }
            else if (ReadIf("[")) {
                var ret = Matrix();
                Expect("]");
                return ret;
            }
            else if (ReadIf(TokenType.NumericLiteral, out tok)) {
                return new Number(tok);
            }
            else if (ReadIf(TokenType.Symbol, out tok)) {
                return new UnaryOp(Operand(), tok);
            }
            else if (ReadIf(TokenType.Word, out tok)) {
                var args = new List<Expr>();
                if (ReadIf("(")) {
                    args.Add(Expr());
                    while (ReadIf(",")) {
                        args.Add(Expr());
                    }
                    Expect(")");
                    return new FuncRef(tok, args.ToArray());
                }
                else {
                    return new VarRef(tok);
                }
            }
            else if (EndOfExpr) {
                throw new SyntaxError(_lex.Position, "missing operand");
            }
            else {
                throw new SyntaxError(_lex.Position, "invalid operand: '" + Peek() + "'");
            }
        }

        public Expr Matrix() {
            var elms = new List<Expr>();

            // 1行目
            int n = 0;
            do {
                elms.Add(Expr());
                n += 1;
            } while (ReadIf(","));

            // 2行目以降
            int m = 1;
            while (ReadIf(";")) {
                m += 1;
                elms.Add(Expr());
                for (int i = 1; i < n; i++) {
                    Expect(",");
                    elms.Add(Expr());
                }
            }

            return new Matrix(m, n, elms.ToArray());
        }

        public Expr UnitExpr() {
            Token tok;
            var a = Unit();
            while (ReadIf("*", out tok)) {
                var b = Unit();
                a = new UnitMultOp(tok, a, b);
            }
            while (ReadIf("/", out tok)) {
                var b = Unit();
                a = new UnitDivOp(tok, a, b);
            }
            return a;
        }

        public Expr Unit() {
            Expect(TokenType.Word, out Token tok);
            return new UnitRef(tok);
        }

        public Token Peek() {
            //_lex.SkipWhite();
            if (_buff == null) {
                _buff = _lex.Pop();
            }
            return _buff;
        }

        public Token Read() {
            if (_buff != null) {
                var ret = _buff;
                _buff = null;
                return ret;
            }
            else {
                return Peek();
            }
        }

        public bool ReadIf(string s) => ReadIf(s, out _);

        public bool ReadIf(string s, out Token tok) {
            tok = Peek();
            if (tok.Text == s) {
                Read();
                return true;
            }
            else {
                return false;
            }
        }

        public bool ReadIf(TokenType typ, out Token tok) {
            tok = Peek();
            if (tok.Type == typ) {
                Read();
                return true;
            }
            else {
                return false;
            }
        }

        public bool Eos 
            => Peek().Type == TokenType.Eos;

        public bool EndOfExpr 
            => Eos || (Peek().Text == ")") || (Peek().Text == "]") || (Peek().Text == ";") || (Peek().Text == ",");

        public void Expect(string s) {
            if (!ReadIf(s)) {
                throw new SyntaxError(_lex.Position, "missing: '" + s + "'");
            }
        }

        public void Expect(TokenType typ, out Token tok) {
            if (!ReadIf(typ, out tok)) {
                throw new SyntaxError(_lex.Position, "missing: '" + typ + "'");
            }
        }

        public static void Test(string s) {
            var expr = Parser.Parse(s);
            Console.Write(s + "=>");
            Console.Write(expr.ToString() + " = ");
            Console.WriteLine(expr.Eval(new EvalContext()));
        }
    }
}
