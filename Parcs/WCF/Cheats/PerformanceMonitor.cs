using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Parcs.WCF.Cheats
{
    public class PerformanceMonitor
    {
        private PerformanceCounter _perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        private PerformanceCounter _perfMemCount = new PerformanceCounter("Memory", "Available Mbytes");

        private readonly MachineInfo _staticInfo = new MachineInfo();
        Queue<float> _averageCpu = new Queue<float>(10);
        private Process _currentProcess = Process.GetCurrentProcess();
        public PerformanceMonitor(string name)
        {
            _staticInfo.Name = name;
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            for (int i = 0; i < 10; i++)
            {
                _averageCpu.Enqueue(0);
            }
            _staticInfo.CoresCount = coreCount;

            var timer = new Timer(1000);
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
            _staticInfo.CpuUsage = _perfCpuCount.NextValue();
        }


        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var cpuUsage = _perfCpuCount.NextValue();
            _staticInfo.CpuUsage = cpuUsage;
            _averageCpu.Dequeue();
            _averageCpu.Enqueue(cpuUsage);
            _staticInfo.CpuAverage = _averageCpu.Average();
            _staticInfo.RamFree = _perfMemCount.NextValue();
            _staticInfo.RamUsed = _currentProcess.WorkingSet64 / (1024 * 1024);
        }

        public MachineInfo GetCurrent()
        {
            return _staticInfo;
        }
    }
}