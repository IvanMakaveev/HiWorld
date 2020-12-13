namespace HiWorld.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels;
    using HiWorld.Web.ViewModels.Posts;
    using HiWorld.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly IPostsService postsService;
        private readonly ICountriesService countriesService;
        private readonly ICommentsService commentsService;
        private readonly IWebHostEnvironment webHost;

        public ProfilesController(
            IProfilesService profilesService,
            IPostsService postsService,
            ICountriesService countriesService,
            ICommentsService commentsService,
            IWebHostEnvironment webHost)
        {
            this.profilesService = profilesService;
            this.postsService = postsService;
            this.countriesService = countriesService;
            this.commentsService = commentsService;
            this.webHost = webHost;
        }

        public IActionResult ById(int id, int pageNumber = 1)
        {
            var viewModel = this.profilesService.GetById<DisplayProfileViewModel>(id);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            viewModel.Posts = this.postsService.GetProfilePosts<PostViewModel>(id, pageNumber);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            viewModel.IsOwner = profileId == id;

            if (!viewModel.IsOwner)
            {
                viewModel.IsFollowing = this.profilesService.IsFollowing(id, profileId);
                viewModel.IsFriend = this.profilesService.IsFriend(id, profileId);

                if (!viewModel.IsFriend)
                {
                    viewModel.IsPending = this.profilesService.IsPending(id, profileId);
                }
            }

            var totalPosts = this.postsService.GetProfileTotalPosts(id);

            viewModel.Paging = new PagingViewModel
            {
                Items = totalPosts,
                ItemsPerPage = 20,
                PageNumber = pageNumber,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Follow(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            await this.profilesService.FollowProfileAsync(id, profileId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        public IActionResult Edit()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var inputModel = this.profilesService.GetByUserId<EditProfileInputModel>(userId);
            inputModel.CountriesItems = this.countriesService.GetAllAsKvp();

            return this.View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.CountriesItems = this.countriesService.GetAllAsKvp();
                return this.View(input);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await this.profilesService.UpdateAsync(userId, input, $"{this.webHost.WebRootPath}/img/users");
            }
            catch (ArgumentException ae)
            {
                this.ModelState.AddModelError(string.Empty, ae.Message);
                input.CountriesItems = this.countriesService.GetAllAsKvp();
                return this.View(input);
            }

            return this.RedirectToAction(nameof(this.ById), new { input.Id });
        }
    }
}
