﻿using System;
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
        public static Expr Parse(TokenQueue q) => new Parser(q).Pop(root: true);

        public Parser(TokenQueue queue) {
            _queue = queue;
        }

        public Expr Pop(bool root) {
            Token tok;
            if (ReadIf("def", out tok)) {
                return Def(tok);
            }
            else {
                return Expr(root);
            }
        }

        public Expr Expr(bool root = false) {
            var vals = new Stack<Expr>();
            var ops = new Stack<BinaryOpExpr>();

            vals.Push(UnaryOpExpr());

            while (!EndOfExpr) {
                var sym = Read();
                var rightOp = new BinaryOpExpr(sym, null, null);
                while (ops.Count > 0 && ops.Peek().OpCode.ComparePriority(rightOp.OpCode) == OpPriorityDir.Left) {
                    var b = vals.Pop();
                    var a = vals.Pop();
                    vals.Push(new BinaryOpExpr(ops.Pop().Token, a, b));
                }
                ops.Push(rightOp);
                vals.Push(UnaryOpExpr());
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
                var trueVal = Expr();
                Expect(":");
                var falseVal = Expr();
                expr = new CondOpExpr(tok, expr, trueVal, falseVal);
            }

            if (root && !Eos) {
                throw new ParserError(_lastToken, "Operator missing");
            }
            return expr;
        }

        public Expr UnaryOpExpr() {
            Token tok;
            if (ReadIf("*", out tok)) {
                Expect(TokenType.Identifier, out Token id);
                return new AsterExpr(tok, new IdExpr(id));
            }
            else if (ReadIf(TokenType.OperatorSymbol, out tok)) {
                var expr = UnaryOpExpr();
                if (expr is LiteralExpr num && num.Value is RealVal val) {
                    if (tok.Text == OpCodes.Plus.GetSymbol()) {
                        return expr;
                    }
                    else if (tok.Text == OpCodes.ArithInv.GetSymbol()) {
                        return new LiteralExpr(new RealVal(-val.AsDecimal));
                    }
                }
                return new UnaryOpExpr(tok, expr);
            }
            else {
                return ElemRefExpr();
            }
        }

        public Expr ElemRefExpr() {
            var operand = Operand();
            if (ReadIf("[", out Token tok)) {
                var from = Expr();
                Expr to = null;
                if (ReadIf(":")) {
                    to = Expr();
                }
                Expect("]");
                return new PartRefExpr(tok, operand, from, to);
            }
            else {
                return operand;
            }
        }

        public Expr Operand() {
            Token tok;
            if (ReadIf("(", out tok)) {
                return Parenthesis(tok);
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
            else if (ReadIf(TokenType.Literal, out tok)) {
                return new LiteralExpr((LiteralToken)tok);
            }
            else if (ReadIf(TokenType.Identifier, out tok)) {
                var args = new List<Expr>();
                if (ReadIf("(")) {
                    if (!ReadIf(")")) {
                        args.Add(Expr());
                        while (ReadIf(",")) {
                            args.Add(Expr());
                        }
                        Expect(")");
                    }
                    return new CallExpr(tok, args.ToArray());
                }
                else if (ReadIf("=>", out Token t)) {
                    return Lambda(new IdExpr[] { new IdExpr(tok) }, t);
                }
                else {
                    return new IdExpr(tok);
                }
            }
            else if (EndOfExpr) {
                throw new ParserError(_lastToken, "Operand missing");
            }
            else {
                var nextToken = Peek();
                throw new ParserError(nextToken, "Invalid operand: " + nextToken);
            }
        }

        public Expr Parenthesis(Token first) {
            var exprs = new List<Expr>();
            Token t;
            Token firstComma = null;
            if (!ReadIf(")")) {
                exprs.Add(Expr());
                while (ReadIf(",", out t)) {
                    if (firstComma == null) firstComma = t;
                    exprs.Add(Expr());
                }
                Expect(")");
            }
            if (ReadIf("=>", out t)) {
                return Lambda(exprs.ToArray(), t);
            }
            else if (exprs.Count == 1) {
                return exprs[0];
            }
            else {
                return new ParenthesisExpr(first, firstComma, exprs.ToArray());
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
            var body = Expr();
            return new LambdaExpr(arrow, new UserFuncDef(Token.Empty, argDefs, body));
        }

        public Expr Def(Token first) {
            Expect(TokenType.Identifier, out Token name);
            Expect("(");
            var args = ArgDefList();
            Expect(")");
            Expect("=");
            var body = Expr(true);
            return new DefExpr(first, name, args, body);
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
                    Expect(TokenType.Identifier, out Token arg);
                    args.Add(new ArgDef(arg));
                } while (ReadIf(","));
                if (args.Count > 0 && ReadIf("[")) {
                    Expect("]");
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

        public bool EndOfExpr
            => Eos || (Peek().Text == ")") || (Peek().Text == "]") || (Peek().Text == ",") || (Peek().Text == ":") || (Peek().Text == "?");

        public void Expect(string s) => Expect(s, out _);

        public void Expect(string s, out Token tok) {
            if (!ReadIf(s, out tok)) {
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
