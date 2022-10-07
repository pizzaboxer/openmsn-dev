using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;
using OpenMSN.Data;
using OpenMSN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [LST] Retrieve Contact List.
    /// Negotiates with the client on what protocol version to use.
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-ver</remarks>
    public class LST_ListContacts
    {
        public const string Command = "LST";

        public static readonly OperationConfig Config = new()
        {
            AuthenticationRequired = true,
            MinProtocolVersion = 0,
            MinArgLength = 1
        };

        public static void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            //  C->S: LST [TransactionID] [ListType]
            //  S->C: LST [TransactionID] [ListType] [ListVersion] [ItemNumber] [TotalItems] [Account] [Nickame]
        }

        public static void RetrieveList(NotificationSession session, int transactionId, string listType)
        {
            List<Contact> contacts;

            //if (listType == "RL")
            //{
            //    ApplicationDbContext dbContext = new();
            //    contacts = dbContext.Contacts.Include(x => x.TargetUser).Where(x => x.TargetUserId == session.User.UserId && x.List == "FL").ToList();
            //}
            //else
            //{
            //    contacts = session.User.Contacts.Where(x => x.List == listType).ToList();
            //}

            contacts = session.User.Contacts.Where(x => x.List == listType).ToList();

            if (contacts.Count == 0)
            {
                // list is empty
                session.SendAsync($"{Command} {transactionId} {listType} {session.User.ContactListVersion} 0 0\r\n");
                return;
            }

            int index = 1;

            foreach (Contact contact in contacts)
            {
                session.SendAsync($"{Command} {transactionId} {listType} {session.User.ContactListVersion} {index} {contacts.Count} {contact.TargetUser.EmailAddress} {contact.TargetUser.Username}\r\n");
                index += 1;
            }
        }
    }
}
