using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [VER] Version Negotiation.
    /// Negotiates with the client on what protocol version to use.
    /// MSNP2: VER [TransactionID] [ProtocolVersion]...
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-ver</remarks>
    public class VER_VersionNegotiation
    {
        public const string Command = "VER";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MinArgLength = 2
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            foreach (string arg in args)
            {
                if (NotificationServer.SupportedVersions.Contains(arg))
                    session.ProtocolVersion = Int32.Parse(arg[4..]);
            }

            if (session.ProtocolVersion == 0)
            {
                session.LogDebug($"Failed to negotiate protocol version ({args})");
                session.SendAsync($"{Command} {transactionId} 0\r\n");
                session.Disconnect();
                return;
            }

            session.LogDebug($"Negotiated protocol version (MSNP{session.ProtocolVersion})");

            session.SendAsync($"{Command} {transactionId} MSNP{session.ProtocolVersion} CVR0\r\n");
        }
    }
}
