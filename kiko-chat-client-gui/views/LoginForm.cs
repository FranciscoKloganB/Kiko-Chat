using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.security_objects;

namespace kiko_chat_client_gui
{

    public partial class LoginForm : Form
    {

        public MemberData MemberProperty { get; set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void FormJsonLogin(MemberData property)
        {
            string directoryPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "run", "json-login"});

            Directory.CreateDirectory(directoryPath);

            using (StreamWriter streamWritter = new StreamWriter(directoryPath + "/login-settings.json"))
            using (JsonWriter jsonWritter = new JsonTextWriter(streamWritter))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonWritter, property);
            }
        }

        private MemberData LoadJsonLogin()
        {
            string filePath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "run", "json-login", "login-settings.json" });

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("There are currently no previous settings to load from. Please create new ones.");
            }

            using (StreamReader streamReader = new StreamReader(@filePath))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                MemberData memberData;
                JsonSerializer serializer = new JsonSerializer();
                memberData = serializer.Deserialize<MemberData>(jsonReader);
                return memberData;
            }
        }

        private void AcceptNewSetting_Click(object sender, EventArgs e)
        {
            string ip = KCSecurityManager.ValidateIP(clientAddress.Text);
            string port = KCSecurityManager.ValidatePort(portBox.Text);
            string nickname = KCSecurityManager.ValidateNickname(nickNameBox.Text, " Nickname *");
            string fullname = KCSecurityManager.ValidateRegularName(fullNameBox.Text, " Full Name");
            string email = KCSecurityManager.ValidateEmail(emailBox.Text);
            string country = KCSecurityManager.ValidateCountry(countryBox.Text, " Country");
            MemberProperty = new MemberData(ip, port, nickname, fullname, email, country);
            FormJsonLogin(MemberProperty);
            this.DialogResult = DialogResult.OK;
        }

        private void UsePreviousSettings_Click(object sender, EventArgs e)
        {
            try
            {
                MemberProperty = LoadJsonLogin();
                this.DialogResult = DialogResult.OK;
            } catch (InvalidOperationException iOE)
            {
                MessageBox.Show(iOE.Message);
            }      
        }

        private void loginForm_Load(object sender, EventArgs e)
        {
            clientAddress.Text = KCSecurityManager.GetLocalPublicAddress();
        }

        private void clientAddress_Enter(object sender, EventArgs e)
        {
            if (clientAddress.Text.Equals(" Internet Location"))
            {
                clientAddress.Text = "";
                clientAddress.ForeColor = Color.Black;
            }
        }

        private void clientAddress_Leave(object sender, EventArgs e)
        {
            if (clientAddress.Text.Equals(""))
            {
                clientAddress.Text = " Internet Location";
                clientAddress.ForeColor = Color.Silver;
            }
        }

        private void portBox_Enter(object sender, EventArgs e)
        {
            if (portBox.Text.Equals(" Port *"))
            {
                portBox.Text = "";
                portBox.ForeColor = Color.Black;
            }
        }

        private void portBox_Leave(object sender, EventArgs e)
        {
            if (portBox.Text.Equals(""))
            {
                portBox.Text = " Port *";
                portBox.ForeColor = Color.Silver;
            }
        }

        private void nickNameBox_Enter(object sender, EventArgs e)
        {
            if (nickNameBox.Text.Equals(" Nickname *"))
            {
                nickNameBox.Text = "";
                nickNameBox.ForeColor = Color.Black;
            }
        }

        private void nickNameBox_Leave(object sender, EventArgs e)
        {
            if (nickNameBox.Text.Equals(""))
            {
                nickNameBox.Text = " Nickname *";
                nickNameBox.ForeColor = Color.Silver;
            }
        }

        private void fullNameBox_Enter(object sender, EventArgs e)
        {
            if (fullNameBox.Text.Equals(" Full Name"))
            {
                fullNameBox.Text = "";
                fullNameBox.ForeColor = Color.Black;
            }
        }

        private void fullNameBox_Leave(object sender, EventArgs e)
        {
            if (fullNameBox.Text.Equals(""))
            {
                fullNameBox.Text = " Full Name";
                fullNameBox.ForeColor = Color.Silver;
            }
        }

        private void emailBox_Enter(object sender, EventArgs e)
        {
            if (emailBox.Text.Equals(" Email"))
            {
                emailBox.Text = "";
                emailBox.ForeColor = Color.Black;
            }
        }

        private void emailBox_Leave(object sender, EventArgs e)
        {
            if (emailBox.Text.Equals(""))
            {
                emailBox.Text = " Email";
                emailBox.ForeColor = Color.Silver;
            }
        }

        private void countryBox_Enter(object sender, EventArgs e)
        {
            if (countryBox.Text.Equals(" Country"))
            {
                countryBox.Text = "";
                countryBox.ForeColor = Color.Black;
            }
        }

        private void countryBox_Leave(object sender, EventArgs e)
        {
            if (countryBox.Text.Equals(""))
            {
                countryBox.Text = " Country";
                countryBox.ForeColor = Color.Silver;
            }
        }
    }
}