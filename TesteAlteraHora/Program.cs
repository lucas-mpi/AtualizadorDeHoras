using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace AlteraHora
{
    class Program
    {
        static void Main(string[] args)
        {


            DateTime d1 = DateTime.Now;
            var horaAtual = PegarDtHoraAtualizada("a.ntp.br");

            if (d1 == horaAtual)
            {
                return;
            }
            Process.Start(@"C:\Users\Acer\Desktop\altera_hora.bat - Atalho");



        }


        public static DateTime PegarDtHoraAtualizada(string ntpServer)
        {
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            //somente IPV4
            var addresses = Dns.GetHostEntry(ntpServer).AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);

            var ipEndPoint = new IPEndPoint(addresses, 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.ReceiveTimeout = 5000; //5 segundos timeout
            socket.Connect(ipEndPoint);
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds).ToLocalTime();

            //ajustando para hora brasil teste ajuste independente do fuso horario
            //networkDateTime = DateTime.ParseExact(networkDateTime.ToString(), "dd /M/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return networkDateTime;
        }
        


    }

  






}


