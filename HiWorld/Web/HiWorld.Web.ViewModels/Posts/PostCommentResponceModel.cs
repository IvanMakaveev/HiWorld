namespace HiWorld.Web.ViewModels.Posts
{
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class PostCommentResponceModel : IMapFrom<Comment>
    {
        public int ProfileId { get; set; }

        public string ProfileFirstName { get; set; }

        public string ProfileLastName { get; set; }

        public string Text { get; set; }
    }
}
