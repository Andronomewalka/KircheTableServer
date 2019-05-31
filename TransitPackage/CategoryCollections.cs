using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TransitPackage
{
    [Serializable]
    public class CategoryCollections
    {
        public List<string> ProjectType { get; set; }
        public List<string> BuildingType { get; set; }
        public List<string> ChurchDistrict { get; set; }
        public List<string> State { get; set; }
        public List<string> Scope { get; set; }
        public List<string> City { get; set; }


        public CategoryCollections()
        {
            ProjectType = new List<string>();
            BuildingType = new List<string>();
            ChurchDistrict = new List<string>();
            State = new List<string>();
            Scope = new List<string>();
            City = new List<string>();
        }

        public byte[] Serialization()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            byte[] res = stream.ToArray();
            return res;
        }

        public static CategoryCollections Deserialization(byte[] serializedAsBytes)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Write(serializedAsBytes, 0, serializedAsBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return (CategoryCollections)formatter.Deserialize(stream);
        }
    }
}
