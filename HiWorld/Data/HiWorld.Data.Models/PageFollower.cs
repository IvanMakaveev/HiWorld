namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class PageFollower : BaseModel<int>
    {
        public int PageId { get; set; }

        public virtual Page Page { get; set; }

        public int FollowerId { get; set; }

        public virtual Profile Follower { get; set; }
    }
}
