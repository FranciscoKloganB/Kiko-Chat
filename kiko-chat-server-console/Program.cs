using System;
using kiko_chat_server_console.server_objects;

namespace kiko_chat_server_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyInfo = Console.ReadKey();
            }

            server.StopServer();
        }
    }
}
