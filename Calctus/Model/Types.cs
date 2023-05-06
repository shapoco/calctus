using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    enum TokenType {
        NumericLiteral,
        BoolLiteral,
        Symbol,
        Word,
        Eos
    }

    /// <summary>演算子の種別</summary>
    enum OpType {
        /// <summary>単項演算子</summary>
        Unary,
        /// <summary>二項演算子</summary>
        Binary,
    }

    enum OpPriorityDir {
        Left,
        Right
    }

    class CalctusError : Exception {
        public CalctusError(string msg, Exception inner = null) : base(msg, inner) {

        }
    }

}
