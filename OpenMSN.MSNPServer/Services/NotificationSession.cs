using System.Net.Sockets;
using System.Text;

using NetCoreServer;

using OpenMSN.MSNPServer.Extensions;
using OpenMSN.MSNPServer.Operations.Base;

using OpenMSN.Data.Entities;

namespace OpenMSN.MSNPServer.Services
{
    public class NotificationSession : TcpSession
    {
        public readonly NotificationServer NotificationServer;

        public int ProtocolVersion = 0;
        public bool Authenticated = false;
        public User User = null!;
        public string Status = "FLN";

        public NotificationSession(NotificationServer server) : base(server) 
        {
            NotificationServer = server;
        }

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

            string operation = args[0];
            int transactionId = 0;

            if (args.Length > 1)
            {
                Int32.TryParse(args[1], out transactionId);

                if (transactionId == 0)
                    args = args.Skip(1).ToArray();
                else if (args.Length > 1)
                    args = args.Skip(2).ToArray();
            }
            else
            {
                if (operation.EndsWith("\r\n"))
                    operation = operation[..^2];
            }

            // strip newline from end of last arg
            if (args.Length > 0)
            {
                int last = args.Length - 1;
                if (args[last].EndsWith("\r\n"))
                    args[last] = args[last][..^2];
            }

            try
            {
                switch (operation)
                {
                    case Operations.VER_VersionNegotiation.Command:
                        Operations.VER_VersionNegotiation.Handle(this, transactionId, args);
                        break;

                    case Operations.INF_AuthInfo.Command:
                        Operations.INF_AuthInfo.Handle(this, transactionId, args);
                        break;

                    case Operations.USR_UserLogon.Command:
                        Operations.USR_UserLogon.Handle(this, transactionId, args);
                        break;

                    case Operations.SYN_SynchronizeContacts.Command:
                        Operations.SYN_SynchronizeContacts.Handle(this, transactionId, args);
                        break;

                    case Operations.CHG_ChangeStatus.Command:
                        Operations.CHG_ChangeStatus.Handle(this, transactionId, args);
                        break;

                    case Operations.CVR_ClientVersion.Command:
                        Operations.CVR_ClientVersion.Handle(this, transactionId, args);
                        break;

                    case Operations.FND_FindPeople.Command:
                        Operations.FND_FindPeople.Handle(this, transactionId, args);
                        break;

                    case Operations.ADD_AddPerson.Command:
                        Operations.ADD_AddPerson.Handle(this, transactionId, args);
                        break;

                    case Operations.LST_ListContacts.Command:
                        Operations.LST_ListContacts.Handle(this, transactionId, args);
                        break;

                    case Operations.SND_SendInvitation.Command:
                        Operations.SND_SendInvitation.Handle(this, transactionId, args);
                        break;

                    case Operations.OUT_Logout.Command:
                        Operations.OUT_Logout.Handle(this, transactionId, args);
                        break;

                    default:
                        LogDebug($"Unknown operation received '{operation}'");
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

        public bool CanContact(NotificationSession targetSession)
        {
            if (!this.Authenticated || !targetSession.Authenticated)
                return false;

            User targetUser = targetSession.User;

            // BLP - Message Privacy Configuration : BLP [TransactionID] [ListVersion] [Setting]
            // https://protogined.wordpress.com/msnp2/#cmd-blp
            // this configures who can message the user
            // AL = anyone not on the block list can message the user
            // BL = only users on the allow list can message the user

            if (targetUser.ContactPolicy == "AL" && targetUser.Contacts.Any(x => x.List == "BL" && x.TargetUserId == this.User.UserId))
                return false;

            if (targetUser.ContactPolicy == "BL" && !targetUser.Contacts.Any(x => x.List == "AL" && x.TargetUserId == this.User.UserId))
                return false;

            return true;
        }

        public void BroadcastToList(string list, string message, bool abideByPrivacy = true)
        {
            this.LogDebug($"Broadcasting message \"{message.ToLiteral()}\" to list {list}");

            foreach (NotificationSession session in this.NotificationServer.NotificationSessions.Values)
            {
                if (!this.User.Contacts.Any(x => x.List == list && x.TargetUserId == session.User.UserId))
                    continue;
                
                if (abideByPrivacy && !session.CanContact(this))
                    continue;

                session.SendAsync(message);
            }
        }
    }
}
