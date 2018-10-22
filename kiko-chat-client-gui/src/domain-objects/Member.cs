using System;
using System.Linq;
using System.Collections.Generic;
using kiko_chat_contracts.data_objects;

namespace kiko_chat_client_gui.domain_objects
{
    public class Member
    {
        private const string NONE = "Not Available";

        public string CurrentLocation { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        private List<GroupData> groups;

        public Member(string currentLocation, string nickname, string fullname = NONE, string email = NONE, string country = NONE, List<GroupData> grouplist = null)
        {
            CurrentLocation = currentLocation;
            Nickname = nickname;
            Name = fullname;
            Country = country;
            Email = email;
            groups = (grouplist == null ? new List<GroupData>() : grouplist);
        }

        public Member(string ip, string port, string nickname, string fullname = NONE, string email = NONE, string country = NONE, List<GroupData> grouplist = null)
        {
            CurrentLocation = ip + ":" + port;
            Nickname = nickname;
            Name = fullname;
            Country = country;
            Email = email;
            groups = (grouplist == null ? new List<GroupData>() : grouplist);
        }

        public MemberData Get_Member_Data()
        {
            return new MemberData(CurrentLocation, Nickname, Name, Email, Country);
        }

        public GroupData Find_Group_By_Name(string groupname)
        {
            return groups.Find(group => String.Equals(group.Name, groupname));
        }

        public void Leave_Group(string groupname)
        {
            groups.RemoveAll(group => String.Equals(group.Name, groupname));
        }

        public void Add_Group(GroupData newgroup)
        {
            if (groups.Any(group => group.Equals(newgroup)))
            {
                throw new InvalidOperationException("You are already part of the group you just tryed to join or create.");
            } else
            { 
                groups.Add(newgroup);
            }
        }

        public List<GroupData> Get_Groups()
        {
            return groups;
        }

        public override string ToString()
        {
            return CurrentLocation + "; " + Nickname + "; " + Name + "; " + Country + "; " + Email + ";";
        }

        public override bool Equals(object obj)
        {
            var item = obj as Member;

            if (item == null)
            {
                return false;
            }
            else if (Name.Equals(item.Name))
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
            return Nickname.GetHashCode();
        }
    }
}
