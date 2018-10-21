using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using kiko_chat_contracts.data_objects;

// BeginInvoke instead of Invoke TO AVOID DEADLOCKS OF SYNCHRONOUS REMOTE OBJECT INVOCATION.
namespace kiko_chat_contracts.web_services
{ 
    // Declares the existance of a delegate MessageArrivedEvent that identifies the function we will use as when some <event> containing a message occurs.
    public delegate void MessageArrivedEvent(string Message);

    /*
    * Interface that Concrete Server Objects need to implement.
    * Our Concrete Client Objects will recognize this interface, but won't know anything about the actual server implementation.
    * This allows for the client call into or get notified of events very loosely on the servers providing the chat functionality.
    */
    [ServiceContract]
    public interface IServerObject
    {
        [OperationContract]
        void CreateGroup(MemberData creator, string groupname);

        [OperationContract]
        void JoinGroup(MemberData subscriber, string groupname);

        [OperationContract]
        void LeaveGroup(MemberData desubscriber, string groupname);

        [OperationContract]
        void Connect(MemberData member, string groupname, DateTime lastknownmessagge);

        [OperationContract]
        void Disconnect(MemberData member, string groupname);

        [OperationContract]
        void PublishMessage(string message, string groupname, DateTime messagetimestamp, MemberData member);

        [OperationContract]
        void UpdateMember(MemberData oldmemberdata, MemberData newmemberdata, List<string> subscribedgroups);

        [OperationContract]
        List<MemberData> RetriveGroupMembers(string groupname, MemberData requestingmember);
    }

    [ServiceContract]
    public interface IClientObject
    {
        /*
        * When server sends a broadcast to the client, he recieves a message belonging to another subscriber on some group, sent at a given time.
        * Each client connection should hold a reference to a chat window. Servers send messages in the following format:
        * [message_time_stamp] sender_nickname: actual message goes here
        * Servers also send the messagetimestamp, so that the Client Connection Manager can store the time at which he recieved the last message.
        * Client windows should add their own New Lines if they so desire.
        */
        [OperationContract]
        void RecieveNewMessage(string message, DateTime messagetimestamp);

        /*
        *Endpoint to inform a member that some other user left a group therefore asking him to update is data.
        */
        [OperationContract]
        void AddMemberToGroup(MemberData newmember);
       
        /*
        * Endpoint to inform a member that some other user left a group therefore asking him to update is data.
        */
        [OperationContract]
        void RemoveMemberFromGroup(MemberData oldmember);

        /*
        * Kicks a member from a specific server with some IP.
        */
        [OperationContract]
        void KickedFromServer();

        /*
        * Allows server to inform this member from other members who requested an update.
        */
        [OperationContract]
        void UpdateGroupMember(MemberData oldmemberdata, MemberData newmemberdata);
    }

    [Serializable]
    public class KikoServerException : ApplicationException
    {
        public string RemoteMessagge { get; set; }

        public KikoServerException(string message) { RemoteMessagge = message; }

        public KikoServerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            RemoteMessagge = (string)info.GetValue("RemoteMessagge", typeof(string));
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RemoteMessagge", RemoteMessagge);
        }
    }
}
