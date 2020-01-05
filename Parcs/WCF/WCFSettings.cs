using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    public class WCFSettings
    {
        public static NetTcpBinding GetTcpBinding() =>
            new NetTcpBinding()
            {
                TransferMode = TransferMode.Streamed,
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                SendTimeout = TimeSpan.FromMinutes(1),
                ReceiveTimeout = TimeSpan.FromMinutes(1)
            };

        public static NetNamedPipeBinding GetNamedPipeBinding() => new NetNamedPipeBinding()
        {
            TransferMode = TransferMode.Streamed,
            MaxReceivedMessageSize = int.MaxValue,
            MaxBufferSize = int.MaxValue,
            SendTimeout = TimeSpan.FromHours(1),
            ReceiveTimeout = TimeSpan.FromHours(1)
        };
    }
}
