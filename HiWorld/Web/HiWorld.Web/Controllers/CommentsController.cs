namespace HiWorld.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CommentsController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly ICommentsService commentsService;

        public CommentsController(
            IProfilesService profilesService,
            ICommentsService commentsService)
        {
            this.profilesService = profilesService;
            this.commentsService = commentsService;
        }


        [Authorize]
        [HttpPost]
        public async Task Like(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.commentsService.LikeCommentAsync(id, profileId);
        }
    }
}
