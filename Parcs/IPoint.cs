using System.Threading.Tasks;

namespace Parcs
{
    public interface IPoint
    {
        Channel Channel { get; }
        Daemon GetDaemon();
        Task<Daemon> GetDaemonAsync();
        Task AddChannelAsync(Channel channel);
        void AddChannel(Channel channel);
        bool Send<T>(T t);
        Task<bool> SendAsync<T>(T t);
        T Get<T>();
        Task<T> GetAsync<T>();
        Task CancelAsync();
        void Cancel();
        Task RunAsync(PointStartInfo pointStartInfo);
        void Run(PointStartInfo pointStartInfo);
        Task StopAsync();
        void Stop();
    }
}