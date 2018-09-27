using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Parcs;
using Parcs.WCF;

namespace DaemonHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Parcs.WCF.DaemonHost host = new Parcs.WCF.DaemonHost();
            host.Start(666);
            Console.ReadLine();
        }
    }
}
