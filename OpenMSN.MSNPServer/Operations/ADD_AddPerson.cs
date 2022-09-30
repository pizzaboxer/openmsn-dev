using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [ADD] Add Person.
    /// Add a person to the address book.
    /// MSNP2: ADD [TransactionID] [ListType] [Account] [Account]
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-add</remarks>
    public class ADD_AddPerson
    {
        public const string Command = "ADD";

        public static readonly OperationConfig Config = new()
        {
            MinProtocolVersion = 0,
            MinArgLength = 3
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            string listType = args[0];
            string address = args[1];

            if (listType != "AL" && listType != "FL" && listType != "BL")
            {
                session.SendAsync($"{OperationError.ERR_INVALID_PARAMETER} {transactionId}");
                return;
            }
        }
    }
}
