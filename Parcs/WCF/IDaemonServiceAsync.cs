using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    [ServiceContract(Name = nameof(IDaemonService))]
    public interface IDaemonServiceAsync : IDaemonService
    {
        
        [OperationContract(Name = nameof(IDaemonService.CreatePoint))]
        Task<Channel> CreatePointAsync(string Name, ChannelType channelType, ControlSpace controlSpaceData);

        [OperationContract(Name = nameof(IDaemonService.DestroyControlSpace))]
        Task<bool> DestroyControlSpaceAsync(ControlSpace data);

        [OperationContract(Name = nameof(IDaemonService.SendFile))]
        Task<bool> SendFileAsync(FileTransferData data);
        
    }
}
