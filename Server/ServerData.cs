using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ServerData
    {
        public List<Socket> socketClientsList;
        public byte[] data;
        public Socket socket;
        public Socket socketClient;
        public IPEndPoint iPEndPoint;
        public ServerData()
        {
            data = new byte[1024];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            socketClientsList = new List<Socket>();
        }
        public ServerData(byte[] data, Socket socket, Socket socketClient, IPEndPoint iPEndPoint)
        {
            this.data = data;
            this.socket = socket;
            this.socketClient = socketClient;
            this.iPEndPoint = iPEndPoint;
        }
        public string GetMsg()
        {
            int bytes = 0;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                do
                {
                    bytes = socketClient.Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (socketClient.Available > 0);
            }
            catch (Exception ex) { }
            return stringBuilder.ToString();

        }

        public string GetMsg(int index)
        {
            int bytes = 0;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                do
                {
                    bytes = socketClientsList[index].Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (socketClientsList[index].Available > 0);
            }
            catch (Exception ex) { }
            return stringBuilder.ToString();

        }

        //public List<byte> GetMsg()
        //{
        //    List<byte> List_data = new List<byte>();
        //    int bytes = 0;
        //    byte[] array = new byte[255];
        //    do
        //    {
        //        bytes = socketClient.Receive(array, array.Length, 0);
        //        for (int i = 0; i < bytes; i++)
        //        {
        //            List_data.Add(array[i]);
        //        }
        //    } while (socketClient.Available > 0);

        //    return List_data;
        //}
    }
}