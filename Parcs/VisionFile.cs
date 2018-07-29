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
                    await cs.AddDirectoryAsync("");
                }

                // Work with Points
                {
                    Point point1 = await cs.CreatePointAsync();
                    Point point2 = await cs.CreatePointAsync("Name", PointType.Any, ChannelType.TCP);
                    Channel info = point1.Channel; //Channel
                    // Point operations
                    {
                        //GetCurrent Daemon
                        Daemon daemon = await point1.GetDaemonAsync();

                       // await point1.AddChannelAsync(new Channel("Uri/name", new[] { ChannelType.TCP }, new Uri(""), Guid.NewGuid()));
                      //  point1.AddChannel(new Channel("Uri/name", new[] { ChannelType.TCP }, new Uri(""), Guid.NewGuid()));
                        //Get current thread CPU/RAM load
                        { }

                        //Data transfer
                        await point1.SendAsync(42);//json/bson/XML serialization to byte array

                        int data = await point2.GetAsync<int>(); // deserialize T from json/bson/XML

                        // Functioning
                        await point1.CancelAsync();

                        await point1.RunAsync(new PointStartInfo(AsyncPointMethodExample)); // at this stage point service may contain channels to other points

                        await point1.StopAsync();
                    }
                }
            }
        }
        public static async Task AsyncPointMethodExample(PointInfo currentPointInfo)
        {
            Point currentPoint = currentPointInfo.CurrentPoint;
            Point parentPoint = currentPointInfo.ParentPoint;
            ControlSpace cs = currentPointInfo.CurrentControlSpace;
            //
            Point otherPoint = currentPointInfo.GetPoint("Getting point by channel name");
           // otherPoint = currentPointInfo.GetPoint(new Channel("", ChannelType.TCP));// at what stage it should contain something?
            IEnumerable<Channel> allChannels = currentPointInfo.Channels;
            await otherPoint.CancelAsync();
            foreach (var item in allChannels)
            {
                
            }
        }
    }
}
