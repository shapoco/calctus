using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Functions {
    abstract class BuiltInFuncCategory {
        public virtual string Name => preferredName;

        private string _preferredName = null;
        private string preferredName {
            get {
                if (_preferredName != null) {
                    return _preferredName;
                }

                var typeName = this.GetType().Name;
                if (typeName.EndsWith("Funcs")) {
                    typeName = typeName.Substring(0, typeName.Length - 5);
                }

                var sb = new StringBuilder();
                foreach (var c in typeName) {
                    if (c == '_') {
                        sb.Append('/');
                    }
                    else if (sb.Length > 0 && sb[sb.Length - 1] != '/' && 'A' <= c && c <= 'Z') {
                        sb.Append(' ').Append(c);
                    }
                    else {
                        sb.Append(c);
                    }
                }

                return _preferredName = sb.ToString();
            }
        }

        private BuiltInFuncDef[] _functions = null;
        public BuiltInFuncDef[] Functions => _functions != null ? _functions : _functions = enumFunctions();
        private BuiltInFuncDef[] enumFunctions() =>
            this.GetType().GetFields()
            .Where(p => !p.IsStatic && (p.FieldType == typeof(BuiltInFuncDef)))
            .Select(p => (BuiltInFuncDef)p.GetValue(this))
            .ToArray();

        public string DocTitle => Name;

        public string DocFileNameBase => this.GetType().Name.ToLower();
    }
}


