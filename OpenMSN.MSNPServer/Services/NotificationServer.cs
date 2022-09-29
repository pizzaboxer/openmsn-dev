using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenMSN.MSNPServer.Services
{
    class NotificationServer : TcpServer
    {
        public static readonly List<string> SupportedVersions = new() { "MSNP2" };

        public NotificationServer(IPAddress address, int port) : base(address, port) 
        {
            Console.WriteLine($"[NotificationServer] Starting on {address}:{port}");
        }

        protected override TcpSession CreateSession() => new NotificationSession(this);

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"[NotificationServer/OnError] Encountered a socket error ({error})");
        }
    }
}
