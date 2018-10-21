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
    * EventProxy inherits from MarshalByRefObject because the EventProxy is serialized from the server to the client and deserialized upon reaching the client side.
    * This inheritance "teaches" the remoting framework needs to know how to marshal the object. MarshalByRefObject means that the object is marshaled across boundaries by reference.
    * The reason we have this proxy class is because the server side needs to know about the implementation of the event consumer on the client side.
    * If we didn't use a proxy class, the server would have to reference the client implementation so it knows how and where to call the function.
    */
    public class EventProxy : MarshalByRefObject
    {
        public event MessageArrivedEvent MessageArrived;

        // Lease will never time out and the object associated with it will have an infinite lifetime. Could return null instead but doing it right follows OECM principle.
        public override object InitializeLifetimeService() {
            ILease lease = (ILease)base.InitializeLifetimeService();
            lease.InitialLeaseTime = TimeSpan.Zero;
            return lease;
        }

        public void LocallyHandleMessageArrived(string Message)
        {
            if (Message != null)
            {
                MessageArrived(Message);
            }
        }
    }

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
