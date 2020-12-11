namespace HiWorld.Web.ViewModels.Posts
{
    using System.ComponentModel.DataAnnotations;

    public class PostCommentInputModel
    {
        public int PostId { get; set; }

        [Required]
        [StringLength(100)]
        public string Text { get; set; }
    }
}
