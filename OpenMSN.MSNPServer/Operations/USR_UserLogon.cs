using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

using OpenMSN.Data;
using OpenMSN.Data.Entities;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [USR] User Logon.
    /// Handles user authentication.
    /// MSNP2: USR [TransactionID] MD5 [Stage] [Parameter]
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-usr</remarks>
    public class USR_UserLogon
    {
        public const string Command = "USR";

        public static readonly OperationConfig Config = new()
        {
            MinArgLength = 3
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            string mode = args[0];

            if (mode == "MD5" && session.ProtocolVersion <= 7)
            {
                // MSNP2 - MSNP7
                HandleMD5(session, transactionId, args);
            }
            else
            {
                throw new OperationException($"Unexpected user authentication mode {mode}");
            }
        }

        public static void HandleMD5(NotificationSession session, int transactionId, string[] args)
        {
            string stage = args[1];

            ApplicationDbContext dbContext = new();

            if (stage == "I")
            {
                //  C->S: USR [TransactionID] MD5 I [Account]
                //  S->C: USR [TransactionID] MD5 S [Challenge]

                User? user = dbContext.Users.Where(x => x.EmailAddress == args[2]).FirstOrDefault();

                if (user is null)
                {
                    session.LogDebug("MD5 authentication failed (user is null)");
                    session.SendAsync($"{OperationError.ERR_AUTHENTICATION_FAILED} {transactionId}\r\n");
                    return;
                }

                session.CurrentUser = user;

                session.SendAsync($"{Command} {transactionId} MD5 S {user.MD5Salt}\r\n");
            }
            else if (stage == "S")
            {
                //  C->S: USR [TransactionID] MD5 S [Hashed Response]
                //  S->C: USR [TransactionID] OK [Account] [Nickname]

                if (session.CurrentUser is null)
                {
                    session.LogDebug("MD5 authentication failed (CurrentUser is null)");
                }
                else if (!session.CurrentUser.VerifyPasswordMD5(args[2]))
                {
                    session.LogDebug("MD5 authentication failed (Failed to verify password)");
                }
                else if (!session.CurrentUser.CanLogin())
                {
                    session.LogDebug("MD5 authentication failed (CurrentUser is not allowed to login at this time)");
                }
                else
                {
                    session.Authenticated = true;
                }

                if (!session.Authenticated)
                {
                    session.SendAsync($"{OperationError.ERR_AUTHENTICATION_FAILED} {transactionId}\r\n");
                    return;
                }

                session.LogDebug($"MD5 authentication succeeded for {session.CurrentUser.EmailAddress}");

                session.SendAsync($"{Command} {transactionId} OK {session.CurrentUser.EmailAddress} {session.CurrentUser.Username}\r\n");
            }
            else
            {
                throw new OperationException($"Unexpected user authentication stage {stage}");
            }
        }
    }
}
