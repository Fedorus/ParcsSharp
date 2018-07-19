using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Parcs.WCF
{
    public class Point : IPoint
    {
        /// <summary>
        /// Channel to communicate with remote point
        /// </summary>
        public Channel Channel { get; }
        /// <summary>
        /// Channel that used to choose optimal channel type with point
        /// </summary>
        internal Channel _pointThatUsingThisPoint { get; }
        internal ControlSpace _controlSpace = null;
        readonly IPointService _PointServiceClient;
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="channelToRemotePoint">Channel to remote point</param>
        /// <param name="currentPointChannel">Channel of point from which request will be performed</param>
        internal Point(Channel channelToRemotePoint, Channel currentPointChannel, ControlSpace space)
        {
            if (currentPointChannel == null)
            {
                currentPointChannel = channelToRemotePoint;
            }
            _pointThatUsingThisPoint = currentPointChannel;
            Channel = channelToRemotePoint;
            _controlSpace = space;
            if (channelToRemotePoint.IP == currentPointChannel.IP && channelToRemotePoint.Type != ChannelType.TCP)
            {
                _PointServiceClient = ChannelFactory<IPointService>.CreateChannel(new NetNamedPipeBinding(), new EndpointAddress($"net.pipe://{channelToRemotePoint.IP}/{channelToRemotePoint.Port}"));
            }
            else
            {
                _PointServiceClient = ChannelFactory<IPointService>.CreateChannel(new NetTcpBinding(), new EndpointAddress($"net.tcp://{channelToRemotePoint.IP}:{channelToRemotePoint.Port}"));
            }
        }
        public void AddChannel(Channel channel)
        {
            throw new System.NotImplementedException();
        }
        public Task AddChannelAsync(Channel channel)
        {
            throw new System.NotImplementedException();
        }
        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
        public Task CancelAsync()
        {
            throw new System.NotImplementedException();
        }
        public T Get<T>()
        {
            throw new System.NotImplementedException();
        }
        public Task<T> GetAsync<T>()
        {
            throw new System.NotImplementedException();
        }
        public Daemon GetDaemon()
        {
            throw new System.NotImplementedException();
        }
        public async Task<Daemon> GetDaemonAsync()
        {
            throw new System.NotImplementedException();
        }
        public void Run(PointStartInfo pointStartInfo)
        {
            _PointServiceClient.StartAsync(_pointThatUsingThisPoint, Channel, pointStartInfo, _controlSpace).GetAwaiter().GetResult();
        }
        public async Task RunAsync(PointStartInfo pointStartInfo)
        {
            await _PointServiceClient.StartAsync(_pointThatUsingThisPoint, Channel, pointStartInfo, _controlSpace);
        }
        public bool Send<T>(T t)
        {
            return _PointServiceClient.SendAsync(Channel, _pointThatUsingThisPoint, Encoding.Default.GetBytes(JsonConvert.SerializeObject(t))).GetAwaiter().GetResult();
        }
        public async Task<bool> SendAsync<T>(T t)
        {
            return await _PointServiceClient.SendAsync(Channel, _pointThatUsingThisPoint, Encoding.Default.GetBytes(JsonConvert.SerializeObject(t)));
        }
        public void Stop()
        {
            throw new System.NotImplementedException();
        }
        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}