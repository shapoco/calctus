using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.UI {
    internal class HistoryItem {
        private string _expression = "";
        private string _answer = "";
        private string _hint = "";
        private bool _error = false;
        private RadixMode _radixMode = RadixMode.Auto;
        private bool _isFreshAnswer = false;
        private bool _isFreshNewItem = true;

        public HistoryItem() { }
        public HistoryItem(HistoryItem prev) {
            _expression = prev._answer;
            _radixMode = prev._radixMode;
            _isFreshAnswer = true;
        }

        public string Expression {
            get => _expression;
            set {
                _expression = value;
                _isFreshAnswer = false;
            }
        }
        
        public string Answer {
            get => _answer;
            set { 
                _answer = value;
                _isFreshAnswer = false;
            }
        }
        
        public string Hint {
            get => _hint;
            set {
                _hint = value;
                _isFreshAnswer = false;
            }
        }
        
        public bool Error {
            get => _error;
            set {
                _error = value;
                _isFreshAnswer = false;
            }
        }

        public RadixMode RadixMode {
            get => _radixMode;
            set {
                _radixMode = value;
                _isFreshAnswer = false;
            }
        }

        public bool IsEmpty => string.IsNullOrEmpty(Expression);
        public bool IsFreshAnswer => _isFreshAnswer;

        public void Deselected() {
            _isFreshNewItem = false;
        }

        public override string ToString() {
            if (IsEmpty || _isFreshNewItem) return ">";

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
