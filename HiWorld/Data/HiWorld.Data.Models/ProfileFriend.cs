namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class ProfileFriend : BaseModel<int>
    {
        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public int FriendId { get; set; }

        public virtual Profile Friend { get; set; }

        public bool IsAccepted { get; set; }
    }
}
