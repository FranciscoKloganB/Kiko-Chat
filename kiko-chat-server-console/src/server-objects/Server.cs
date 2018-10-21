using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using kiko_chat_contracts.data_objects;
using kiko_chat_contracts.web_services;

namespace kiko_chat_server_console.server_objects
{
    class Server : MarshalByRefObject, IServerObject
    {
        /*
        * OnlineMembers maps the users who are online for each group.
        * Hosted Groups is a dictionary containg all Group Names hosted in the server and all members belonging to it, regardless of their online status.
        * LastMessageRegistry is a dictionary containing all Group Names hosted in the server and the last messages recieved in that group, for chat updating purposes when the subscriber come online on that group.
        */
        #region Fields

        private const string client_api_object = "clientObject";
        private const string server_api_object = "serverObject";
        private string wellKnownPort = "8080";
        private string persistenceDirectory;
        private ObjRef internalRef;
        private BinaryServerFormatterSinkProvider serverProvider;
        private TcpServerChannel tcpChannel;
        private Dictionary<string, List<IClientObject>> HostedGroupsOnlineMembers;
        private Dictionary<string, List<MemberData>> HostedGroups;
        private Dictionary<string, DateTime> LastMessageRegistry;

        #endregion

        #region Constructors

        public Server(string port, bool loadprevioussettings)
        {
            ParsePort(port);

            if (loadprevioussettings) {
                HostedGroupsOnlineMembers = new Dictionary<string, List<IClientObject>>();
                LoadGroupData();
            } else
            {
                HostedGroupsOnlineMembers = new Dictionary<string, List<IClientObject>>();
                HostedGroups = new Dictionary<string, List<MemberData>>();
                LastMessageRegistry = new Dictionary<string, DateTime>();
            }

            Hashtable providerProperties = new Hashtable() {
                { "port", wellKnownPort }
            };

            serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

            tcpChannel = new TcpServerChannel(providerProperties, serverProvider);
            ChannelServices.RegisterChannel(tcpChannel, false);

            internalRef = RemotingServices.Marshal(this, server_api_object, typeof(Server));
        }

        #endregion

        #region Contract Implementation

        /*
        *   Creates a new group at the server and adds the creator as a subscriber.
        *   The subscriber is not marked as online after he creates the group. For him to send
        * or listen to messages in the group he created he needs to connect.
        */
        public void CreateGroup(MemberData creator, string groupname)
        {
            if (HostedGroups.ContainsKey(groupname))
            {
                throw new KikoServerException("The group you tryed to create already exists on the specified server.");
            }

            HostedGroups.Add(groupname, new List<MemberData>() { creator });
            HostedGroupsOnlineMembers.Add(groupname, new List<IClientObject>());
            LastMessageRegistry.Add(groupname, DateTime.Now);
            CreatePersistenceChatFile(groupname);
        }

        /*
        *   Adds the requestor as a subscriber to the the specified group.
        *   The subscriber is not marked as online after he joins the group. For him to send
        * or listen to messages in the group he joined he needs to connect.
        */
        public void JoinGroup(MemberData subscriber, string groupname)
        {
            if (!HostedGroups.ContainsKey(groupname))
            {
                throw new KikoServerException("The group you tryed to join does not exist on the specified server.");
            }

            HostedGroups[groupname].Add(subscriber);
        }

        /*
        *   Unsubscribes the requestor from the specified group.
        *   He won't be able to send or listen to messages in this group until he rejoins. If group becomes empty, the group is destroyed.
        */
        public void LeaveGroup(MemberData desubscriber, string groupname)
        {
            if (!HostedGroups.ContainsKey(groupname))
            {
                throw new KikoServerException("The group you tryed to unsubscribe no longer exists at the specified server");
            }

            HostedGroups[groupname].Remove(desubscriber);

            if (HostedGroups[groupname].Count == 0)
            {
                HostedGroups.Remove(groupname);
                LastMessageRegistry.Remove(groupname);
            }
        }

        /*
        *   Marks a group subscriber as an active listener.
        *   If the last message that subscriber recieved is older than the last message registred at the server,
        * all messages happening from the user message onward are sent to him, so he can update is chat.
        */
        public void Connect(MemberData member, string groupname, DateTime lastknownmessagge)
        {
            IClientObject client_proxy;

            HandleConnectionRequest(member, groupname);

            try
            {
                client_proxy = GetRemoteClientProxy(member);
                HostedGroupsOnlineMembers[groupname].Add(client_proxy);
                UpdateClientChat(client_proxy, groupname, lastknownmessagge);
            }
            catch (Exception exc) {
                if (exc is ArgumentNullException || exc is RemotingException || exc is MemberAccessException)
                {
                    Console.WriteLine($"User <{member.Nickname}>, request rejected. Reason: {exc.Message}");
                } else
                {
                    Console.WriteLine("Unkwon exception occured during client proxy obtention...");
                }
            }
        }

        /*
        *   Removes a group subscriber from the active listeners collection.
        */
        public void Disconnect(MemberData member, string groupname)
        {
            if (!HostedGroupsOnlineMembers.ContainsKey(groupname))
            {
                throw new KikoServerException("The group you tryed to disconnect from is not currently available at the specified server");
            }
            HostedGroupsOnlineMembers[groupname].Remove(GetRemoteClientProxy(member));
        }

        /*
        *   Obtains a list of online members for the specified group hosted at this server and broadcasts the message
        * for all of those subscribers.
        *   The message is persisted at the server's chat history before being broadcasted.
        */ 
        public void PublishMessage(string message, string groupname, DateTime messagetimestamp, MemberData member)
        {
            string newMessage;
            List<IClientObject> activeListeners;

            try
            {
                HostedGroupsOnlineMembers.TryGetValue(groupname, out activeListeners);
                newMessage = $"[{messagetimestamp}] {member.Nickname}: {message}";

                PersistNewMessage(newMessage, groupname);

                foreach (IClientObject listener in activeListeners)
                {
                    listener.RecieveNewMessage(newMessage, messagetimestamp);
                }
            }
            catch (ArgumentNullException)
            {
                throw new KikoServerException("You tryed to post a message to a non existing group.");
            }
        }

        /*
        *   Returns a list of all members within a group.
        *   Groups always have at least one member, so if the key exists, it will never be empty.
        *   If the requesting member does not belong to the group or if the group does not exist, an error is returned
        * to the requestor.
        */
        public List<MemberData> RetriveGroupMembers(string groupname, MemberData requestingmember)
        {
            List<MemberData> groupMembers;
            if (HostedGroups.TryGetValue(groupname, out groupMembers))
            {
                foreach (MemberData member in groupMembers)
                {
                    if (member.Equals(requestingmember))
                    {
                        return groupMembers;
                    }
                }
            }
            throw new KikoServerException("Unauthorized request: Only members within the group can request for other group members information.");
        }

        /*
        * Recieves a list of groups, ideally all groups the user belongs to on this server and requests the update.
        */
        public void UpdateMember(MemberData oldmemberdata, MemberData newmemberdata, List<string> subscribedgroups)
        {
            foreach (string groupname in subscribedgroups)
            {
                int memberIndex = HostedGroups[groupname].FindIndex(member => member.Equals(oldmemberdata));
                HostedGroups[groupname][memberIndex] = newmemberdata;

                foreach(IClientObject activelisteners in HostedGroupsOnlineMembers[groupname])
                {
                    activelisteners.UpdateGroupMember(oldmemberdata, newmemberdata);
                }
            }
        }

        #endregion

        #region Other class methods that interact with client proxies.

        private IClientObject GetRemoteClientProxy(MemberData member)
        {
            return (IClientObject)Activator.GetObject(typeof(IClientObject), string.Join("tcp://", member.HostAddress(), "/", client_api_object, member.Port));
        }

        #endregion

        #region Other class methods

        private void ParsePort(string port)
        {
            Int32 port_as_int;

            if (!Int32.TryParse(port, out port_as_int))
            {
                port_as_int = Int32.Parse(wellKnownPort);
            }
            else
            {
                wellKnownPort = port;
            }
            CreatePersistenceDirectory(port);
        }

        private void HandleConnectionRequest(MemberData member, string groupname)
        {
            Console.WriteLine($"User <{member.ToString()}> came online and is trying to join <{groupname}>{Environment.NewLine}");
            if (HostedGroups.ContainsKey(groupname) && HostedGroupsOnlineMembers.ContainsKey(groupname))
            {
                Console.WriteLine($"User <{member.Nickname}>, request accepted");
            }
            else
            {
                Console.WriteLine($"User <{member.Nickname}>, request rejected. Group is not on Hosted Groups, Online Group Members or both.");
                throw new KikoServerException("The group your tryed to connect is not available. Try again later.");
            }
        }

        private void UpdateClientChat(IClientObject clientproxy, string groupname, DateTime lastknownmessage)
        {
            DateTime lastMessage = LastMessageRegistry[groupname];
            if (DateTime.Compare(lastknownmessage, lastMessage) < 0)
            {
                clientproxy.RecieveNewMessage(PersistenceFileAsString(groupname), lastMessage);
            }
        }

        private void CreatePersistenceDirectory(string port)
        {
            persistenceDirectory = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "run", "server-group-chats", wellKnownPort });
            Directory.CreateDirectory(persistenceDirectory);
        }

        public void StartServer()
        {
            tcpChannel.StartListening(null);
            Console.WriteLine($"Server proxy can now be obtained at <{tcpChannel.GetUrlsForUri(server_api_object)[0]}>{Environment.NewLine}");
            Console.WriteLine("Press <Enter> to shutdown server...");
        }

        public void StopServer()
        {
            tcpChannel.StopListening(null);
            ChannelServices.UnregisterChannel(tcpChannel);
            RemotingServices.Disconnect(this);

            SaveGroupData();
        }

        #endregion

        #region Chat Logging Methods

        private void CreatePersistenceChatFile(string groupname)
        {
            string filePath = GetChatPath(groupname);
            Console.WriteLine($"Attempting to create file for this group at: {filePath}...{Environment.NewLine}");
            try
            {
                using (new FileStream(filePath, FileMode.CreateNew)) { }
            }
            catch (IOException)
            {
                Console.WriteLine($"File for group named {groupname} already exists. Skipping this step...{Environment.NewLine}");
            }
        }

        private void PersistNewMessage(string message, string groupname)
        {
            string filePath = GetChatPath(groupname);
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(message);
                }
            } catch (Exception exc)
            {
                Console.WriteLine($"Error while trying to persistent new message of group: <{groupname}>...{Environment.NewLine}{exc.Message} ");
            }
        }

        private string PersistenceFileAsString(string groupname)
        {
            string filePath = GetChatPath(groupname);
            try
            {
                using(FileStream fileStream = new FileStream(filePath, FileMode.Open))
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    string fileAsString = "";
                    while (streamReader.Peek() >= 0)
                    {
                        string.Join(fileAsString, Environment.NewLine, streamReader.ReadLine());
                    }
                }

            } catch (Exception exc)
            {
                Console.WriteLine($"Error while trying to read persistent chat log of group: <{groupname}>...{Environment.NewLine}{exc.Message} ");
            }
            return "";
        }

        private string GetChatPath(string groupname)
        {
            return string.Join(persistenceDirectory, "/", groupname, ".txt");
        }

        #endregion

        #region Group Logging Methods

        private void SaveGroupData()
        {
            string directoryPath = persistenceDirectory + "/server-settings";

            SaveHostedGroups(directoryPath);
            SaveLatestTimeStamps(directoryPath);
        }

        private void SaveHostedGroups(string directorypath)
        {
            try
            {
                using (StreamWriter streamWritter = new StreamWriter(directorypath + "/hosted-groups.json"))
                using (JsonWriter jsonWritter = new JsonTextWriter(streamWritter))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jsonWritter, HostedGroups);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error while storing hosted groups as JSON file...{Environment.NewLine}{exc.Message} ");
            }
        }

        private void SaveLatestTimeStamps(string directorypath)
        {
            try
            {
                using (StreamWriter streamWritter = new StreamWriter(directorypath + "message-registry.json"))
                using (JsonWriter jsonWritter = new JsonTextWriter(streamWritter))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jsonWritter, LastMessageRegistry);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error while storing message timestamps as JSON file...{Environment.NewLine}{exc.Message} ");
            }
        }

        private void LoadGroupData()
        {
            string directoryPath = persistenceDirectory + "/server-settings/";
            try
            {
                HostedGroups = LoadHostedGroups(directoryPath);
                LastMessageRegistry = LoadLatestTimeStamps(directoryPath);
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Error loading server settings...{Environment.NewLine}{exc.Message}");
            }
        }

        private Dictionary<string, List<MemberData>> LoadHostedGroups(string directorypath)
        {
            string filePath = directorypath + "/hosted-groups.json";

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("There is no HostedGroups to load previous settings from.");
            }

            using (StreamReader streamReader = new StreamReader(@filePath))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                Dictionary<string, List<MemberData>> hostedGroups;
                JsonSerializer serializer = new JsonSerializer();
                hostedGroups = serializer.Deserialize<Dictionary<string, List<MemberData>>> (jsonReader);
                return hostedGroups;
            }
        }

        private Dictionary<string, DateTime> LoadLatestTimeStamps(string directorypath)
        {

            string filePath = directorypath + "/message-registry.json";
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("There is no LatestTimeStamps to load previous settings from.");
            }

            using (StreamReader streamReader = new StreamReader(@filePath))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                Dictionary<string, DateTime> lastMessageRegistry;
                JsonSerializer serializer = new JsonSerializer();
                lastMessageRegistry = serializer.Deserialize<Dictionary<string, DateTime>>(jsonReader);
                return lastMessageRegistry;
            }
        }

        #endregion
    }
}
