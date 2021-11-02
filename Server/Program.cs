﻿using System;
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
        static int ID = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                serverData.socket.Bind(serverData.iPEndPoint);
                serverData.socket.Listen(10);

                Task.Factory.StartNew(() => Connect());

                Task.Factory.StartNew(() => SendData());
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
                ID++;

                tasks.Add(new Task(() => GetTank()));
                tasks.Last().Start();

                Console.WriteLine($"Client { tanks.Last().ID} connected!");
            }
        }
        static void GetTank()
        {
            int index = ID;
            int id = ID;
            string json = string.Empty;
            bool isConnected = true;

            while (isConnected)
            {
                try
                {
                    if (index >= serverData.socketClientsList.Count)
                        index--;
                    if (serverData.socketClientsList[index].Connected)
                    {
                        json = serverData.GetMsg(index);
                        tanks[index] = JsonSerializer.Deserialize<Tank>(json);

                        tanks[index].ID = id;

                    }
                    else
                    {
                        isConnected = false;
                        tanks.RemoveAt(index);
                        serverData.socketClientsList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetTank(): " + ex.Message);
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
                    json = JsonSerializer.Serialize<List<Tank>>(tanks);
                    foreach (var item in serverData.socketClientsList)
                    {
                        if (item.Connected)
                        {
                            item.Send(Encoding.Unicode.GetBytes(json));
                        }

                    }
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SendData(): " + ex.Message);
                }
            }
        }
    }
}