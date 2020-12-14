using HiWorld.Web.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HiWorld.Web.ViewModels.Groups
{
    public class CreateGroupInputModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Description { get; set; }

        [ImageValidation]
        [Display(Name = "Group Picture")]
        public IFormFile Image { get; set; }
    }
}
