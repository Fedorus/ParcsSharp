using System.Reflection;
using Aq.ExpressionJsonSerializer;
using Newtonsoft.Json;
using Parcs;
using Parcs.Logging;

namespace ParcsExpressions
{
    public class ExpressionSerializer : IParcsSerializer
    {
        public string Serialize<T>(T item)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ExpressionJsonConverter(
                Assembly.GetAssembly(typeof (Program))
            ));
            return JsonConvert.SerializeObject(item, settings);
        }

        public T Deserialize<T>(string item)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ExpressionJsonConverter(
                Assembly.GetAssembly(typeof (Program))
            ));
            return JsonConvert.DeserializeObject<T>(item, settings);
        }
    }
}