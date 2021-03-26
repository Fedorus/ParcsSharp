using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Parcs.WCF.DTO;

namespace Parcs.WCF
{
    [ServiceContract]
    public interface IDaemonService 
    {
        [OperationContract]
        Task<Channel> CreatePointAsync(string Name, ChannelType channelType, ControlSpaceDTO controlSpaceData);

        [OperationContract]
        Task DestroyControlSpaceAsync(ControlSpaceDTO data);

        [OperationContract]
        Task SendFileAsync(FileTransferData data);
        
        [OperationContract]
        Task<bool> TestWork();

        [OperationContract]
        Task<MachineInfo> GetMachineInfo();

        [OperationContract]
        Task<List<ControlSpaceInfo>> GetControlSpaces();
    }
}
