namespace kiko_chat_contracts.data_objects
{
    public class GroupData
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }

        public GroupData(string ip, string port, string name)
        {
            Ip = ip;
            Port = port;
            Name = name;
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
