using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ���װ�UDP�����������ṩ�κζ��๦�ܣ���֧��ֱ�Ӵ���
    /// </summary>
    public class MEzUDPServer : MUDPServerBase
    {
        public MEzUDPServer(string ip, int port) : base(ip, port) { }
        public MEzUDPServer(IPEndPoint ep) : base(ep) { }

        public event Action<EndPoint, string> OnReceive;
        public event Action<EndPoint, byte[]> OnSend;

        protected override void ReceiveData()
        {
            //Tip�����������ó���64KB�������ضϴӶ��޷���ȡ
            byte[] bytes = new byte[64 * 1024];//��󻺳�����С
            _server.BeginReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref endPoint, new AsyncCallback(OnReceiveData), bytes);
        }
        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                byte[] bytes = (byte[])result.AsyncState;
                int len = _server.EndReceiveFrom(result, ref endPoint);
                if (len > 0)
                {
                    string message = Encoding.UTF8.GetString(bytes);
                    message = message.TrimEnd('\0');

                    Debug.Log($"�յ����Կͻ���<{endPoint}>����Ϣ��{message}");
                    MainThreadUtility.Post<EndPoint, string>(OnReceive, endPoint, message);
                }

                //������������
                ReceiveData();
            }
            catch (Exception e)
            {
                Debug.Log("���ݽ���ʧ�ܣ�" + e.Message);
            }
        }

        public void SendUTF(EndPoint endPoint, string message = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = endPoint, Buff = buff };

            Send(context);
        }
        public void SendASCII(EndPoint endPoint, string message = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = endPoint, Buff = buff };

            Send(context);
        }
        public void SendBytes(EndPoint endPoint, byte[] buff = null)
        {
            SendContext context = new SendContext() { EndPoint = endPoint, Buff = buff };

            Send(context);
        }
        protected override void Send(SendContext context)
        {
            try
            {
                //����Buff
                _server.BeginSendTo(context.Buff, 0, context.Buff.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, byte[]>(OnSend, context.EndPoint, context.Buff);
                }), _server);
            }
            catch (SocketException ex)
            {
                Debug.Log(ex);
            }
        }
    }
}
