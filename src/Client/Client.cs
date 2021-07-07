using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    class Client
    {
        private const int BUFFER_SIZE = 1024;
        private static byte[] buffer = new byte[BUFFER_SIZE];
        private static readonly Socket ClientSocket = new Socket
           (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static EndPoint remoteEnd = null;
        private static TextBox chatOut = null;
        public static void ConnectToServer(string ip, int port, TextBox chat)
        {
            int intents = 0;
            chatOut = chat;
            while (!ClientSocket.Connected)
            {
                try
                {
                    intents++;
                    Console.WriteLine("Intents de connexió: " + intents);
                    ClientSocket.Connect(IPAddress.Parse(ip), port);

                }
                catch (SocketException)
                {
                }
            }
            Console.WriteLine("Connectat");
            remoteEnd = ClientSocket.RemoteEndPoint;
            ClientSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEnd, new AsyncCallback(OperatorCallBack), buffer);
        }

        public static string MostrarIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string ipAdr = "";
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ipAdr= ip.ToString();
            }
            return ipAdr;
        }
        private static void OperatorCallBack(IAsyncResult ar)
        {
            try
            {
                int size = ClientSocket.EndReceiveFrom(ar, ref remoteEnd);

                if (size > 0)//Si tenim bytes actuem
                {
                    byte[] aux = new byte[BUFFER_SIZE];
                    aux = (byte[])ar.AsyncState;
                    int i = aux.Length - 1;
                    while (aux[i] == 0)
                        --i;
                    byte[] auxtrim = new byte[i + 1];
                    Array.Copy(aux, auxtrim, i + 1);
                    string msg = Encoding.ASCII.GetString(auxtrim);
                    chatOut.Invoke(new Action(() => chatOut.Text += Environment.NewLine + "Missatge del servidor: " + msg));
                    if (msg.Equals("prou"))
                    {
                        chatOut.Parent.Invoke(new Action(() => ((Form)chatOut.Parent).Close()));
                        Exit();
                    }
                    else
                    {
                        buffer = new byte[BUFFER_SIZE];
                        ClientSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEnd, new AsyncCallback(OperatorCallBack), buffer);
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void SendString(string text)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch { }
        }

        public static void Exit()
        {
            try
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
            }
            catch { }
        }
    }
}
