using Parcs.WCF;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Parcs
{
    public class Daemon
    {
        private readonly IDaemonService daemon;
        private readonly ControlSpace LinkedControlSpace;
        internal Daemon(string daemonIPAndPort, ControlSpace space)
        {
            LinkedControlSpace = space;
            Name = daemonIPAndPort;
            daemon = ChannelFactory<IDaemonService>.CreateChannel(new NetTcpBinding() {
                MaxReceivedMessageSize = 1024 * 1024 * 64,
                MaxBufferSize = 1024 * 1024 * 64,
                SendTimeout = TimeSpan.FromHours(1),
                ReceiveTimeout = TimeSpan.FromHours(1)
            }, new EndpointAddress(daemonIPAndPort));
        }

        public string Name { get;}


        internal async Task<Point> CreatePointAsync(string name, ChannelType channelType)
        {
            var channel = await daemon.CreatePointAsync(name, channelType, LinkedControlSpace);
            var point = new Point(channel, LinkedControlSpace.CurrentPoint?.Channel, LinkedControlSpace);
            return point;
        }

        internal Task<Point> CreatePointAsync()
        {
            return CreatePointAsync("", ChannelType.Any);
        }
        public Task SendFileAsync(FileTransferData data)
        {
           return daemon.SendFileAsync(data);
        }
    }
}