using HiWorld.Data.Models;
using HiWorld.Services.Data;
using HiWorld.Web.ViewModels.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiWorld.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostsService postsService;
        private readonly IWebHostEnvironment webHost;
        private readonly UserManager<ApplicationUser> userManager;

        public PostsController(UserManager<ApplicationUser> userManager, IPostsService postsService, IWebHostEnvironment webHost)
        {
            this.postsService = postsService;
            this.webHost = webHost;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Create()
        {
            var viewModel = new CreatePostInputModel();
            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var profileId = user.ProfileId;

            await this.postsService.CreateForProfile(profileId, input, $"{this.webHost.WebRootPath}/img/posts");

            return this.RedirectToAction("ById", "Profiles", new { id = profileId });
        }

        [Authorize]
        [HttpPost]
        public async Task Like(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var profileId = user.ProfileId;

            await this.postsService.LikePost(profileId, id);
        }
    }
}
