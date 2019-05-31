using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransitPackage
{
    public class Message
    {
        private BinaryReader reader;
        private BinaryWriter writer;

        public Message(NetworkStream stream)
        {
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
        }

        public byte[] Read()
        {
            int packageSize = reader.ReadInt32();

            ReadyForNextRead();

            return reader.ReadBytes(packageSize);
        }

        public void Write(byte[] message)
        {
            writer.Write(message.Length);

            WaitForReady();

            writer.Write(message);
        }

        public void WriteRequest(ActionEnum operation)
        {
            writer.Write((int)operation);
            WaitForReady();
        }

        public void WrtieResult(OperationResult result)
        {
            writer.Write((int)result);
            WaitForReady();
        }

        public ActionEnum ReadRequest()
        {
            ActionEnum res = (ActionEnum)reader.ReadInt32();
            ReadyForNextRead();
            return res;
        }

        public OperationResult ReadResult()
        {
            OperationResult res = (OperationResult)reader.ReadInt32();
            ReadyForNextRead();
            return res;
        }

        private void ReadyForNextRead() => writer.Write(true);
        private void WaitForReady() => reader.Read();
    }
}
