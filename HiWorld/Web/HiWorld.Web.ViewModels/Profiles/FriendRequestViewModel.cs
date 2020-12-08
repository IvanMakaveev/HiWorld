namespace HiWorld.Web.ViewModels.Profiles
{
    using System;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class FriendRequestViewModel : IMapFrom<ProfileFriend>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string ProfileFirstName { get; set; }

        public string ProfileLastName { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ProfileFriend, FriendRequestViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Profile.Image != null ? $"{x.Profile.Image.Id}.{x.Profile.Image.Extension}" : null));
        }
    }
}
