using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Parcs.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,  IncludeExceptionDetailInFaults = true)]
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

        public async Task<bool> SendAsync(Channel from, Channel to, byte[] data, string type)
        {
            var pointData = Points.ContainsKey(to.PointID) ? Points[to.PointID] : null;
            if (pointData == null)
            {
                throw new Exception("Point not found");
            }
            lock (pointData.CurrentPoint.Data)
            {
                pointData.CurrentPoint.Data.Add(new DataTransferObject()
                { Data = Encoding.UTF8.GetString(data), From = from, To = to, Time = DateTime.Now, Type = type });
            }
            return true;
        }
        public async Task<bool> StartAsync(Channel from, Channel to, PointStartInfo info, ControlSpace space)
        {
            var pointData = Points.ContainsKey(to.PointID) ? Points[to.PointID] : throw new Exception("Point not found"); 
            pointData.CurrentControlSpace = space;
            pointData.CurrentControlSpace.CheckDaemons();

            var currentPoint = new Point(to, to, space);
            currentPoint.Data = new DataObjectsContainer<DataTransferObject>();
            pointData._currentPoint = currentPoint;
            pointData.CurrentControlSpace.CurrentPoint = currentPoint;
            pointData.ParentPoint = new Point(from, to, space);
            
            if (info != null)
            {
                object instance = null;
                MethodBase method = null;
                var assembly = Assembly.LoadFrom(info.AssemblyName);
                if (info.IsStatic)
                {
                    var type = assembly.GetType(info.NamespaceAndClass);
                    method = type.GetMethod(info.MethodName);
                }
                else
                {
                    instance = assembly.CreateInstance(info.NamespaceAndClass);
                    method = instance.GetType().GetMethod(info.MethodName);
                }
              /*  pointData.PointTask = Task.Factory.StartNew(
                    async () => {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        await ((Task)method.Invoke(instance, new object[] { pointData })).ConfigureAwait(false);
                        Console.WriteLine("point task done in "+sw.Elapsed);
                    }, 
                    pointData.CancellationToken, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default);*/
                pointData.PointThread = new System.Threading.Thread(async () =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    await ((Task)method.Invoke(instance, new object[] { pointData })).ConfigureAwait(false);
                    Console.WriteLine($"point task {to.Name} done in " + sw.Elapsed);
                });
                pointData.PointThread.Start();
            }
            return true;
        }
    }
}
