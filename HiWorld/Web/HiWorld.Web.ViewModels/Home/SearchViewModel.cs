namespace HiWorld.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using HiWorld.Web.ViewModels.Posts;

    public class SearchViewModel : PagingViewModel
    {
        public string SearchText { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

        public IEnumerable<ProfileSearchViewModel> Profiles { get; set; }

        public IEnumerable<PageSearchViewModel> Pages { get; set; }
    }
}
