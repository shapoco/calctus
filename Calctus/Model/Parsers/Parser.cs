using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Functions;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Parsers {
    class Parser {
        private Token _buff = null;
        private Token _lastToken = null;
        private TokenQueue _queue;
        public TokenQueue Queue => _queue;

        public static Expr Parse(string s) => Parse(new Lexer(s).PopToEnd());
        public static Expr Parse(TokenQueue q) => new Parser(q).Start();

        public Parser(TokenQueue queue) {
            _queue = queue;
        }

        public Expr Start() {
            Token tok;
            Expr ret;
            if (ReadIf("def", out tok)) {
                ret = DefFollowing(tok);
            }
            else {
                ret = Expr(true);
            }
            if (!Eos) throw new ParserError(Read(), "EOS is expected");
            return ret;
        }

        public Expr DefFollowing(Token first) {
            var name = Expect(TokenType.Identifier);
            var parOpen = Expect("(");
            var args = ArgDefList();
            Expect(")", parOpen);
            Expect("=");
            var body = Expr(allowColon: true);
            return new DefExpr(first, name, args, body);
        }

        public Expr Expr(bool allowColon) {
            var vals = new Stack<Expr>();
            var ops = new Stack<BinaryOpExpr>();

            vals.Push(UnaryOpExpr(allowColon));

            while (!EndOfExpr(allowColon)) {
                var sym = Read();
                var rightOp = new BinaryOpExpr(sym, null, null);
                while (ops.Count > 0 && ops.Peek().OpCode.ComparePriority(rightOp.OpCode) == OpPriorityDir.Left) {
                    var b = vals.Pop();
                    var a = vals.Pop();
                    vals.Push(new BinaryOpExpr(ops.Pop().Token, a, b));
                }
                ops.Push(rightOp);
                vals.Push(UnaryOpExpr(allowColon));
            }

            while (ops.Count > 0) {
                var b = vals.Pop();
                var a = vals.Pop();
                vals.Push(new BinaryOpExpr(ops.Pop().Token, a, b));
            }

            if (vals.Count != 1) {
                throw new CalctusError("Internal Error: stack broken");
            }

            var expr = vals.Pop();

            // 条件演算子
            if (ReadIf("?", out Token tok)) {
                var trueVal = Expr(allowColon: false);
                Expect(":");
                var falseVal = Expr(allowColon: false);
                expr = new CondOpExpr(tok, expr, trueVal, falseVal);
            }

            return expr;
        }

        public Expr UnaryOpExpr(bool allowColon) {
            if (ReadIf("*", out Token aster)) {
                var id = Expect(TokenType.Identifier);
                return new AsterExpr(aster, new IdExpr(id));
            }
            else if (ReadIf(TokenType.OperatorSymbol, out Token op)) {
                var expr = UnaryOpExpr(allowColon);
                return new UnaryOpExpr(op, expr);
            }
            else {
                return ElemRefExpr(allowColon);
            }
        }

        public Expr ElemRefExpr(bool allowColon) {
            var operand = Operand(allowColon);
            if (ReadIf("[", out Token openBracket)) {
                var from = Expr(false);
                Expr to = null;
                if (ReadIf(":")) {
                    to = Expr(false);
                }
                Expect("]", openBracket);
                return new PartRefExpr(openBracket, operand, from, to);
            }
            else {
                return operand;
            }
        }

        public Expr Operand(bool allowColon) {
            if (ReadIf("(", out Token openPar)) {
                return ParenthesisFollowing(openPar);
            }
            else if (ReadIf("[", out Token openBracket)) {
                var elms = new List<Expr>();
                if (!ReadIf("]")) {
                    elms.Add(Expr(true));
                    while (ReadIf(",")) {
                        elms.Add(Expr(true));
                    }
                    Expect("]", openBracket);
                }
                return new ArrayExpr(openBracket, elms.ToArray());
            }
            else if (ReadIf(TokenType.Literal, out Token literalToken)) {
                return new LiteralExpr((LiteralToken)literalToken);
            }
            else if (ReadIf(TokenType.Identifier, out Token id)) {
                var args = new List<Expr>();
                if (ReadIf("(", out Token argListOpen)) {
                    if (!ReadIf(")")) {
                        args.Add(Expr(true));
                        while (ReadIf(",")) {
                            args.Add(Expr(true));
                        }
                        Expect(")", argListOpen);
                    }
                    return new CallExpr(id, args.ToArray());
                }
                else if (ReadIf("=>", out Token arrow)) {
                    return Lambda(new IdExpr[] { new IdExpr(id) }, arrow);
                }
                else {
                    return new IdExpr(id);
                }
            }
            else if (EndOfExpr(allowColon)) {
                if (!allowColon && Peek().Text == ":") {
                    throw new ParserError(Peek(), "Operator ':' is not allowed in this context.");
                }
                else {
                    throw new ParserError(_lastToken, "Operand missing");
                }
            }
            else {
                var nextToken = Peek();
                throw new ParserError(nextToken, "Invalid operand: " + nextToken);
            }
        }

        public Expr ParenthesisFollowing(Token parOpen) {
            var exprs = new List<Expr>();
            Token t;
            Token firstComma = null;
            if (!ReadIf(")")) {
                exprs.Add(Expr(true));
                while (ReadIf(",", out t)) {
                    if (firstComma == null) firstComma = t;
                    exprs.Add(Expr(true));
                }
                Expect(")", parOpen);
            }

            if (ReadIf("=>", out Token arrow)) {
                return Lambda(exprs.ToArray(), arrow);
            }
            else if (exprs.Count == 1) {
                return exprs[0];
            }
            else {
                throw new ParserError(_lastToken, "Operator '=>' is expected.");
            }
        }

        public Expr Lambda(Expr[] argExprs, Token arrow) {
            ArgDef[] args = new ArgDef[argExprs.Length];
            int vecArgIndex = -1;
            for (int i = 0; i < argExprs.Length; i++) {
                if (argExprs[i] is IdExpr id) {
                    args[i] = new ArgDef(id.Token);
                }
                else if (argExprs[i] is AsterExpr aster) {
                    if (vecArgIndex >= 0) throw new ParserError(aster.Token, "Only one argument is vectorizable.");
                    vecArgIndex = i;
                    args[i] = new ArgDef(aster.Id.Token);
                }
                else {
                    throw new ParserError(argExprs[i].Token, "Single identifier is expected.");
                }
            }
            var argDefs = new ArgDefList(args, VariadicMode.None, vecArgIndex, -1);
            var body = Expr(true);
            return new LambdaExpr(arrow, new UserFuncDef(Token.Empty, argDefs, body));
        }

        public ArgDefList ArgDefList() {
            var args = new List<ArgDef>();
            var mode = VariadicMode.None;
            var vecArgIndex = -1;
            if (Peek().Text != ")") {
                do {
                    if (ReadIf("*", out Token aster)) {
                        if (vecArgIndex >= 0) throw new ParserError(aster, "Only one argument is vectorizable.");
                        vecArgIndex = args.Count;
                    }
                    var argName = Expect(TokenType.Identifier);
                    args.Add(new ArgDef(argName));
                } while (ReadIf(","));
                if (args.Count > 0 && ReadIf("[", out Token openBracket)) {
                    Expect("]", openBracket);
                    Expect("...");
                    if (vecArgIndex >= 0) throw new CalctusError("Variadic argument and vectorizable argument cannot coexist.");
                    mode = VariadicMode.Array;
                }
            }
            return new ArgDefList(args.ToArray(), mode, vecArgIndex, -1);
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

        public bool EndOfExpr(bool allowColon) =>
            Eos || (Peek().Text == ")") || (Peek().Text == "]") || (Peek().Text == ",") || (Peek().Text == "?") ||
            (!allowColon && Peek().Text == ":");
        
        public Token Expect(string s, Token errorToken = null) {
            if (ReadIf(s, out Token tok)) {
                return tok;
            }
            else {
                if (errorToken == null) errorToken = Peek();
                throw new ParserError(errorToken, "Missing: '" + s + "'");
            }
        }

        public Token Expect(TokenType typ) {
            if (ReadIf(typ, out Token tok)) {
                return tok;
            }
            else {
                throw new ParserError(Peek(), "Missing: '" + typ + "'");
            }
        }
    }
}
