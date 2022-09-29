using System.Net.Sockets;
using System.Text;

using NetCoreServer;

using OpenMSN.MSNPServer.Extensions;
using OpenMSN.MSNPServer.Operations.Base;

namespace OpenMSN.MSNPServer.Services
{
    public class NotificationSession : TcpSession
    {
        public int ProtocolVersion = 0;
        public bool Authenticated = false;

        public NotificationSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"[Notification/{Id}/OnConnected] Received new connection");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"[Notification/{Id}/OnDisconnected] Client disconnected");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            
            Console.WriteLine($"[Notification/{Id}/OnReceived] Received \"{message.ToLiteral()}\"");

            // TODO: handle multi-line requests

            string[] args = message.Split(' ');

            if (args.Length < 1)
                return;

            string opcode = args[0];
            int transactionId;

            Int32.TryParse(args[1], out transactionId);

            if (transactionId == 0)
                args = args.Skip(1).ToArray();
            else if (args.Length > 1)
                args = args.Skip(2).ToArray();

            try
            {
                switch (opcode)
                {
                    case Operations.VER_Version.Command:
                        Operations.VER_Version.Handle(this, transactionId, args);
                        break;

                    case Operations.INF_AuthInfo.Command:
                        Operations.INF_AuthInfo.Handle(this, transactionId, args);
                        break;
                }
            }
            catch (OperationException ex)
            {
                Console.WriteLine($"[Notification/{Id}/OnReceived] Encountered operation exception");
                Console.WriteLine(ex);
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"[Notification/{Id}/OnError] Encountered a socket error ({error})");
        }

        public override bool SendAsync(byte[] buffer)
        {
            string message = Encoding.Default.GetString(buffer);
            Console.WriteLine($"[Notification/{Id}/OnReceived] Sending \"{message.ToLiteral()}\"");
            return base.SendAsync(buffer);
        }

        public void LogDebug(string message)
        {
            Console.WriteLine($"[Notification/{Id}/LogDebug] {message}");
        }
    }
}
