using System.Runtime.Serialization;

namespace Parcs
{
    [DataContract]
    public enum ChannelType
    {
        [EnumMember]
        Any = 0,
        [EnumMember]
        TCP,
        [EnumMember]
<<<<<<< HEAD
        NamedPipe // comment
=======
        NamedPipe
>>>>>>> refs/remotes/origin/master
    }
}
