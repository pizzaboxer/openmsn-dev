using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [FND] Find People.
    /// Requests a lookup against the Hotmail user directory.
    /// MSNP2: FND [TransactionID] <Parameters>
    /// </summary>
    /// <remarks>https://wiki.nina.chat/wiki/Protocols/MSNP/Commands/FND</remarks>
    public class FND_FindPeople
    {
        public const string Command = "FND";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MaxProtocolVersion = 7,
            MinArgLength = 5
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            // return no results
            session.SendAsync($"{Command} {transactionId} 0 0\r\n");
        }
    }
}
