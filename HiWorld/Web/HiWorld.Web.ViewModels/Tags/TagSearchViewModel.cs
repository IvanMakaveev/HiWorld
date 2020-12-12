namespace HiWorld.Web.ViewModels.Tags
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Pages;
    using HiWorld.Web.ViewModels.Posts;

    public class TagSearchViewModel : IMapFrom<Tag>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public List<PostViewModel> Posts { get; set; }

        public IEnumerable<PageInfoViewModel> Pages { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Tag, TagSearchViewModel>()
                .ForMember(x => x.Posts, opt =>
                    opt.MapFrom(x => x.PostTags.Select(y => y.Post)))
                .ForMember(x => x.Pages, opt =>
                    opt.MapFrom(x => x.PageTags.Select(y => y.Page)));
        }
    }
}
