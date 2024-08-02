using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Shapoco.Windows {
    class ClipboardListener : NativeWindow, IDisposable {

        private Form _parent = null;
        private bool _assigned = false;
        private IntPtr _nextHandle = IntPtr.Zero;
        private bool _disposed = false;

        private Timer _eventTimer = new Timer();
        public event EventHandler ClipboardChanged = delegate { };

        public ClipboardListener(Form parent) {
            _parent = parent;
            _parent.HandleCreated += delegate { start(); };
            _parent.HandleDestroyed += delegate { stop(); };

            _eventTimer.Tick += _eventTimer_Tick;
            
            if (_parent.IsHandleCreated) {
                start();
            }
        }

        ~ClipboardListener() {
            Dispose(false);
        }

        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    stop();
                    _eventTimer.Dispose();
                }
                _disposed = true;
            }
        }

        public void Refresh() {
            stop();
            start();
        }

        void start() {
            if (_assigned) return;
            _assigned = true;

            this.AssignHandle(_parent.Handle);
            _nextHandle = WinUser.SetClipboardViewer(this.Handle);
        }

        void stop() {
            if (!_assigned) return;
            _assigned = false;

            bool ret = WinUser.ChangeClipboardChain(this.Handle, _nextHandle);
            this.ReleaseHandle();
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case WinUser.WM_DRAWCLIPBOARD:
                    _parent.Invoke(new MethodInvoker(delegate {
                        _eventTimer.Stop();
                        _eventTimer.Interval = 100;
                        _eventTimer.Start();
                    }));
                    if (_nextHandle != IntPtr.Zero) {
                        WinUser.SendMessage(_nextHandle, m.Msg, m.WParam, m.LParam);
                    }
                    break;
                case WinUser.WM_CHANGECBCHAIN:
                    if (m.WParam == _nextHandle) {
                        _nextHandle = (IntPtr)m.LParam;
                    }
                    else {
                        WinUser.SendMessage(_nextHandle, m.Msg, m.WParam, m.LParam);
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void _eventTimer_Tick(object sender, EventArgs e) {
            _eventTimer.Stop();
            ClipboardChanged(this, EventArgs.Empty);
        }
    }
}
