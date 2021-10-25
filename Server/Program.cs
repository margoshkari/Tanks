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

                TimerCallback timerCallback = new TimerCallback(SendData);
                Timer timer = new Timer(timerCallback, 0, 0, 16);


                int count = 0;
                while (true)
                {
                    if (serverData.socketClientsList.Count > 0 && isSend == false )
                    {
                        tasks.Add(new Task(() => {
                            //while(true)
                            //{
                                int index = 0;
                                string json = string.Empty;
                                if (tanks.Last() != null)
                                {
                                    index = tanks.IndexOf(tanks.Last());
                                }
                                serverData.socketClient = serverData.socketClientsList[index];
                                json = Encoding.Unicode.GetString(serverData.GetMsg().ToArray());
                                tanks[index] = JsonSerializer.Deserialize<Tank>(json);
                           // }
                        }));
                        tasks.Last().Start();
                        isSend = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
        static void Connect()
        {
            while (true)
            {
                serverData.socketClient = serverData.socket.Accept();
                serverData.socketClientsList.Add(serverData.socketClient);
                tanks.Add(new Tank());
            }
        }
        static void SendData(object obj)
        {
            string json = string.Empty;
            if (serverData.socketClientsList.Count > 0 && isSend)
            {
                json = JsonSerializer.Serialize<List<Tank>>(tanks);
                foreach (var item in serverData.socketClientsList)
                {
                    item.Send(Encoding.Unicode.GetBytes(json));
                }
                isSend = false;
            }
        }
    }
}