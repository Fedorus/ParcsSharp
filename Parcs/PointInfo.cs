using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Parcs.WCF;

namespace Parcs
{
    public class PointInfo
    {
        public IPoint CurrentPoint { get; internal set; }
        public ControlSpace CurrentControlSpace { get; internal set; }
        public IPoint ParentPoint { get; internal set; }
        internal Thread PointThread { get; set; }

        internal PointInfo(ControlSpace spaceData)
        {
            CurrentControlSpace = spaceData;
        }

        public IPoint GetPoint(Channel channel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Channel> GetChannels()
        {
            throw new NotImplementedException();
        }

        public IPoint GetPoint(string v)
        {
            throw new NotImplementedException();
        }
    }
}