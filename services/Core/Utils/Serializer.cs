using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Utils
{
    public class Serializer
    {
        public static string ToXmlString<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, obj);
                stringWriter.Flush();
                return stringWriter.ToString();
            }
        }

        public static T FromXmlString<T>(string xmlString) where T :class
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xmlString))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
