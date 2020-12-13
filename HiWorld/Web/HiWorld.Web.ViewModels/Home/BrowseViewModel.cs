namespace HiWorld.Web.ViewModels.Home
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HiWorld.Web.ViewModels.Posts;

    public class BrowseViewModel : PagingViewModel
    {
        public IEnumerable<ProfileFollowingViewModel> Following { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }
    }
}
