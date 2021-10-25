using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static ServerData serverData = new ServerData();
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                serverData.socket.Bind(serverData.iPEndPoint);
                serverData.socket.Listen(10);

                Task.Factory.StartNew(() => Connect());
            }
            catch (Exception ex) { }

            Console.ReadLine();
        }
        static void Connect()
        {
            while (true)
            {
                serverData.socketClient = serverData.socket.Accept();
                serverData.socketClientsList.Add(serverData.socketClient);
            }
        }
    }
}