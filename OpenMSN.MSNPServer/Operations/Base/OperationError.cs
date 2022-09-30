using Npgsql.PostgresTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMSN.MSNPServer.Operations.Base
{
    // https://datatracker.ietf.org/doc/html/draft-movva-msn-messenger-protocol#section-7.11
    public class OperationError
    {
        public const int ERR_SYNTAX_ERROR                 = 200;
        public const int ERR_INVALID_PARAMETER            = 201;
        public const int ERR_INVALID_USER                 = 205;
        public const int ERR_FQDN_MISSING                 = 206;
        public const int ERR_ALREADY_LOGIN                = 207;
        public const int ERR_INVALID_USERNAME             = 208;
        public const int ERR_INVALID_FRIENDLY_NAME        = 209;
        public const int ERR_LIST_FULL                    = 210;
        public const int ERR_ALREADY_THERE                = 215;


        public const int ERR_NOT_ON_LIST                  = 216;
        public const int ERR_ALREADY_IN_THE_MODE          = 218;
        public const int ERR_ALREADY_IN_OPPOSITE_LIST     = 219;
        public const int ERR_SWITCHBOARD_FAILED           = 280;
        public const int ERR_NOTIFY_XFR_FAILED            = 281;

        public const int ERR_REQUIRED_FIELDS_MISSING      = 300;
        public const int ERR_NOT_LOGGED_IN                = 302;
        public const int ERR_INTERNAL_SERVER              = 500;
        public const int ERR_DB_SERVER                    = 501;
        public const int ERR_FILE_OPERATION               = 510;
        public const int ERR_MEMORY_ALLOC                 = 520;
        public const int ERR_SERVER_BUSY                  = 600;
        public const int ERR_SERVER_UNAVAILABLE           = 601;
        public const int ERR_PEER_NS_DOWN                 = 602;
        public const int ERR_DB_CONNECT                   = 603;
        public const int ERR_SERVER_GOING_DOWN            = 604;
        public const int ERR_CREATE_CONNECTION            = 707;
        public const int ERR_BLOCKING_WRITE               = 711;
        public const int ERR_SESSION_OVERLOAD             = 712;
        public const int ERR_USER_TOO_ACTIVE              = 713;
        public const int ERR_TOO_MANY_SESSIONS            = 714;
        public const int ERR_NOT_EXPECTED                 = 715;
        public const int ERR_BAD_FRIEND_FILE              = 717;
        public const int ERR_AUTHENTICATION_FAILED        = 911;
        public const int ERR_NOT_ALLOWED_WHEN_OFFLINE     = 913;
        public const int ERR_NOT_ACCEPTING_NEW_USERS      = 920;
    }
}
