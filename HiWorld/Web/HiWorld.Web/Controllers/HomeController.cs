namespace HiWorld.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels;
    using HiWorld.Web.ViewModels.Home;
    using HiWorld.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IProfilesService profilesService;
        private readonly IBrowseService browseService;
        private readonly IPostsService postsService;
        private readonly ICommentsService commentsService;

        public HomeController(
            IProfilesService profilesService,
            IBrowseService browseService,
            IPostsService postsService,
            ICommentsService commentsService)
        {
            this.profilesService = profilesService;
            this.browseService = browseService;
            this.postsService = postsService;
            this.commentsService = commentsService;
        }

        public IActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction(nameof(this.Browse));
            }

            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Browse(int id = 1)
        {
            id = id < 1 ? 1 : id;

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            var totalPostsCount = this.browseService.GetPostsCount(profileId);
            var newestPosts = this.browseService.GetNewestPosts<PostViewModel>(profileId, id, 10)
                .OrderByDescending(x => x.CreatedOn);

            var following = this.browseService.GetFollowing(profileId).OrderBy(x => x.Name).ToList();

            var viewModel = new BrowseViewModel()
            {
                Following = following,
                Posts = newestPosts,
                PageNumber = id,
                ItemsPerPage = 20,
                Items = totalPostsCount,
            };

            return this.View(viewModel);
        }
    }
}
