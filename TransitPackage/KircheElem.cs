using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TransitPackage
{
    [Serializable]
    public class KircheElem
    {
        public int Id { get; set; }
        public string Church_District { get; set; }
        public string Church { get; set; }
        public List<string> Project_Type { get; set; }
        public DateTime? Year_Start { get; set; }
        public DateTime? Year_End { get; set; }
        public int? Price { get; set; }
        public string Description { get; set; }

        public KircheElem()
        {
            Project_Type = new List<string>();
        }

        public byte[] Serialization()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            byte[] res = stream.ToArray();
            return res;
        }

        public static KircheElem Deserialization(byte[] serializedAsBytes)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Write(serializedAsBytes, 0, serializedAsBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return (KircheElem)formatter.Deserialize(stream);
        }

        public static byte[] SerializationList(List<KircheElem> KircheElems)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, KircheElems);
                return stream.ToArray();
            }
        }

        public static List<KircheElem> DeserializationList(byte[] serializedAsBytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(serializedAsBytes, 0, serializedAsBytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return (List<KircheElem>)formatter.Deserialize(stream);
            }
        }
    }
}
