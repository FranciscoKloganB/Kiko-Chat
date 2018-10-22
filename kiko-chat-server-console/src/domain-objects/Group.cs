using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using kiko_chat_contracts.data_objects;

namespace kiko_chat_server_console.domain {
    public class Group
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        private List<MemberData> members { get; set; }

        public string HostAddress
        {
            get
            {
                return Ip + ":" + Port;
            }
        }

        public Group(string ip, string port, string groupname, List<MemberData> memberslist = null) {
            Ip = ip;
            Port = port;
            Name = groupname;
            members = (memberslist == null ? new List<MemberData>() : memberslist);
        }

        public void Add_Member(MemberData newmember)
        {
            if (members.Any(member => member.Equals(newmember)))
            {
                throw new InvalidOperationException("You are already part of the group you just tryed to join or create.");
            }
            else
            {
                members.Add(newmember);
            }
        }
        
        public string Members_ToString()
        {
            string stringifiedMembers = "";

            foreach (MemberData member in members)
            {
                stringifiedMembers = stringifiedMembers + member.Nickname + Environment.NewLine);
            }

            return stringifiedMembers;
        }

        public override string ToString()
        {
            return Ip + ":" + Port + "; " + Name + ";" + Environment.NewLine + members.ToString();
        }

        public override bool Equals(object obj)
        {
            var item = obj as Group;

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
    }
}
