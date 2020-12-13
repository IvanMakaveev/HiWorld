using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Home
{
    public class PageSearchViewModel : IMapFrom<Page>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImagePath { get; set; }

        public void CreateMappings(AutoMapper.IProfileExpression configuration)
        {
            configuration.CreateMap<Page, PageSearchViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => x.Image == null ? null : $"{x.Image.Id}.{x.Image.Extension}"));
        }
    }
}
