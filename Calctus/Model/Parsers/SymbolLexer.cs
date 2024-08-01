using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    class SymbolLexer {
        public static readonly SymbolLexer Instance = new SymbolLexer();

        private readonly SymbolLexerNode _root = new SymbolLexerNode('\0', false, TokenType.Empty);
        private SymbolLexer() {
            foreach (OpInfo op in OpInfo.Items) {
                registerSymbol(TokenType.OperatorSymbol, op.Symbol);
            }
            registerSymbol(TokenType.GeneralSymbol, "(", ")", "[", "]", ",", "?", ":");
        }
        private void registerSymbol(TokenType type, params string[] symArray) {
            foreach (var sym in symArray) {
                var node = _root;
                int n = sym.Length;
                for (int i = 0; i < n; i++) {
                    node = node.AppendNode(sym[i], i == n - 1, type);
                }
            }
        }

        public bool TryRead(StringReader sr, out Token token) {
            SymbolLexerNode node = _root;
            SymbolLexerNode following = null;
            while (node.TryFindFollowing(sr.Peek(), out following)) {
                sr.Read();
                node = following;
            }
            if (_root == node) {
                token = null;
                return false;
            }
            else if (node.Terminal) {
                token = sr.FinishToken(node.TokenType);
                return true;
            }
            else {
                var charList = string.Join(" or ", node.Followings.Select(p => CalctusUtils.ToString(p.Char.ToString())));
                throw new LexerError(sr.Position, charList + " is expected");
            }
        }
    }

    class SymbolLexerNode {
        public readonly char Char;
        public bool Terminal { get; private set; }
        public TokenType TokenType { get; private set; }
        public readonly List<SymbolLexerNode> Followings = new List<SymbolLexerNode>();
        public SymbolLexerNode(char c, bool term, TokenType type) {
            this.Char = c;
            this.Terminal = term;
            this.TokenType = term ? type : TokenType.Empty;
        }

        public SymbolLexerNode AppendNode(char c, bool term, TokenType type) {
            if (TryFindFollowing(c, out SymbolLexerNode node)) {
                if (term) node.SetTerm(type);
                return node;
            }
            node = new SymbolLexerNode(c, term, type);
            Followings.Add(node);
            return node;
        }

        public bool TryFindFollowing(int c, out SymbolLexerNode node) {
            foreach (var n in Followings) {
                if (n.Char == c) {
                    node = n;
                    return true;
                }
            }
            node = null;
            return false;
        }

        public void SetTerm(TokenType type) {
            if (this.Terminal && type != this.TokenType) {
                throw new InvalidOperationException("Symbol conflicted");
            }
            this.Terminal = true;
            this.TokenType = type;
        }
    }
}
