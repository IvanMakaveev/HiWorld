using HiWorld.Web.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HiWorld.Web.ViewModels.Posts
{
    public class CreatePostInputModel
    {
        [StringLength(500, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Text { get; set; }

        [Display(Name = "Profile Picture")]
        [ImageValidation]
        public IFormFile Image { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
