using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.UI.Sheets {
    delegate void QueryScreenCursorLocationEventHandler(object sender, QueryScreenCursorLocationEventArgs e);
    delegate void QueryTokenEventHandler(object sender, QueryTokenEventArgs e);

    class QueryScreenCursorLocationEventArgs : EventArgs {
        public readonly int CursorPosition;
        public Point Result;

        public QueryScreenCursorLocationEventArgs(int cursorPosition) {
            CursorPosition = cursorPosition;
        }
    }

    class QueryTokenEventArgs : EventArgs {
        public readonly int CursorPosition;
        public readonly TokenType TokenType;
        public Token Result;

        public QueryTokenEventArgs(int cursorPosition, TokenType tokenType) {
            CursorPosition = cursorPosition;
            TokenType = tokenType;
        }
    }

    class RpnOperation {
        public readonly int StartIndex;
        public readonly int EndIndex;
        public readonly string Expression;
        public readonly Exception Error;
        public RpnOperation(int start, int end, string expr, Exception err) {
            StartIndex = start;
            EndIndex = end;
            Expression = expr;
            Error = err;
        }
    }

}
