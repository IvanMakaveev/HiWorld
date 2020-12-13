namespace HiWorld.Web.ViewModels.Tags
{
    using System.Collections.Generic;
    using System.Linq;

    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Pages;
    using HiWorld.Web.ViewModels.Posts;

    public class TagSearchViewModel : PagingViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

        public IEnumerable<PageInfoViewModel> Pages { get; set; }
    }
}
