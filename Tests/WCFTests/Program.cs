using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DaemonServ.DaemonServiceClient client = new DaemonServ.DaemonServiceClient(new NetTcpBinding(), new EndpointAddress("net.tcp://192.168.56.1:666"));
            Console.WriteLine(client.State);
            PointServ.PointServiceClient client2 = new PointServ.PointServiceClient(new NetTcpBinding(), new EndpointAddress("net.tcp://192.168.56.1:667"));

            var sw = new Stopwatch();
            sw.Start();
            var list = new List<Task>();
            for (int i = 0; i < 900; i++)
            {
                var task = client2.TestWorkAsync();
                Console.WriteLine($"{i}  { task } " );
                list.Add(task);
            }
            await Task.WhenAll(list);
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();
        }
    }
}
