using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CNET_DDNS
{
    public partial class MainForm : Form
    {
        static string SettingsFileName = Path.Combine(Directory.GetCurrentDirectory(), "CNet-DDNS-settings.bin");
        const char DefaultPasswordChar = '\u25CF';
        const string BaseUrl = "https://ddns.ckts.info/";
        const string CheckUrl = "checkip.php";
        const string UpdateUrl = "updateip.php?hostname={0}&myip={1}";
        private static string currentIP = "";

        public MainForm()
        {
            InitializeComponent();
            InitForm();
            LoadSettings();
            AddInfo();
        }

        private async void checkButton_Click(object sender, EventArgs e) => await CheckIP();
        private async void updateButton_Click(object sender, EventArgs e) => await UpdateIP();
        private void saveButton_Click(object sender, EventArgs e) => SaveSettings();
        private void showKeyButton_Click(object sender, EventArgs e) => ToggleKey();
        private void aboutButton_Click(object sender, EventArgs e) => ShowAbout();

        private void AddLog(string text) => logTextBox.AppendText($"{text}\r\n");
        private void ClearLog() => logTextBox.Clear();

        private void AddInfo()
        {
            AddLog("With this program, you can check or update your C*NET DDNS IP-address.");
            AddLog("Press 'check' to view your current IP-address in the C*NET DNS.");
            AddLog("Press 'update' to update your current IP-address in the C*NET DNS.");
            AddLog("Click one of the buttons, or close this program to quit.");
        }

        private void InitForm()
        {
            this.Icon = Properties.Resources.MainIcon;
            this.Size = new Size { Height = 400, Width = 600 };
            this.MinimumSize = this.Size;
            this.MaximumSize = new Size { Height = this.Size.Height * 2, Width = this.Size.Width * 2 };
            foreach (TabPage tab in tabControl.TabPages) {
                tab.BackColor = this.BackColor;
                tab.UseVisualStyleBackColor = false;
                tab.BorderStyle = BorderStyle.None;
            }
            keyTextBox.PasswordChar = DefaultPasswordChar;
        }

        // Settings

        private bool ValidSettings()
        {
            var success =  
                !string.IsNullOrWhiteSpace(domainTextBox.Text) &&
                !string.IsNullOrWhiteSpace(userNameTextBox.Text) &&
                !string.IsNullOrWhiteSpace(keyTextBox.Text);
            if (!success) {
                AddLog("Please check your settings and try again!");
            }
            return success;
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsFileName)) {
                try {
                    var xDoc = XDocument.Parse(EncryptHelper.UnProtect(File.ReadAllBytes(SettingsFileName)));
                    domainTextBox.Text = xDoc.Root.Element("domain").Value;
                    userNameTextBox.Text = xDoc.Root.Element("username").Value;
                    keyTextBox.Text = xDoc.Root.Element("key").Value;
                }
                catch (Exception err) {
                    AddLog($"Error loading settings: {err.Message}");
                }
            } else {
                AddLog($"No settings found, please update and save your settings.");
            };
        }

        private void SaveSettings()
        {
            ClearLog();
            try {
                var xDoc = new XDocument();
                xDoc.Add(new XComment("This XML file contains settings for C*NET DDNS."));
                xDoc.Add(new XElement("settings"));
                xDoc.Root.Add(new XElement("domain", domainTextBox.Text));
                xDoc.Root.Add(new XElement("username", userNameTextBox.Text));
                xDoc.Root.Add(new XElement("key", keyTextBox.Text));
                File.WriteAllBytes(SettingsFileName, EncryptHelper.Protect(xDoc.ToString()));
                AddLog($"Settings are saved to {SettingsFileName}.");
                AddInfo();
            }
            catch (Exception err) {
                AddLog($"Error while saving settings: {err.Message}");
            }
            tabControl.SelectedTab = tabControl.TabPages[0];
        }

        private async Task SetCurrentIP() => currentIP = await DdnsWeb.MakeRequest(BaseUrl + CheckUrl);

        private void ToggleKey() => keyTextBox.PasswordChar = (keyTextBox.PasswordChar == DefaultPasswordChar) ? default : DefaultPasswordChar;

        private async Task CheckIP()
        {
            ClearLog();
            if (ValidSettings()) {
                AddLog("Check started, please wait...");
                await SetCurrentIP();
                AddLog($"Your current IP is: {currentIP}");
                var domain = domainTextBox.Text;
                AddLog($"Getting IP-addresses belonging to '{domain}', please wait...");
                var dnsLog = await DdnsDns.GetCurrentIpForHostName(domain);
                AddLog(string.Join("\r\n", dnsLog));
                AddLog("Check completed.");
            }
        }

        private async Task UpdateIP()
        {
            ClearLog();
            if (ValidSettings()) {
                AddLog("Update has started, please wait...");
                await SetCurrentIP();
                AddLog($"Your current IP-address is: {currentIP}.");
                var domain = domainTextBox.Text;
                var userName = userNameTextBox.Text;
                var password = keyTextBox.Text;
                var url = BaseUrl + string.Format(UpdateUrl, domain, currentIP);
                var response = await DdnsWeb.MakeRequest(url, userName, password);
                AddLog($"Update response: {response}");
                AddLog($"If you see 'good' and your current IP-address, the update completed successfully.");
                if (!response.Contains("good")) {
                    AddLog($"If you see 'badauth', check your settings (domain/username/password).");
                }
                AddLog("Update completed.");
            }
        }

        private void ShowAbout()
        {
            ClearLog();
            AddLog("This program was created by Gerard van Til.");
            AddLog("If you want to talk to me, please call me on C*NET!");
            AddLog("Between 8:30PM and 9:30PM (CET), you can reach me on C*NET at +31 5900 99009.");
        }

    }
}
