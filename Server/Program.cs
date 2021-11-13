using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TankDll;
using System.Threading;
using System.IO;

namespace Server
{
    class Program
    {
        static ServerData serverData = new ServerData();
        static List<Tank> tanks = new List<Tank>();
        static List<Task> tasks = new List<Task>();
        static int ID = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                serverData.socket.Bind(serverData.iPEndPoint);
                serverData.socket.Listen(10);
                SaveMap();

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
                if (serverData.socketClientsList.Count <= 4)
                {
                    tanks.Add(new Tank());
                    ID++;

                    tasks.Add(new Task(() => GetTank()));
                    tasks.Last().Start();

                    Console.WriteLine($"Client { ID} connected!");
                }
                else
                {
                    serverData.socketClient.Send(Encoding.Unicode.GetBytes("serverfull"));
                    serverData.socketClientsList.Remove(serverData.socketClientsList.Last());
                }
            }
        }
        static void GetTank()
        {
            int index = ID;
            int id = ID;
            string json = string.Empty;
            bool isConnected = true;
            bool isSaveRating = true;

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

                        if (tanks[index].HP <= 0 && isSaveRating)
                        {
                            isSaveRating = false;
                            SaveRating();
                        }
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
            Console.WriteLine($"Client { id} disconnected!");
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
        static void SaveMap()
        {
            if (!File.Exists(@$"C:\ProgramData\Tanks\map.txt"))
            {
                var file = File.Create(@$"C:\ProgramData\Tanks\map.txt");
                file.Close();
            }

            if (File.Exists(@$"C:\ProgramData\Tanks\map.txt"))
                File.WriteAllText(@$"C:\ProgramData\Tanks\map.txt", string.Empty);

            for (int i = 0; i < MapCreation.map.GetLength(0); i++)
            {
                for (int j = 0; j < MapCreation.map.GetLength(1); j++)
                {
                    File.AppendAllText(@$"C:\ProgramData\Tanks\map.txt", MapCreation.map[i, j].ToString());
                }
                File.AppendAllText(@$"C:\ProgramData\Tanks\map.txt", "\n");
            }
        }
        static void SaveRating()
        {
            string text = string.Empty;
            if (!File.Exists(@$"C:\ProgramData\Tanks\rating.txt"))
            {
                var createfile = File.Create(@$"C:\ProgramData\Tanks\rating.txt");
                createfile.Close();
            }

            text = Sort();
            RewriteFile(text);
        }
        static string Sort()
        {
            Dictionary<int, int> values = new Dictionary<int, int>();

            for (int i = 0; i < tanks.Count; i++)
                values.Add(tanks[i].ID, tanks[i].Score);

            values = values.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return $"ID: {values.Last().Key} Score: {values.Last().Value}\n";
        }
        static void RewriteFile(string text)
        {
            bool isAdd = false;
            List<string> lines = File.ReadLines(@$"C:\ProgramData\Tanks\rating.txt").ToList().Where(item => item != string.Empty).ToList();

            if (lines.Any(item => item.Contains(text.Split("Score:")[0])))
            {
                if (lines.Count == 1)
                    lines[lines.FindIndex(item => item.Contains(text.Split("Score:")[0]))] = text;
                else
                {
                    isAdd = true;
                    lines.Remove(lines.Find(item => item.Contains(text.Split("Score:")[0])));
                }

            }
            else
                isAdd = true;

            if (lines.Count >= 3)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (int.Parse(lines[i].Split("Score: ")[1]) < int.Parse(text.Split("Score: ")[1]))
                    {
                        lines.Remove(lines[i]);
                    }
                }
            }

            if (lines.Count <= 2 && isAdd)
                lines.Add(text);

            File.WriteAllLines(@$"C:\ProgramData\Tanks\rating.txt", lines);
        }
    }
}