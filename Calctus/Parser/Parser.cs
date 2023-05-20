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
        private Token _lastToken = null;
        private TokenQueue _queue;
        public TokenQueue Queue => _queue;

        public static Expr Parse(string s) => Parse(new Lexer(s).PopToEnd());
        public static Expr Parse(TokenQueue q) => new Parser(q).Pop(last: true);

        public Parser(TokenQueue queue) {
            _queue = queue;
        }

        public Expr Pop(bool last = true) {
            return Expr(last);
        }

        public Expr Expr(bool last = false) {
            var vals = new Stack<Expr>();
            var ops = new Stack<BinaryOp>();

            vals.Push(UnaryOpExpr());

            while (!EndOfExpr) {
                var rightOp = new BinaryOp(Read(), null, null);
                while (ops.Count > 0 && ops.Peek().Method.ComparePriority(rightOp.Method) == OpPriorityDir.Left) {
                    var b = vals.Pop();
                    var a = vals.Pop();
                    vals.Push(new BinaryOp(ops.Pop().Token, a, b));
                }
                ops.Push(rightOp);
                vals.Push(UnaryOpExpr());
            }

            while (ops.Count > 0) {
                var b = vals.Pop();
                var a = vals.Pop();
                vals.Push(new BinaryOp(ops.Pop().Token, a, b));
            }

            if (vals.Count != 1) {
                throw new CalctusError("Internal Error: stack broken");
            }

            var expr = vals.Pop();
            if (last && !Eos) {
                throw new ParserError(_lastToken, "Operator missing");
            }
            return expr;
        }

        public Expr UnaryOpExpr() {
            var peek = Peek();
            if (ReadIf(TokenType.OperatorSymbol, out Token tok)) {
                return new UnaryOp(UnaryOpExpr(), tok);
            }
            else {
                return ElemRefExpr();
            }
        }

        public Expr ElemRefExpr() {
            var operand = Operand();
            if (ReadIf("[", out Token tok)) {
                var index = Expr();
                Expect("]");
                return new ElemRef(tok, operand, index);
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
            else if (ReadIf("[", out tok)) {
                var elms = new List<Expr>();
                if (!ReadIf("]")) {
                    elms.Add(Expr());
                    while (ReadIf(",")) {
                        elms.Add(Expr());
                    }
                    Expect("]");
                }
                return new ArrayExpr(tok, elms.ToArray());
            }
            else if (ReadIf(TokenType.NumericLiteral, out tok)) {
                return new Number(tok);
            }
            else if (ReadIf(TokenType.BoolLiteral, out tok)) {
                return new BoolLiteral(tok);
            }
            else if (ReadIf(TokenType.Word, out tok)) {
                var args = new List<Expr>();
                if (ReadIf("(")) {
                    if (!ReadIf(")")) {
                        args.Add(Expr());
                        while (ReadIf(",")) {
                            args.Add(Expr());
                        }
                        Expect(")");
                    }
                    return new FuncRef(tok, args.ToArray());
                }
                else {
                    return new VarRef(tok);
                }
            }
            else if (EndOfExpr) {
                throw new ParserError(_lastToken, "Operand missing");
            }
            else {
                var nextToken = Peek();
                throw new ParserError(nextToken, "Invalid operand: '" + nextToken + "'");
            }
        }

        public Token Peek() {
            if (_buff == null) {
                _buff = _queue.PopFront();
            }
            return _buff;
        }

        public Token Read() {
            if (_buff != null) {
                _lastToken = _buff;
                _buff = null;
                return _lastToken;
            }
            else {
                return _lastToken = Peek();
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
            => Eos || (Peek().Text == ")") || (Peek().Text == "]") || (Peek().Text == ",");

        public void Expect(string s) {
            if (!ReadIf(s)) {
                throw new ParserError(Peek(), "Missing: '" + s + "'");
            }
        }

        public void Expect(TokenType typ, out Token tok) {
            if (!ReadIf(typ, out tok)) {
                throw new ParserError(Peek(), "Missing: '" + typ + "'");
            }
        }
    }
}
