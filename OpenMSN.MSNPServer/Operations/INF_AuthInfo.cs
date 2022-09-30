using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [INF] Authentication Information.
    /// Delivers information on how to handle user authentication.
    /// MSNP2: INF [TransactionID] MD5
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-inf</remarks>
    public class INF_AuthInfo
    {
        public const string Command = "INF";

        public static readonly OperationConfig Config = new()
        {
            MaxProtocolVersion = 7
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            // always returns MD5
            session.SendAsync($"{Command} {transactionId} MD5\r\n");
        }
    }
}
