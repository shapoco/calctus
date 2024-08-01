using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus {
    static class BasicExtensions {
        public static bool IsInteger(this decimal x) => x == Math.Floor(x);

    }
}
