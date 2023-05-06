using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Parser {
    class Lexer {
        private static readonly Regex _wordRegexes = new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*");

        private StringMatchReader _tr;
        private bool _eosReaded = false;

        // 長い順に並べた演算子記号
        private readonly Regex SymbolRule;

        // 数値リテラル
        private readonly NumberFormatter[] _numberFormatters;
        private readonly Regex[] _literalRegexes;

        public Lexer(string exprStr, int pos = 0) {
            // 全ての記号にマッチする正規表現の生成
            var symbols = OpDef.AllSymbols
                .OrderByDescending(p => p.Length)
                .Concat(new string[] { "(", ")", "[", "]", "," })
                .Select(p => "(" + Regex.Escape(p) + ")")
                .ToArray();
            SymbolRule = new Regex("(" + string.Join("|", symbols) + ")");

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
                    throw new SyntaxError(Position, "Internal error: Parser overrun");
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
            else if (_tr.Pop(SymbolRule, out tok)) {
                // 記号
                return new Token(TokenType.Symbol, pos, tok);
            }
            else {
                throw new SyntaxError(pos, "Unknown token starts with: '" + (char)_tr.Peek() + "'");
            }
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
                    if (t.Type != TokenType.Symbol) {
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

        public static void Test(string exprStr) {
            Lexer lex = new Lexer(exprStr);
            while (!lex.Eos) {
                Console.Write("'" + lex.Pop().Text + "'\t");
            }
            Console.WriteLine();
        }
    }
}
