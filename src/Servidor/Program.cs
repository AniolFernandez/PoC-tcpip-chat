using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Servidor
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("IP: " + Server.MostrarIp()+ "\nIntrodueix el port a escoltar >>> ");
            int port = int.Parse(Console.ReadLine());
            Chat chat = new Chat();
            Server.Start(port, chat.textOutput);
            chat.ShowDialog();
            Server.Exit();
        }
    }
}
