using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Shapoco.Maths;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.UI.Books;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI {
    public partial class SettingsDialog : Form {
        private FolderBrowserDialog _folderBrowserDialog = new FolderBrowserDialog();
        private ColorPickerBox[] _colorPickers;

        private Timer _reloadTimer = new Timer();

        public SettingsDialog() {
            InitializeComponent();

            Windows.DwmApi.SetDarkModeEnable(this, Settings.Instance.Appearance_IsDarkTheme);

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            using (var g = this.CreateGraphics()) {
                var colorTextSize = Size.Ceiling(g.MeasureString("#AAAAAA", this.Font));
                var scaleFactor = (float)this.DeviceDpi / 96;
                var colorLabels = new List<ColorPickerBox>();
                var xPadding = (int)Math.Ceiling(10 * scaleFactor);
                var centerPadding = (int)Math.Ceiling(30 * scaleFactor);
                var yPadding = (int)Math.Ceiling(20 * scaleFactor);
                var x = xPadding;
                var y = yPadding;
                var wColor = (int)(colorTextSize.Width * 1.75f);
                var wName = (colorGroup.ClientSize.Width - centerPadding - xPadding * 2) / 2 - wColor;
                var hLabel = (int)(colorTextSize.Height * 1.5f);
                var yStride = (int)(colorTextSize.Height * 1.5f);
                foreach (var dispName in Settings.ColorProperties.Keys) {
                    var prop = Settings.ColorProperties[dispName];
                    var nameLabel = new Label();
                    nameLabel.Text = dispName;
                    nameLabel.AutoSize = false;
                    nameLabel.TextAlign = ContentAlignment.MiddleLeft;
                    nameLabel.SetBounds(x, y, wName, hLabel);
                    colorGroup.Controls.Add(nameLabel);
                    var colorLabel = new ColorPickerBox();
                    colorLabel.Tag = prop.Name;
                    colorLabel.AutoSize = false;
                    colorLabel.SetBounds(x + wName, y, wColor, hLabel);
                    colorLabel.SelectedColorChanged += ColorBox_SelectedColorChanged;
                    colorGroup.Controls.Add(colorLabel);
                    colorLabels.Add(colorLabel);
                    y += yStride;
                    if (y + hLabel > toggleLightDarkModeButton.Top) {
                        x += wName + wColor + centerPadding;
                        y = yPadding;
                    }
                }
                _colorPickers = colorLabels.ToArray();
            }

            tabControl.SelectedIndex = 0;

            var s = Settings.Instance;

            Startup_TrayIcon.CheckedChanged += (sender, e) => { s.Startup_TrayIcon = ((CheckBox)sender).Checked; };
            Startup_AutoStart.CheckedChanged += Startup_AutoStart_CheckedChanged;
            Window_RememberPosition.CheckedChanged += (sender, e) => { s.Window_RememberPosition = ((CheckBox)sender).Checked; };
            SaveSettingsInInstallDirectoryCheckBox.CheckedChanged += SaveSettingsInInstallDirectoryCheckBox_CheckedChanged;
            openSettingFolderButton.Click += OpenSettingFolderButton_Click;
            History_KeepPeriod.ValueChanged += (sender, e) => { s.History_KeepPeriod = (int)((NumericUpDown)sender).Value; };

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

            NumberFormat_Separator_Thousands.CheckedChanged += (sender, e) => { s.NumberFormat_Separator_Thousands = ((CheckBox)sender).Checked; };
            NumberFormat_Separator_Hexadecimal.CheckedChanged += (sender, e) => { s.NumberFormat_Separator_Hexadecimal = ((CheckBox)sender).Checked; };

            Calculation_Limit_MaxArrayLength.ValueChanged += (sender, e) => { s.Calculation_Limit_MaxArrayLength = (int)((NumericUpDown)sender).Value; };
            Calculation_Limit_MaxStringLength.ValueChanged += (sender, e) => { s.Calculation_Limit_MaxStringLength = (int)((NumericUpDown)sender).Value; };
            Calculation_Limit_MaxCallRecursions.ValueChanged += (sender, e) => { s.Calculation_Limit_MaxCallRecursions = (int)((NumericUpDown)sender).Value; };

            Appearance_Font_Button_Name.Items.Clear();
            Appearance_Font_Expr_Name.Items.Clear();
            foreach (var ff in new System.Drawing.Text.InstalledFontCollection().Families) {
                Appearance_Font_Button_Name.Items.Add(ff.Name);
                Appearance_Font_Expr_Name.Items.Add(ff.Name);
            }
            Appearance_Font_Button_Name.SelectedIndexChanged += (sender, e) => { s.Appearance_Font_Button.Face = ((ComboBox)sender).Text; requestAppearancePreview(); };
            Appearance_Font_Button_Name.TextChanged += (sender, e) => { s.Appearance_Font_Button.Face = ((ComboBox)sender).Text; requestAppearancePreview(); };
            Appearance_Font_Size.ValueChanged += (sender, e) => { s.Appearance_Font_Button.Size = (int)((NumericUpDown)sender).Value; requestAppearancePreview(); };
            Appearance_Font_Button_Bold.CheckedChanged += (sender, e) => { s.Appearance_Font_Button.Bold = ((CheckBox)sender).Checked; requestAppearancePreview(); };
            Appearance_Font_Button_Italic.CheckedChanged += (sender, e) => { s.Appearance_Font_Button.Italic = ((CheckBox)sender).Checked; requestAppearancePreview(); };
            Appearance_Font_Expr_Name.SelectedIndexChanged += (sender, e) => { s.Appearance_Font_Expr.Face = ((ComboBox)sender).Text; requestAppearancePreview(); };
            Appearance_Font_Expr_Name.TextChanged += (sender, e) => { s.Appearance_Font_Expr.Face = ((ComboBox)sender).Text; requestAppearancePreview(); };
            Appearance_Font_Expr_Size.ValueChanged += (sender, e) => { s.Appearance_Font_Expr.Size = (int)((NumericUpDown)sender).Value; requestAppearancePreview(); };
            Appearance_Font_Expr_Bold.CheckedChanged += (sender, e) => { s.Appearance_Font_Expr.Bold = ((CheckBox)sender).Checked; requestAppearancePreview(); };
            Appearance_Font_Expr_Italic.CheckedChanged += (sender, e) => { s.Appearance_Font_Expr.Italic = ((CheckBox)sender).Checked; requestAppearancePreview(); };
            swapColorRbButton.Click += SwapColorRbButton_Click;
            shiftColorHueButton.Click += RotateColorHueButton_Click;
            toggleLightDarkModeButton.Click += ToggleLightDarkModeButton_Click;

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

            _reloadTimer.Enabled = false;
            _reloadTimer.Tick += _previewDelayTimer_Tick;
        }

        private void SettingsDialog_Load(object sender, EventArgs e) {
            var s = Settings.Instance;
            try {
                Startup_AutoStart.Checked = Shapoco.Windows.StartupShortcut.CheckStartupRegistration();

                Startup_TrayIcon.Checked = s.Startup_TrayIcon;
                Window_RememberPosition.Checked = s.Window_RememberPosition;
                loadSettingLocation();

                Hotkey_Enabled.Checked = s.Hotkey_Enabled;
                Hotkey_KeyCode.SetKeyCode(s.HotKey_Win, s.HotKey_Alt, s.HotKey_Ctrl, s.HotKey_Shift, s.HotKey_KeyCode);

                setNudValue(History_KeepPeriod, s.History_KeepPeriod);

                Input_IdAutoCompletion.Checked = s.Input_IdAutoCompletion;
                Input_AutoCloseBrackets.Checked = s.Input_AutoCloseBrackets;
                Input_AutoInputAns.Checked = s.Input_AutoInputAns;

                setNudValue(NumberFormat_Decimal_MaxLen, s.NumberFormat_Decimal_MaxLen);

                NumberFormat_Exp_Enabled.Checked = s.NumberFormat_Exp_Enabled;
                setNudValue(NumberFormat_Exp_NegativeMax, s.NumberFormat_Exp_NegativeMax);
                setNudValue(NumberFormat_Exp_PositiveMin, s.NumberFormat_Exp_PositiveMin);
                NumberFormat_Exp_Alignment.Checked = s.NumberFormat_Exp_Alignment;

                NumberFormat_Separator_Thousands.Checked = s.NumberFormat_Separator_Thousands;
                NumberFormat_Separator_Hexadecimal.Checked = s.NumberFormat_Separator_Hexadecimal;

                setNudValue(Calculation_Limit_MaxArrayLength, s.Calculation_Limit_MaxArrayLength);
                setNudValue(Calculation_Limit_MaxStringLength, s.Calculation_Limit_MaxStringLength);
                setNudValue(Calculation_Limit_MaxCallRecursions, s.Calculation_Limit_MaxCallRecursions);

                Appearance_Font_Button_Name.Text = s.Appearance_Font_Button.Face;
                Appearance_Font_Button_Bold.Checked = s.Appearance_Font_Button.Bold;
                Appearance_Font_Button_Italic.Checked = s.Appearance_Font_Button.Italic;
                setNudValue(Appearance_Font_Size, (decimal)s.Appearance_Font_Button.Size);
                Appearance_Font_Expr_Name.Text = s.Appearance_Font_Expr.Face;
                Appearance_Font_Expr_Bold.Checked = s.Appearance_Font_Expr.Bold;
                Appearance_Font_Expr_Italic.Checked = s.Appearance_Font_Expr.Italic;
                setNudValue(Appearance_Font_Expr_Size, (decimal)s.Appearance_Font_Expr.Size);

                reloadColorSettings();

                foreach (var c in s.GetUserConstants()) {
                    addConst(c);
                }
                constDelButton.Enabled = false;
                constEditButton.Enabled = false;

                foreach (var sf in s.GetScriptFilters()) {
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

        private void setNudValue(NumericUpDown nud, decimal value) {
            nud.Value = MathEx.Clip(nud.Minimum, nud.Maximum, value);
        }

        private void Startup_AutoStart_CheckedChanged(object sender, EventArgs e) {
            Shapoco.Windows.StartupShortcut.SetStartupRegistration(((CheckBox)sender).Checked);
        }

        private bool _settingDirectoryChanging = false;
        private void SaveSettingsInInstallDirectoryCheckBox_CheckedChanged(object sender, EventArgs e) {
            var chkSender = (CheckBox)sender;

            if (_settingDirectoryChanging) return;
            if (chkSender.Checked == AppDataManager.UseAssemblyPath) return;
            _settingDirectoryChanging = true;
            
            string settingPathFrom, settingPathTo;
            string historyPathFrom, historyPathTo;
            string notebookPathFrom, notebookPathTo;
            if (chkSender.Checked) {
                settingPathFrom = Settings.PathInRoamingDirectory;
                settingPathTo = Settings.PathInInstallDirectory;
                notebookPathFrom = Path.Combine(AppDataManager.RoamingUserDataPath, Book.NotebookFolderName);
                notebookPathTo = Path.Combine(AppDataManager.AssemblyPath, Book.NotebookFolderName);
                historyPathFrom = Path.Combine(AppDataManager.RoamingUserDataPath, Book.HistoryFolderName);
                historyPathTo = Path.Combine(AppDataManager.AssemblyPath, Book.HistoryFolderName);
            }
            else {
                settingPathTo = Settings.PathInRoamingDirectory;
                settingPathFrom = Settings.PathInInstallDirectory;
                notebookPathTo = Path.Combine(AppDataManager.RoamingUserDataPath, Book.NotebookFolderName);
                notebookPathFrom = Path.Combine(AppDataManager.AssemblyPath, Book.NotebookFolderName);
                historyPathTo = Path.Combine(AppDataManager.RoamingUserDataPath, Book.HistoryFolderName);
                historyPathFrom = Path.Combine(AppDataManager.AssemblyPath, Book.HistoryFolderName);
            }

            bool copySuccess = false;
            try {
                bool go = true;
                string fileExists = "";
                if (File.Exists(settingPathTo)) fileExists += "\r\n - " + settingPathTo;
                if (Directory.Exists(historyPathTo)) fileExists += "\r\n - " + historyPathTo;
                if (!string.IsNullOrEmpty(fileExists)) {
                    if (DialogResult.OK != MessageBox.Show("The destination file already exists. Are you sure you want to overwrite it?\r\n" + fileExists,
                        Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)) {
                        go = false;
                    }
                }
                if (go) {
                    if (!Directory.Exists(Path.GetDirectoryName(settingPathTo))) {
                        // 移動先ディレクトリの作成
                        Directory.CreateDirectory(Path.GetDirectoryName(settingPathTo));
                    }
                    if (File.Exists(settingPathFrom)) {
                        // 設定ファイルのコピー
                        File.Copy(settingPathFrom, settingPathTo, true);
                    }
                    if (Directory.Exists(notebookPathFrom)) {
                        // ノートブックのコピー
                        if (!Directory.Exists(notebookPathTo)) {
                            Directory.CreateDirectory(notebookPathTo);
                        }
                        foreach (var file in Directory.GetFiles(notebookPathFrom, "*.txt")) {
                            File.Copy(file, Path.Combine(notebookPathTo, Path.GetFileName(file)), true);
                        }
                    }
                    if (Directory.Exists(historyPathFrom)) {
                        // 履歴ファイルのコピー
                        if (!Directory.Exists(historyPathTo)) {
                            Directory.CreateDirectory(historyPathTo);
                        }
                        foreach (var file in Directory.GetFiles(historyPathFrom, "*.txt")) {
                            File.Copy(file, Path.Combine(historyPathTo, Path.GetFileName(file)), true);
                        }
                    }
                    AppDataManager.UseAssemblyPath = chkSender.Checked;
                    Settings.Instance.Save();
                    copySuccess = true;
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to move files:\r\n\r\n" + ex.Message, 
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (copySuccess) {
                try {
                    if (File.Exists(settingPathFrom)) {
                        File.Delete(settingPathFrom);
                    }
                    if (Directory.Exists(notebookPathFrom)) {
                        Directory.Delete(notebookPathFrom, true);
                    }
                    if (Directory.Exists(historyPathFrom)) {
                        Directory.Delete(historyPathFrom, true);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("The files were successfully copied, but the original files failed to be deleted.:\r\n\r\n" + ex.Message,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            loadSettingLocation();
            _settingDirectoryChanging = false;
        }

        void loadSettingLocation() {
            SaveSettingsInInstallDirectoryCheckBox.Checked = AppDataManager.UseAssemblyPath;
            txtSettingDirectoryPath.Text = AppDataManager.UseAssemblyPath ?
                Settings.PathInInstallDirectory : Settings.PathInRoamingDirectory;
        }

        private void OpenSettingFolderButton_Click(object sender, EventArgs e) {
            var path = Path.GetDirectoryName(txtSettingDirectoryPath.Text);
            try {
                if (!Directory.Exists(path)) {
                    if (DialogResult.Yes == MessageBox.Show(
                            "The folder does not exist. Do you want to create it?",
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question)) {
                        Directory.CreateDirectory(path);
                    }
                    else {
                        return;
                    }
                }
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ColorBox_SelectedColorChanged(object sender, EventArgs e) {
            colorPickerToSetting((ColorPickerBox)sender);
        }
        private void colorPickerToSetting(ColorPickerBox picker) {
            var prop = typeof(Settings).GetProperty((string)picker.Tag);
            if ((Color)prop.GetValue(Settings.Instance) != picker.SelectedColor) {
                prop.SetValue(Settings.Instance, picker.SelectedColor);
                requestAppearancePreview();
            }
        }

        private void ToggleLightDarkModeButton_Click(object sender, EventArgs e) {
            Settings.Instance.InvertColors();
            reloadColorSettings();
            requestAppearancePreview();
        }
        private void RotateColorHueButton_Click(object sender, EventArgs e) {
            Settings.Instance.RotateColorHue(60);
            reloadColorSettings();
            requestAppearancePreview();
        }
        private void SwapColorRbButton_Click(object sender, EventArgs e) {
            Settings.Instance.SwapColorRb();
            reloadColorSettings();
            requestAppearancePreview();
        }
        private void reloadColorSettings() {
            var s = Settings.Instance;
            foreach (var picker in _colorPickers) {
                var prop = typeof(Settings).GetProperty((string)picker.Tag);
                picker.SelectedColor = Color.FromArgb(255, (Color)prop.GetValue(s));
            }
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
                if (!Directory.Exists(Script_FolderPath.Text)) {
                    if (DialogResult.Yes == MessageBox.Show(
                            "The folder does not exist. Do you want to create it?", 
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ) {
                        Directory.CreateDirectory(Script_FolderPath.Text);
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

        private void requestAppearancePreview() {
            _reloadTimer.Stop();
            _reloadTimer.Interval = 100;
            _reloadTimer.Start();
        }

        private void _previewDelayTimer_Tick(object sender, EventArgs e) {
            ((Timer)sender).Stop();
            MainForm.Instance?.ReloadColorSettings();
            MainForm.Instance?.ReloadFontSettings();
        }
    }
}
