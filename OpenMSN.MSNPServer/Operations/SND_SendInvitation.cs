using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [SND] Send e-mail invitation.
    /// Sends an e-mail to a user not using MSN Messenger to start using the network.
    /// MSNP2: SND [TransactionID] [EmailAddress]...
    /// </summary>
    public class SND_SendInvitation
    {
        public const string Command = "SND";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MinArgLength = 1
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            // stub, don't do anything

            session.SendAsync($"{Command} {transactionId} OK\r\n");
        }
    }
}
