﻿namespace HiWorld.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class PostsController : Controller
    {
        private readonly IPostsService postsService;
        private readonly IProfilesService profilesService;
        private readonly IWebHostEnvironment webHost;

        public PostsController(IPostsService postsService, IProfilesService profilesService,  IWebHostEnvironment webHost)
        {
            this.postsService = postsService;
            this.profilesService = profilesService;
            this.webHost = webHost;
        }

        [Authorize]
        public IActionResult Create()
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = new CreatePostInputModel();
            viewModel.ReturnId = profileId;

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

            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.postsService.CreateForProfile(profileId, input, $"{this.webHost.WebRootPath}/img/posts");

            return this.RedirectToAction("ById", "Profiles", new { id = profileId });
        }

        [Authorize]
        [HttpPost]
        public async Task Like(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.postsService.LikePost(profileId, id);
        }

        [Authorize]
        [HttpPost]
        public async Task DeleteFromProfile(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.postsService.DeletePostFromProfile(profileId, id);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostCommentResponceModel>> AddComment(PostCommentInputModel input)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = await this.postsService.AddComment<PostCommentResponceModel>(profileId, input);
            return viewModel;
        }
    }
}