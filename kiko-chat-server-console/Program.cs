using System;
using kiko_chat_server_console.server_objects;

namespace kiko_chat_server_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Provide a well known port for your server... {Environment.NewLine}");
            string port = Console.ReadLine();

            Console.WriteLine($"{Environment.NewLine}Do you wish to load previous settings? Press <AnyKey> for Yes, Press <0> for No.{Environment.NewLine}");
            Console.WriteLine($"Notice: Loading previous settings is only advisable if you know this server app has already ran on this IP and on the provided port.{Environment.NewLine}");
            bool loadPreviousSettings = (Console.ReadKey().Key == ConsoleKey.D0) ? false : true; 

            Server server = new Server(port, loadPreviousSettings);
            server.StartServer();

            Console.WriteLine($"{Environment.NewLine}Press <Enter> to shutdown server...");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            server.StopServer();

            System.Threading.Thread.Sleep(5000);
        }
    }
}
