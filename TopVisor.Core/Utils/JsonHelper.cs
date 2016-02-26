using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TopVisor.Core.Utils
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(String data)
        {
            using (var reader = new StringReader(data))
            {
                T result;
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    result = serializer.Deserialize<T>(jsonReader);
                }
                return result;
            }
        }

        public static Boolean TryDeserialize<T>(String data, out T result)
        {
            try
            {
                result = Deserialize<T>(data);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        public static T Convert<T>(Object obj)
        {
            return obj == null ? default(T) : Deserialize<T>(obj.ToString());
        }

        public static Boolean TryConvert<T>(Object obj, out T result)
        {
            if (obj == null)
            {
                result = default(T);
                return false;
            }
            else
                return TryDeserialize(obj.ToString(), out result);
        }

        public static String Serialize(Object obj)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
                return stringBuilder.ToString();
            }
        }
    }
}