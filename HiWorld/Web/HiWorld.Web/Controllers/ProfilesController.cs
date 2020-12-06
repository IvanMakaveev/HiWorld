using HiWorld.Services.Data;
using HiWorld.Web.ViewModels.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiWorld.Web.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly ICountriesService countriesService;
        private readonly IWebHostEnvironment webHost;

        public ProfilesController(IProfilesService profilesService, ICountriesService countriesService, IWebHostEnvironment webHost)
        {
            this.profilesService = profilesService;
            this.countriesService = countriesService;
            this.webHost = webHost;
        }

        [Authorize]
        public IActionResult ById(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = this.profilesService.GetByIdForAccessor(id, userId);

            if (viewModel == null)
            {
                return this.NotFound();
            }

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
        public IActionResult Edit(int id)
        {
            var inputModel = this.profilesService.GetById<EditProfileInputModel>(id);
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

            try
            {
                await this.profilesService.UpdateAsync(id, input, $"{this.webHost.WebRootPath}/img/users");
            }
            catch (ArgumentException ae)
            {
                this.ModelState.AddModelError(string.Empty, ae.Message);
                input.CountriesItems = this.countriesService.GetAllAsKvp();
                return this.View(input);
            }

            return this.RedirectToAction(nameof(this.ById), new { id });
        }
    }
}
