namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class ProfileFollower : BaseModel<int>
    {
        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public int FollowerId { get; set; }

        public virtual Profile Follower { get; set; }
    }
}
