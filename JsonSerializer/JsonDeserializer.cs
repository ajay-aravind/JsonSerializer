using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonSerializer
{
    internal class JsonDeserializer
    {
        private JsonSerializerSettings settings;
        private JsonSerializerFormatter formatter;
        private JsonReader reader;
        private IReflectionAPI reflectionAPI;

        public JsonDeserializer(JsonSerializerSettings settings, JsonSerializerFormatter formatter)
        {
            this.settings = settings;
            this.formatter = formatter;
            this.reflectionAPI = new ReflectionAPI();
        }

        public T Deserialize<T>(string inputJson)
        {
            T result = default(T);
            this.reader = new JsonReader(inputJson);


            return result;
        }
    }
}
