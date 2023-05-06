using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapoco.Calctus.UI {
    public partial class CreateTimerForm : Form {
        private bool _timerMode = false;
        private Timer _timer = new Timer();

        public CreateTimerForm() {
            InitializeComponent();

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            timerRadioButton.CheckedChanged += delegate { setMode(timerRadioButton.Checked); };
            alarmRadioButton.CheckedChanged += delegate { setMode(timerRadioButton.Checked); };
            timeUpDown.ValueChanged += delegate { updateTimeoutTime(); };
            secRadioButton.CheckedChanged += delegate { updateTimeoutTime(); };
            minRadioButton.CheckedChanged += delegate { updateTimeoutTime(); };
            hourRadioButton.CheckedChanged += delegate { updateTimeoutTime(); };
            dateTimeText.TextChanged += delegate { updateStartButtonEnabled(); };
            _timer.Tick += delegate { updateTimeoutTime(); };

            setMode(true);
            dateTimeText.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public DateTime TimeoutTime {
            get {
                if (_timerMode) {
                    if (secRadioButton.Checked) {
                        return DateTime.Now + TimeSpan.FromSeconds((int)timeUpDown.Value);
                    }
                    else if (minRadioButton.Checked) {
                        return DateTime.Now + TimeSpan.FromMinutes((int)timeUpDown.Value);
                    }
                    else {
                        return DateTime.Now + TimeSpan.FromHours((int)timeUpDown.Value);
                    }
                }
                else {
                    if (DateTime.TryParse(dateTimeText.Text, out DateTime dateTime)) {
                        return dateTime;
                    }
                    else {
                        return DateTime.Now;
                    }
                }
            }
        }

        private void setMode(bool mode) {
            if (mode == _timerMode) return;
            _timerMode = mode;
            timerGroup.Enabled = _timerMode;
            alarmGroup.Enabled = !_timerMode;
            _timer.Enabled = _timerMode;
            _timer.Interval = 1000;
            updateTimeoutTime();
            updateStartButtonEnabled();
        }

        private void updateTimeoutTime() {
            if (_timerMode) {
                timeoutTimeLabel.Text = "Timeout time: " + TimeoutTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else {
                timeoutTimeLabel.Text = "";
            }
        }

        private void updateStartButtonEnabled () {
            if (_timerMode) {
                startButton.Enabled = true;
                dateTimeText.BackColor = SystemColors.Window;
            }
            else {
                var formatOk = (DateTime.TryParse(dateTimeText.Text, out _));
                dateTimeText.BackColor = formatOk ? SystemColors.Window : Color.Yellow;
                startButton.Enabled = formatOk;
            }
        }

    }
}
