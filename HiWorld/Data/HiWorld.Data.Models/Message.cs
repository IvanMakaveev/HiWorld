namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class Message : BaseDeletableModel<int>
    {
        public string Text { get; set; }

        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
