using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Parcs.WCF.DTO;

namespace Parcs.WCF
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single, 
        ConcurrencyMode = ConcurrencyMode.Multiple,  
        IncludeExceptionDetailInFaults = true)]
    public class PointService : IPointService
    {
        public Dictionary<Guid, PointInfo> Points { get; set; } = new Dictionary<Guid, PointInfo>();

        public async Task<bool> AddChannelAsync(Channel to, Channel channel)
        {
            var pointData = Points.ContainsKey(to.PointID) ? Points[to.PointID] : null;
            if (pointData == null)
            {
                throw new Exception("Point not found");
            }
            pointData.Channels.Add(channel);
            return true;
        }

        public async Task<ReceiveConfirmation> SendAsync(SendDataParams sendData)
        {
           // var s = new Stopwatch(); s.Start();
            var pointData = Points.ContainsKey(sendData.To.PointID) ? Points[sendData.To.PointID] : null;
            if (pointData == null)
            {
                throw new Exception("Point not found");
            }
            lock (pointData.Point.Data)
            {
                pointData.Point.Data.Add(sendData);
            }
//Console.WriteLine("Server wasted:  "  +s.Elapsed);
            return new ReceiveConfirmation() { Result = true };
        }

        public async Task<bool> StartAsync(Channel from, Channel to, PointStartInfo info, ControlSpaceDTO space)
        {
            var pointData = Points.ContainsKey(to.PointID) ? Points[to.PointID] : throw new Exception("Point not found");
            pointData.ControlSpace = new ControlSpace(space);

            var currentPoint = new Point(to, to, pointData.ControlSpace);
            currentPoint.Data = new DataObjectsContainer<SendDataParams>();
            pointData._currentPoint = currentPoint;
            pointData.ControlSpace.CurrentPoint = currentPoint;
            pointData.ParentPoint = new Point(from, to, pointData.ControlSpace);

            if (info != null)
            {
                object instance = null;
                MethodBase method = null;
                var assembly = Assembly.LoadFrom(Directory.GetCurrentDirectory()+'\\'+space.Name+'\\'+ info.AssemblyName);
                if (info.IsStatic)
                {
                    var type = assembly.GetType(info.NamespaceAndClass);
                    method = type.GetMethod(info.MethodName);
                }
                else
                {
                    instance = assembly.CreateInstance(info.NamespaceAndClass);
                    method = instance?.GetType().GetMethod(info.MethodName);
                }
                var sw = new Stopwatch();
                sw.Start();
                pointData.PointTask = (Task)method?.Invoke(instance, new object[] { pointData });
                pointData.PointTask?.ContinueWith((t) => {
                        sw.Stop();
                        Console.WriteLine($"point task {to.Name} done in {sw.Elapsed} task status {t.Status}");
                        if (t.Status == TaskStatus.Faulted)
                        {
                            Console.WriteLine(t.Exception);
                        }
                }).ConfigureAwait(false);

                /*pointData.PointThread = new System.Threading.Thread( () =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    ((Task)method.Invoke(instance, new object[] { pointData })).GetAwaiter().GetResult();
                    Console.WriteLine($"point task {to.Name} done in " + sw.Elapsed);
                    sw.Stop();
                });
                pointData.PointThread.Start();*/
            }
            return true;
        }

        public async Task<bool> TestWork()
        {
            return true;
        }

        public async Task<PointInfoDTO> GetInfoAsync(Channel to)
        {
            Console.WriteLine(Points[to.PointID].PointTask.Status);
            return new PointInfoDTO();
        }
    }
}
