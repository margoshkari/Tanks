using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class ClientData
    {
        public byte[] data;
        public Socket socket;
        public IPEndPoint iPEndPoint;
        public ClientData()
        {
            data = new byte[256];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
        }
        public ClientData(byte[] data, Socket socket, IPEndPoint iPEndPoint)
        {
            this.data = data;
            this.socket = socket;
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
                    bytes = socket.Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (socket.Available > 0);
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
        //        bytes = socket.Receive(array, array.Length, 0);
        //        for (int i = 0; i < bytes; i++)
        //        {
        //            List_data.Add(array[i]);
        //        }
        //    } while (socket.Available > 0);

        //    return List_data;
        //}
    }
}