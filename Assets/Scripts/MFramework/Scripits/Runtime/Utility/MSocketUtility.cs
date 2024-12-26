using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MFramework
{
    public static class MSocketUtility
    {
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }

        public static List<IPAddress> GetAllIPV4Addresses()
        {
            List<IPAddress> res = new List<IPAddress>();

            IPHostEntry hostEntry = Dns.GetHostEntry(GetHostName());
            foreach (var ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)//InterNetwork---IPV4
                {
                    res.Add(ip);
                }
            }

            if (res.Count == 0) return null;

            return res;
        }

        public static IPAddress GetDefaultNICIPV4Address()
        {
            // ��ȡ��������ӿ���Ϣ
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // ������������ӿ�
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // �������ӿ�����Ϊ��̫������״̬Ϊ������
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // ��ȡ����ӿڵ� IP ����
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    // ��������ӿڵ� IPv4 ��ַ��Ϣ
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        // ����� IPv4 ��ַ
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }

            return null;
        }
    }
}
