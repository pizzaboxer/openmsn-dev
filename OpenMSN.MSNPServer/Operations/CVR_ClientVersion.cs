using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [CVR] Client Version Information.
    /// Client reports version information about the client, while the server informs of minimum/recommended versions.
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-cvr</remarks>
    public class CVR_ClientVersion
    {
        public const string Command = "CVR";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MinArgLength = 6
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            // TODO: collect version information for analytics
            // it's usually something like '0x0409 win 4.10 i386 MSMSGS 1.0.0863'

            session.SendAsync($"{Command} {transactionId} 1.0.0863 1.0.0863 1.0.0863 http://messenger.hotmail.com/mmsetup.exe http://messenger.hotmail.com\r\n");
        }
    }
}
