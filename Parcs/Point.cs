using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parcs.WCF;

namespace Parcs
{
    public class Point 
    {
        /// <summary>
        /// Channel to communicate with remote point
        /// </summary>
        public Channel Channel { get; }
        internal DataObjectsContainer<DataTransferObject> Data = null; 
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
        public Task AddChannelAsync(Channel channel)
        {
            return _PointServiceClient.AddChannelAsync(Channel, channel);
        }

        public Task CancelAsync()
        {
            throw new System.NotImplementedException();
        }
       
        public async Task<T> GetAsync<T>()
        {
            var tcs = new TaskCompletionSource<T>();
            void waitForresultEvent(object sender, DataReceivedEventArgs<DataTransferObject> e)
            {
                if (e.ReceivedItem == null || tcs.Task.IsCompleted == true)
                    return;
                if (e.ReceivedItem.From == Channel && e.ReceivedItem.To == _pointThatUsingThisPoint && e.ReceivedItem.Type == typeof(T).ToString())
                {
                    var returnValue = e.ReceivedItem;
                    e.ReceivedItem = null;
                    tcs.SetResult(JsonConvert.DeserializeObject<T>(returnValue.Data));
                    return;
                }
            }

            lock (_controlSpace.CurrentPoint.Data)
            {
                if (_controlSpace.CurrentPoint.Data != null)
                {
                    var result = _controlSpace.CurrentPoint.Data._items.Find(x =>
                        x.From == Channel &&
                        x.To == _pointThatUsingThisPoint &&
                        x.Type == typeof(T).ToString()
                        );
                    if (result != null)
                    {
                        return JsonConvert.DeserializeObject<T>(result.Data);
                    }
                }
                _controlSpace.CurrentPoint.Data.OnAdd += waitForresultEvent;
            }
            T awaitedResultValue = await tcs.Task.ConfigureAwait(true);
            _controlSpace.CurrentPoint.Data.OnAdd -= waitForresultEvent;
            return awaitedResultValue;
        }

        public async Task<Daemon> GetDaemonAsync()
        {
            throw new System.NotImplementedException();
        }
        public async Task RunAsync(PointStartInfo pointStartInfo)
        {
            await _PointServiceClient.StartAsync(_pointThatUsingThisPoint, Channel, pointStartInfo, _controlSpace);
        }
        public Task<bool> SendAsync<T>(T t)
        {
            return _PointServiceClient.SendAsync(_pointThatUsingThisPoint, 
                Channel, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(t)), t.GetType().ToString());
        }
        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}