using AutoMapper;
using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Groups
{
    public class GroupInfoViewModel : IMapFrom<Group>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public int GroupMembersCount { get; set; }

        public bool IsOwner { get; set; }

        public bool IsAdmin { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Group, GroupInfoViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image != null ? $"{x.Image.Id}.{x.Image.Extension}" : null));
        }
    }
}
