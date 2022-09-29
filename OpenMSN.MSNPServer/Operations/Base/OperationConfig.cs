using OpenMSN.MSNPServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMSN.MSNPServer.Operations.Base
{
    public class OperationConfig
    {
        public int MinProtocolVersion = 2;
        public int MaxProtocolVersion = 0;

        public int MinArgLength = 0;
        public bool TransactionIdRequired = true;

        public bool AuthenticationRequired = false;

        public void Assert(NotificationSession session, int transactionId, string[] args)
        {
            if (MinProtocolVersion != 0 && session.ProtocolVersion < MinProtocolVersion)
                throw new OperationException($"Unsupported protocol version (MSNP{session.ProtocolVersion} < MSNP{MinProtocolVersion})");

            if (MaxProtocolVersion != 0 && session.ProtocolVersion > MaxProtocolVersion)
                throw new OperationException($"Unsupported protocol version (MSNP{session.ProtocolVersion} > MSNP{MinProtocolVersion})");

            if (args.Length < MinArgLength)
                throw new OperationException($"Insufficient args ({args.Length} < {MinArgLength})");

            if (TransactionIdRequired && transactionId == 0)
                throw new OperationException("Invalid transaction ID");

            if (AuthenticationRequired && !session.Authenticated)
                throw new OperationException("Not authenticated");
        }
    }
}
