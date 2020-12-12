using AutoMapper;
using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Friends
{
    public class FriendViewModel
    {
        public int FriendId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ProfileFriend, FriendViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Profile.Image != null ? $"{x.Profile.Image.Id}.{x.Profile.Image.Extension}" : null));
        }
    }
}
