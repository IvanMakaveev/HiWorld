namespace HiWorld.Web.ViewModels.Posts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using HiWorld.Web.Infrastructure.Attributes;
    using Microsoft.AspNetCore.Http;

    public class CreatePostInputModel
    {
        public int ReturnId { get; set; }

        [Required]
        [StringLength(750, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Text { get; set; }

        [Display(Name = "Profile Picture")]
        [ImageValidation]
        public IFormFile Image { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public bool IsProfile { get; set; }
    }
}
