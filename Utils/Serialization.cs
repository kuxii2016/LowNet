using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace LowNet.Utils
{
    /// <summary>
    /// Smal Serializion
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Save to XML File
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        public static void SaveXml<T>(string path, object obj)
        {
            using (StreamWriter textWriter = new StreamWriter(path))
            {
                new XmlSerializer(typeof(T)).Serialize(textWriter, obj);
            }
        }

        /// <summary>
        /// Load from XML File
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object LoadXml<T>(string path)
        {
            using (StreamReader textReader = new StreamReader(path))
            {
                return new XmlSerializer(typeof(T)).Deserialize(textReader);
            }
        }

        /// <summary>
        /// Get Object in Bytes
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static byte[] FromObject(object Object)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, Object);
                return memoryStream.GetBuffer();
            }
        }

        /// <summary>
        /// Get Object from Bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object ToObject(byte[] bytes)
        {
            using (MemoryStream serializationStream = new MemoryStream(bytes))
            {
                return new BinaryFormatter().Deserialize(serializationStream);
            }
        }
    }
}