using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Parcs.WCF;
using System.Threading.Tasks;

namespace Parcs
{
    public class PointInfo
    {
        internal Point _currentPoint;
        public Point CurrentPoint
        {
            get => _currentPoint;
        }
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public ControlSpace CurrentControlSpace { get; internal set; }
        public Point ParentPoint { get; internal set; }
        internal Thread PointThread { get; set; }
        internal Task PointTask { get; set; }
        internal CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public CancellationToken CancellationToken { get => cancellationTokenSource.Token; }
        internal PointInfo(ControlSpace spaceData)
        {
            CurrentControlSpace = spaceData;
        }

        public Point GetPoint(Channel channel)
        {
            if (channel==null)
            { throw new NullReferenceException("channel == null"); }
            return new Point(channel, CurrentPoint.Channel, CurrentControlSpace);
        }

        public Point GetPoint(string name)
        {
            return new Point(Channels.FirstOrDefault(x => x.Name == name), CurrentPoint.Channel, CurrentControlSpace);
        }
    }
}