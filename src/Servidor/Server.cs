using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Servidor
{
    class Server
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int BUFFER_SIZE = 1024;
        private static byte[] buffer = new byte[BUFFER_SIZE];
        private static Socket client;
        private static EndPoint remoteEnd = null;
        private static TextBox chatOut = null;
        public static void Start(int port, TextBox chat)
        {
            try
            {
                chatOut = chat;
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));//Accepta conexions desde qualsevol interficie amb el port especificat
                serverSocket.Listen(0);//Permet que els clients es conectin al servidor
                Console.WriteLine("Esperant una connexio...");
                client = serverSocket.Accept();//Espera per una connexió
                //Mostrem l'ip del client connectat
                Console.WriteLine("Client connectat: " + client.RemoteEndPoint.ToString());
                remoteEnd = client.RemoteEndPoint;
                client.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,ref remoteEnd, new AsyncCallback(OperatorCallBack), buffer);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Exit()
        {
            client.Shutdown(SocketShutdown.Both);
        }
        public static void SendString(string text)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch { }
        }
        public static string MostrarIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string ipAdr = "";
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAdr = ip.ToString();
                }
            }
            return ipAdr;
        }
        private static void OperatorCallBack(IAsyncResult ar)
        {
            try
            {
                int size = client.EndReceiveFrom(ar, ref remoteEnd);

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
                    chatOut.Invoke(new Action(() => chatOut.Text += Environment.NewLine+"Missatge del client: " + msg));
                    if (msg.Equals("prou"))
                    {
                        chatOut.Parent.Invoke(new Action(() => ((Form)chatOut.Parent).Close()));
                        Exit();
                    }
                    else
                    {
                        buffer = new byte[BUFFER_SIZE];
                        client.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEnd, new AsyncCallback(OperatorCallBack), buffer);
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

}
