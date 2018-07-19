using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcs
{
    class VisionFile
    {
        
        public async Task GeneralIdea()
        {
            // CS constructors
            //Creating Point-service for data exchange with createdPoints.
            ControlSpace cs = new ControlSpace("TaskName");
            cs = new ControlSpace("TaskName", "uri");
            cs = new ControlSpace("TaskName", new List<string>() { "uri" });

            // CS Functions
            {
                // Working With Daemons
                {
                    IEnumerable<Daemon> daemons = cs.Daemons;
                    var daemon = daemons.FirstOrDefault();
                    cs.AddDaemons("daemon uri");
                    cs.AddDaemons("daemon uri", "another one");
                    // Daemon functions
                    {
                        string DaemonNameOrUri = daemon.Name;
                        // Daemon Points Pool
                        // Daemon Points 
                        {
                            //Points Service Operations 
                        }
                        // Daemon Statistics
                        {
                            //PC workload etc
                        }
                    }
                }

                // AddingFiles to CS (every daemon)
                {
                    await cs.AddFileAsync("");
                    cs.AddFile("");
                    await cs.AddDirectoryAsync("");
                    cs.AddDirectory("");
                }

                // Work with Points
                {
                    IPoint point1 = await cs.CreatePointAsync();
                    point1 = cs.CreatePoint();
                    IPoint point2 = await cs.CreatePointAsync("Name", PointType.Any, ChannelType.TCP);
                    point2 = cs.CreatePoint("Name", PointType.Any, ChannelType.TCP);
                    Channel info = point1.Channel; //Channel
                    // Point operations
                    {
                        //GetCurrent Daemon
                        Daemon daemon = await point1.GetDaemonAsync();
                        daemon = point1.GetDaemon();

                       // await point1.AddChannelAsync(new Channel("Uri/name", new[] { ChannelType.TCP }, new Uri(""), Guid.NewGuid()));
                      //  point1.AddChannel(new Channel("Uri/name", new[] { ChannelType.TCP }, new Uri(""), Guid.NewGuid()));
                        //Get current thread CPU/RAM load
                        { }

                        //Data transfer
                        bool sended = point1.Send(new int());//json/bson/XML serialization to byte array
                        sended = await point1.SendAsync(new int());

                        int data = point2.Get<int>(); // deserialize T from json/bson/XML
                        data = await point2.GetAsync<int>();
                        // Functioning
                        await point1.CancelAsync();
                        point1.Cancel();

                         await point1.RunAsync(new PointStartInfo(AsyncPointMethodExample)); // at this stage point service may contain channels to other points
                        point1.Run(new PointStartInfo((Action<PointInfo>)VoidPointMethod));

                        await point1.StopAsync();
                        point1.Stop();
                    }
                }
            }
        }
        public static void VoidPointMethod(PointInfo info)
        {

        }
        public static async Task AsyncPointMethodExample(PointInfo currentPointInfo)
        {
            IPoint currentPoint = currentPointInfo.CurrentPoint;
            IPoint parentPoint = currentPointInfo.ParentPoint;
            ControlSpace cs = currentPointInfo.CurrentControlSpace;
            //
            IPoint otherPoint = currentPointInfo.GetPoint("Getting point by channel name");
           // otherPoint = currentPointInfo.GetPoint(new Channel("", ChannelType.TCP));// at what stage it should contain something?
            IEnumerable<Channel> allChannels = currentPointInfo.GetChannels();
            await otherPoint.CancelAsync();
            foreach (var item in allChannels)
            {
                
            }
        }
    }
}
