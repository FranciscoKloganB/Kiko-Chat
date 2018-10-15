using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kiko_chat_contracts
{
    public class GroupData
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }

        public GroupData(string ip, string port, string name)
        {
            Ip = ip;
            Port = port;
            Name = name;
        }

        public string HostAddress()
        {
            return Ip + ":" + Port;
        }

        public override string ToString()
        {
            return Ip + ":" + Port + "; " + Name + ";";
        }
    }
}
