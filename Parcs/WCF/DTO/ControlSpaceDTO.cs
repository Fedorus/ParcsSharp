using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Parcs.WCF.DTO
{
    [DataContract]
    public class ControlSpaceDTO
    {
        /// <summary>
        /// Human description of process context
        /// </summary>
        [DataMember]
        public string Name { get; internal set; }
        /// <summary>
        /// Control Space UID
        /// </summary>
        [DataMember]
        public Guid ID { get; internal set; }
        [DataMember]
        internal List<string> DaemonAddressees { get; set; } = new List<string>();
        
        
    }
}