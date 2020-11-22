namespace HiWorld.Data.Models
{
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;

    public class Page : BaseDeletableModel<int>
    {
        public Page()
        {
            this.Posts = new HashSet<Post>();
            this.PageFollowers = new HashSet<PageFollower>();
            this.PageTags = new HashSet<PageTag>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public int ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<PageFollower> PageFollowers { get; set; }

        public virtual ICollection<PageTag> PageTags { get; set; }
    }
}
