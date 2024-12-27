using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace MFramework
{
    public abstract class MUDPServerBase
    {
        public string IP;//������IP
        public int Port;//������Port
        public EndPoint EP;//������EP

        protected Socket _server;
        protected EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);//����ͻ���EndPoint

        protected abstract void Send(SendContext context);
        protected abstract void ReceiveData();

        public MUDPServerBase(string ip, int port)
        {
            IP = ip;
            Port = port;
            EP = new IPEndPoint(IPAddress.Parse(ip), port);

            InitSettings((IPEndPoint)EP);
        }
        public MUDPServerBase(IPEndPoint ep)
        {
            EP = ep;
            IP = ep.Address.ToString();
            Port = ep.Port;

            InitSettings(ep);
        }

        public void InitSettings(IPEndPoint ep)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _server.Bind(ep);

            Debug.Log($"{typeof(MUDPServerBase)}��������<{_server.LocalEndPoint}>�ѿ�ʼ����");

            ReceiveData();
        }

        public void Quit()
        {
            if (_server != null) _server.Close();
        }
    }
}
