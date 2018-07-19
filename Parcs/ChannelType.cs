using System.Runtime.Serialization;

namespace Parcs
{
    [DataContract]
    public enum ChannelType
    {
        [EnumMemberAttribute]
        Any = 0,

        [EnumMemberAttribute]
        TCP,
        [EnumMemberAttribute]
        NamedPipe
    }
}
