namespace HiWorld.Data.Models
{
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;

    public class Post : BaseDeletableModel<int>
    {
        public Post()
        {
            this.PostTags = new HashSet<PostTag>();
        }

        public string Text { get; set; }

        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public int? ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public int? PageId { get; set; }

        public virtual Page Page { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}
