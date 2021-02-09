using System.Threading.Tasks;

namespace Parcs
{
    public interface IParcsSerializer
    {
        string Serialize<T>(T item);
        T Deserialize<T>(string item);
    }
}