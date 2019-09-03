using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace client
{
    class Client
    {
        private const string Value = ".bye";

        static void Main(string[] args)
        {
            Console.WriteLine("CLIENT APPLICATION");
            ExecuteClient();
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
            byte[] messageSent = Encoding.ASCII.GetBytes(msgSend);
            int byteSent = s.Send(messageSent);
            return true;
        }

        public static bool ReceiveMessage(Socket s)
        {
            string msgRcv = null;
            byte[] messageReceived = new byte[1024];
            int byteRecv = s.Receive(messageReceived);
            msgRcv = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
            if (msgRcv.Equals(Value))
            {
                Console.WriteLine("\nServer has ended the connection, press any key to exit");
                return false;
            }
            Console.WriteLine("Server: {0}", msgRcv);
            return true;
        }

        static void ExecuteClient()
        {
            try
            {
                IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ip, 8888);

                Socket sender = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(localEndPoint);

                    // We print EndPoint information that we are connected 
                    Console.WriteLine("Connected to server");
                    Console.WriteLine("Send .bye for end of connection\n");                    

                    do
                    {
                        if (!ReceiveMessage(sender)) break;
                        if (!SendMessage(sender)) break;
                    } while (true);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                catch (SocketException se)
                {
                    Console.WriteLine("SocketException WRONG SOCKET : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
