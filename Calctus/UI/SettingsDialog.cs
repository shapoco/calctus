using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.UI {
    public partial class SettingsDialog : Form {
        private FolderBrowserDialog _folderBrowserDialog = new FolderBrowserDialog();

        public SettingsDialog() {
            InitializeComponent();

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            tabControl.SelectedIndex = 0;

            var s = Settings.Instance;

            Startup_TrayIcon.CheckedChanged += (sender, e) => { s.Startup_TrayIcon = ((CheckBox)sender).Checked; };
            Startup_AutoStart.CheckedChanged += Startup_AutoStart_CheckedChanged;
            Window_RememberPosition.CheckedChanged += (sender, e) => { s.Window_RememberPosition = ((CheckBox)sender).Checked; };

            Hotkey_Enabled.CheckedChanged += (sender, e) => {
                s.Hotkey_Enabled = ((CheckBox)sender).Checked;
                Hotkey_KeyCode.Enabled = s.Hotkey_Enabled;
            };
            Hotkey_KeyCode.KeyCodeChanged += (sender, e) => {
                var kcb = (KeyCodeBox)sender;
                s.HotKey_Win = kcb.Win;
                s.HotKey_Alt = kcb.Alt;
                s.HotKey_Ctrl = kcb.Ctrl;
                s.HotKey_Shift = kcb.Shift;
                s.HotKey_KeyCode = kcb.KeyCode;
            };

            Input_IdAutoCompletion.CheckedChanged += (sender, e) => { s.Input_IdAutoCompletion = ((CheckBox)sender).Checked; };
            Input_AutoCloseBrackets.CheckedChanged += (sender, e) => { s.Input_AutoCloseBrackets = ((CheckBox)sender).Checked; };
            Input_AutoInputAns.CheckedChanged += (sender, e) => { s.Input_AutoInputAns = ((CheckBox)sender).Checked; };

            NumberFormat_Decimal_MaxLen.ValueChanged += (sender, e) => { s.NumberFormat_Decimal_MaxLen = (int)((NumericUpDown)sender).Value; };

            NumberFormat_Exp_Enabled.CheckedChanged += (sender, e) => {
                s.NumberFormat_Exp_Enabled = ((CheckBox)sender).Checked;
                NumberFormat_Exp_NegativeMax.Enabled = s.NumberFormat_Exp_Enabled;
                NumberFormat_Exp_PositiveMin.Enabled = s.NumberFormat_Exp_Enabled;
                NumberFormat_Exp_Alignment.Enabled = s.NumberFormat_Exp_Enabled;
            };
            NumberFormat_Exp_NegativeMax.ValueChanged += (sender, e) => { s.NumberFormat_Exp_NegativeMax = (int)((NumericUpDown)sender).Value; };
            NumberFormat_Exp_PositiveMin.ValueChanged += (sender, e) => { s.NumberFormat_Exp_PositiveMin = (int)((NumericUpDown)sender).Value; };
            NumberFormat_Exp_Alignment.CheckedChanged += (sender, e) => { s.NumberFormat_Exp_Alignment = ((CheckBox)sender).Checked; };

            Appearance_Font_Button_Name.Items.Clear();
            Appearance_Font_Expr_Name.Items.Clear();
            foreach (var ff in new System.Drawing.Text.InstalledFontCollection().Families) {
                Appearance_Font_Button_Name.Items.Add(ff.Name);
                Appearance_Font_Expr_Name.Items.Add(ff.Name);
            }
            Appearance_Font_Button_Name.SelectedIndexChanged += (sender, e) => { s.Appearance_Font_Button_Name = ((ComboBox)sender).Text; };
            Appearance_Font_Button_Name.TextChanged += (sender, e) => { s.Appearance_Font_Button_Name = ((ComboBox)sender).Text; };
            Appearance_Font_Expr_Name.SelectedIndexChanged += (sender, e) => { s.Appearance_Font_Expr_Name = ((ComboBox)sender).Text; };
            Appearance_Font_Expr_Name.TextChanged += (sender, e) => { s.Appearance_Font_Expr_Name = ((ComboBox)sender).Text; };
            Appearance_Font_Size.ValueChanged += (sender, e) => { s.Appearance_Font_Size = (int)((NumericUpDown)sender).Value; };
            Appearance_Font_Bold.CheckedChanged += (sender, e) => { s.Appearance_Font_Bold = ((CheckBox)sender).Checked; };

            constList.SelectedIndexChanged += ConstList_SelectedIndexChanged;
            constList.DoubleClick += ConstList_DoubleClick;
            constAddButton.Click += ConstAddButton_Click;
            constDelButton.Click += ConstDelButton_Click;
            constEditButton.Click += ConstEditButton_Click;

            Script_Enable.CheckedChanged += (sender, e) => { s.Script_Enable = ((CheckBox)sender).Checked; scriptGroup.Enabled = ((CheckBox)sender).Checked; };
            Script_FolderPath.TextChanged += (sender, e) => { s.Script_FolderPath = ((TextBox)sender).Text; };
            scriptFilterList.SelectedIndexChanged += ScriptFilterList_SelectedIndexChanged;
            scriptFilterList.DoubleClick += ScriptFilterList_DoubleClick;
            scriptFolderChangeButton.Click += ScriptFolderChangeButton_Click;
            scriptFolderOpenButton.Click += ScriptFolderOpenButton_Click;
            scriptFilterAddButton.Click += ScriptFilterAddButton_Click;
            scriptFilterDelButton.Click += ScriptFilterDelButton_Click;
            scriptFilterEditButton.Click += ScriptFilterEditButton_Click;
            scriptFilterMoveUp.Click += ScriptFilterMoveUp_Click;
            scriptFilterMoveDown.Click += ScriptFilterMoveDown_Click;

            closeButton.Click += delegate { this.Close(); };
            this.FormClosed += SettingsDialog_FormClosed;
            this.Load += SettingsDialog_Load;
        }

        private void SettingsDialog_Load(object sender, EventArgs e) {
            var s = Settings.Instance;
            try {
                Startup_AutoStart.Checked = Shapoco.Windows.StartupShortcut.CheckStartupRegistration();

                Startup_TrayIcon.Checked = s.Startup_TrayIcon;
                Window_RememberPosition.Checked = s.Window_RememberPosition;

                Hotkey_Enabled.Checked = s.Hotkey_Enabled;
                Hotkey_KeyCode.SetKeyCode(s.HotKey_Win, s.HotKey_Alt, s.HotKey_Ctrl, s.HotKey_Shift, s.HotKey_KeyCode);

                Input_IdAutoCompletion.Checked = s.Input_IdAutoCompletion;
                Input_AutoCloseBrackets.Checked = s.Input_AutoCloseBrackets;
                Input_AutoInputAns.Checked = s.Input_AutoInputAns;

                NumberFormat_Decimal_MaxLen.Value = s.NumberFormat_Decimal_MaxLen;

                NumberFormat_Exp_Enabled.Checked = s.NumberFormat_Exp_Enabled;
                NumberFormat_Exp_NegativeMax.Value = s.NumberFormat_Exp_NegativeMax;
                NumberFormat_Exp_PositiveMin.Value = s.NumberFormat_Exp_PositiveMin;
                NumberFormat_Exp_Alignment.Checked = s.NumberFormat_Exp_Alignment;

                Appearance_Font_Button_Name.Text = s.Appearance_Font_Button_Name;
                Appearance_Font_Expr_Name.Text = s.Appearance_Font_Expr_Name;
                Appearance_Font_Size.Value = s.Appearance_Font_Size;
                Appearance_Font_Bold.Checked = s.Appearance_Font_Bold;

                foreach(var c in s.GetUserConstants()) {
                    addConst(c);
                }
                constDelButton.Enabled = false;
                constEditButton.Enabled = false;

                foreach(var sf in s.GetScriptFilters()) {
                    addScriptFilter(sf);
                }
                scriptFilterDelButton.Enabled = false;
                scriptFilterEditButton.Enabled = false;
                scriptFilterMoveUp.Enabled = false;
                scriptFilterMoveDown.Enabled = false;

                Script_Enable.Checked = scriptGroup.Enabled = s.Script_Enable;
                Script_FolderPath.Text = s.Script_FolderPath;
            }
            catch { }
        }

        private void Startup_AutoStart_CheckedChanged(object sender, EventArgs e) {
            Shapoco.Windows.StartupShortcut.SetStartupRegistration(((CheckBox)sender).Checked);
        }

        private void ConstList_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = (constList.SelectedItems.Count == 1);
            constDelButton.Enabled = selected;
            constEditButton.Enabled = selected;
        }

        private void ConstList_DoubleClick(object sender, EventArgs e) {
            if (constList.SelectedItems.Count <= 0) return;
            constEditButton.PerformClick();
        }

        private void ConstAddButton_Click(object sender, EventArgs e) {
            constList.SelectedIndices.Clear();
            addConst(new UserConstant("ID", "1", "user-defined constant")).Selected = true;
            constEditButton.PerformClick();
        }

        private void ConstDelButton_Click(object sender, EventArgs e) {
            if (constList.SelectedItems.Count <= 0) return;
            constList.Items.Remove(constList.SelectedItems[0]);
        }

        private void ConstEditButton_Click(object sender, EventArgs e) {
            if (constList.SelectedItems.Count <= 0) return;
            var lvi = constList.SelectedItems[0];
            var c = (UserConstant)lvi.Tag;
            var dlg = new ConstEditForm();
            dlg.Target = c;
            dlg.ShowDialog();
            dlg.Dispose();
            lvi.Text = c.Id;
            lvi.SubItems[1].Text = c.ValueString;
            lvi.SubItems[2].Text = c.Description;
        }

        private ListViewItem addConst(UserConstant c) {
            var lvi = new ListViewItem(new string[] { c.Id, c.ValueString, c.Description });
            lvi.Tag = c;
            constList.Items.Add(lvi);
            return lvi;
        }

        private void ScriptFolderChangeButton_Click(object sender, EventArgs e) {
            _folderBrowserDialog.Description = "Specify the folder where the script files will be located.";
            _folderBrowserDialog.SelectedPath = Script_FolderPath.Text;
            if (_folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                Script_FolderPath.Text = _folderBrowserDialog.SelectedPath;
            }
        }

        private void ScriptFolderOpenButton_Click(object sender, EventArgs e) {
            try {
                if (!System.IO.Directory.Exists(Script_FolderPath.Text)) {
                    if (DialogResult.Yes == MessageBox.Show(
                            "The folder does not exist. Do you want to create it?", 
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ) {
                        System.IO.Directory.CreateDirectory(Script_FolderPath.Text);
                    }
                    else {
                        return;
                    }
                }
                System.Diagnostics.Process.Start(Script_FolderPath.Text);
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScriptFilterList_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = (scriptFilterList.SelectedItems.Count == 1);
            scriptFilterDelButton.Enabled = selected;
            scriptFilterEditButton.Enabled = selected;
            scriptFilterMoveUp.Enabled = selected && scriptFilterList.SelectedIndices[0] > 0;
            scriptFilterMoveDown.Enabled = selected && scriptFilterList.SelectedIndices[0] < scriptFilterList.Items.Count - 1;
        }

        private void ScriptFilterList_DoubleClick(object sender, EventArgs e) {
            if (scriptFilterList.SelectedItems.Count <= 0) return;
            scriptFilterEditButton.PerformClick();
        }

        private void ScriptFilterAddButton_Click(object sender, EventArgs e) {
            scriptFilterList.SelectedIndices.Clear();
            addScriptFilter(new ScriptFilter("*.*", "", ScriptFilter.DefaultParameter)).Selected = true;
            scriptFilterEditButton.PerformClick();
        }

        private void ScriptFilterDelButton_Click(object sender, EventArgs e) {
            if (scriptFilterList.SelectedItems.Count <= 0) return;
            scriptFilterList.Items.Remove(scriptFilterList.SelectedItems[0]);
        }

        private void ScriptFilterEditButton_Click(object sender, EventArgs e) {
            if (scriptFilterList.SelectedItems.Count <= 0) return;
            var lvi = scriptFilterList.SelectedItems[0];
            var sf = (ScriptFilter)lvi.Tag;
            var dlg = new ScriptFilterEditForm();
            dlg.Target = sf;
            dlg.ShowDialog();
            dlg.Dispose();
            setScriptFilterListItem(lvi, sf);
        }

        private void ScriptFilterMoveUp_Click(object sender, EventArgs e) {
            if (scriptFilterList.SelectedItems.Count <= 0) return;
            var index = scriptFilterList.SelectedIndices[0];
            if (index > 0) {
                var lvi = scriptFilterList.SelectedItems[0];
                scriptFilterList.Items.Remove(lvi);
                scriptFilterList.Items.Insert(index - 1, lvi);
            }
        }

        private void ScriptFilterMoveDown_Click(object sender, EventArgs e) {
            if (scriptFilterList.SelectedItems.Count <= 0) return;
            var index = scriptFilterList.SelectedIndices[0];
            if (index < scriptFilterList.Items.Count - 1) {
                var lvi = scriptFilterList.SelectedItems[0];
                scriptFilterList.Items.Remove(lvi);
                scriptFilterList.Items.Insert(index + 1, lvi);
            }
        }

        private ListViewItem addScriptFilter(ScriptFilter sf) {
            var lvi = new ListViewItem();
            lvi.SubItems.Add("");
            lvi.SubItems.Add("");
            setScriptFilterListItem(lvi, sf);
            scriptFilterList.Items.Add(lvi);
            return lvi;
        }

        private void setScriptFilterListItem(ListViewItem lvi, ScriptFilter sf) {
            lvi.Tag = sf;
            lvi.Text = sf.Filter;
            lvi.SubItems[1].Text = sf.CommandLabel;
            lvi.SubItems[2].Text = sf.ParameterLabel ;
        }

        private void SettingsDialog_FormClosed(object sender, FormClosedEventArgs e) {
            var consts = new List<UserConstant>();
            foreach(var item in constList.Items) {
                consts.Add((UserConstant)((ListViewItem)item).Tag);
            }
            Settings.Instance.SetUserConstants(consts);

            var filters = new List<ScriptFilter>();
            foreach (var item in scriptFilterList.Items) {
                filters.Add((ScriptFilter)((ListViewItem)item).Tag);
            }
            Settings.Instance.SetScriptFilters(filters);

            Settings.Instance.Save();
        }
    }
}
