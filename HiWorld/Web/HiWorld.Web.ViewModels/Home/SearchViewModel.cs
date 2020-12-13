using HiWorld.Web.ViewModels.Posts;
using System.Collections.Generic;

namespace HiWorld.Web.ViewModels.Home
{
    public class SearchViewModel : PagingViewModel
    {
        public string SearchText { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

        public IEnumerable<ProfileSearchViewModel> Profiles { get; set; }

        public IEnumerable<PageSearchViewModel> Pages { get; set; }
    }
}
