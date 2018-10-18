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
    public delegate void SetGroupMembers(string Nickname);

    class Client : MarshalByRefObject, IClientObject
    {
        #region Fields

        private const string client_api_object = "clientObject.rem";
        private const string server_api_object = "serverObject.rem";
        private string server_proxy_url = "";
        private bool connected = false;
        private RichTextBox chat_window;
        private TextBox chat_members_box;
        private MemberData member_data;
        private GroupData group_data;
        private TcpChannel tcpChannel;
        private BinaryClientFormatterSinkProvider clientProvider;
        private BinaryServerFormatterSinkProvider serverProvider;
        private EventProxy eventProxy;
        private IServerObject server_proxy;

        #endregion

        #region Constructors

        public Client(RichTextBox chatwindow, TextBox chatmembersbox, MemberData memberdata, GroupData groupdata)
        {
            chat_window = chatwindow;
            chat_members_box = chatmembersbox;
            member_data = memberdata;
            group_data = groupdata;

            clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

            eventProxy = new EventProxy();
            eventProxy.MessageArrived += new MessageArrivedEvent(eventProxy_MessageArrived);

            // TODO Name needs to be different for each client. " + Port"
            Hashtable channelProperties = new Hashtable() {
                { "name", client_api_object},
                { "port", Int32.Parse(groupdata.Port) }
            };

            tcpChannel = new TcpChannel(channelProperties, clientProvider, serverProvider);
            ChannelServices.RegisterChannel(tcpChannel, false);

            server_proxy_url = string.Join("tcp://", group_data.HostAddress(), "/", server_api_object);
            RemotingConfiguration.RegisterWellKnownClientType(new WellKnownClientTypeEntry(typeof(IServerObject), server_proxy_url));

            Do_Connect();
        }

        #endregion

        #region Delegate Invoking Methods

        private void SetTextBox(string Message)
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

        private void SetGroupMembers(string Nickname)
        {
            Nickname = string.Join(Nickname, Environment.NewLine);
            if (chat_window.InvokeRequired)
            {
                chat_window.BeginInvoke(new SetGroupMembers(chat_window.AppendText), Nickname);
                return;
            }
            else
            {
                chat_window.AppendText(Nickname);
            }
        }

        #endregion

        #region Contract Implementation

        public void RecieveNewMessage(string message, DateTime messagetimestamp, MemberData sender, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void AddMemberToGroup(MemberData newmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void RemoveMemberFromGroup(MemberData oldmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void KickedFromServer(string HostAddress)
        {
            throw new NotImplementedException();
        }

        public void UpdateGroupMember(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Client Features Interacting With The Server Proxy

        // This is the function we will use to add a message to the chat box of the proper group.
        private void eventProxy_MessageArrived(string Message)
        {
            SetTextBox(Message);
        }

        public void Do_Connect()
        {
            string chatHistory;
            try
            {
                server_proxy = (IServerObject)Activator.GetObject(typeof(IServerObject), server_proxy_url);
                // TODO >> Consider "Connect" not returning a byte[], but instead having the server send a chat byte[] on it's own upon recieving the connection.
                chatHistory = Convert.ToBase64String(server_proxy.Connect(member_data, group_data));
                SetTextBox(chatHistory);
                // Now we have to attach the events...
                server_proxy.MessageArrived += new MessageArrivedEvent(eventProxy.LocallyHandleMessageArrived);
                connected = true;
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
                SetGroupMembers(member.Nickname);
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
