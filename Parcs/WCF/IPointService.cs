using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    [ServiceContract]
    public interface IPointService
    {
        [OperationContract]
        Task<bool> StartAsync(Channel from, Channel to, PointStartInfo info, ControlSpace space);
        [OperationContract]
        Task<bool> SendAsync(Channel from, Channel to, byte[] data, string type);
        [OperationContract]
        Task<bool> AddChannelAsync(Channel to, Channel channel);
    }
}