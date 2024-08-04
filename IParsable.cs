using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco {
    public interface IParsable {
        bool TryParse(string s);
    }
}
