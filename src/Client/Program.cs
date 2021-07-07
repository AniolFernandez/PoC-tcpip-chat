using System;


namespace Client
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("IP: " + Client.MostrarIp());
            Console.Write("Introdueix l'IP del servidor >>> ");
            string ip = Console.ReadLine();
            Console.Write("Introdueix el port del servidor >>> ");
            int port = int.Parse(Console.ReadLine());
            Chat chat = new Chat();
            Client.ConnectToServer(ip, port, chat.textOutput);
            chat.ShowDialog();
            Client.Exit();
        }
    }
}
