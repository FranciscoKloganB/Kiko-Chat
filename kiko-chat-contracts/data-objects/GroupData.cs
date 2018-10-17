using System;
using System.Runtime.Serialization;

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
        }


        /*
        * This method is called on serialization.
        */
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("props", Ip, typeof(string));
            info.AddValue("props", Port, typeof(string));
            info.AddValue("props", Name, typeof(string));
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
