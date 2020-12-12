namespace HiWorld.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Friends;
    using HiWorld.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Mvc;

    public class FriendsController : Controller
    {
        private readonly IFriendsService friendsService;
        private readonly IProfilesService profilesService;

        public FriendsController(
            IFriendsService friendsService,
            IProfilesService profilesService)
        {
            this.friendsService = friendsService;
            this.profilesService = profilesService;
        }

        public IActionResult FriendRequests()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            var viewModel = this.friendsService.GetFriendRequests<FriendRequestViewModel>(profileId);

            return this.View(viewModel);
        }

        public IActionResult FriendsList()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            var viewModel = this.friendsService.GetFriends(profileId);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            await this.friendsService.RemoveFriendAsync(id, profileId);

            return this.RedirectToAction("ById", "Profiles", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DenyFriend(int id)
        {
            await this.friendsService.DenyFriendshipAsync(id);

            return this.RedirectToAction(nameof(this.FriendRequests));
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriend(int id)
        {
            await this.friendsService.AcceptFriendshipAsync(id);

            return this.RedirectToAction(nameof(this.FriendRequests));
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            await this.friendsService.SendFriendRequestAsync(id, profileId);

            return this.RedirectToAction("ById", "Profiles", new { id });
        }
    }
}
