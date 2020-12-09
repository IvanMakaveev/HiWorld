namespace HiWorld.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class ProfilesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly IPostsService postsService;
        private readonly ICountriesService countriesService;
        private readonly IWebHostEnvironment webHost;

        public ProfilesController(
            IProfilesService profilesService,
            IPostsService postsService,
            ICountriesService countriesService,
            IWebHostEnvironment webHost)
        {
            this.profilesService = profilesService;
            this.postsService = postsService;
            this.countriesService = countriesService;
            this.webHost = webHost;
        }

        [Authorize]
        public IActionResult ById(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = this.profilesService.GetById<DisplayProfileViewModel>(id);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.IsOwner = this.profilesService.IsOwner(userId, id);
            if (!viewModel.IsOwner)
            {
                viewModel.IsFollowing = this.profilesService.IsFollowing(id, userId);
                viewModel.IsFriend = this.profilesService.IsFriend(id, userId);

                if (!viewModel.IsFriend)
                {
                    viewModel.IsPending = this.profilesService.IsPending(id, userId);
                }
            }

            viewModel.Posts.ForEach(x => x.IsLiked = this.postsService.IsLiked(x.Id, userId));

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFriend(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.profilesService.SendFriendRequest(id, userId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Follow(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.profilesService.FollowProfile(id, userId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.profilesService.RemoveFriend(id, userId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DenyFriend(int id)
        {
            await this.profilesService.DenyFriendship(id);

            return this.RedirectToAction(nameof(this.FriendRequests));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AcceptFriend(int id)
        {
            await this.profilesService.AcceptFriendship(id);

            return this.RedirectToAction(nameof(this.FriendRequests));
        }

        [Authorize]
        public IActionResult Edit()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var inputModel = this.profilesService.GetByUserId<EditProfileInputModel>(userId);
            inputModel.CountriesItems = this.countriesService.GetAllAsKvp();

            return this.View(inputModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditProfileInputModel input)
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

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        public IActionResult FriendRequests()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = this.profilesService.GetFriendRequests<FriendRequestViewModel>(userId);

            return this.View(viewModel);
        }
    }
}
