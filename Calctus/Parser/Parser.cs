using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Parser {
    class Parser {
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

            vals.Push(Operand());

            while (!EndOfExpr) {
                var rightOp = new BinaryOp(Read(), null, null);
                while (ops.Count > 0 && ops.Peek().Method.ComparePriority(rightOp.Method) == OpPriorityDir.Left) {
                    var b = vals.Pop();
                    var a = vals.Pop();
                    vals.Push(new BinaryOp(ops.Pop().Token, a, b));
                }
                ops.Push(rightOp);
                vals.Push(Operand());
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
                throw new SyntaxError(_lex.Position, "Operator missing");
            }
            return expr;
        }

        public Expr Operand() {
            Token tok;
            if (ReadIf("(")) {
                var ret = Expr();
                Expect(")");
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
                throw new SyntaxError(_lex.Position, "Operand missing");
            }
            else {
                throw new SyntaxError(_lex.Position, "Invalid operand: '" + Peek() + "'");
            }
        }

        public Token Peek() {
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
            => Eos || (Peek().Text == ")") || (Peek().Text == ",");

        public void Expect(string s) {
            if (!ReadIf(s)) {
                throw new SyntaxError(_lex.Position, "Missing: '" + s + "'");
            }
        }

        public void Expect(TokenType typ, out Token tok) {
            if (!ReadIf(typ, out tok)) {
                throw new SyntaxError(_lex.Position, "Missing: '" + typ + "'");
            }
        }
    }
}
