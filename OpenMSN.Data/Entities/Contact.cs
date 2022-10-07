using Microsoft.EntityFrameworkCore;

namespace OpenMSN.Data.Entities
{
    //public enum ListType
    //{
    //    ForwardList,
    //    AddList,
    //    BlockList,
    //    ReverseList
    //}

    // EF Core wrongly assumes TargetUserId is a unique column when building migrations
    [Index(nameof(TargetUserId), IsUnique = false)]
    public class Contact
    {
        public int ContactId { get; set; }
        public string List { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TargetUserId { get; set; }
        public User TargetUser { get; set; } = null!;

        public DateTimeOffset TimeAdded { get; set; } = DateTimeOffset.UtcNow;
    }
}
