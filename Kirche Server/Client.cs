using RawData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using TransitPackage;

namespace Kirche_Server
{
    class Client
    {
        TcpServer server;
        TcpClient tcpClient;
        NetworkStream stream;
        Message message;
        bool streamIsOpen;
        public string District { get; private set; }
        public DataQueue DataQueue { get; }

        public Client(TcpClient client, TcpServer server)
        {
            this.server = server;
            this.tcpClient = client;
            stream = client.GetStream();
            message = new Message(stream);
            DataQueue = new DataQueue();
            streamIsOpen = true;
            Console.WriteLine("User {0} connected ",
                ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
        }

        public void DefineAction()
        {
            try
            {
                while (streamIsOpen)
                {
                    if (DataQueue.UpdatedElems.Count != 0)
                    {
                        Process(ActionEnum.update_send);
                    }
                    else if (DataQueue.DeletedElems.Count != 0)
                    {
                        Process(ActionEnum.del_send);
                    }
                    else if (stream.DataAvailable)
                    {
                        ActionEnum request = message.ReadRequest();
                        Process(request);
                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("DefineAction exception - {0}",
                // ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());
            }
        }

        private void Process(ActionEnum action)
        {
            try
            {
                switch (action)
                {
                    case ActionEnum.login:
                        LoginActionResponse();
                        break;
                    case ActionEnum.logout_receive:
                        LogoutActionResponse();
                        break;
                    case ActionEnum.get_all:
                        GetAllActionResponse();
                        break;
                    case ActionEnum.update_receive:
                        UpdateReceiveActionResponse();
                        break;
                    case ActionEnum.update_send:
                        UpdateSendAction();
                        break;
                    case ActionEnum.del_receive:
                        DeleteReceiveActionResponse();
                        break;
                    case ActionEnum.del_send:
                        DeleteSendAction();
                        break;
                    case ActionEnum.get_categories:
                        GetCategoriesActionResponse();
                        break;
                    case ActionEnum.connection_check:
                        ConnectionCheckActionResponse();
                        break;

                }
            }
            catch (Exception e)
            {
                // Console.WriteLine("Process exception - {0}",
                //     ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());
            }
        }

        private void DeleteSendAction()
        {
            try
            {
                Dictionary<int, string> deletedElems = DataQueue.DeletedElems[0];
                byte[] deletedElemsBytes = null;
                var formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, deletedElems);
                    deletedElemsBytes = stream.ToArray();
                }

                message.WriteRequest(ActionEnum.del_receive);

                message.Write(deletedElemsBytes);

                DataQueue.DeletedElems.RemoveAt(0);
                Console.WriteLine("For delete send");
            }
            catch (Exception e)
            {
                Console.WriteLine("DeleteSend exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                LogoutActionResponse();
            }
        }

        private void DeleteReceiveActionResponse()
        {
            try
            {

                byte[] data = message.Read();

                var formatter = new BinaryFormatter();

                Dictionary<int, string> forDelete = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    forDelete = (Dictionary<int, string>)formatter.Deserialize(stream);
                }

                bool DBDeleteResult = Raw.Delete(forDelete);
                if (DBDeleteResult)
                {
                    message.WrtieResult(OperationResult.Good);
                    server.DeleteForConnectedIp(this, forDelete);
                    Console.WriteLine("For delete recived - {0}",
                        ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());
                }
                else
                    message.WrtieResult(OperationResult.Bad);
            }
            catch (Exception e)
            {
                Console.WriteLine("DeleteReceive exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                LogoutActionResponse();
                // Action for log 
            }
        }

        private void UpdateSendAction()
        {
            try
            {
                message.WriteRequest(ActionEnum.update_receive);

                List<KircheElem> tempUpdatedElems = DataQueue.UpdatedElems[0];
                byte[] updatedElems = KircheElem.SerializationList(tempUpdatedElems);

                message.Write(updatedElems);

                DataQueue.UpdatedElems.RemoveAt(0);
                Console.WriteLine("Update send - {0}",
                    ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("UpdateSend exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                LogoutActionResponse();
                // Action for log 
            }
        }

        private void UpdateReceiveActionResponse()
        {
            try
            {
                byte[] data = message.Read();

                List<KircheElem> elems = KircheElem.DeserializationList(data);

                bool DBUpdateResult = Raw.Update(elems);

                if (DBUpdateResult)
                {
                    message.WrtieResult(OperationResult.Good);

                    server.UpdateForConnectedIp(this, elems);

                    Console.WriteLine("Update recived - {0}",
                        ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
                }
                else
                    message.WrtieResult(OperationResult.Bad);
            }
            catch (Exception e)
            {
                Console.WriteLine("UpdateReceive exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                LogoutActionResponse();
                // Action for log 
            }
        }

        private void GetAllActionResponse()
        {
            try
            {

                List<KircheElem> res = Raw.GetData();

                //for (int i = 10; i < 50000; i++)
                //{
                //    res.Add(new KircheElem()
                //    {
                //        Id = i,
                //        Church = "All",
                //        Church_District = "Altholstein",
                //        Year_Start = DateTime.Parse("10.10.2012"),
                //        Year_End = DateTime.Parse("10.10.2014"),
                //        Price = 1000,
                //        Description = "somsomesomeosmeomwseiownfiebgiabndfjeguybevm"
                //    });
                //}

                byte[] data = KircheElem.SerializationList(res);
                message.Write(data);


                Console.WriteLine("GetAll send - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

            }
            catch (Exception e)
            {

                Console.WriteLine("GetAll exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                LogoutActionResponse();
                // Action for log 
            }

        }

        private void GetCategoriesActionResponse()
        {
            try
            {
                byte[] categories = server.Categories.Serialization();

                message.Write(categories);

                Console.WriteLine("Get categories send - {0}",
                    ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("GetCategories exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                LogoutActionResponse();
                // Action for log 
            }
        }

        private void ConnectionCheckActionResponse()
        {
            try
            {
                message.Write(new byte[1]);
            }
            catch
            {
                Console.WriteLine("ConnectionCheck exception - {0}",
                    ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());
            }
        }

        private void LoginActionResponse()
        {
            try
            {
                Console.WriteLine("User {0} try to login ",
                    ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());

                byte[] data = message.Read();
                string district = Encoding.Unicode.GetString(data);

                data = message.Read();
                string key = Encoding.Unicode.GetString(data);

                bool check = Raw.PasswordCheck(district, key);

                if (server.ConnectedIp.Exists(client => client.District == district))
                    server.ConnectedIp.Find(client => client.District == district)
                        .LogoutActionResponse();

                if (check)
                {
                    Console.WriteLine("User {0} Logged in ",
                        ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
                    District = district;
                }
                else
                    Console.WriteLine("User {0} Bad try ",
                        ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString());

                data = new byte[1] { Convert.ToByte(check) };

                message.Write(data);
            }
            catch (Exception e)
            {

                Console.WriteLine("Login exception - {0} ({1})",
                   ((IPEndPoint)tcpClient.Client?.RemoteEndPoint).Address.ToString(), e.Message);

                tcpClient.Close();
                streamIsOpen = false;
                server.ConnectedIp.Remove(this);
                // Action for log 
            }
        }

        private void LogoutActionResponse()
        {
            try
            {
                Console.WriteLine("User {0} Logged out and disconnected ",
                    ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());

                tcpClient.Close();
                streamIsOpen = false;
                server.ConnectedIp.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine("Logout receive exception - {0}", District);
                // Action for log 
            }
        }
    }
}
