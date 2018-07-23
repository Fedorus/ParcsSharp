using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    [ServiceContract]
    public interface IDaemonService 
    {
        [OperationContract]
        Task<Channel> CreatePointAsync(string Name, ChannelType channelType, ControlSpace controlSpaceData);

        [OperationContract]
        Task DestroyControlSpaceAsync(ControlSpace data);

        [OperationContract]
        Task SendFileAsync(FileTransferData data);
    }
}
