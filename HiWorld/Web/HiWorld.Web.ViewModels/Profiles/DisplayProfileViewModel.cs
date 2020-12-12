namespace HiWorld.Web.ViewModels.Profiles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using AutoMapper;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Posts;

    public class DisplayProfileViewModel : IMapFrom<Data.Models.Profile>, IHaveCustomMappings
    {
        public int Id { get; set; }

        [IgnoreMap]
        public bool IsOwner { get; set; }

        [IgnoreMap]
        public bool IsFriend { get; set; }

        [IgnoreMap]
        public bool IsPending { get; set; }

        [IgnoreMap]
        public bool IsFollowing { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public string ImagePath { get; set; }

        public string CountryName { get; set; }

        public int FriendsCount { get; set; }

        public int FollowersCount { get; set; }

        public List<PostViewModel> Posts { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Data.Models.Profile, DisplayProfileViewModel>()
                .ForMember(x => x.Gender, opt =>
                    opt.MapFrom(x => x.Gender.ToString()))
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image == null ? null : $"{x.Image.Id}.{x.Image.Extension}"))
                .ForMember(x => x.FriendsCount, opt =>
                    opt.MapFrom(x => x.FriendsRecieved.Where(x => x.IsAccepted == true).Count() + x.FriendsSent.Where(x => x.IsAccepted == true).Count()));
        }
    }
}
