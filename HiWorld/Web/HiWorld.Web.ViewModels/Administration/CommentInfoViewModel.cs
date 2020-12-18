namespace HiWorld.Web.ViewModels.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class CommentInfoViewModel : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }

        public int PostId { get; set; }

        public int ProfileId { get; set; }
    }
}
