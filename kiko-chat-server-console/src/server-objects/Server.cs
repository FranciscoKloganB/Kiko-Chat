using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.web_services;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace kiko_chat_server_console.server_objects
{
    class Server : MarshalByRefObject, IServerObject
    {
        #region Fields

        private const string client_api_object = "clientObject";
        private const string server_api_object = "serverObject";
        private string well_known_port = "8080";
        private bool server_is_active = false;
        private TcpServerChannel tcpChannel;
        private ObjRef internalRef;
        private Dictionary<GroupData, List<MemberData>> hosted_groups;
        #endregion

        public event MessageArrivedEvent MessageArrived;

        #region Constructors

        public Server(string port)
        {
            Int32 port_as_int;

            if (!Int32.TryParse(port, out port_as_int))
            {
                port_as_int = Int32.Parse(well_known_port);
            }
            else
            {
                well_known_port = port;
            }

            hosted_groups = new Dictionary<GroupData, List<MemberData>>();

            TcpChannel tcpChannel = new TcpChannel(port_as_int);
            ChannelServices.RegisterChannel(tcpChannel, false);

            internalRef = RemotingServices.Marshal(this, server_api_object, typeof(Server));
        }

        #endregion

        #region Contract Implementation

        public byte[] Connect(MemberData member, GroupData group)
        {
            if (hosted_groups.ContainsKey(group))
            {
                hosted_groups[group].Add(member);
            } else
            {
                List<MemberData> memberList = new List<MemberData>();
                memberList.Add(member);
                hosted_groups.Add(group, memberList);
            }

            Console.WriteLine($"Client with the following info just joined the server :{Environment.NewLine + member.ToString() + Environment.NewLine}, On group {group.Name} ");
            IClientObject client_proxy = GetRemoteClientProxy(member);

            client_proxy.RecieveNewMessage(Group_Load_ChatHistory(), DateTime.Now);

            return null;
        }

        public DateTime CreateGroup(MemberData owner, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        public DateTime JoinGroup(MemberData subscribingmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void LeaveGroup(MemberData unsubscribingmember, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void PublishMessage(string message, DateTime messagetimestamp, MemberData member, GroupData group)
        {
            string newMessage;
            List<MemberData> memberList;
            IClientObject member_proxy;

            try
            {
                hosted_groups.TryGetValue(group, out memberList);
                newMessage = $"[{messagetimestamp}] {member.Nickname}: {message}";
                Persist_New_Message(newMessage, group);

                foreach (MemberData subscriber in memberList)
                {
                    member_proxy = GetRemoteClientProxy(subscriber);
                    member_proxy.RecieveNewMessage(newMessage, messagetimestamp);
                }
            }
            catch (ArgumentNullException)
            {
                throw new KikoServerException("You tryed to post a message to a non existing group.");
            }
        }

        public List<MemberData> RetriveGroupMembers(MemberData member, GroupData group)
        {
            throw new NotImplementedException();
        }

        public void UpdateMember(MemberData member)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Other class methods that interact with client proxies.

        private IClientObject GetRemoteClientProxy(MemberData member)
        {
            return (IClientObject)Activator.GetObject(typeof(IClientObject), string.Join("tcp://", member.HostAddress(), "/", client_api_object, member.Port));
        }

        #endregion

        #region Other class methods

        private void Persist_New_Message(string message, GroupData group)
        {
            throw new NotImplementedException();
        }

        private string Group_Load_ChatHistory()
        {
            throw new NotImplementedException();
        }

        public void StartServer()
        {
            tcpChannel.StartListening(null);
            Console.WriteLine("Press <Enter> to shutdown server...");
        }

        public void StopServer()
        {
            tcpChannel.StopListening(null);
            ChannelServices.UnregisterChannel(tcpChannel);
            RemotingServices.Disconnect(this);
        }

        #endregion
    }
}
