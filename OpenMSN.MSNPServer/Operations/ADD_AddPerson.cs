using OpenMSN.Data;
using OpenMSN.Data.Entities;
using OpenMSN.MSNPServer.Operations.Base;
using OpenMSN.MSNPServer.Services;
using System.Collections.Generic;

namespace OpenMSN.MSNPServer.Operations
{
    /// <summary>
    /// [ADD] Add Person.
    /// Add a person to the address book.
    /// MSNP2: ADD [TransactionID] [ListType] [Account] [Account]
    /// </summary>
    /// <remarks>https://protogined.wordpress.com/msnp2/#cmd-add</remarks>
    public static class ADD_AddPerson
    {
        public const string Command = "ADD";

        static readonly string[] Lists = new[]
        {
            "FL",
            "AL",
            "BL"
        };

        static readonly OperationConfig Config = new()
        {
            AuthenticationRequired = true,
            MinProtocolVersion = 0,
            MinArgLength = 3
        };

        public static async void Handle(NotificationSession session, int transactionId, string[] args)
        {
            Config.Assert(session, transactionId, args);

            //  C->S: ADD [TransactionID] [ListType] [Account] [Account]
            //  S->C: ADD [TransactionID] [ListType] [ListVersion] [Account] [Account]

            string listType = args[0];
            string address = args[1];

            if (!Lists.Contains(listType))
            {
                session.SendAsync($"{OperationError.ERR_INVALID_PARAMETER} {transactionId}\r\n");
                return;
            }

            ApplicationDbContext dbContext = new();

            User? targetUser = dbContext.Users.Where(x => x.EmailAddress == address).FirstOrDefault();

            if (targetUser is null)
            {
                session.SendAsync($"{OperationError.ERR_INVALID_USER} {transactionId}\r\n");
                return;
            }

            List<Contact>? contactsOfTargetUser = session.User.Contacts.Where(x => x.TargetUser.UserId == targetUser.UserId).ToList();

            if (contactsOfTargetUser is not null)
            {
                if (contactsOfTargetUser.Any(x => x.List == listType))
                {
                    session.SendAsync($"{OperationError.ERR_ALREADY_THERE} {transactionId}\r\n");
                    return;
                }

                if (listType == "AL" && contactsOfTargetUser.Any(x => x.List == "BL"))
                {
                    session.SendAsync($"{OperationError.ERR_ALREADY_IN_OPPOSITE_LIST} {transactionId}\r\n");
                    return;
                }

                if (listType == "BL" && contactsOfTargetUser.Any(x => x.List == "AL"))
                {
                    session.SendAsync($"{OperationError.ERR_ALREADY_IN_OPPOSITE_LIST} {transactionId}\r\n");
                    return;
                }
            }

            dbContext.Add(new Contact 
            {
                List = listType,
                UserId = session.User.UserId,
                TargetUserId = targetUser.UserId
            });

            session.User.ContactListVersion += 1;

            NotificationSession? targetSession = session.NotificationServer.GetSessionFromUserId(targetUser.UserId);

            session.SendAsync($"{Command} {transactionId} {listType} {session.User.ContactListVersion} {address} {address}\r\n");

            if (listType == "FL")
            {
                // add to target user's reverse list

                dbContext.Add(new Contact
                {
                    List = "RL",
                    UserId = targetUser.UserId,
                    TargetUserId = session.User.UserId
                });

                // technically the target's contact list has updated too now that we're on their reverse list
                targetUser.ContactListVersion += 1;

                if (targetSession is not null)
                {
                    targetSession.SendAsync($"ADD 0 RL {targetUser.ContactListVersion} {session.User.EmailAddress} {session.User.EmailAddress}\r\n");

                    // if we can contact the target session, then we should be notified of the target's status
                    if (session.CanContact(targetSession))
                        session.SendAsync($"ILN {transactionId} {session.Status} {address} {session.User.Username}\r\n");
                }
            }
            else if (targetSession is not null && session.User.Contacts.Any(x => x.TargetUserId == targetUser.UserId && x.List == "RL"))
            {
                // user presence changes (if target is in our reverse list only)

                if (listType == "BL")
                {
                    // notify target session that we're appearing offline (blocked)
                    targetSession.SendAsync($"FLN {session.User.EmailAddress}");
                }
                else if (listType == "AL" && session.User.ContactPolicy == "BL")
                {
                    // if our policy is set to BL (allow list only), notify them that we're online
                    targetSession.SendAsync($"NLN {session.Status} {session.User.EmailAddress} {session.User.Username}");
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
