using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace OpenMSN.MSNPServer.Services
{
    public class NotificationServer : TcpServer
    {
        public readonly ConcurrentDictionary<Guid, NotificationSession> NotificationSessions = new();

        public static readonly List<string> SupportedVersions = new() { "MSNP2" };

        public NotificationServer(IPAddress address, int port) : base(address, port) 
        {
            Console.WriteLine($"[NotificationServer] Starting on {address}:{port}");
        }

        protected override TcpSession CreateSession()
        {
            NotificationSession session = new(this);
            NotificationSessions.TryAdd(session.Id, session);
            return session;
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"[NotificationServer/OnError] Encountered a socket error ({error})");
        }

        public NotificationSession? GetSessionFromUserId(int userId)
        {
            var pair = NotificationSessions.Where(x => x.Value.Authenticated && x.Value.User.UserId == userId).FirstOrDefault();
            return pair.Value;
        }
    }
}
