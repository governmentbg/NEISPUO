namespace MON.Shared.Extensions
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public static class SerializationUtils
    {
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return default;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }

        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return default;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            return binaryFormatter.Deserialize(memoryStream) as T;
        }

        public static string ToXml<T>(this T obj) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            using var xmlWriter = XmlWriter.Create(sb);
            serializer.Serialize(xmlWriter, obj);
            return sb.ToString();
        }

        public static string ToJson<T>(this T obj, JsonSerializerSettings settings = null) where T : class
        {
            if (obj == null) return "";
            return JsonConvert.SerializeObject(obj, settings);          
        }

    }

    public class JsonIgnoreResolver : DefaultContractResolver
    {
        private readonly HashSet<string> ignoreProps;

        public JsonIgnoreResolver(IEnumerable<string> propNamesToIgnore)
        {
            this.ignoreProps = new HashSet<string>(propNamesToIgnore);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (this.ignoreProps.Contains(property.PropertyName))
            {
                property.ShouldSerialize = _ => false;
            }
            return property;
        }
    }
}
