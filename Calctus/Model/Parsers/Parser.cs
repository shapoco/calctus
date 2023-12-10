using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Expressions;

namespace Shapoco.Calctus.Model.Parsers {
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
            Token tok;
            if (ReadIf("def", out tok)) {
                return Def(tok);
            }
            else {
                return Expr(last);
            }
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

            // 条件演算子
            if (ReadIf("?", out Token tok)) {
                var trueVal = Expr();
                Expect(":");
                var falseVal = Expr();
                expr = new CondOp(tok, expr, trueVal, falseVal);
            }

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
                var from = Expr();
                var to = from;
                if (ReadIf(":")) {
                    to = Expr();
                }
                Expect("]");
                return new PartRef(tok, operand, from, to);
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
            else if (ReadIf("solve", out tok)) {
                return Solve(tok);
            }
            else if (ReadIf("extend", out tok)) {
                return Extend(tok);
            }
            else if (ReadIf("plot", out tok)) {
                return Plot(tok);
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

        public Expr Def(Token first) {
            Expect(TokenType.Word, out Token name);
            Expect("(");
            var args = new List<ArgDef>();
            var vecArgIndex = -1;
            if (!ReadIf(")")) {
                do {
                    Expect(TokenType.Word, out Token arg);
                    if (ReadIf("*", out Token aster)) {
                        if (vecArgIndex >= 0) throw new ParserError(aster, "Only one argument is vectorizable.");
                        vecArgIndex = args.Count;
                    }
                    args.Add(new ArgDef(arg));
                } while (ReadIf(","));
                Expect(")");
            }
            Expect("=");
            var body = Expr(true);
            return new UserFuncExpr(first, name, args.ToArray(), vecArgIndex, body);
        }

        public Expr Solve(Token first) {
            Expect("(");
            var equation = Expr(false);
            Expect(",");
            Expect(TokenType.Word, out Token variant);
            Expr param0 = null;
            Expr param1 = null;
            if (ReadIf(",")) {
                param0 = Expr(false);
                if (ReadIf(",")) {
                    param1 = Expr(false);
                }
            }
            Expect(")");
            return new SolveExpr(first, equation, variant, param0, param1);
        }

        public Expr Extend(Token first) {
            Expect("(");
            Expect(TokenType.Word, out Token arrayName);
            Expect("=");
            var seed = Expr(false);
            Expect(",");
            var generator = Expr(false);
            Expect(",");
            var count = Expr(false);
            Expect(")");
            return new ExtendExpr(first, arrayName, seed, generator, count);
        }

        public Expr Plot(Token first) {
            Expect("(");
            Expect(TokenType.Word, out Token variant);
            Expect(",");
            var equation = Expr(false);
            Expect(")");
            return new PlotExpr(first, null, equation, variant);
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
            => Eos || (Peek().Text == ")") || (Peek().Text == "]") || (Peek().Text == ",") || (Peek().Text == ":") || (Peek().Text == "?");

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
