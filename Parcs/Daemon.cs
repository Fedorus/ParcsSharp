using Parcs.WCF;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Parcs
{
    public class Daemon
    {
        private readonly IDaemonServiceAsync daemon;
        private readonly ControlSpace LinkedControlSpace;
        internal Daemon(string daemonIPAndPort, ControlSpace space)
        {
            LinkedControlSpace = space;
            Name = daemonIPAndPort;
            daemon = ChannelFactory<IDaemonServiceAsync>.CreateChannel(new NetTcpBinding() {
                MaxReceivedMessageSize = 1024 * 1024 * 64,
                MaxBufferSize = 1024 * 1024 * 64,
                SendTimeout = TimeSpan.FromHours(1),
                ReceiveTimeout = TimeSpan.FromHours(1)
            }, new EndpointAddress(daemonIPAndPort));
        }

        public string Name { get;}

        internal async Task<IPoint> CreatePointAsync(string name, ChannelType channelType)
        {
            var channel = await daemon.CreatePointAsync(name, channelType, LinkedControlSpace);
            return new Point(channel, LinkedControlSpace.CurrentPoint?.Channel, LinkedControlSpace);
        }

        internal IPoint CreatePoint(string name, ChannelType channelType)
        {
            var channel = daemon.CreatePoint(name, channelType, LinkedControlSpace);
            return new Point(channel, LinkedControlSpace.CurrentPoint?.Channel, LinkedControlSpace);
        }

        internal IPoint CreatePoint()
        {
            Channel channel = daemon.CreatePoint("", ChannelType.Any, LinkedControlSpace);
            return new Point(channel, LinkedControlSpace.CurrentPoint?.Channel, LinkedControlSpace);
        }

        public async Task<IPoint> CreatePointAsync()
        {
            var channel = await daemon.CreatePointAsync("", ChannelType.Any, LinkedControlSpace);
            return new Point(channel, LinkedControlSpace.CurrentPoint?.Channel, LinkedControlSpace);
        }

        internal void SendFile(FileTransferData data)
        {
            daemon.SendFile(data);
        }
        public Task<bool> SendFileAsync(FileTransferData data)
        {
           return daemon.SendFileAsync(data);
        }
    }
}