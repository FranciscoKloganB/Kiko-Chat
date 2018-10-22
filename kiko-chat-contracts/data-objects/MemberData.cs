using System;
using System.Runtime.Serialization;

namespace kiko_chat_contracts.data_objects
{
    [Serializable]
    public class MemberData : ISerializable
    {
        public string Ip { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

        public MemberData(string ip, string nickname, string name, string email, string country)
        {
            Ip = ip;
            Nickname = nickname;
            Name = name;
            Email = email;
            Country = country;
        }

        /*
        * The special constructor is used to deserialize values.
        */
        public MemberData(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            Ip = (string)info.GetValue("Ip", typeof(string));
            Nickname = (string)info.GetValue("Nickname", typeof(string));
            Name = (string)info.GetValue("Name", typeof(string));
            Email = (string)info.GetValue("Email", typeof(string));
            Country = (string)info.GetValue("Country", typeof(string));
        }

        /*
        * This method is called on serialization.
        */
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Use the AddValue method to specify serialized values.
            info.AddValue("Ip", Ip, typeof(string));
            info.AddValue("Nickname", Nickname, typeof(string));
            info.AddValue("Name", Name, typeof(string));
            info.AddValue("Email", Email, typeof(string));
            info.AddValue("Country", Country, typeof(string));
        }

        public override string ToString()
        {
            return Ip + "; " + Nickname + ";" + Name + ";" + Email + "; " + Country;
        }

        public override bool Equals(object obj)
        {
            var item = obj as MemberData;

            if (item == null)
            {
                return false;
            }
            else if (Nickname.Equals(item.Name) && Ip.Equals(item.Ip))
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
            return Nickname.GetHashCode() ^ Ip.GetHashCode();
        }

    }
}
