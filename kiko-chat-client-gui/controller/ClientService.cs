using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using kiko_chat_contracts;

namespace kiko_chat_client_gui.controller
{ 
    public class ClientService : MarshalByRefObject, IClientServices
    {
        public void AddMemberToGroup(MemberData newmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void KickedFromServer(string HostAddress)
        {
            throw new NotImplementedException();
        }

        public void RecieveNewMessage(string message, DateTime messagetimestamp, MemberData sender, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void RemoveMemberFromGroup(MemberData oldmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void UpdateGroupMember(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }
    }
}
