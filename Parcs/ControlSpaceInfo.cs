using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Parcs
{
    [Serializable]
    [DataContract]
    public class ControlSpaceInfo
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<PointData> Data { get; set; }
        public ControlSpaceInfo(string name, IEnumerable<PointInfo> pointInfos)
        {
            Name = name;
            Data = new List<PointData>();
            foreach (var info in pointInfos)
            {
                Data.Add(new PointData()
                {
                    Channel = info.Channel,
                    Channels = info.Channels,
                    Parent = info.ParentPoint?.Channel,
                    IsRunning = info.PointTask != null && !(info.PointTask.IsCanceled || info.PointTask.IsCompleted || info.PointTask.IsFaulted)
                });
            }
        }
    }
    [Serializable]
    [DataContract]
    public class PointData
    {
        [DataMember]
        public Channel Channel { get; set; }
        [DataMember]
        public List<Channel> Channels { get; set; }
        [DataMember]
        public Channel Parent { get; set; }
        [DataMember]
        public bool IsRunning { get; set; }
    }
}