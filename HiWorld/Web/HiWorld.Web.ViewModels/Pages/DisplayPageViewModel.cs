namespace HiWorld.Web.ViewModels.Pages
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Posts;

    public class DisplayPageViewModel : IMapFrom<Page>, IHaveCustomMappings
    {
        public int Id { get; set; }

        [IgnoreMap]
        public bool IsOwner { get; set; }

        [IgnoreMap]
        public bool IsFollowing { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int PageFollowersCount { get; set; }

        [IgnoreMap]
        public IEnumerable<PostViewModel> Posts { get; set; }

        [IgnoreMap]
        public PagingViewModel Paging { get; set; }

        public ICollection<KeyValuePair<int, string>> PageTags { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Page, DisplayPageViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image == null ? null : $"{x.Image.Id}.{x.Image.Extension}"))
                .ForMember(x => x.PageTags, opt =>
                    opt.MapFrom(x => x.PageTags.Select(tag => new KeyValuePair<int, string>(tag.Tag.Id, tag.Tag.Name)).ToList()));
        }
    }
}
