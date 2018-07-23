using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Parcs.WCF;

namespace Parcs
{
    public class PointInfo
    {
        internal Point _currentPoint;
        public Point CurrentPoint
        {
            get => _currentPoint;
        }
        public ControlSpace CurrentControlSpace { get; internal set; }
        public Point ParentPoint { get; internal set; }
        internal Thread PointThread { get; set; }

        internal PointInfo(ControlSpace spaceData)
        {
            CurrentControlSpace = spaceData;
        }

        public Point GetPoint(Channel channel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Channel> GetChannels()
        {
            throw new NotImplementedException();
        }

        public Point GetPoint(string v)
        {
            throw new NotImplementedException();
        }
    }
}