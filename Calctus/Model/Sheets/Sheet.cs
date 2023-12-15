using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Sheets {
    class Sheet {
        public const string LastAnsId = "ans";
        
        public event PreviewExecuteEventHandler PreviewExecute;

        public readonly ObservableCollection<SheetItem> Items = new ObservableCollection<SheetItem>();

        public EvalContext Run() => Run(new Dictionary<string, Val>());

        public EvalContext Run(Dictionary<string, Val> vars) {
            EvalContext e = new EvalContext();

            var s = Settings.Instance;
            foreach (var c in s.GetUserConstants()) {
                try {
                    var expr = Parser.Parse(c.ValueString);
                    e.DefConst(c.Id, expr.Eval(e), c.Description);
                }
                catch { }
            }

            // 外部からの変数の注入
            foreach (var name in vars.Keys) {
                e.DefConst(name, vars[name], "plot variant");
            }

            // 外部関数の呼び出しを許可する
            e.EvalSettings.AllowExternalFunctions = true;

            int step = 0;
            int pc = 0;
            int n = Items.Count;
            while (pc < n && step < 10000) { 
                var item = Items[pc];

                var preview = new PreviewExecuteEventArgs(pc, e, item);
                PreviewExecute?.Invoke(this, preview);
                if (preview.Overrided) {
                    item.SetStatus(preview.Answer, preview.SyntaxError, preview.EvalError);
                }
                else {
                    item.Eval(e);
                }

                if (item.EvalError == null) {
                    e.Ref(LastAnsId, true).Value = item.AnsVal;
                }
                else {
                    e.Undef(LastAnsId, true);
                }

                pc++;
                step++;
            }

            return e;
        }
    }
}
