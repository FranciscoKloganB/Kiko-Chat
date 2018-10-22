using System;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.RegularExpressions;


namespace kiko_chat_contracts.security_objects
{
    public static class Security
    {
        public const int MAX_CHARS = 100;
        public const int MAX_NICK_LENGTH = 16;
        public const int MAX_COUNTRY_CHARS = 60;

        public static string GetLocalPublicAddress()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    return addr.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string ValidateIP(string ip, string placeholder = null)
        {
            IPAddress clientIpAddr;
            if (ip == "localhost")
            {
                return ip;
            }
            else if (IPAddress.TryParse(ip, out clientIpAddr))
            {
                return clientIpAddr.ToString();
            }
            else
            {
                throw new ArgumentException("Unexpected IP address found. Make sure not to tamper with the given IP.");
            }
        }

        public static string ValidatePort(string port, string placeholder = null)
        {
            Int32 int32;
            if (Int32.TryParse(port, out int32) && 1 < int32 && int32 < 65534)
            {
                return int32.ToString();
            } else
            {
                return new Random().Next(1025, 65534).ToString();
            }
        }

        public static string ValidateNickname(string nickname, string placeholder = null)
        {
            if (!nickname.Equals(placeholder))
            {
                Regex nicknameRegex = new Regex(@"^[A-Za-z]+([A-Za-z\d_-]){2,}");
                if (nicknameRegex.IsMatch(nickname) && nickname.Length < MAX_NICK_LENGTH)
                {
                    return nickname;
                }
            }

            Guid guid = Guid.NewGuid();
            string guidString = "AnonymousUser_" + Convert.ToBase64String(guid.ToByteArray());
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("+", "");
            guidString = guidString.Replace("\\", "");

            return guidString;
        }

        public static string ValidateRegularName(string fullname, string placeholder = null)
        {
            if (!fullname.Equals(placeholder))
            {
                Regex nameRegex = new Regex(@"[A-Za-z]{2,}( [A-Za-z]{2,})*");
                if (nameRegex.IsMatch(fullname) && fullname.Length < MAX_CHARS)
                {
                    return fullname;
                }
            }
            return "";
        }

        public static string ValidateEmail(string email, string placeholder = null)
        {
            if (email.Length < MAX_CHARS)
            {
                try
                {
                    var addr = new MailAddress(email);
                    return email;
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        public static string ValidateCountry(string country, string placeholder = null)
        {
            if (!country.Equals(placeholder) && !country.Trim().Equals(""))
            {
                Regex countryRegex = new Regex(@"[A-Za-z]{2,}( [A-Za-z]{2,})*");
                if (countryRegex.IsMatch(country) && country.Length < MAX_COUNTRY_CHARS)
                {
                    return country;
                }
            }
            return "";
        }
    }
}
