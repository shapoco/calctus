using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model {
    class CalctusError : Exception {
        public CalctusError(string msg, Exception inner = null) : base(msg, inner) {

        }
    }

}
