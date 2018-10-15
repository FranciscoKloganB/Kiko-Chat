using System;
using System.Collections.Generic;
using kiko_chat_contracts.data_objects;

// BeginInvoke instead of Invoke TO AVOID DEADLOCKS OF SYNCHRONOUS REMOTE OBJECT INVOCATION.
namespace kiko_chat_contracts.web_services
{
    // TODO VERIFY THAT SERVERS ALSO INFORM OF NAME CHANGES FROM OTHER MEMBERS WHILE SOME MEMBER WAS DISCONNECTED FROM GROUP;
    public interface IServerServices
    {
        // Establish a connection between the member and the server for the referenced Chat Group. Should verify if the user as permissions.
        // User sends timestamp of last message he owns so that he can update is chat by turning the recieved byte array into a File using StreamReader.
        byte[] Connect(DateTime lastseenmessage, MemberData member, GroupData group);
        // Close connection between member group. Whenever a user recieves a new message he should store the timestamp, so the disconnect method does not to return timestamps.
        void Disconnect(MemberData member, GroupData group);
        // When a server recieves a message, it should be able to know from what user it comes from and what is the destination group of that message.
        // This message is than broadcasted to the users of that group by invoking RecieveBroadCast on each member of that group.
        void ProcessMessage(string message, DateTime messagetimestamp, MemberData member, GroupData group);
        // Servers must allow a Member to change his data, including but not limited to, Nickname and IP.
        // They must recieve the old member data for matching purposes and the new data to set.
        void UpdateMember(MemberData member);
        // This method allows a user to Request whose users belong to the group upon connection.
        List<MemberData> RetriveGroupMembers(MemberData member, GroupData group);
        // This method allows users to create a new group. MemberData is sent together with the group data in order to place the user in that group immediatly.
        DateTime CreateGroup(MemberData owner, GroupData group);
        // User subscribe to an existing group. Recieves a blank chat with the time stamp of the last message he recieved; Inform other users about new user.
        DateTime JoinGroup(MemberData subscribingmember, GroupData group);
        // User unsubscribes user from an existing group. Should notifiy other subscribers about this. 
        void LeaveGroup(MemberData unsubscribingmember, GroupData group);
    }


    public interface IClientServices
    {
        // When server sends a broadcast to the client, he recieves a message belonging to another subscriber on some group, sent at a given time.
        void RecieveNewMessage(string message, DateTime messagetimestamp, MemberData sender, GroupData group);
        // Endpoint to inform a member that some other user left a group therefore asking him to update is data.
        void AddMemberToGroup(MemberData newmember, GroupData group);
        // Endpoint to inform a member that some other user left a group therefore asking him to update is data.
        void RemoveMemberFromGroup(MemberData oldmember, GroupData group);
        // Kicks a member from a specific server with some IP.
        void KickedFromServer(string HostAddress);
        // Allows server to inform this member from other members who requested an update.
        void UpdateGroupMember(MemberData member, GroupData group);
    }
}
