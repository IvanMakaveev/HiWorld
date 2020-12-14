namespace HiWorld.Web.ViewModels.Pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using HiWorld.Web.Infrastructure.Attributes;
    using Microsoft.AspNetCore.Http;

    public class CreatePageInputModel
    {
        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Description { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [ImageValidation]
        [Display(Name = "Page Picture")]
        public IFormFile Image { get; set; }

        public int ReturnId { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
