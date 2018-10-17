using System;
using System.Collections;
using System.Net;
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

    class Client : MarshalByRefObject, IClientObject
    {
        #region Fields

        private const string client_api_object = "clientObject.rem";
        private const string server_api_object = "serverObject.rem";
        private string server_proxy_url = "";
        private bool connected = false;
        private RichTextBox chat_window;
        private MemberData member_data;
        private GroupData group_data;
        private TcpChannel tcpChannel;
        private BinaryClientFormatterSinkProvider clientProvider;
        private BinaryServerFormatterSinkProvider serverProvider;
        private EventProxy eventProxy;
        private IServerObject server_proxy;


        #endregion


        #region Constructors

        public Client(RichTextBox chatwindow, MemberData memberdata, GroupData groupdata)
        {
            chat_window = chatwindow;
            member_data = memberdata;
            group_data = groupdata;

            clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

            eventProxy = new EventProxy();
            eventProxy.MessageArrived += new MessageArrivedEvent(eventProxy_MessageArrived);

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

        #region Client Features

        // This is the function we will use to add a message to the chat box of the proper group.
        private void eventProxy_MessageArrived(string Message)
        {
            SetTextBox(chat_window, Message);
        }

        private void Do_Connect()
        {
            try
            {
                server_proxy = (IServerObject)Activator.GetObject(typeof(IServerObject), server_proxy_url);
                server_proxy.Connect(member_data, group_data);
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

        public void Do_Disconnect(MemberData member, GroupData group)
        {
            if (!connected) { return; }
            // TODO Not Completed.
            // First remove the event, then, close the channel and store conversation in local file and put last time stmap in group data for serialization upon gui closure.
            server_proxy.MessageArrived -= eventProxy.LocallyHandleMessageArrived;
            ChannelServices.UnregisterChannel(tcpChannel);
        }

        public void Do_Send(string message, MemberData member, GroupData group)
        {
            if (!connected) { return; }
            server_proxy.PublishMessage(message, DateTime.Now, member, group);
        }

        private void SetTextBox(RichTextBox chatWindow, string Message)
        {
            Message = string.Join(Message, Environment.NewLine);
            // InvokeRequired verifies if the owner chatWindow is another thread other than the one calling this method. It certainly is, checking for sanity.
            if (chatWindow.InvokeRequired)
            {
                chat_window.BeginInvoke(new SetBoxText(chat_window.AppendText), Message);
                return;
            }
            else
            {
                chatWindow.AppendText(Message);
            }
        }

        #endregion
    }
}
