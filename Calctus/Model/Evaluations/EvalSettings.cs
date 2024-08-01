using System;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Evaluations {
    class EvalSettings : ICloneable {
        /// <summary>外部関数の呼び出しを許可する</summary>
        public bool AllowExternalFunctions { get; set; } = false;

        /// <summary>false の場合、ニュートン法などの実行時に速度優先の計算方法を適用する</summary>
        public bool AccuracyPriority { get; set; } = true;

        /// <summary>false の場合、ニュートン法などの実行時に分数を無効化する</summary>
        public bool FractionEnabled { get; set; } = true;

        public object Clone() => MemberwiseClone();
    }
}
