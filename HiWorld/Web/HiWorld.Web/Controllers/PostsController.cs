namespace HiWorld.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostsService postsService;
        private readonly IProfilesService profilesService;
        private readonly IPagesService pagesService;
        private readonly IWebHostEnvironment webHost;

        public PostsController(
            IPostsService postsService,
            IProfilesService profilesService,
            IPagesService pagesService,
            IWebHostEnvironment webHost)
        {
            this.postsService = postsService;
            this.profilesService = profilesService;
            this.pagesService = pagesService;
            this.webHost = webHost;
        }

        public IActionResult CreateForProfile()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            var viewModel = new CreatePostInputModel();
            viewModel.ReturnId = profileId;
            viewModel.IsProfile = true;

            return this.View("Create", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateForProfile(CreatePostInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            await this.postsService.CreateForProfileAsync(profileId, input, $"{this.webHost.WebRootPath}/img/posts");

            return this.RedirectToAction("ById", "Profiles", new { id = profileId });
        }

        public IActionResult CreateForPage(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = this.pagesService.IsOwner(userId, id);

            if (isOwner)
            {
                var viewModel = new CreatePostInputModel();
                viewModel.ReturnId = id;
                viewModel.IsProfile = false;

                return this.View("Create", viewModel);
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateForPage(CreatePostInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = this.pagesService.IsOwner(userId, input.ReturnId);

            if (isOwner)
            {
                await this.postsService.CreateForPageAsync(input.ReturnId, input, $"{this.webHost.WebRootPath}/img/posts");

                return this.RedirectToAction("ById", "Pages", new { id = input.ReturnId });
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task Like(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            await this.postsService.LikePostAsync(profileId, id);
        }

        [HttpPost]
        public async Task DeleteFromProfile(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.postsService.DeletePostFromProfileAsync(profileId, id);
        }

        [HttpPost]
        public async Task DeleteFromPage(int id, int pageId)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);
            var isOwner = this.pagesService.IsOwner(profileId, pageId);

            if (isOwner)
            {
                await this.postsService.DeletePostFromPageAsync(profileId, id);
            }
        }
    }
}
