using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Functions {
    class BuiltInFuncDef : FuncDef {
        public const string EmbeddedLibraryNamespace = "Shapoco.Calctus.Model.Functions.BuiltIns";

        public Func<EvalContext, Val[], Val> Method { get; protected set; }
        public readonly FuncTest[] Tests;

        public BuiltInFuncDef(string prototype, string desc, Func<EvalContext, Val[], Val> method, params FuncTest[] tests)
            : base(prototype, desc) {
            this.Method = method;
            this.Tests = tests;
        }

        public string DocTitle => GetDeclarationText();

        protected override Val OnCall(EvalContext e, Val[] args) {
            return Method(e, args);
        }

#if DEBUG
        public void DoTest(EvalContext e) {
            if (HasTest) {
                foreach (var test in Tests) {
                    test.DoTest(e, this);
                }
            }
        }

        public bool HasTest => (Tests != null || Tests.Length > 0);
#endif
    }
}
