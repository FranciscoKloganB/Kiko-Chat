using System;
using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.web_services;

namespace kiko_chat_client_gui.domain_objects
{ 
    public class ClientService : MarshalByRefObject, IClientObject
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
