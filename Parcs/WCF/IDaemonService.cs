using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    [ServiceContract]
    public interface IDaemonService
    {
        string Address { get; }
        [OperationContract]
        Channel CreatePoint(string Name, ChannelType channelType, ControlSpace controlSpace);
        [OperationContract]
        bool DestroyControlSpace(ControlSpace data);
        [OperationContract]
        bool SendFile(FileTransferData data);
    }
}
