using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels.Tcp;
using kiko_chat_contracts.web_services;

namespace kiko_chat_client_gui.domain_objects
{
    class OnGoingClientCommunications
    {
        private IServerObject server_api;
        private TcpChannel client_tcp_channel;
        private ClientService client_api_implementation;
        private string objectUri;
        private string serverUri;

        public OnGoingClientCommunications()
        {
            // Create the client TCP channel
            TcpChannel clientChannel = new TcpChannel();
            // Register that TCP channel
            ChannelServices.RegisterChannel(clientChannel, false);
            // Register as client for remote object. Fetches a ServerService Remote Object at specified address and informs the server of our one Entry Point
            WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(typeof(IServerObject), "tcp://localhost:9090/RemoteObject.rem");
            RemotingConfiguration.RegisterWellKnownClientType(remoteType);

            IMessageSink messageSink = clientChannel.CreateMessageSink("tcp://localhost:9090/RemoteObject.rem", null, out objectUri);

            // Create an instance of the remote object.
            // RemoteObject service = new RemoteObject();

            // Invoke a method on the remote object.
            Console.WriteLine("The client is invoking the remote object.");
            //Console.WriteLine("The remote object has been called {0} times.", service.GetCount());

            //clientService = (ClientService) Activator.GetObject(typeof(ClientService), "tcp://localhost:8086/MyRemoteObjectName");
        }

    }
}
