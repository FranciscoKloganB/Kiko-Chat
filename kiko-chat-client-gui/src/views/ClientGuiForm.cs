using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.security_objects;
using kiko_chat_client_gui.domain_objects;

namespace kiko_chat_client_gui
{
    /*
    Forms have a Point attribute that represents the upper-left corner of the Form in screen coordinates composed of X and Y integers.
        See @https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.form.location?view=netframework-4.7.2
    */
    public partial class ClientGuiForm : Form
    {
        #region Fields

        private bool mouseDown;
        private Point lastLocation;
        private Member member;
        private Dictionary<string, List<RichTextBox>> chatWindowsDictionary;
        private Dictionary<string, List<ListBox>> chatMembersDictionary;
        private List<Client> openClients;

        #endregion

        #region Constructors

        public ClientGuiForm()
        {
            InitializeComponent();

            chatWindowsDictionary = new Dictionary<string, List<RichTextBox>>();
            chatMembersDictionary = new Dictionary<string, List<ListBox>>();
            openClients = new List<Client>();

            using (LoginForm loginForm = new LoginForm())
            {
                MemberData memberData;      
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    memberData = loginForm.MemberProperty;
                    this.member = new Member(memberData.Ip, memberData.Port, memberData.Nickname, memberData.Name, memberData.Email, memberData.Country);
                }
                loginForm.Dispose();
            }
            FillGuiInfo();
        }

        #endregion

        #region Client Gui Loading Methods

        private void FillGuiInfo()
        {
            try
            {
                List<GroupData> groups = Load_Groups();
                foreach (GroupData group in groups)
                {
                    MessageBox.Show(group.ToString());
                    member.Add_Group(group);
                    groupSelectorBox.Items.Add(group.Name);
                }
                groupSelectorBox.SelectedIndex = 0;
            }
            catch (ArgumentOutOfRangeException) { }
            catch (FileNotFoundException) { }
            catch (NullReferenceException) { }
            catch (JsonException)
            {
                MessageBox.Show("Error occured during the loading of your previous groups. We are sorry for the inconvinience");
            } 
        }

        public List<GroupData> Load_Groups()
        {
            string filePath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "run", "json-groups", "member-groups.json" });

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("No file to load groups from.");
            }

            using (StreamReader streamReader = new StreamReader(@filePath))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                List<GroupData> groups;
                JsonSerializer serializer = new JsonSerializer();
                groups = serializer.Deserialize<List<GroupData>>(jsonReader);
                return groups;
            }
        }

        #endregion

        #region Other Client Methods

        private string GetSelectedGroupHostAddress()
        {
            return member.Find_Group_By_Name(groupSelectorBox.SelectedItem.ToString()).HostAddress();
        }

        private void UpdateGroupAndMember(GroupData group)
        {
            member.Add_Group(group);
            groupSelectorBox.Items.Add(group.Name);
        }

        #endregion

        #region Client Gui Local Event Handlers

        private void GenerateGroupForm(object sender, EventArgs e, bool newgroup = false)
        {
            using (GroupForm groupForm = new GroupForm())
            {
                GroupData groupData;

                if (groupForm.ShowDialog() == DialogResult.OK)
                {
                    groupData = groupForm.GroupProperty;
                    UpdateGroupAndMember(new GroupData(groupData.Ip, groupData.Port, groupData.Name, default(DateTime)));
                    groupSelectorBox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Invalid Parameters were given. Try again. If you need help take a look at the tooltips.");
                }
                groupForm.Dispose();
            }
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            string hostAddress = GetSelectedGroupHostAddress();
            string groupName = groupSelectorBox.SelectedItem.ToString();
            string message = newMessageBox.Text;

            if (!message.Trim().Equals(""))
            {
                // TODO Send message and self to server, who should then broadcast the message to other peoples chat.
            }
            newMessageBox.Clear();
        }

        private void DeleteNewMessageButton_Click(object sender, EventArgs e)
        {
            newMessageBox.Clear();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // MARTELO ::: Not currently using chatWindowsDictionary nor chatMembersDictionary. Can only be connected to a group at a time.
            GroupData groupdata = member.Find_Group_By_Name(groupSelectorBox.SelectedItem.ToString());
            Client client = new Client(chatWindow, chatMembersBox, member.Get_Member_Data(), groupdata);
            if (!openClients.Contains(client))
            {
                openClients.Add(client);
            }
            client.Do_RetriveGroupMembers();
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            GroupData groupdata = member.Find_Group_By_Name(groupSelectorBox.SelectedItem.ToString());
            foreach (Client openChat in openClients)
            {
                if (openChat.Equals(groupdata))
                {
                    openChat.Do_Disconnect(appshutdown: false);
                }
            }
            // TODO >> Store conversation in a local FILE and update group_data with latest message timestamp;
        }

        private void CreateGroupButton_Click(object sender, EventArgs e)
        {
            GenerateGroupForm(sender, e, newgroup: true);
        }

        private void JoinNewGroupButton_Click(object sender, EventArgs e)
        {
            GenerateGroupForm(sender, e, newgroup: false);
        }

        private void LeaveGroupButton_Click(object sender, EventArgs e)
        {
            string groupName = groupSelectorBox.SelectedItem.ToString();
            member.Leave_Group(groupName);
            // TODO >> Inform server that this member left the group.
            groupSelectorBox.Items.Remove(groupSelectorBox.SelectedItem);
        }

        private void applyNickNameChangeButton_Click(object sender, EventArgs e)
        {
            string nickname = Security.ValidateNickname(nickNameChangeBox.Text, " New nickname");
            // TODO >> Send new nickname to server and broadcast it to all other users.
            throw new NotImplementedException();
            //member.Nickname = nickname;
        }

        private void nickNameChangeBox_Enter(object sender, EventArgs e)
        {
            if (nickNameChangeBox.Text.Equals(" New nickname"))
            {
                nickNameChangeBox.Text = "";
                nickNameChangeBox.ForeColor = Color.Black;
            }
        }

        private void nickNameChangeBox_Leave(object sender, EventArgs e)
        {
            if (nickNameChangeBox.Text.Equals(" New nickname"))
            {
                nickNameChangeBox.Text = "";
                nickNameChangeBox.ForeColor = Color.Silver;
            }
        }

        #endregion

        #region Other Local Event Handling Methods

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        #endregion

        #region Client Gui Exiting Methods

        private void ClientGuiForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Client client in openClients) {
                client.Do_Disconnect(appshutdown: true);
            }

            {
                string directoryPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "run", "json-groups" });

                Directory.CreateDirectory(directoryPath);

                using (StreamWriter streamWritter = new StreamWriter(directoryPath + "/member-groups.json"))
                using (JsonWriter jsonWritter = new JsonTextWriter(streamWritter))
                {
                    try
                    {
                        List<GroupData> property = member.Get_Groups();
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jsonWritter, property);
                    } catch (NullReferenceException)
                    {
                        // NOP
                    }
                }
            }
        }

        #endregion
    }
}
