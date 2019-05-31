using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TransitPackage;
using RawData;

namespace Kirche_Server
{
    class TcpServer
    {
        private TcpListener listener;
        public List<Client> ConnectedIp { get; private set; }
        public CategoryCollections Categories { get; set; }
        private Mutex ConnectedIpChanged;

        public TcpServer()
        {
            listener = new TcpListener(IPAddress.Any, 1815);
            listener.Server.ReceiveTimeout = 10000;
            listener.Server.SendTimeout = 10000;

            ConnectedIp = new List<Client>();
            ConnectedIpChanged = new Mutex();

            Categories = Raw.GetCategories();
        }

        public async Task Start()
        {
            listener.Start();
            while (true)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                ProcessClient(tcpClient); // await не нужен
            }
        }

        private async Task ProcessClient(TcpClient tcpClient)
        {
            Client client = new Client(tcpClient, this);

            ConnectedIpChanged.WaitOne();
            ConnectedIp.Add(client);
            ConnectedIpChanged.ReleaseMutex();

            await Task.Run(() => client.DefineAction()); // нельзя ожидать ничего (void) исправить ?
        }

        public void UpdateForConnectedIp(Client sender, List<KircheElem> updatedElems)
        {
            ConnectedIpChanged.WaitOne();

            foreach (var item in ConnectedIp)
                if (item != sender && item.District != null)
                    item.DataQueue.UpdatedElems.Add(updatedElems);

            ConnectedIpChanged.ReleaseMutex();
        }

        public void DeleteForConnectedIp(Client sender, Dictionary<int, string> forDelete)
        {
            ConnectedIpChanged.WaitOne();

            foreach (var item in ConnectedIp)
                if (item != sender && item.District != null)
                    item.DataQueue.DeletedElems.Add(forDelete);

            ConnectedIpChanged.ReleaseMutex();
        }
    }
}
