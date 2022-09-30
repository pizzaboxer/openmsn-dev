using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [SYN] User Synchronization.
    /// Synchronizes current user state between client and server.
    /// MSNP2: SYN [TransactionID] [AddressBookVersion]
    /// </summary>
    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/draft-movva-msn-messenger-protocol#section-7.5
    /// https://protogined.wordpress.com/msnp2/#cmd-syn
    /// </remarks>
    public class SYN_Synchronization
    {
        public const string Command = "SYN";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MinArgLength = 1
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            // C->S: SYN [TransactionID] [ClientABVersion]
            // S->C: SYN [TransactionID] [ServerABVersion]

            int clientABVersion = Int32.Parse(args[0]);

            // TODO: address book versioning
            // for now, we'll just echo back whatever the client says

            session.SendAsync($"{Command} {transactionId} {clientABVersion}\r\n");
        }
    }
}
