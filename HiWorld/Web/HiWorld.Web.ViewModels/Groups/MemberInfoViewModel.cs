using HiWorld.Data.Models;
using HiWorld.Data.Models.Enums;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Groups
{
    public class MemberInfoViewModel : IMapFrom<GroupMember>, IHaveCustomMappings
    {
        public int MemberId { get; set; }

        public string MemberFirstName { get; set; }

        public string MemberLastName { get; set; }

        public string ImagePath { get; set; }

        public bool IsOwner { get; set; }

        public bool IsAdmin { get; set; }

        public void CreateMappings(AutoMapper.IProfileExpression configuration)
        {
            configuration.CreateMap<GroupMember, MemberInfoViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Member.Image == null ? null : $"{x.Member.Image.Id}.{x.Member.Image.Extension}"))
                .ForMember(x => x.IsOwner, opt =>
                    opt.MapFrom(x => x.Role == GroupRole.Owner ? true : false))
                .ForMember(x => x.IsAdmin, opt =>
                    opt.MapFrom(x => x.Role == GroupRole.Admin ? true : false));
        }
    }
}
