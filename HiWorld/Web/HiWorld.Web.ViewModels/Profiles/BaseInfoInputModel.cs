namespace HiWorld.Web.ViewModels.Profiles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using HiWorld.Data.Models.Enums;
    using HiWorld.Web.Infrastructure.Attributes;

    public class BaseInfoInputModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        [DateValidation(1900)]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string SelectedGender { get; set; } = nameof(Gender.Male);

        public IEnumerable<string> Genders { get; set; } = new[]
        {
                nameof(Gender.Male),
                nameof(Gender.Female),
                nameof(Gender.Other),
        };
    }
}
