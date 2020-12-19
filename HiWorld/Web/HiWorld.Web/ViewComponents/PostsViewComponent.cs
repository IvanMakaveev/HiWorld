namespace HiWorld.Web.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Mvc;

    public class PostsViewComponent : ViewComponent
    {
        private readonly IPostsService postsService;
        private readonly ICommentsService commentsService;
        private readonly IProfilesService profilesService;

        public PostsViewComponent(
            IPostsService postsService,
            ICommentsService commentsService,
            IProfilesService profilesService)
        {
            this.postsService = postsService;
            this.commentsService = commentsService;
            this.profilesService = profilesService;
        }

        public IViewComponentResult Invoke(IEnumerable<PostViewModel> model)
        {
            var userId = this.UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            var viewModel = model.ToList();

            foreach (var post in viewModel)
            {
                post.IsOwner = this.postsService.IsOwner(post.Id, post.IsProfilePost, profileId);
                post.IsLiked = this.postsService.IsLiked(post.Id, profileId);

                foreach (var comment in post.Comments)
                {
                    comment.IsLiked = this.commentsService.IsLiked(comment.Id, profileId);
                }

                post.Comments.OrderByDescending(x => x.CreatedOn);
            }

            return this.View(viewModel);
        }
    }
}
