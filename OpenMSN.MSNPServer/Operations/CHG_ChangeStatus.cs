using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [CHG] Change User State.
    /// Changes the current status of the user.
    /// MSNP2: CHG [TransactionID] [Status]
    /// </summary>
    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/draft-movva-msn-messenger-protocol#section-7.7
    /// https://protogined.wordpress.com/msnp2/#cmd-chg
    /// </remarks>
    public class CHG_ChangeStatus
    {
        public const string Command = "CHG";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MinArgLength = 1
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            string status = args[0];

            // TODO: propagate status to other clients on NS

            // all we have to do is just echo it back for acknowledgement
            session.SendAsync($"{Command} {transactionId} {status}\r\n");
        }
    }
}
