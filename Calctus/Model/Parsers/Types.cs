using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    enum TokenType {
        NumericLiteral,
        BoolLiteral,
        OperatorSymbol,
        GeneralSymbol,
        Keyword,
        Word,
        Eos,
        Empty
    }
}
