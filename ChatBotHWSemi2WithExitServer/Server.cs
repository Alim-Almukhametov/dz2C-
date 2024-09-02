using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics.Eventing.Reader;

namespace ChatBotHWSemi2WithExitServer
{
    internal class Server
    {
        private static bool exitRequested = false;
        /* private static int port = 8080;
         private static int maxClients = 10;
         private static List<Client> clients = new List<Client>();*/
        public static async Task GetMessage()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(8080);

            Console.WriteLine("Server started");

            Task exitTask = Task.Run(() =>
            {

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    exitRequested = true;
            });


            while (!exitRequested)
            {

                await Task.Run(async () =>
                {
                    var data = udpClient.Receive(ref ep);
                    string message = System.Text.Encoding.UTF8.GetString(data);

                    User user = User.FromJson(message);


                    if (user.IsCancelled)
                    {
                        try
                        {
                            user.CancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        catch (OperationCanceledException ex)
                        {
                            Console.WriteLine(ex.Message);
                            udpClient.Close();
                            exitRequested = true;

                        }


                    }
                    else
                    {
                        Console.WriteLine(user.ToString());

                        User response = new User("Admin", " Server accepted your message");
                        var jsonToSend = response.ToJson();
                        await udpClient.SendAsync(System.Text.Encoding.UTF8.GetBytes(jsonToSend), jsonToSend.Length, ep);
                    }

                });
            }

            exitTask.Wait();

            /*public static async Task ServerStart() 
            {
                await Task.Run(() => ServerStartProcess());

            }*/
            /*
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

            }*/
        }
    }
}
