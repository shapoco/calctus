using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Evaluations {
    class Var : ICloneable {
        public readonly Token Name;
        public readonly string Description;
        public readonly bool IsReadonly;

        private Val _value;
        public Val Value { 
            get => _value;
            set {
                if (IsReadonly) {
                    throw new CalctusError(Name + " is read only.");
                }
                _value = value;
            }
        }

        public Var(Token name, Val value, bool isReadonly, string desc) {
            this.Name = name;
            this.Value = value;
            this.IsReadonly = isReadonly;
            this.Description = desc;
        }

        public Var(Token name, string desc) {
            this.Name = name;
            this.Value = new RealVal(0);
            this.IsReadonly = false;
            this.Description = desc;
        }

        public object Clone() => MemberwiseClone();
    }
}
