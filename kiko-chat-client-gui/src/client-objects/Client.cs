using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using kiko_chat_contracts.web_services;
using kiko_chat_contracts.data_objects;
using System.Windows.Forms;
using kiko_chat_contracts.security_objects;

// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.remoting.wellknownclienttypeentry?view=netframework-4.7.2#remarks Explaining why you dont need to use WellKnownClass and Activator.GetObject simultaneously
// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.remoting.remotingservices.marshal?view=netframework-4.7.2
namespace kiko_chat_client_gui.domain_objects
{
    public delegate void SetBoxText(string Message);
    public delegate void SetGroupMember(string Nickname);
    public delegate void UnsetGroupMember(params object[] args);

    class Client : MarshalByRefObject, IClientObject
    {
        #region Fields
        private const string client_api_object = "clientObject";
        private const string server_api_object = "serverObject";
        private string server_proxy_url = "";
        private bool connected = false;
        private IServerObject server_proxy;
        private TcpChannel tcpChannel;
        private ObjRef internalRef;
        private RichTextBox chat_window;
        private ListBox chat_members_box;
        private MemberData member_data;
        private GroupData group_data;
        private object groupLocker = new object();

        /*
        private BinaryClientFormatterSinkProvider clientProvider;
        private BinaryServerFormatterSinkProvider serverProvider;
        */
        #endregion

        #region Delegate Invoking Methods

        private void SetMessage(string Message)
        {
            Message = Message + Environment.NewLine;
            // InvokeRequired verifies if the owner chatWindow is another thread other than the one calling this method. It certainly is, checking for sanity.
            if (chat_window.InvokeRequired)
            {
                chat_window.BeginInvoke(new SetBoxText(chat_window.AppendText), Message);
                return;
            }
            else
            {
                chat_window.AppendText(Message);
            }
        }

        private void SetGroupMember(MemberData member)
        {
            if (chat_members_box.InvokeRequired)
            {
                chat_members_box.BeginInvoke(new SetGroupMember(delegate { chat_members_box.Items.Add(member); }));
                return;
            }
            else
            {
                chat_members_box.Items.Add(member);
            }
        }

        private void UnsetGroupMember(MemberData member)
        {
            if (chat_members_box.InvokeRequired)
            {
                chat_members_box.BeginInvoke(new UnsetGroupMember(delegate { chat_members_box.Items.Remove(member); }));
                return;
            }
            else
            {
                chat_members_box.Items.Remove(member);
            }
        }

        #endregion

        #region Constructors

        public Client(RichTextBox chatwindow, ListBox chatmembersbox, MemberData memberdata, GroupData groupdata)
        {
            chat_window = chatwindow;
            chat_members_box = chatmembersbox;

            string randomPort = Security.ValidatePort("");
            member_data = memberdata;
            member_data.Ip = $"{member_data.Ip}:{randomPort}";

            lock (groupLocker)
            {
                group_data = groupdata;
            }

            string serverAddress = group_data.HostAddress();
            string unique_name = client_api_object + randomPort;

            server_proxy_url = $"tcp://{serverAddress}/{server_api_object}";
            MessageBox.Show("Server_proxy_url: " + server_proxy_url);

             /*
            clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            */

            Hashtable channelProperties = new Hashtable() {
                { "name", unique_name },
                { "port", Int32.Parse(randomPort) }
            };

            /* tcpChannel = new TcpChannel(channelProperties, clientProvider, serverProvider); */
            tcpChannel = new TcpChannel(channelProperties, null, null);
            ChannelServices.RegisterChannel(tcpChannel, false);
            //tcpChannel.StartListening(null);

            // Create a ObjRef type of this Client with the specified URI
            internalRef = RemotingServices.Marshal(this, unique_name, typeof(Client));
            server_proxy = (IServerObject)Activator.GetObject(typeof(IServerObject), server_proxy_url);
        }

        #endregion

        #region Contract Implementation

        public void RecieveNewMessage(string message, DateTime messagetimestamp)
        {
            lock (groupLocker)
            {
                group_data.LastKnownMessage = messagetimestamp;
            }
            SetMessage(message);
        }

        public void AddMemberToGroup(MemberData newmember)
        {
            SetGroupMember(newmember);
        }

        public void RemoveMemberFromGroup(MemberData oldmember)
        {
            UnsetGroupMember(oldmember);
        }

        public void UpdateGroupMember(MemberData oldmemberdata, MemberData memberData)
        {
            UnsetGroupMember(oldmemberdata);
            SetGroupMember(memberData);
        }

        public void KickedFromServer()
        {
            Do_Disconnect();
        }
         
        #endregion

        #region Client Features Interacting With The Server Proxy

        public void Do_CreatGroup()
        {
            server_proxy.CreateGroup(member_data, group_data.Name);
        }

        public void Do_JoinGroup()
        {
            server_proxy.LeaveGroup(member_data, group_data.Name);
        }

        public void Do_LeaveGroup()
        {
            server_proxy.LeaveGroup(member_data, group_data.Name);
        }

        public void Do_Connect()
        {
            string groupName;
            DateTime lastMessageTimeStamp;

            if (connected) { return;  }
            
            try
            {
                lock (groupLocker)
                {
                    groupName = group_data.Name;
                    lastMessageTimeStamp = group_data.LastKnownMessage;
                }
                server_proxy.Connect(member_data, groupName, lastMessageTimeStamp);
                connected = true;
            } catch (Exception exc)
            {
                throw exc;
            }
        }

        public void Do_Disconnect(bool appshutdown = false)
        {
            string groupName;

            if (!connected) { return; }

            lock (groupLocker)
            {
                groupName = group_data.Name;
            }

            server_proxy.Disconnect(member_data, group_data.Name);
            // Instructs the current channel to stop listening for requests.
            if (!appshutdown) {
                tcpChannel.StopListening(null);
            } else
            {
                // Unregister tcpChannel from the registred channels list.
                ChannelServices.UnregisterChannel(tcpChannel);
                // Stops this client from recieving any messages through the registred remoting channels
                RemotingServices.Disconnect(this);
            }
            // TODO >> Store group_data && chat.
        }

        public void Do_Send(string message)
        {
            if (!connected) { throw new InvalidOperationException("You need to be connected to this group in order to send messages."); }

            server_proxy.PublishMessage(message, group_data.Name, DateTime.Now, member_data);
        }

        public void Do_RetriveGroupMembers()
        {
            if (!connected) { throw new OperationCanceledException("Program error: Tryed to retrieve group members from a group you are not currently connected to."); }

            List<MemberData> members = server_proxy.RetriveGroupMembers(group_data.Name, member_data);

            foreach (MemberData member in members)
            {
                SetGroupMember(member);
            }
        }

        #endregion

        #region Other class methods

        public bool BelongsToGroup(GroupData groupdata, out Client client)
        {
            if (group_data.Equals(groupdata))
            {
                client = this;
                return true;
            }
            else
            {
                client = null;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return group_data.Equals(obj);
        }

        public override int GetHashCode()
        {
            return group_data.GetHashCode();
        }

        #endregion
    }
}
