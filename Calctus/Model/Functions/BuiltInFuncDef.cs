using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class BuiltInFuncDef : FuncDef {
        public const string EmbeddedLibraryNamespace = "Shapoco.Calctus.Model.Functions.BuiltIns";

        public Func<EvalContext, Val[], Val> Method { get; protected set; }
        public readonly ExprTest[] Tests;

        public BuiltInFuncDef(string prototype, string desc, Func<EvalContext, Val[], Val> method, params FuncTest[] tests)
            : base(prototype, desc) {
            this.Method = method;
            this.Tests = tests.Select(p => p.GenTest(this)).ToArray();
        }

        public string DocTitle => GetDeclarationText();

        protected override Val OnCall(EvalContext e, Val[] args) {
            return Method(e, args);
        }

#if DEBUG
        public void DoTest(EvalContext e) {
            if (HasTest) {
                foreach (var test in Tests) {
                    test.DoTest(e);
                }
            }
            else {
                Test.Untested("No test defined: " + this);
            }
        }

        public bool HasTest => (Tests != null && Tests.Length > 0);
#endif
    }
}
