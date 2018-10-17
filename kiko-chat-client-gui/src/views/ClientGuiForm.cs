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
        private bool mouseDown;
        private Point lastLocation;
        private Member member;
        private List<Client> OpenChats = new List<Client>();

        public ClientGuiForm()
        {
            InitializeComponent();
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
            catch (FileNotFoundException) { }
            catch (NullReferenceException) { }
            catch (JsonException jE)
            {
                MessageBox.Show(jE.ToString());
                // MessageBox.Show("Error occured during the loading of your previous groups. We are sorry for the inconvinience");
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

        private string GetSelectedGroupHostAddress()
        {
            return member.Find_Group_By_Name(groupSelectorBox.SelectedItem.ToString()).HostAddress();
        }

        private void JoinGroupRequest(GroupData groupProperty, Member member)
        {
            // TODO >> Inform server that this member left the group.
           throw new NotImplementedException();
        }

        private void CreateGroupRequest(GroupData groupProperty, Member member)
        {
            // TODO >> Inform server that this member left the group.
            throw new NotImplementedException();
        }

        private void UpdateGroupAndMember(GroupData group)
        {
            member.Add_Group(group);
            groupSelectorBox.Items.Add(group.Name);
        }

        private void GenerateGroupForm(object sender, EventArgs e, bool newgroup = false)
        {
            using (GroupForm groupForm = new GroupForm())
            {
                GroupData groupData;

                if (groupForm.ShowDialog() == DialogResult.OK)
                {
                    groupData = groupForm.GroupProperty;
                    try
                    {
                        MessageBox.Show($"Requesting server to {(newgroup ? "create" : "join")} group... Please wait");
                        if (newgroup)
                        {
                            CreateGroupRequest(groupData, this.member);
                        } else
                        {
                            JoinGroupRequest(groupData, this.member);
                        }
                        UpdateGroupAndMember(new GroupData(groupData.Ip, groupData.Port, groupData.Name, default(DateTime)));
                    }
                    catch (Exception exc)
                    {
                        // TODO Use better exceptions.
                        MessageBox.Show(exc.Message);
                    }
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

        private void ShowGroupMembers()
        {
            // TODO >> Make group members textbox an actual list containing Member objects so that future versions can include sending direct messages to those members or obtain info about them.
            // TODO >> Display members of the current selected "Group Tab", which also needs to be implemented.
            GroupData group = member.Find_Group_By_Name(groupSelectorBox.SelectedItem.ToString());
            // TODO >> Request Server to send MemberData list belonging to this group.
            // chatMembersBox.Text = group.Members_ToString();
            throw new NotImplementedException();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // TODO >> Inform server that this member is about to connect to this group, temporarily >> Open connection.
            GroupData groupdata = member.Find_Group_By_Name(groupSelectorBox.SelectedItem.ToString());
            OpenChats.Add(new Client(chatWindow, member.Get_Member_Data(), groupdata));
            // ShowGroupMembers();
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            // TODO >> Inform server that this member is about to disconnect from this group, temporarily >> Close connection.
            string hostAddress = GetSelectedGroupHostAddress();
            // Send message to SV
            // Close TCP connection
            throw new NotImplementedException();
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

        private void ClientGuiForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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
    }
}
