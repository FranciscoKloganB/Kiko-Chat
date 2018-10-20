using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using kiko_chat_contracts.data_objects;

namespace kiko_chat_contracts.data_objects
{
    [Serializable]
    public class GroupData : ISerializable
    {
        private object mutex = new object();

        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }     
        public DateTime LastKnownMessage
        {
            get { lock (mutex) { return LastKnownMessage; } }
            set { lock (mutex) { LastKnownMessage = value; } }
        }

        // DEPRECATED >> public List<MemberData> GroupMembers { get; set; }

        public GroupData(string ip, string port, string name, DateTime lastknownmessage)
        {
            Ip = ip;
            Port = port;
            Name = name;
            LastKnownMessage = lastknownmessage;
            // DEPRECATED >> List<MemberData> groupmembers = null GroupMembers = (groupmembers != null) ? groupmembers : new List<MemberData>();
        }

        /*
        * The special constructor is used to deserialize values.
        */
        public GroupData(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            Ip = (string)info.GetValue("Ip", typeof(string));
            Port = (string)info.GetValue("Port", typeof(string));
            Name = (string)info.GetValue("Name", typeof(string));
            LastKnownMessage = (DateTime)info.GetValue("LastKnownMessage", typeof(DateTime));
            // DEPRECATED >> GroupMembers = (List<MemberData>)info.GetValue("GroupMembers", typeof(List<MemberData>));
        }


        /*
        * This method is called on serialization.
        */
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("Ip", Ip, typeof(string));
            info.AddValue("Port", Port, typeof(string));
            info.AddValue("Name", Name, typeof(string));
            info.AddValue("LastKnownMessage", LastKnownMessage, typeof(DateTime));
            // DEPRECATED >> info.AddValue("GroupMembers", GroupMembers, typeof(List<MemberData>));
        }

        public override bool Equals(object obj)
        {
            var item = obj as GroupData;

            if (item == null)
            {
                return false;
            }
            else if (Name.Equals(item.Name) && Ip.Equals(item.Ip) && Port.Equals(item.Port))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
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
