using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.UI {
    internal class HistoryItem {
        public string Expression = "";
        public string Answer = "";
        public string Hint = "";
        public bool Error = false;
        public RadixMode RadixMode = RadixMode.Auto;

        public HistoryItem() { }

        public bool IsEmpty => string.IsNullOrEmpty(Expression);

        public override string ToString() {
            if (IsEmpty) return ">";

            var sb = new StringBuilder();
            sb.Append(Expression);
            
            if (!string.IsNullOrEmpty(Answer)) {
                sb.Append("\t= ").Append(Answer);
            }

            if (!string.IsNullOrEmpty(Hint)) {
                sb.Append("\t(").Append(Hint).Append(")");
            }

            return sb.ToString();
        }
    }
}
