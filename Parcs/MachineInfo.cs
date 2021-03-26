using System.Diagnostics;
using System.Text;

namespace Parcs
{
    public class MachineInfo
    {
        public MachineInfo()
        {
        }

        public string Name { get; set; }
        public int CoresCount { get; set; }
        public float CpuUsage { get; set; }
        public float CpuAverage { get; set; }
        public float RamFree { get; set; }
        public long RamUsed { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Name)}: {Name}");
            sb.AppendLine($"{nameof(CoresCount)}: {CoresCount}");
            sb.AppendLine($"{nameof(CpuUsage)}: {CpuUsage:F}%");
            sb.AppendLine($"{nameof(CpuAverage)}: {CpuAverage:F}%");
            sb.AppendLine($"{nameof(RamFree)}: {RamFree:F} Mbytes");
            sb.AppendLine($"{nameof(RamUsed)}: {RamUsed} Mbytes");
            
            return sb.ToString();
        }
    }
}