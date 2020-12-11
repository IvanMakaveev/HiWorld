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

        [Authorize]
        public IActionResult ById(int id)
        {
            var viewModel = this.profilesService.GetById<DisplayProfileViewModel>(id);

            if (viewModel == null)
            {
                return this.NotFound();
            }

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

            viewModel.Posts.ForEach(x => x.IsLiked = this.postsService.IsLiked(x.Id, profileId));
            viewModel.Posts.ForEach(x => x.Comments.ForEach(y => y.IsLiked = this.commentsService.IsLiked(y.Id, profileId)));

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFriend(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.profilesService.SendFriendRequestAsync(id, userId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Follow(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.profilesService.FollowProfileAsync(id, userId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await this.profilesService.RemoveFriendAsync(id, userId);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DenyFriend(int id)
        {
            await this.profilesService.DenyFriendshipAsync(id);

            return this.RedirectToAction(nameof(this.FriendRequests));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AcceptFriend(int id)
        {
            await this.profilesService.AcceptFriendshipAsync(id);

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
