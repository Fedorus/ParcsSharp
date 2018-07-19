using System;
using System.Runtime.Serialization;

namespace Parcs
{
    [DataContract]
    public class Channel
    {
        [DataMember] public string Name { get; internal set; }
        [DataMember] public ChannelType Type { get; internal set; }
        [DataMember] public string IP { get; internal set; }
        [DataMember] public Guid PointID { get; internal set; }
        [DataMember] public int Port { get; internal set; }
        public Channel()
        { }
        public Channel(string pointName, ChannelType type, string ip, int port, Guid pointID)
        {
            this.Name = pointName;
            this.Type = type;
            this.IP = ip;
            this.PointID = pointID;
            this.Port = port;
        }
    }
}