using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.web_services;
using kiko_chat_contracts.security_objects;
using System.Collections;

namespace kiko_chat_server_console.server_objects
{
    /*
    * Server inherits from MarshByRefObject because we want our server to be marshaled across boundaries using a reference to the server object (Proxy on the client side)
    * Server implements IServerObject to force the implementation of the contracts the clients of this server expect to find on the proxy.
    */
    class Server : MarshalByRefObject, IServerObject
    {
        #region Fields
        /*
        * TcpServerChannel is a reference to the TCP remoting channel that we are using for our server.
        * ObjRef holds an internal reference to the object being presented (marshaled) for remoting
        */
        private const int RANDOM_PORT = 0;
        private const string server_api_object = "serverObject.Rem";
        private bool server_is_active = false;
        private TcpServerChannel serverChannel;
        private ObjRef internalRef;

        #endregion
        #region IServerObject Member Implementation

        public event MessageArrivedEvent MessageArrived;

        public byte[] Connect(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        public DateTime CreateGroup(MemberData owner, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        public DateTime JoinGroup(MemberData subscribingmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void LeaveGroup(MemberData unsubscribingmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void PublishMessage(string message, DateTime messagetimestamp, MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        public List<MemberData> RetriveGroupMembers(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void UpdateMember(MemberData member)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Server Implementation

        public void StartServer()
        {
            if (server_is_active) { return; }

            // Create properties for a TcpServerChannel that uses a Binary Sink.
            Hashtable channelProperties = new Hashtable() {
                { "name", server_api_object},
                { "port", RANDOM_PORT }
            };

            // BinaryServerFormatterSinkProvider we provide events across remoting boundaries with binary implementation instead of XML. Filter to Full in order for events to work properly.
            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = TypeFilterLevel.Full;
            serverChannel = new TcpServerChannel(channelProperties, serverProv);

            Console.WriteLine($"Server running at: {serverChannel.GetUrlsForUri(server_api_object)[0]}");
            Console.WriteLine("Press 'Enter' Key to shutdown the server");

            try
            {
                ChannelServices.RegisterChannel(serverChannel, false);
                internalRef = RemotingServices.Marshal(this, channelProperties["name"].ToString());
                server_is_active = true;
            }
            catch (RemotingException rE)
            {
                Console.WriteLine("Could not start the server... " + Environment.NewLine + rE.Message);
            }
        }

        public void StopServer()
        {
            if (!server_is_active) { return; }

            RemotingServices.Unmarshal(internalRef);

            try
            {
                ChannelServices.UnregisterChannel(serverChannel);
            }
            catch (Exception) { }

            Console.WriteLine($"Stoped server running at: {serverChannel.GetUrlsForUri(server_api_object)[0]}");
        }

        private void SafeInvokeMessageArrived(string Message)
        {
            if (!server_is_active) { return;  }
            if (MessageArrived == null) { return; }

            // We create a temporary delegate for the listener and then store the current invocation list that our event holds.
            MessageArrivedEvent listener = null;         
            Delegate[] dels = MessageArrived.GetInvocationList();
            // We do this because while we are iterating through the invocation list, a client could remove itsel from the invocation list geting us into a thread un-safe situation.

           // Loop through all the delegates and try to invoke them with the message.
            foreach (Delegate del in dels)
            {
                try
                {
                    listener = (MessageArrivedEvent)del;
                    listener.Invoke(Message);
                }
                catch (Exception)
                {
                    // If the invocation throws an exception, we remove it from the invocation list, effectively removing that client from receiving notifications
                    MessageArrived -= listener;
                }
            }
        }

        #endregion
    }
}
