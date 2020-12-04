namespace HiWorld.Web.ViewModels.Profiles
{
    using System;
    using System.Collections.Generic;

    public class ProfilePostViewModel
    {
        public string Text { get; set; }

        public string ImagePath { get; set; }

        public virtual ICollection<string> PostTags { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
