using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using kiko_chat_contracts.data_objects;

namespace kiko_chat_contracts.data_objects
{
    [Serializable]
    public class GroupData : ISerializable
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }     
        public DateTime LastKnownMessage { get; set; }  

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
        }

        /*
        * This method is called on serialization.
        */
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Ip", Ip, typeof(string));
            info.AddValue("Port", Port, typeof(string));
            info.AddValue("Name", Name, typeof(string));
            info.AddValue("LastKnownMessage", LastKnownMessage, typeof(DateTime));
        }

        public string HostAddress()
        {
            return Ip + ":" + Port;
        }

        public override string ToString()
        {
            return HostAddress() + "; " + Name + "; " + LastKnownMessage.ToShortTimeString();
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
            return Name.GetHashCode() ^ HostAddress().GetHashCode();
        }
    }
}
