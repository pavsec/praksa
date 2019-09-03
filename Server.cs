using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace server
{
    class Server
    {
        private const string Value = ".bye";

        static void Main(string[] args)
        {
            Console.WriteLine("SERVER APPLICATION");
            ExecuteServer();
            Console.ReadKey();
        }

        public static bool SendMessage(Socket s)
        {
            string msgSend = null;
            Console.Write("Me: ");
            msgSend = Console.ReadLine();
            if (msgSend.Equals(Value))
            {
                Console.WriteLine("\nEnd of connection, press any key to exit");
                byte[] end = Encoding.ASCII.GetBytes(Value);
                s.Send(end);
                return false;
            }
            byte[] message = Encoding.ASCII.GetBytes(msgSend);
            s.Send(message);
            return true;
        }

        public static bool ReceiveMessage(Socket s)
        {
            byte[] bytes = new Byte[1024];
            string msgRcv = null;

            int numByte = s.Receive(bytes);
            msgRcv = Encoding.ASCII.GetString(bytes, 0, numByte);
            if (msgRcv.Equals(Value))
            {
                Console.WriteLine("\nClient has ended the connection, press any key to exit");
                return false;
            }

            Console.WriteLine("Client: {0} ", msgRcv);
            return true;
        }

        public static void ExecuteServer()
        {
            IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ip, 8888);

            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(0);

                Socket client = listener.Accept();
                Console.WriteLine("Connected to client");
                Console.WriteLine("Send .bye for end of connection\n");
 
                do
                {
                    if (!SendMessage(client)) break;
                    if (!ReceiveMessage(client)) break;  
                } while (true);

                client.Shutdown(SocketShutdown.Both);
                client.Close();  
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
