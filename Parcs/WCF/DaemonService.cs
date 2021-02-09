using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;
using Parcs.WCF.Cheats;
using System.ServiceModel.Description;

namespace Parcs.WCF
{
    [ServiceBehavior(
        AutomaticSessionShutdown = false, 
        ConcurrencyMode = ConcurrencyMode.Single, 
        InstanceContextMode = InstanceContextMode.Single, 
        IncludeExceptionDetailInFaults = true,
        UseSynchronizationContext = false)
     ] //, ConcurrencyMode = ConcurrencyMode.Multiple
    public class DaemonService : IDaemonService
    {
        private string IP { get; set; }
        private int Port { get; set; }
        public string Address => $"net.tcp://{IP}:{Port}/";
        public DaemonService(int port = 666, bool addMetadata = false)
        {
            Port = port;
            PointService = new PointService();
            IP = IPAddressReceive.GetLocalIPAddress();

            var baseAddress = new Uri("net.tcp://localhost:" + (port + 1));
            var host = new ServiceHost(PointService);
            host.AddServiceEndpoint(typeof(IPointService), WCFSettings.GetTcpBinding(),
                baseAddress);
            var baseAddress2 = new Uri("net.pipe://localhost/" + (port + 1));
            host.AddServiceEndpoint(typeof(IPointService), WCFSettings.GetNamedPipeBinding(),
                baseAddress2);
            if (addMetadata)
            {
                var metadataAdress = new Uri($"http://localhost:{port + 3}/met");
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri($"http://localhost:{port + 3}/met");
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                host.AddServiceEndpoint(typeof(IPointService), new WSHttpBinding(), $"http://localhost:{port + 3}/met");
            }

            host.Open();

            Console.WriteLine($"Daemon was hosted on URI:");
            Console.WriteLine($"net.tcp://{IP}:{Port}");
            Console.WriteLine($"Point service hosted on addresses:");
            Console.WriteLine($"net.pipe://{IP}/{Port + 1}");
            Console.WriteLine($"net.tcp://{IP}:{Port + 1}");
        }
        //public List<ControlSpace> controlSpaces { get; set; } = new List<ControlSpace>();
        public ServiceHost host;
        public PointService PointService;
      
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
        public async Task<Channel> CreatePointAsync(string Name, ChannelType channelType, ControlSpace controlSpace)
        {
           // var cs = GetOrCreateControlSpace(controlSpace);
            Guid newPointGuid = Guid.NewGuid();

            var pointInfo = new PointInfo(controlSpace, Name);

            PointService.Points.Add(newPointGuid, pointInfo);
            
            Channel channel = new Channel(Name, channelType, IP, Port + 1, newPointGuid);
            //cs.ChannelsOnCurrentDaemon.Add(channel);
            return channel;
        }
        public async Task DestroyControlSpaceAsync(ControlSpace data)
        {
            List<Guid> points = new List<Guid>();
            foreach (var item in PointService.Points)
            {
                if (item.Value.CurrentControlSpace.ID== data.ID)
                {
                    if (item.Value.PointTask.Status == TaskStatus.Running)
                    {
                        item.Value.cancellationTokenSource.Dispose();
                    }
                    points.Add(item.Key);
                }
            }
            foreach (var item in points)
            {
                points.Remove(item);
                PointService.Points.Remove(item);
            }
        }
        public async Task SendFileAsync(FileTransferData data)
        {
            string futureFilePath = $"{(string.IsNullOrWhiteSpace(data.ControlSpace.Name) ? data.ControlSpace.ID.ToString() : data.ControlSpace.Name)}/{((data.Path != null) ? data.Path.Trim('/') + "/" : "")}";

            string fullFilename = futureFilePath + data.FileName;
            if (File.Exists(fullFilename))
            {
                if (data.Hash == FileChecksum.Calculate(fullFilename))
                {
#if DEBUG
                    Console.WriteLine($"File {data.Path}\\{data.FileName} already exist");
#endif
                    return;
                }
            }
            Directory.CreateDirectory(futureFilePath);
            File.WriteAllBytes(fullFilename, data.FileData);
#if DEBUG
            Console.WriteLine($"{data.Path}\\{data.FileName} transfered");
#endif
        }

        public async Task<bool> TestWork()
        {
            return true;
        }

        public async Task<MachineInfo> GetMachineInfo()
        {
            return new MachineInfo();
        }

        public async Task<List<ControlSpaceInfo>> GetControlSpaces()
        {
            return new List<ControlSpaceInfo>();
        }
    }
}