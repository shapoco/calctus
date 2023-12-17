using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI {
    class SheetFileNode : TreeNode {
        private string _path = null;
        private SheetView _view = null;

        public string FilePath {
            get => _path;
            set {
                if (value == _path) return;
                _path = value;
                if (_path != null) {
                    Name = Path.GetFileNameWithoutExtension(_path);
                }
                this.Text = Name;
            }
        }

        public SheetView View {
            get => _view;
            private set {
                if (value == _view) return;
                _view = value;
            }
        }

        public SheetFileNode(string name, string path, SheetView view) : base(name) {
            Name = name;
            _path = path;
            View = view;
        }

        public void CreateView() {
            if (_view != null) return;
            var view = new SheetView();
            view.Load(_path);
            this.View = view;
        }
    }
}
