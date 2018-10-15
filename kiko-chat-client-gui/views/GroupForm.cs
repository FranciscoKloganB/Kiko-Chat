using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using kiko_chat_contracts;

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
