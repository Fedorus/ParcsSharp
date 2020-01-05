using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    public class DaemonHost
    {
        ServiceHost _host;
        internal DaemonService _service;
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
                    ReceiveTimeout = TimeSpan.FromHours(1),
                },
                baseAddress);
            //_host.Description.Behaviors.Add(new System.ServiceModel.Discovery.ServiceDiscoveryBehavior())
            //starts
            var smb = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb ==null)
            {
                smb = new ServiceMetadataBehavior();
            }
            smb.HttpGetEnabled = true;
            smb.HttpGetUrl = new Uri($"http://localhost:{port+2}/met");
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            _host.Description.Behaviors.Add(smb);
            _host.AddServiceEndpoint(typeof(IDaemonService), new WSHttpBinding(), $"http://localhost:{port + 2}/met");

            _host.Open();
        }

        public void Stop()
        {
            _host.Close();
        }
    }
}
