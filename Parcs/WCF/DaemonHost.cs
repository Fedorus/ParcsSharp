using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    public class DaemonHost
    {
        ServiceHost _host;
        DaemonService _service;
        public DaemonHost()
        {
            _service = new DaemonService();
        }
        public string Address => _service.Address;
        public void Start(int port)
        {
            if (_host != null)
            {
                throw new Exception("Daemon already running");
            }
            var baseAddress = new Uri("net.tcp://localhost:"+port);
            _host = new ServiceHost(_service, baseAddress);
            _host.AddServiceEndpoint(typeof(IDaemonService),
                new NetTcpBinding()
                {
                    MaxReceivedMessageSize = 1024 * 1024 * 64,
                    MaxBufferSize = 1024 * 1024 * 64,
                    SendTimeout = TimeSpan.FromHours(1),
                    ReceiveTimeout = TimeSpan.FromHours(1)
                },
                baseAddress);
            //starts
            _host.Open();
        }

        public void Stop()
        {
            _host.Close();
        }
    }
}
