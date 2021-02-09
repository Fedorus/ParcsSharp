using Newtonsoft.Json;

namespace Parcs
{
    public class JsonDotNetParcsSerializer : IParcsSerializer
    {
        public string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public T Deserialize<T>(string item)
        {
            return JsonConvert.DeserializeObject<T>(item);
        }
    }
}