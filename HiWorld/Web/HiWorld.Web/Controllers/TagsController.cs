﻿namespace HiWorld.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Pages;
    using HiWorld.Web.ViewModels.Posts;
    using HiWorld.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class TagsController : Controller
    {
        private readonly ITagsService tagsService;
        private readonly IPostsService postsService;
        private readonly ICommentsService commentsService;
        private readonly IProfilesService profilesService;

        public TagsController(
            ITagsService tagsService,
            IPostsService postsService,
            ICommentsService commentsService,
            IProfilesService profilesService)
        {
            this.tagsService = tagsService;
            this.postsService = postsService;
            this.commentsService = commentsService;
            this.profilesService = profilesService;
        }

        public IActionResult ById(int id, int pageNumber = 1)
        {
            var name = this.tagsService.GetName(id);

            if (name == null)
            {
                return this.NotFound();
            }

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            var viewModel = new TagSearchViewModel()
            {
                Id = id,
                Name = name,
                Pages = this.tagsService.SearchPagesByTag<PageInfoViewModel>(id, pageNumber),
                Posts = this.tagsService.SearchPostsByTag<PostViewModel>(id, pageNumber),
                Items = Math.Max(this.tagsService.SearchPostsByTagCount(id), this.tagsService.SearchPagesByTagCount(id)),
                ItemsPerPage = 20,
                PageNumber = pageNumber,
            };

            return this.View(viewModel);
        }
    }
}
