namespace HiWorld.Web.ViewModels.Profiles
{
    using System;
    using System.Collections.Generic;

    public class ProfilePostViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string ImagePath { get; set; }

        public virtual ICollection<KeyValuePair<int, string>> PostTags { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsLiked { get; set; }

        public int Likes { get; set; }
    }
}
