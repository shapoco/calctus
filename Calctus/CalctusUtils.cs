using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus {
    static class CalctusUtils {
        public static string ToString(object val) {
            if (val == null) {
                return "null";
            }
            else if (val is string) {
                return "\"" + val + "\"";
            }
            else if (val is char) {
                return "'" + val + "'";
            }
            else {
                return val.ToString();
            }
        }

    }
}
