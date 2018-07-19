using Parcs.WCF.Cheats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    [DataContract]
    public class FileTransferData
    {
        [DataMember]
        public byte[] FileData { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Hash { get; private set; }
        [DataMember]
        public ControlSpace ControlSpace { get; set; }

        public void ComputeHash()
        {
            Hash = FileChecksum.Calculate(FileData);
        }
    }
}
