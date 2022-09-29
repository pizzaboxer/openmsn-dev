using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [VER] Version Negotiation.
    /// Negotiates with the client on what protocol version to use.
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-ver</remarks>
    public class VER_Version
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
                throw new OperationException($"Failed to negotiate protocol version ({args})");

            session.LogDebug($"Negotiated protocol version (MSNP{session.ProtocolVersion})");

            session.SendAsync($"{Command} {transactionId} MSNP{session.ProtocolVersion} CVR0\r\n");
        }
    }
}
