using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Maths;

namespace Shapoco.Calctus.Model.Graphs {
    class AxisSettings {
        public const decimal LinearPosMin = decimal.MinValue / 4;
        public const decimal LinearPosMax = decimal.MaxValue / 4;
        public const decimal Log10PosMin = -26;
        public const decimal Log10PosMax = 26;

        public event EventHandler Changed;

        private AxisType _type = AxisType.Linear;
        private decimal _bottom = -10.5m;
        private decimal _range = 21m;

        public AxisType Type {
            get => _type;
            set {
                if (value == _type) return;
                _type = value;
                PerformChanged();
            }
        }

        public decimal PosBottom {
            get => _bottom;
            set {
                if (value == _bottom) return;
                _bottom = value;
                PerformChanged();
            }
        }

        public decimal PosTop => _bottom + _range;
        
        public decimal PosRange {
            get => _range;
            set {
                if (value == _range) return;
                _range = value;
                PerformChanged();
            }
        }

        public decimal PosMin {
            get {
                switch (_type) {
                    case AxisType.Linear: return LinearPosMin;
                    case AxisType.Log10: return Log10PosMin;
                    default: throw new NotImplementedException();
                }
            }
        }
        
        public decimal PosMax {
            get {
                switch (_type) {
                    case AxisType.Linear: return LinearPosMax;
                    case AxisType.Log10: return Log10PosMax;
                    default: throw new NotImplementedException();
                }
            }
        }

        public void PerformChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public void CopyTo(AxisSettings target) {
            target._type = _type;
            target._bottom = _bottom;
            target._range = _range;
            target.PerformChanged();
        }

        public decimal PosToValue(decimal pos) {
            switch (_type) {
                case AxisType.Linear: return pos;
                case AxisType.Log10: return MathEx.Pow10(pos);
                default: throw new NotImplementedException();
            }
        }

        public bool ValueToPos(decimal val, out decimal pos) {
            switch (_type) {
                case AxisType.Linear:
                    if (LinearPosMin <= val && val <= LinearPosMax) {
                        pos = val;
                        return true;
                    }
                    else {
                        pos = 0;
                        return false;
                    }
                case AxisType.Log10:
                    if (val > 0) {
                        pos = MathEx.Log10(val);
                        return true;
                    }
                    else {
                        pos = 0;
                        return false;
                    }
                default: throw new NotImplementedException();
            }
        }
    }
}
