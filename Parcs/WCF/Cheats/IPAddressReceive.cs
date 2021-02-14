using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF.Cheats
{
    class IPAddressReceive
    {
        public static string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return localIP;
            /*  var host = Dns.GetHostEntry(Dns.GetHostName());
              foreach (var ip in host.AddressList)
              {
                  if (ip.AddressFamily == AddressFamily.InterNetwork)
                  {
                      return ip.ToString();
                  }
              }
              throw new Exception("No network adapters with an IPv4 address in the system!");*/
        }
    }
}
