namespace HiWorld.Web.ViewModels.Pages
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Posts;

    public class DisplayPageViewModel : IMapFrom<Page>, IHaveCustomMappings
    {
        public int Id { get; set; }

        [NotMapped]
        public bool IsOwner { get; set; }

        [NotMapped]
        public bool IsFollowing { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int FollowersCount { get; set; }

        public List<PostViewModel> Posts { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Data.Models.Profile, DisplayPageViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image == null ? null : $"{x.Image.Id}.{x.Image.Extension}"));
        }
    }
}
