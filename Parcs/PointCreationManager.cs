using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Parcs
{
    [DataContract]
    internal class PointCreationManager 
    {
        [DataMember]
        Random rand = new Random();
        public virtual Daemon ChooseDaemon(List<Daemon> daemons)
        {
            return daemons[rand.Next(daemons.Count)];
        }
    }
}