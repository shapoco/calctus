using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus {
    // todo abstract にする？
    class CalctusError : Exception {
        public CalctusError(Exception inner = null) : base(null, inner) { }
        public CalctusError(string msg, Exception inner = null) : base(msg, inner) { }
    }

    class CalctusArgError : CalctusError {
        private static string getInnerExcetion(string funcName, string argName, string msg) {
            string exMsg;
            if (string.IsNullOrEmpty(argName)) {
                exMsg = "Bad arg for " + funcName + "()";
            }
            else {
                exMsg = "Bad arg " + argName + " for " + funcName + "()";
            }
            if (!string.IsNullOrEmpty(msg)) {
                exMsg += ": " + msg;
            }
            return exMsg;
        }

        public CalctusArgError(string funcName, string msg) : this(funcName, null, msg) { }
        public CalctusArgError(string funcName, string argName, string msg) : base(getInnerExcetion(funcName, argName, msg)) { }
        
    }
}
