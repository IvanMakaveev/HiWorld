namespace HiWorld.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Administration;
    using Microsoft.AspNetCore.Mvc;

    public class PostsController : AdministrationController
    {
        private readonly IPostsService postsService;

        public PostsController(IPostsService postsService)
        {
            this.postsService = postsService;
        }

        public IActionResult List()
        {
            var viewModel = this.postsService.GetAllPosts<PostInfoViewModel>();

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.postsService.DeleteAsync(id);

            return this.RedirectToAction(nameof(this.List));
        }
    }
}
