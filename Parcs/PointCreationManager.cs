using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Parcs
{
    [DataContract]
    internal class PointCreationManager 
    {
        Random rand = new Random();
        public virtual Daemon ChooseDaemon(List<Daemon> daemons)
        {
            if (daemons.Count == 1)
            {
                return daemons[0];
            }
            return daemons[rand.Next(daemons.Count)];
        }
    }
}