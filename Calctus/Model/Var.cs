using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class Var {
        public readonly Token Name;
        public readonly bool IsReadonly;

        private Val _value;
        public Val Value { 
            get => _value;
            set {
                if (IsReadonly) {
                    throw new CalctusError("'" + Name + "'は読み取り専用です。");
                }
                _value = value;
            }
        }

        public Var(Token name, Val value, bool isReadonly) {
            this.Name = name;
            this.Value = value;
            this.IsReadonly = isReadonly;
        }

        public Var(Token name) {
            this.Name = name;
            this.Value = new RealVal(0.0);
            this.IsReadonly = false;
        }
    }
}
