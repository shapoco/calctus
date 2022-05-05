using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class Dim {
        public readonly string Id;
        public readonly string DisplayName;
        public Dim(string id, string dispName) {
            this.Id = id;
            this.DisplayName = dispName;
        }

        public Dim(string id) : this(id, id) { }

        public static readonly Dim Dimless = new Dim("void");
        public static readonly Dim Time = new Dim("time");
        public static readonly Dim Length = new Dim("length");
        public static readonly Dim Current = new Dim("current");
        public static readonly Dim Mass = new Dim("mass");
        public static readonly Dim Temperature = new Dim("temperature");

        //public override bool Equals(object obj) {
        //    return base.Equals(obj);
        //}
    }
}
