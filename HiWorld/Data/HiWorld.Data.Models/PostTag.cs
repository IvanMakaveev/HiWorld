namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class PostTag : BaseModel<int>
    {
        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
