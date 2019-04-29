using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        private UdpClient udpClient = null;


        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void ServerUDP(string address, int port)
        {
            
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            var end = new IPEndPoint(IPAddress.Parse(address), port);
            _socket.Bind(end);
          
            Receive();
        }


        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
         
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
          
            }, state);
        }

        public bool Client(string address, int port)
        {
            try
            {
                udpClient = new UdpClient(port);
                _socket.ReceiveTimeout = 5000;
                _socket.Connect(IPAddress.Parse(address), port);
                udpClient.Connect(IPAddress.Parse(address), port);
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Byte[] sendBytes = Encoding.ASCII.GetBytes("?");
                udpClient.Send(sendBytes, sendBytes.Length);
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            return true;

        }
        }
}
