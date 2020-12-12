﻿namespace HiWorld.Web.Controllers
{
    using System;
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
                viewModel.IsFollowing = this.pagesService.IsFollowing(profileId, id);
            }

            viewModel.Posts.ForEach(x => x.IsLiked = this.postsService.IsLiked(x.Id, profileId));
            viewModel.Posts.ForEach(x => x.Comments.ForEach(y => y.IsLiked = this.commentsService.IsLiked(y.Id, profileId)));
            viewModel.Posts.ForEach(x => x.Comments.OrderByDescending(x => x.CreatedOn));

            return this.View(viewModel);
        }

        public IActionResult Create()
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = new CreatePageInputModel();
            viewModel.ReturnId = profileId;

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePageInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var pageId = await this.pagesService.CreateAsync(profileId, input, $"{this.webHost.WebRootPath}/img/pages");

            return this.RedirectToAction(nameof(this.ById), new { id = pageId });
        }

        public IActionResult MyPages()
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = this.pagesService.GetForId<PageInfoViewModel>(profileId);

            return this.View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = this.pagesService.IsOwner(userId, id);

            if (isOwner)
            {
                var inputModel = this.pagesService.GetById<EditPageInputModel>(id);

                return this.View(inputModel);
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPageInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isOwner = this.pagesService.IsOwner(userId, input.Id);

            if (isOwner)
            {
                await this.pagesService.UpdateAsync(input, $"{this.webHost.WebRootPath}/img/pages");

                return this.RedirectToAction(nameof(this.ById), new { input.Id });
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Follow(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            await this.pagesService.FollowPageAsync(profileId, id);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }
    }
}
