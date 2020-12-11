namespace HiWorld.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Pages;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class PagesController : Controller
    {
        private readonly IPagesService pagesService;
        private readonly IWebHostEnvironment webHost;
        private readonly ICommentsService commentsService;
        private readonly IPostsService postsService;
        private readonly IProfilesService profilesService;

        public PagesController(
            IPagesService pagesService,
            IWebHostEnvironment webHost,
            ICommentsService commentsService,
            IPostsService postsService,
            IProfilesService profilesService)
        {
            this.pagesService = pagesService;
            this.webHost = webHost;
            this.commentsService = commentsService;
            this.postsService = postsService;
            this.profilesService = profilesService;
        }

        public IActionResult ById(int id)
        {
            var viewModel = this.pagesService.GetById<DisplayPageViewModel>(id);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            viewModel.IsOwner = this.pagesService.IsOwner(profileId, id);

            if (!viewModel.IsOwner)
            {
                viewModel.IsFollowing = this.pagesService.IsFollowing(id, profileId);
            }

            viewModel.Posts.ForEach(x => x.IsLiked = this.postsService.IsLiked(x.Id, profileId));
            viewModel.Posts.ForEach(x => x.Comments.ForEach(y => y.IsLiked = this.commentsService.IsLiked(y.Id, profileId)));
            viewModel.Posts.ForEach(x => x.Comments.OrderByDescending(x => x.CreatedOn));

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult Create()
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = new CreatePageInputModel();
            viewModel.ReturnId = profileId;

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePageInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.pagesService.Create(profileId, input, $"{this.webHost.WebRootPath}/img/pages");

            return this.RedirectToAction(nameof(this.ById), new { id = input.ReturnId });
        }

        public IActionResult MyPages()
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = this.pagesService.GetForId<PageInfoViewModel>(profileId);

            return this.View(viewModel);
        }
    }
}
