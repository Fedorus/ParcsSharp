using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Parcs;

namespace Parcs.WCF
{
    [ServiceContract]
    public interface IPointService
    {
        [OperationContract]
        Task<bool> StartAsync(Channel from, Channel to, PointStartInfo info, ControlSpace space);
        
        [OperationContract]
        Task<ReceiveConfirmation> SendAsync(SendDataParams sendData);
        
        [OperationContract]
        Task<bool> AddChannelAsync(Channel to, Channel channel);
        
        [OperationContract]
        Task<bool> TestWork();

        [OperationContract]
        Task<PointInfoDTO> GetInfoAsync(Channel to);
    }

    [DataContract]
    public class ReceiveConfirmation
    {
        [DataMember]
        public bool Result;
        [DataMember]
        public string ErrorMessage;
    }

    [DataContract]
    public class SendDataParams
    {
        [DataMember]
        public Channel From;
        [DataMember]
        public Channel To;
        [DataMember]
        public string Type;
        [DataMember]
        public string Data;
    }
}