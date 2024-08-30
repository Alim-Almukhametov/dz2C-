using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChatBotHWSemi2WithExitServer
{
    internal class Server
    {
        public static void ServerStartInThread() 
        {
                  var t = new Thread(() => ServerStartProcess());
                  t.Start();
        }
        /*public static async Task ServerStart() 
        {
            await Task.Run(() => ServerStartProcess());
             
        }*/
        public static void ServerStartProcess()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(12345);
            Console.WriteLine("Server started and waiting for connections...");

            while (true)
            {
                try
                {
                    var bytes = udpClient.Receive(ref localEP);
                    string json = Encoding.UTF8.GetString(bytes);

                    User user = User.GetFromJSON(json);

                    if (user != null)
                    {
                        if (user.TextMessage == "Exit")
                        {
                            Console.WriteLine("Server stopped");
                            udpClient.Close();
                            break;
                        }
                        else 
                        {
                            Console.WriteLine(user.ToString());

                            User suser = new User("Server", "Message received");
                            var jsonToSend = suser.GetJSON();
                            var bytesToSend = Encoding.UTF8.GetBytes(jsonToSend);
                            udpClient.Send(bytesToSend, bytesToSend.Length, localEP);
                        }
                       
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong , user is null");
                    }
                   

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }


            }

        }
    }
}
