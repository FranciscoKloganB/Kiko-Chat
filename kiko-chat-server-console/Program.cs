using System;
using kiko_chat_server_console.server_objects;

namespace kiko_chat_server_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Provide a well known port for your server");
            Server server = new Server(Console.ReadLine());

            server.StartServer();

            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            server.StopServer();
        }
    }
}
