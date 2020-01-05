using System;
using System.IO;
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
    }

    [MessageContract]
    public class ReceiveConfirmation
    {
        public bool Result;
        public string ErrorMessage;
    }

    [MessageContract]
    public class SendDataParams
    {
        [MessageHeader]
        public Channel From;
        [MessageHeader]
        public Channel To;
        [MessageHeader]
        public string Type;

        [MessageBodyMember]
        public Stream Data;
    }
}