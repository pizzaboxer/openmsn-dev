using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;
using System.Numerics;
using System;

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
    public static class CHG_ChangeStatus
    {
        public const string Command = "CHG";

        static readonly string[] Statuses = new[]
        {
            "NLN",
            "BSY",
            "IDL", 
            "BRB",
            "AWY",
            "PHN",
            "LUN",
            "HDN"
        };

        static readonly OperationConfig Config = new()
        {
            AuthenticationRequired = true,
            MinProtocolVersion = 0,
            MinArgLength = 1
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            //  C->S: CHG [TransactionID] [Status]
            //  S->C: CHG [TransactionID] [Status]

            string status = args[0];

            if (!Statuses.Contains(status))
            {
                session.SendAsync($"{OperationError.ERR_INVALID_PARAMETER} {transactionId}\r\n");
                return;
            }

            if (status == session.Status)
            {
                session.SendAsync($"{OperationError.ERR_ALREADY_IN_THE_MODE} {transactionId}\r\n");
                return;
            }

            session.Status = status;
            session.BroadcastToList("FL", $"NLN {status} {session.User.EmailAddress} {session.User.Username}\r\n");

            session.SendAsync($"{Command} {transactionId} {status}\r\n");
        }
    }
}
