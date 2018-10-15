using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace kiko_chat_client_gui.domain_objects
{
    class OpenClient
    {
        private TcpChannel tcpChannel;
        private ClientService clientService;

        public OpenClient()
        {
            tcpChannel = new TcpChannel();
            ChannelServices.RegisterChannel(tcpChannel, true);
            clientService = (ClientService) Activator.GetObject(typeof(ClientService), "tcp://localhost:8086/MyRemoteObjectName");
        }
    }
}
