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

namespace kiko_chat_client_gui.domain_objects
{
    public delegate void SetBoxText(string Message);
    public delegate void SetGroupMember(string Nickname);
    public delegate void UnsetGroupMember(params object[] args);

    class Client : MarshalByRefObject, IClientObject
    {
        #region Fields

        private const string client_api_object = "clientObject";
        private const string server_api_object = "serverObject.rem";
        private string server_proxy_url = "";
        private bool connected = false;
        private RichTextBox chat_window;
        private ListBox chat_members_box;
        private MemberData member_data;
        private GroupData group_data;
        private TcpChannel tcpChannel;
        private BinaryClientFormatterSinkProvider clientProvider;
        private BinaryServerFormatterSinkProvider serverProvider;
        private EventProxy eventProxy;
        private IServerObject server_proxy;

        #endregion

        #region Delegate Invoking Methods

        private void SetMessage(string Message)
        {
            Message = string.Join(Message, Environment.NewLine);
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
                chat_members_box.BeginInvoke(new UnsetGroupMember(delegate { chat_members_box.Items.Add(member); }));
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
            member_data = memberdata;
            group_data = groupdata;
            server_proxy_url = string.Join("tcp://", group_data.HostAddress(), "/", server_api_object);

            // Publishes this Client as a remote object that can be later referenced by some server.
            int port_as_int = Int32.Parse(groupdata.Port);
            string unique_name = string.Join(client_api_object, port_as_int, ".rem");
            RemotingServices.Marshal(this, unique_name, typeof(Client));

            /*
            clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            */
            Hashtable channelProperties = new Hashtable() {
                { "name", unique_name },
                { "port", port_as_int }
            };

            /* tcpChannel = new TcpChannel(channelProperties, clientProvider, serverProvider); */
            tcpChannel = new TcpChannel(channelProperties, null, null);

            ;
            ChannelServices.RegisterChannel(tcpChannel, false);
            RemotingConfiguration.RegisterWellKnownClientType(new WellKnownClientTypeEntry(typeof(IServerObject), server_proxy_url));

            Do_Connect();
        }

        #endregion

        #region Contract Implementation

        public void RecieveNewMessage(string message, DateTime messagetimestamp)
        {
            group_data.LastKnownMessage = messagetimestamp;
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

        public void UpdateGroupMember(MemberData member)
        {
            UnsetGroupMember(member);
            SetGroupMember(member);
        }

        public void KickedFromServer()
        {
            Do_Disconnect();
        }
         
        #endregion

        #region Client Features Interacting With The Server Proxy

        public void Do_Connect()
        {
            string chatHistory = "";
            try
            {
                // TODO >> Consider "Connect" not returning a byte[], but instead having the server send a chat byte[] on it's own upon recieving the connection.
                server_proxy = (IServerObject)Activator.GetObject(typeof(IServerObject), server_proxy_url);
                // Send a member_data and group_data so that a client can make the registration of this user.
                chatHistory = Convert.ToBase64String(server_proxy.Connect(member_data, group_data));
                // Set connected to true;
                connected = true;
                // Append the recieved chat history upon connection to the chat box of this group.
                SetMessage(chatHistory);
            }
            catch (SocketException sE)
            {
                connected = false;
                throw sE;
            }
        }

        public void Do_Disconnect()
        {
            if (!connected) { throw new InvalidOperationException("Not currenctly connected to the specified group."); }
            server_proxy.Disconnect(member_data, group_data);
            server_proxy.MessageArrived -= eventProxy.LocallyHandleMessageArrived;
            ChannelServices.UnregisterChannel(tcpChannel);
            // TODO >> Store group_data && chat.
        }

        public void Do_Send(string message)
        {
            if (!connected) { throw new InvalidOperationException("You need to be connected to this group in order to send messages."); }

            server_proxy.PublishMessage(message, DateTime.Now, member_data, group_data);
        }

        public void Do_RetriveGroupMembers()
        {
            if (!connected) { throw new OperationCanceledException("Program error: Tryed to retrieve group members from a group you are not currently connected to."); }

            List<MemberData> members = server_proxy.RetriveGroupMembers(member_data, group_data);

            foreach (MemberData member in members)
            {
                SetGroupMember(member);
            }
        }

        #endregion

        #region Other class methods

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
