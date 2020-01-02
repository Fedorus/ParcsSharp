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
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = obj as Channel;
            if (item!=null)
            {
                return PointID == item.PointID;
            }
            return false;
        }
        public static bool operator ==(Channel b1, Channel b2)
        {
            return b1?.PointID == b2?.PointID;
        }
        public static bool operator !=(Channel x, Channel y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return this.PointID.GetHashCode();
        }
    }
}