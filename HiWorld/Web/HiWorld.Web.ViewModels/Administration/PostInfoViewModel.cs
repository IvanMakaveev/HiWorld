namespace HiWorld.Web.ViewModels.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class PostInfoViewModel : IMapFrom<Post>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }

        public string ImageId { get; set; }

        public int? ProfileId { get; set; }

        public int? PageId { get; set; }

        public int CommentsCount { get; set; }

        public int PostLikesCount { get; set; }
    }
}
