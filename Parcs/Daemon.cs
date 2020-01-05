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
            daemon = ChannelFactory<IDaemonService>.CreateChannel(WCFSettings.GetTcpBinding(), new EndpointAddress(daemonIPAndPort));
        }

        public string Name { get;}


        internal async Task<Point> CreatePointAsync(string name, ChannelType channelType)
        {
            var channel = await daemon.CreatePointAsync(name, channelType, LinkedControlSpace);
            var point = new Point(channel, LinkedControlSpace.CurrentPoint?.Channel, LinkedControlSpace);
            return point;
        }

        internal async Task<Point> CreatePointAsync()
        {
            return await CreatePointAsync("", ChannelType.Any);
        }
        public async Task SendFileAsync(FileTransferData data)
        {
            await daemon.SendFileAsync(data);
        }
    }
}