using Parcs.WCF;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Parcs
{
    public class Daemon
    {
        private readonly IDaemonService _daemon;
        private readonly ControlSpace _linkedControlSpace;
        internal Daemon(string daemonIPAndPort, ControlSpace space)
        {
            _linkedControlSpace = space;
            Name = daemonIPAndPort;
            _daemon = ChannelFactory<IDaemonService>.CreateChannel(WCFSettings.GetTcpBinding(), new EndpointAddress(daemonIPAndPort));
        }

        public string Name { get;}


        internal async Task<Point> CreatePointAsync(string name, ChannelType channelType)
        {
            var channel = await _daemon.CreatePointAsync(name, channelType, _linkedControlSpace);
            var point = new Point(channel, _linkedControlSpace.CurrentPoint?.Channel, _linkedControlSpace);
            return point;
        }

        internal async Task<Point> CreatePointAsync()
        {
            return await CreatePointAsync("", ChannelType.Any);
        }
        public async Task SendFileAsync(FileTransferData data)
        {
            await _daemon.SendFileAsync(data);
        }
    }
}