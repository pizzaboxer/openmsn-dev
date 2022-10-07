using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;
using OpenMSN.Data.Entities;
using OpenMSN.Data;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [SYN] Contact List Synchronization.
    /// Synchronizes current user state between client and server.
    /// MSNP2: SYN [TransactionID] [AddressBookVersion]
    /// </summary>
    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/draft-movva-msn-messenger-protocol#section-7.5
    /// https://protogined.wordpress.com/msnp2/#cmd-syn
    /// </remarks>
    public class SYN_SynchronizeContacts
    {
        public const string Command = "SYN";

        public static readonly OperationConfig Config = new()
        {
            AuthenticationRequired = true,
            MinProtocolVersion = 0,
            MinArgLength = 1
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            // C->S: SYN [TransactionID] [ClientListVersion]
            // S->C: SYN [TransactionID] [ServerListVersion]

            int cListVersion = Int32.Parse(args[0]);
            int sListVersion = session.User.ContactListVersion;

            session.SendAsync($"{Command} {transactionId} {sListVersion}\r\n");

            if (cListVersion == sListVersion)
                return;

            // synchronize contact list and settings

            // GTC - Contact Add Alert Configuration : GTC [TransactionID] [ListVersion] [Setting]
            // https://protogined.wordpress.com/msnp2/#cmd-gtc
            // this configures whether the user should be alerted when someone adds them to their contacts
            // A = alert user on contact add
            // N = silently accept contact add
            session.SendAsync(String.Format("GTC {0} {1} {2}\r\n", transactionId, sListVersion, session.User.ContactAlertOnAdd ? "A" : "N"));

            // BLP - Message Privacy Configuration : BLP [TransactionID] [ListVersion] [Setting]
            // https://protogined.wordpress.com/msnp2/#cmd-blp
            // this configures who can message the user
            // AL = anyone not on the block list can message the user
            // BL = only users on the allow list can message the user
            session.SendAsync($"BLP {transactionId} {sListVersion} {session.User.ContactPolicy}\r\n");

            LST_ListContacts.RetrieveList(session, transactionId, "FL");
            LST_ListContacts.RetrieveList(session, transactionId, "AL");
            LST_ListContacts.RetrieveList(session, transactionId, "BL");
            LST_ListContacts.RetrieveList(session, transactionId, "RL");

            // TODO: ILN?
        }
    }
}
