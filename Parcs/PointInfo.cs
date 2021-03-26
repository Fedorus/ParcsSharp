using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Parcs.WCF;
using System.Threading.Tasks;
using Parcs.Logging;

namespace Parcs
{
    public class PointInfo
    {
        internal Point _currentPoint;
        public Point Point
        {
            get => _currentPoint;
        }
        public Channel Channel { get; set; }
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public ControlSpace ControlSpace { get; internal set; }
        public Point ParentPoint { get; internal set; }
        internal Thread PointThread { get; set; }
        internal Task PointTask { get; set; }
        internal CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public CancellationToken CancellationToken { get => cancellationTokenSource.Token; }
        public IParcsLogger Logger { get; set; }
        
        internal PointInfo(ControlSpace spaceData, Channel channel)
        {
            ControlSpace = spaceData;
            Channel = channel;
            Logger = new TextFileLogger(channel.Name);
        }

        public Point GetPoint(Channel channel)
        {
            if (channel==null)
            { throw new NullReferenceException("channel == null"); }
            return new Point(channel, Point.Channel, ControlSpace);
        }

        public Point GetPoint(string name)
        {
            return new Point(Channels.FirstOrDefault(x => x.Name == name), Point.Channel, ControlSpace);
        }
    }
}