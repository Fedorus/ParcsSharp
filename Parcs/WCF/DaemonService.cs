using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;
using Parcs.WCF.Cheats;

namespace Parcs.WCF
{
    [ServiceBehavior(AutomaticSessionShutdown = false, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DaemonService : IDaemonService
    {
        private string IP { get; set; }
        private int Port { get; set; }
        public string Address => $"net.tcp://{IP}:{Port}/";

        public DaemonService(int port = 666)
        {
            Port = port;
            PointService = new PointService();
            IP = IPAddressReceive.GetLocalIPAddress();

            var baseAddress = new Uri("net.tcp://localhost:" + (port + 1));
            var host = new ServiceHost(PointService);
            host.AddServiceEndpoint(typeof(IPointService),
                new NetTcpBinding()
                {
                    MaxReceivedMessageSize = 1024 * 1024 * 64,
                    MaxBufferSize = 1024 * 1024 * 64,
                    SendTimeout = TimeSpan.FromHours(1),
                    ReceiveTimeout = TimeSpan.FromHours(1)
                },
                baseAddress);
            var baseAddress2 = new Uri("net.pipe://localhost/" + (port + 1));
            host.AddServiceEndpoint(typeof(IPointService),
                new NetNamedPipeBinding()
                {
                    MaxReceivedMessageSize = 1024 * 1024 * 64,
                    MaxBufferSize = 1024 * 1024 * 64,
                    SendTimeout = TimeSpan.FromHours(1),
                    ReceiveTimeout = TimeSpan.FromHours(1)
                },
                baseAddress2);
            host.Open();
            Console.WriteLine($"Daemon was hosted on URI:");
            Console.WriteLine($"net.tcp://{IP}:{Port}");
            Console.WriteLine($"Point service hosted on addresses:");
            Console.WriteLine($"net.pipe://{IP}/{Port + 1}");
            Console.WriteLine($"net.tcp://{IP}:{Port + 1}");
        }
        public List<ControlSpace> controlSpaces = new List<ControlSpace>();
        public ServiceHost host;
        public PointService PointService;
        private ControlSpace GetOrCreateControlSpace(ControlSpace cs)
        {
            var controlSpace = controlSpaces.FirstOrDefault(x => x.ID == cs.ID);
            if (controlSpace == null)
            {
               /* controlSpace = new ControlSpace(cs.Name, cs.DaemonAdresses);
                controlSpace.ID = cs.ID;
                controlSpace.PointDirectory = cs.PointDirectory;*/
                controlSpace = cs;
                controlSpaces.Add(cs);
            }
            else
            {
                controlSpace.DaemonAdresses = cs.DaemonAdresses;
            }
            return controlSpace;
        }
        private Uri MakeUri(ChannelType type)
        {
            switch (type)
            {
                case ChannelType.Any:
                    //return host.BaseAddresses[0].ToString;
                case ChannelType.TCP:
                    break;
                case ChannelType.NamedPipe:
                    break;
                default:
                    break;
            }
            return null;
        }
        public Channel CreatePoint(string Name, ChannelType channelType, ControlSpace controlSpace)
        {
            var cs = GetOrCreateControlSpace(controlSpace);
            Guid newPointGuid = Guid.NewGuid();
            PointService.Points.Add(newPointGuid, new PointInfo(cs));
            Channel channel = new Channel(Name, channelType, IP, Port + 1, newPointGuid);
            cs.ChannelsOnCurrentDaemon.Add(channel);
            return channel;
        }
        public bool DestroyControlSpace(ControlSpace data)
        {
            var cs = GetOrCreateControlSpace(data);
            List<Guid> points = new List<Guid>();
            foreach (var item in PointService.Points)
            {
                if (item.Value.CurrentControlSpace.ID== data.ID)
                {
                    if (item.Value.PointThread.ThreadState == System.Threading.ThreadState.Running)
                    {
                        item.Value.PointThread.Abort();
                    }
                    points.Add(item.Key);
                }
            }
            foreach (var item in points)
            {
                points.Remove(item);
                PointService.Points.Remove(item);
            }
            return true;
        }
        public bool SendFile(FileTransferData data)
        {
            var cs = GetOrCreateControlSpace(data.ControlSpace);
            string futureFilePath = $"{(string.IsNullOrWhiteSpace(data.ControlSpace.Name) ? data.ControlSpace.ID.ToString() : data.ControlSpace.Name)}/{((data.Path != null) ? data.Path.Trim('/') + "/" : "")}";
            cs.PointDirectory = futureFilePath;

            string FullFilename = futureFilePath + data.FileName;
            if (File.Exists(FullFilename))
            {
                if (data.Hash == FileChecksum.Calculate(FullFilename))
                {
                    Console.WriteLine($"File {data.Path}\\{data.FileName} already exist");
                    return true;
                }
            }
            Directory.CreateDirectory(futureFilePath);
            File.WriteAllBytes(FullFilename, data.FileData);
            Console.WriteLine($"{data.Path}\\{data.FileName} transfered");
            return true;
        }
    }
}
