namespace HiWorld.Data.Models
{

    using HiWorld.Data.Common.Models;

    public class Comment : BaseDeletableModel<int>
    {
        public string Text { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
