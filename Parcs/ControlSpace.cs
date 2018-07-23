﻿using Parcs.WCF;
using Parcs.WCF.Cheats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Parcs
{
    [DataContract]
    public class ControlSpace
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
        public Point CurrentPoint { get; set; }

        #region Constructors
        public ControlSpace(string name, int port = 666) : this(name, false)
        {
            hostedDaemon = new DaemonHost();
            hostedDaemon.Start(port);
            AddDaemons(hostedDaemon.Address);
            CurrentPoint = Daemons[0].CreatePointAsync("Main", ChannelType.Any).GetAwaiter().GetResult();
            CurrentPoint.RunAsync(null).GetAwaiter().GetResult();
            //Thread.Sleep(5000);
            CurrentPoint = hostedDaemon._service.PointService.Points[CurrentPoint.Channel.PointID].CurrentPoint;
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
            CurrentPoint = Daemons[0].CreatePointAsync("Main", ChannelType.Any).GetAwaiter().GetResult();
        }
        public ControlSpace(string name, List<string> list) : this(name, true)
        {
            AddDaemons(list.ToArray());
            var daemon = Daemons.Find(x => x.Name.Contains(IPAddressReceive.GetLocalIPAddress()));
            if (daemon==null)
            {
                daemon = Daemons[0];
            }
            CurrentPoint = daemon.CreatePointAsync("Main", ChannelType.Any).GetAwaiter().GetResult();
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
            var RecursiveTasks = new Task[directories.Length];
            for (int i = 0; i < directories.Length; i++)
            {
                RecursiveTasks[i] = AddDirectoryRecursiveAsync(directories[i], startingDirecory);
            }
            await Task.WhenAll(RecursiveTasks);
            var files = Directory.GetFiles(directory);
            Parallel.For(0, files.Length, async (i) => { await AddFileAsync(files[i], directory.Remove(0, startingDirecory.Length)); });
        }

        public async Task<Point> CreatePointAsync()
        {
            Daemon daemonForPoint = Creator.ChooseDaemon(Daemons);
            return await daemonForPoint.CreatePointAsync();
        }

        public async Task<Point> CreatePointAsync(string Name, PointType pointType, ChannelType channelType)
        {
            Daemon daemonForPoint = Creator.ChooseDaemon(Daemons);
            return await daemonForPoint.CreatePointAsync(Name, channelType);
        }
    }
}