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
        event MessageArrivedEvent MessageArrived;

        /* 
        * Establish a connection between the member and the server for the referenced Chat Group. Should verify if the user as permissions.
        * User sends timestamp of last message he owns so that he can update is chat by turning the recieved byte array into a File using StreamReader.
        */
        [OperationContract]
        byte[] Connect(MemberData member, GroupData group);

        /*
        * Close connection between member group. Whenever a user recieves a new message he should store the timestamp, so the disconnect method does not to return timestamps.
        */
        [OperationContract]
        void Disconnect(MemberData member, GroupData group);

        /*
        * When a server recieves a message, it should be able to know from what user it comes from and what is the destination group of that message.
        * This message is than broadcasted to the users of that group by invoking RecieveBroadCast on each member of that group.
        */
        [OperationContract]
        void PublishMessage(string message, DateTime messagetimestamp, MemberData member, GroupData group);

        /*
        * Servers must allow a Member to change his data, including but not limited to, Nickname and IP. They must recieve the old member data for matching purposes and the new data to set.
        */
        [OperationContract]
        void UpdateMember(MemberData member);

        /*
        * This method allows a user to Request whose users belong to the group upon connection.
        */
        [OperationContract]
        List<MemberData> RetriveGroupMembers(MemberData member, GroupData group);

        /*
        * This method allows users to create a new group. MemberData is sent together with the group data in order to place the user in that group immediatly.
        */
        [OperationContract]
        DateTime CreateGroup(MemberData owner, GroupData group);

        /*
        * User subscribe to an existing group. Recieves a blank chat with the time stamp of the last message he recieved; Inform other users about new user.
        */
        [OperationContract]
        DateTime JoinGroup(MemberData subscribingmember, GroupData group);

        /* 
        * User unsubscribes user from an existing group. Should notifiy other subscribers about this.
        */
        [OperationContract]
        void LeaveGroup(MemberData unsubscribingmember, GroupData group);
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
        void UpdateGroupMember(MemberData member);
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
