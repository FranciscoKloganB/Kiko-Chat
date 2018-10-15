﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kiko_chat_contracts
{
    public class MemberData
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

        public MemberData(string ip, string port, string nickname, string name, string email, string country)
        {
            Ip = ip;
            Port = port;
            Nickname = nickname;
            Name = name;
            Email = email;
            Country = country;
        }
    }
}