using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalSettings : ICloneable {
        public int DecimalLengthToDisplay { get; set; } = 9;
        public bool ENotationEnabled { get; set; } = true;
        public int ENotationExpPositiveMin { get; set; } = 15;
        public int ENotationExpNegativeMax { get; set; } = -5;
        public bool ENotationAlignment { get; set; } = false;
        public bool AllowExternalFunctions { get; set; } = false;

        /// <summary>false の場合、ニュートン法などの実行時に速度優先の計算方法を適用する</summary>
        public bool AccuracyPriority { get; set; } = true;

        /// <summary>false の場合、ニュートン法などの実行時に分数を無効化する</summary>
        public bool FractionEnabled { get; set; } = true;

        public object Clone() => MemberwiseClone();
    }
}
