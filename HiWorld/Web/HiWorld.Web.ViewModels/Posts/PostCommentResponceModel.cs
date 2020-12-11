namespace HiWorld.Web.ViewModels.Posts
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class PostCommentResponceModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public string ProfileFirstName { get; set; }

        public string ProfileLastName { get; set; }

        public string Text { get; set; }

        public int Likes { get; set; }

        public DateTime CreatedOn { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, PostCommentResponceModel>()
                .ForMember(x => x.Likes, opt =>
                    opt.MapFrom(x => x.CommentLikes.Count()));
        }
    }
}
