using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI {
    class SheetFileNode : TreeNode {
        private string _path = null;
        private SheetView _view = null;

        public string Path => _path;
        public SheetView View => _view;

        public SheetFileNode(string text, string path, SheetView view) : base(text) {
            _path = path;
            _view = view;
        }

        public void CreateView() {
            if (_view != null) return;
            _view = new SheetView();
            _view.Load(_path);
        }
    }
}
