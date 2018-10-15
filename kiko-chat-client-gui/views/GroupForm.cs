using System;
using System.Windows.Forms;
using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.security_objects;

namespace kiko_chat_client_gui
{
    public partial class GroupForm : Form
    {
        public GroupData GroupProperty { get; set; }

        public GroupForm()
        {
            InitializeComponent();
        }

        private void ConfirmGroupForm_Click(object sender, EventArgs e)
        {
            string ip = KCSecurityManager.ValidateIP(ipBox.Text);
            string port = KCSecurityManager.ValidatePort(portBox.Text);
            string groupName = KCSecurityManager.ValidateRegularName(groupNameBox.Text, " Full Name");

            GroupProperty = new GroupData(ip, port, groupName);

            this.DialogResult = DialogResult.OK;
        }

        private void CancelGroupForm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
