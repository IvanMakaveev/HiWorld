namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;
    using HiWorld.Data.Models.Enums;

    public class GroupMember : BaseModel<int>
    {
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        public int MemberId { get; set; }

        public virtual Profile Member { get; set; }

        public GroupRole Role { get; set; }
    }
}
