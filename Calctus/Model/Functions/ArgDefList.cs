using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Functions {
    class ArgDefList {
        public readonly ArgDef[] Items;
        public readonly VariadicMode Mode;
        public readonly int VectorizableArgIndex;

        public int Count => Items.Length;
        public ArgDef this[int index] => Items[index];

        public ArgDefList() : this(new ArgDef[0], VariadicMode.None, -1) { }

        public ArgDefList(ArgDef[] args, VariadicMode mode, int vecArgIndex) {
            Items = args;
            Mode = mode;
            VectorizableArgIndex = vecArgIndex;
        }
    }
}
