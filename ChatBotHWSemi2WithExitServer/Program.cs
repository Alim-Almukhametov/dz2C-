using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotHWSemi2WithExitServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
           await Server.GetMessage();
        }
    }
}
