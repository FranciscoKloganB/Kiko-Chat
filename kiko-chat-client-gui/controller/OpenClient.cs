using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace kiko_chat_client_gui.controller
{
    class OpenClient
    {
        private TcpChannel tcpChannel;
        private ClientService clientService;

        public OpenClient()
        {
            throw new NotImplementedException();
            tcpChannel = new TcpChannel();
            ChannelServices.RegisterChannel(tcpChannel, true);
            clientService = (ClientService) Activator.GetObject(typeof(ClientService), "tcp://localhost:8086/MyRemoteObjectName");
        }
    }
}
