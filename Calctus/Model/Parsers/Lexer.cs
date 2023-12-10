using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Parsers {
    class Lexer {
        public const string IdPattern = @"[\p{L}_][\p{L}\p{N}_]*";
        private static readonly Regex _wordRegexes = new Regex(IdPattern);

        private StringMatchReader _tr;
        private bool _eosReaded = false;

        // 長い順に並べた演算子記号
        private readonly Regex OpSymbolRule;

        // 演算子以外の記号
        private readonly Regex GeneralSymbolRule = new Regex(@"[()\[\],:?]");

        // キーワード
        private readonly Regex KeywordRule = new Regex(@"(def|solve|extend|plot)\b");

        // 数値リテラル
        private readonly NumberFormatter[] _numberFormatters;
        private readonly Regex[] _literalRegexes;

        public Lexer(string exprStr, int pos = 0) {
            // 全ての記号にマッチする正規表現の生成
            var opSymbols = OpDef.AllOperatorSymbols
                .OrderByDescending(p => p.Length)
                .Select(p => "(" + Regex.Escape(p) + ")")
                .ToArray();
            OpSymbolRule = new Regex("(" + string.Join("|", opSymbols) + ")");

            _numberFormatters = NumberFormatter.NativeFormats;
            _literalRegexes = _numberFormatters.Select(p => p.Pattern).ToArray();

            _tr = new StringMatchReader(exprStr, pos);

        }

        public TextPosition Position => _tr.Position;
        public bool Eos {
            get {
                _tr.SkipWhite();
                return _tr.Eos;
            }
        }

        /// <summary>トークンを1つ読み出す</summary>
        public Token Pop() {
            _tr.SkipWhite();

            if (Eos) {
                if (!_eosReaded) {
                    _eosReaded = true;
                    return new Token(TokenType.Eos, Position, null);
                }
                else {
                    throw new LexerError(Position, "Internal error: Parser overrun");
                }
            }

            var pos = _tr.Position;
            string tok;

            int capIndex;
            Match m;
            if (_tr.Pop(_literalRegexes, out capIndex, out m )) {
                // リテラル
                var f = _numberFormatters[capIndex];
                var val = f.Parse(m);
                tok = m.Value;
                return new Token(TokenType.NumericLiteral, pos, tok, new NumberTokenHint(val));
            }
            else if (_tr.Pop(KeywordRule, out tok)) {
                // キーワード
                return new Token(TokenType.Keyword, pos, tok);
            }
            else if (_tr.Pop(_wordRegexes, out tok)) {
                if (tok == BoolVal.TrueKeyword || tok == BoolVal.FalseKeyword) {
                    // 真偽値
                    return new Token(TokenType.BoolLiteral, pos, tok);
                }
                else {
                    // ワード
                    return new Token(TokenType.Word, pos, tok);
                }
            }
            else if (_tr.Pop(OpSymbolRule, out tok)) {
                // 演算子記号
                return new Token(TokenType.OperatorSymbol, pos, tok);
            }
            else if (_tr.Pop(GeneralSymbolRule, out tok)) {
                // 演算子以外の記号
                return new Token(TokenType.GeneralSymbol, pos, tok);
            }
            else {
                throw new LexerError(pos, "Unknown token starts with: '" + (char)_tr.Peek() + "'");
            }
        }

        /// <summary>文字列の末端まで全てのトークンを読み出す</summary>
        public TokenQueue PopToEnd() {
            var queue = new TokenQueue();
            while (!_eosReaded) {
                queue.PushBack(Pop());
            }
            return queue;
        }

        /// <summary>
        /// 文字列が演算子のみで構成されていればそれを返す
        /// </summary>
        public static bool TryGetRpnSymbols(string expr, out Token[] symbols) {
            try {
                var lexer = new Lexer(expr);
                var list = new List<Token>();
                while (!lexer.Eos) {
                    var t = lexer.Pop();
                    if (t.Type != TokenType.OperatorSymbol) {
                        symbols = null;
                        return false;
                    }
                    list.Add(t);
                }
                symbols = list.ToArray();
                return symbols.Length > 0;
            }
            catch {
                symbols = null;
                return false;
            }
        }

        public static bool IsFirstIdChar(char c) => char.IsLetter(c) || c == '_';
        public static bool IsFollowingIdChar(char c) => char.IsLetter(c) || char.IsDigit(c) || c == '_';

        public static void Test(string exprStr) {
            Lexer lex = new Lexer(exprStr);
            while (!lex.Eos) {
                Console.Write("'" + lex.Pop().Text + "'\t");
            }
            Console.WriteLine();
        }
    }
}
