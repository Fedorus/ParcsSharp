using Parcs.WCF;
using Parcs.WCF.Cheats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

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
        [DataMember]
        public Guid ID { get; internal set; }
        [DataMember]
        internal List<Channel> ChannelsOnCurrentDaemon { get; set; } = new List<Channel>();
        [DataMember]
        public string PointDirectory { get; internal set; } = Directory.GetCurrentDirectory();
        [DataMember]
        internal List<string> DaemonAdresses { get; set; } = new List<string>();

        public List<Daemon> Daemons { get; internal set; } = new List<Daemon>();
        DaemonHost hostedDaemon;
        [DataMember]
        PointCreationManager Creator { get; set; } = new PointCreationManager();
        public IPoint CurrentPoint { get; set; }
        #region Constructors
        
        public ControlSpace(string name, int port = 666) : this(name, false)
        {
            hostedDaemon = new DaemonHost();
            hostedDaemon.Start(port);
            AddDaemons(hostedDaemon.Address);
            CurrentPoint = Daemons[0].CreatePoint("Main", ChannelType.Any);
        }
        private ControlSpace(string name, bool NotHost)
        {
            this.Name = name;
            ID = Guid.NewGuid();
            PointDirectory = PointDirectory.TrimEnd('\\','/')+"\\" + name+"\\";
        }

        public ControlSpace(string name, string daemonUri) : this(name, true)
        {
            AddDaemons(daemonUri);
            CurrentPoint = Daemons[0].CreatePoint("Main", ChannelType.Any);
        }
        
        public ControlSpace(string name, List<string> list) : this(name, true)
        {
            AddDaemons(list.ToArray());
            var daemon = Daemons.Find(x => x.Name.Contains(IPAddressReceive.GetLocalIPAddress()));
            if (daemon==null)
            {
                daemon = Daemons[0];
            }
            CurrentPoint = daemon.CreatePoint("Main", ChannelType.Any);
        }
        #endregion

        /// <summary>
        /// Used just for fix of WCF deserialization. Which don`t populate Daemons properly
        /// </summary>
        internal void CheckDaemons()
        {
            if (Daemons==null)
            {
                var temp = DaemonAdresses;
                DaemonAdresses = new List<string>();
                Daemons = new List<Daemon>();
                AddDaemons(temp.ToArray());
            }
        }
        internal void AddDaemons(params string[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Daemons.Add(new Daemon(items[i], this));
            }
            DaemonAdresses.AddRange(items);
        }
        public void AddFile(string file, string directory = "")
        {
            AddFileAsync(file, directory).GetAwaiter().OnCompleted(()=> { return; });
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



        public void AddDirectory(string directory, bool recursive = false)
        {
            AddDirectoryAsync(directory, recursive).Wait();
        }
        private async Task AddDirectoryRecursiveAsync(string directory, string startingDirecory)
        {
            var directories = Directory.GetDirectories(directory);
            var RecursiveTasks = new Task[directories.Length];
            for (int i = 0; i < directories.Length; i++)
            {
                RecursiveTasks[i] = AddDirectoryRecursiveAsync(directories[i], startingDirecory);
            }
            await Task.WhenAll(RecursiveTasks);
            var files = Directory.GetFiles(directory);
            Parallel.For(0, files.Length, (i) => { AddFile(files[i], directory.Remove(0, startingDirecory.Length)); });
        }

        public async Task<IPoint> CreatePointAsync()
        {
            Daemon daemonForPoint = await Creator.ChooseDaemonAsync(Daemons);
            return await daemonForPoint.CreatePointAsync();
        }

        public IPoint CreatePoint()
        {
            Daemon daemonForPoint = Creator.ChooseDaemon(Daemons);
            return daemonForPoint.CreatePoint();
        }

        public async Task<IPoint> CreatePointAsync(string Name, PointType pointType, ChannelType channelType)
        {
            Daemon daemonForPoint = await Creator.ChooseDaemonAsync(Daemons);
            return await daemonForPoint.CreatePointAsync(Name, channelType);
        }

        public IPoint CreatePoint(string Name, PointType pointType, ChannelType channelType)
        {
            Daemon daemonForPoint = Creator.ChooseDaemon(Daemons);
            return daemonForPoint.CreatePoint(Name, channelType);
        }


        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (hostedDaemon!=null) // if we created daemon - stop it
                    {
                        hostedDaemon.Stop();
                    }
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }
                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~ControlSpace() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }


        #endregion
    }
}