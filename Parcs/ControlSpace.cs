using Parcs.WCF;
using Parcs.WCF.Cheats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Parcs.WCF.DTO;

namespace Parcs
{
    [DataContract]
    public class ControlSpace : IDisposable
    {
        /// <summary>
        /// Human description of process context
        /// </summary>
        [DataMember]
        public string Name { get; internal set; }
        
        /// <summary>
        /// Control Space UID
        /// </summary>
        [DataMember]
        public Guid ID { get; internal set; }
        public string PointDirectory { get => $"{(string.IsNullOrWhiteSpace(Name) ? ID.ToString() : Name)}/"; } 
        
        [DataMember]
        internal List<string> DaemonAdresses { get; set; } = new List<string>();
        public List<Daemon> Daemons { get; internal set; } = new List<Daemon>();
        public static DaemonHost hostedDaemon;
        
        PointCreationManager Creator { get; set; } = new PointCreationManager();
        /// <summary>
        /// Designed only for Main point.
        /// </summary>
        public Point CurrentPoint { get; internal set; }
        #region Constructors
        public ControlSpace(string name, List<string> list = null, int port = 666) 
        {
            this.Name = name;
            ID = Guid.NewGuid();
            if (hostedDaemon == null)
            {
                hostedDaemon = new DaemonHost();
                hostedDaemon.Start(port);
            }
            AddDaemons(hostedDaemon.Address);
            CurrentPoint = Daemons[0].CreatePointAsync("Main", ChannelType.Any).GetAwaiter().GetResult();
            CurrentPoint.RunAsync(null).GetAwaiter().GetResult();
            CurrentPoint = hostedDaemon._service.PointService.Points[CurrentPoint.Channel.PointID].Point;
            if(list !=null)
                AddDaemons(list.ToArray());
        }
        public ControlSpace(string name, string daemonUri, int port = 666) 
            : this(name, new List<string>(){daemonUri}, port)
        {
        }

        internal ControlSpace(ControlSpaceDTO dto)
        {
            Name = dto.Name;
            DaemonAdresses = dto.DaemonAddressees;
            ID = dto.ID;
            Daemons.AddRange(dto.DaemonAddressees.Select(x=>new Daemon(x, this)));
        }

        #endregion
        
        internal void AddDaemons(params string[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Daemons.Add(new Daemon(items[i], this));
            }
            DaemonAdresses.AddRange(items);
        }
        public async Task AddFileAsync(string file, string directory = "")
        {
            FileInfo info = new FileInfo(file);
            FileTransferData data = new FileTransferData
            {
                FileData = File.ReadAllBytes(file),
                FileName = info.Name,
                ControlSpace = this
            };
            if (!string.IsNullOrWhiteSpace(directory))
            {
                data.Path = directory;
            }
            data.ComputeHash();
            if (info.Exists)
            {
                var fileSends = new Task[Daemons.Count];
                for (int i = 0; i < Daemons.Count; i++)
                {
                    fileSends[i] = Daemons[i].SendFileAsync(data);
                }
                await Task.WhenAll(fileSends);
                //Task.WaitAll(fileSends);
            }
            return;
        }

        public async Task AddDirectoryAsync(string directory, bool recursive = false)
        {
            if (Directory.Exists(directory))
            {
                if (recursive)
                {
                    await AddDirectoryRecursiveAsync(directory, directory);
                }
                else
                    foreach (var item in Directory.GetFiles(directory))
                    {
                        await AddFileAsync(item);
                    }
            }
        }

        private async Task AddDirectoryRecursiveAsync(string directory, string startingDirecory)
        {
            var directories = Directory.GetDirectories(directory);
            var recursiveTasks = new Task[directories.Length];
            for (int i = 0; i < directories.Length; i++)
            {
                recursiveTasks[i] = AddDirectoryRecursiveAsync(directories[i], startingDirecory);
            }
            await Task.WhenAll(recursiveTasks);
            var files = Directory.GetFiles(directory);
            Parallel.For(0, files.Length, async (i) => { await AddFileAsync(files[i], directory.Remove(0, startingDirecory.Length)); });
        }

        public async Task<Point> CreatePointAsync()
        {
            Daemon daemonForPoint = Creator.ChooseDaemon(Daemons);
            return await daemonForPoint.CreatePointAsync();
        }
        public async Task<Point> CreatePointAsync(string name)
        {
            return await CreatePointAsync(name, PointType.Any, ChannelType.Any);
        }
        public async Task<Point> CreatePointAsync(string Name, PointType pointType, ChannelType channelType)
        {
            Daemon daemonForPoint = Creator.ChooseDaemon(Daemons);
            return await daemonForPoint.CreatePointAsync(Name, channelType);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ControlSpace() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}