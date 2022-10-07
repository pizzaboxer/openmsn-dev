using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;
using System.Numerics;
using System;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [OUT] Log out.
    /// </summary>
    /// <remarks>
    /// https://datatracker.ietf.org/doc/html/draft-movva-msn-messenger-protocol#section-7.10
    /// https://protogined.wordpress.com/msnp2/#cmd-out
    /// </remarks>
    public static class OUT_Logout
    {
        public const string Command = "OUT";

        static readonly OperationConfig Config = new()
        {
            AuthenticationRequired = true,
            MinProtocolVersion = 0,
            MinArgLength = 0
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            //  C->S: OUT
            //  S->C: OUT (Reason)

            session.Status = "FLN";
            session.BroadcastToList("FL", $"FLN {session.User.EmailAddress}\r\n");

            session.SendAsync($"{Command}\r\n");
        }
    }
}
