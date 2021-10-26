using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TankDll;
using System.Threading;

namespace Server
{
    class Program
    {
        static ServerData serverData = new ServerData();
        static List<Tank> tanks = new List<Tank>();
        static bool isSend = true;
        static List<Task> tasks = new List<Task>();
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                serverData.socket.Bind(serverData.iPEndPoint);
                serverData.socket.Listen(10);

                Task.Factory.StartNew(() => Connect());

                // TimerCallback timerCallback = new TimerCallback(SendData);
                // Timer timer = new Timer(timerCallback, 0, 0, 16);

                Task.Factory.StartNew(() => SendData());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
        static int id = 0;
        static void Connect()
        {
            while (true)
            {
                serverData.socketClient = serverData.socket.Accept();
                serverData.socketClientsList.Add(serverData.socketClient);
                tanks.Add(new Tank());
                tanks.Last().ID = id;

                tasks.Add(new Task(() =>
                {
                    Task.Factory.StartNew(() => GetTank());

                }));
                tasks.Last().Start();
                id++;

                Console.WriteLine("Client connected!");
            }
        }
        static void GetTank()
        {
            int index = 0;
            if (tanks.Count > 0)
            {
                index = tanks.IndexOf(tanks.Last());
            }
            string json = string.Empty;

            while (true)
            {
                try
                {
                    Console.WriteLine(serverData.socketClientsList.Count);

                    serverData.socketClient = serverData.socketClientsList[index];

                    json = serverData.GetMsg(index);
                    tanks[index] = JsonSerializer.Deserialize<Tank>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        static void SendData()
        {

            string json = string.Empty;
            while (true)
            {
                try
                {
                    if (serverData.socketClientsList.Count > 0)
                    {
                        json = JsonSerializer.Serialize<List<Tank>>(tanks);
                        foreach (var item in serverData.socketClientsList)
                        {
                            item.Send(Encoding.Unicode.GetBytes(json));
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    }
}