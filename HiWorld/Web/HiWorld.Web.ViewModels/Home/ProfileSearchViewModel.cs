using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Home
{
    public class ProfileSearchViewModel : IMapFrom<Profile>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImagePath { get; set; }

        public void CreateMappings(AutoMapper.IProfileExpression configuration)
        {
            configuration.CreateMap<Profile, ProfileSearchViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image == null ? null : $"{x.Image.Id}.{x.Image.Extension}"));
        }
    }
}
