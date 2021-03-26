using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parcs.WCF;
using Parcs.WCF.Cheats;

namespace Parcs
{
    public class Point 
    {
        /// <summary>
        /// Channel to communicate with remote point
        /// </summary>
        public Channel Channel { get; }
        internal DataObjectsContainer<SendDataParams> Data = null; 
        /// <summary>
        /// Channel that used to choose optimal channel type with point
        /// </summary>
        internal Channel PointThatUsingThisPoint { get; }
        internal readonly ControlSpace _controlSpace = null;
        readonly IPointService _pointServiceClient;
        public IParcsSerializer Serializer { get; set; } = new JsonDotNetParcsSerializer(); 
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
            PointThatUsingThisPoint = currentPointChannel;
            Channel = channelToRemotePoint;
            _controlSpace = space;
          /*  if (channelToRemotePoint.IP == currentPointChannel.IP && channelToRemotePoint.Type != ChannelType.TCP)
            {
                _pointServiceClient = ChannelFactory<IPointService>.CreateChannel(WCFSettings.GetNamedPipeBinding(), new EndpointAddress($"net.pipe://{channelToRemotePoint.IP}/{channelToRemotePoint.Port}"));
            }
            else*/
            {
                _pointServiceClient = ChannelFactory<IPointService>.CreateChannel(WCFSettings.GetTcpBinding(), new EndpointAddress($"net.tcp://{channelToRemotePoint.IP}:{channelToRemotePoint.Port}"));
            } 
        }

        public async Task AddChannelAsync(Channel channel)
        {
            await _pointServiceClient.AddChannelAsync(Channel, channel);
        }

        public Task CancelAsync()
        {
            throw new System.NotImplementedException();
        }
       
        public async Task<T> GetAsync<T>()
        {
            var tcs = new TaskCompletionSource<T>();
            void WaitForResultEvent(object sender, DataReceivedEventArgs<SendDataParams> e)
            {
                if (e.ReceivedItem == null || tcs.Task.IsCompleted == true)
                    return;
                if (e.ReceivedItem.From == Channel && e.ReceivedItem.To == PointThatUsingThisPoint && e.ReceivedItem.Type == typeof(T).ToString())
                {
                    var returnValue = e.ReceivedItem;
                    e.ReceivedItem = null;
                    _controlSpace.CurrentPoint.Data.OnAdd -= WaitForResultEvent;
                    T data;
                    //using (var reader = new StreamReader(returnValue.Data)) //TODO deserialization interface
                    {
                        //data = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                        data = Serializer.Deserialize<T>(returnValue.Data);// JsonConvert.DeserializeObject<T>(returnValue.Data);
                    }
                    Task.Factory.StartNew(()=> tcs.TrySetResult(data), TaskCreationOptions.LongRunning).ConfigureAwait(false);
                }
            }

            lock (_controlSpace.CurrentPoint.Data)
            {
                var result = _controlSpace.CurrentPoint.Data._items.Find(x =>
                    x.From == Channel &&
                    x.To == PointThatUsingThisPoint &&
                    x.Type == typeof(T).ToString()
                    );
                if (result != null)
                {
                    _controlSpace.CurrentPoint.Data._items.Remove(result);
                    /* using (var reader = new StreamReader(result.Data)) //TODO serialization interface
                     {
                         return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                     }*/
                    return Serializer.Deserialize<T>(result.Data);
                }

                _controlSpace.CurrentPoint.Data.OnAdd += WaitForResultEvent;
            }
            T awaitedResultValue = await tcs.Task.ConfigureAwait(false);
            return awaitedResultValue;
        }

        public async Task<Daemon> GetDaemonAsync()
        {
            throw new System.NotImplementedException();
        }
        public async Task RunAsync(PointStartInfo pointStartInfo)
        {
             await _pointServiceClient.StartAsync(PointThatUsingThisPoint, Channel, pointStartInfo, _controlSpace.ToDto());
        }
        public async Task<bool> SendAsync<T>(T t)
        {
            return (await _pointServiceClient.SendAsync(new SendDataParams(){
                From =  PointThatUsingThisPoint,
                To = Channel,
                Data = Serializer.Serialize(t),
                Type = t.GetType().ToString() })
               .ConfigureAwait(false)).Result;
        }
        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task GetInfo()
        {
            await _pointServiceClient.GetInfoAsync(Channel);
        }
    }
}