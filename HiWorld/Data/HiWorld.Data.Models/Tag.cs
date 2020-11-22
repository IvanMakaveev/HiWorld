namespace HiWorld.Data.Models
{
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;

    public class Tag : BaseModel<int>
    {
        public Tag()
        {
            this.PostTags = new HashSet<PostTag>();
            this.PageTags = new HashSet<PageTag>();
        }

        public string Name { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; }

        public virtual ICollection<PageTag> PageTags { get; set; }
    }
}
