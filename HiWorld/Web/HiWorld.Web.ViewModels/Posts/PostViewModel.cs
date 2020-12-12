namespace HiWorld.Web.ViewModels.Posts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class PostViewModel : IMapFrom<Post>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string OwnerName { get; set; }

        public int OwnerId { get; set; }

        public string OwnerImage { get; set; }

        public bool IsProfilePost { get; set; }

        public string Text { get; set; }

        public string ImagePath { get; set; }

        public ICollection<KeyValuePair<int, string>> PostTags { get; set; }

        public DateTime CreatedOn { get; set; }

        [IgnoreMap]
        public bool IsLiked { get; set; }

        public int Likes { get; set; }

        public List<PostCommentResponceModel> Comments { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Post, PostViewModel>()
                .ForMember(x => x.OwnerName, opt =>
                    opt.MapFrom(x => x.Profile != null ? $"{x.Profile.FirstName} {x.Profile.LastName}" : $"{x.Page.Name}"))
                .ForMember(x => x.OwnerId, opt =>
                    opt.MapFrom(x => x.ProfileId != null ? x.ProfileId : x.PageId))
                .ForMember(x => x.OwnerImage, opt =>
                    opt.MapFrom(x => x.ProfileId != null ?
                        x.Profile.Image != null ? $"{x.Profile.Image.Id}.{x.Profile.Image.Extension}" : null :
                        x.Page.Image != null ? $"{x.Page.Image.Id}.{x.Page.Image.Extension}" : null))
                .ForMember(x => x.IsProfilePost, opt =>
                    opt.MapFrom(x => x.ProfileId != null ? true : false))
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image == null ? null : $"{x.Image.Id}.{x.Image.Extension}"))
                .ForMember(x => x.PostTags, opt =>
                    opt.MapFrom(x => x.PostTags.Select(tag => new KeyValuePair<int, string>(tag.Tag.Id, tag.Tag.Name)).ToList()))
                .ForMember(x => x.Likes, opt =>
                    opt.MapFrom(x => x.PostLikes.Count()));
        }
    }
}
