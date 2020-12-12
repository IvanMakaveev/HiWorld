namespace HiWorld.Web.ViewModels.Profiles
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using AutoMapper;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.Infrastructure.Attributes;
    using Microsoft.AspNetCore.Http;

    public class EditProfileInputModel : BaseInfoInputModel, IMapFrom<HiWorld.Data.Models.Profile>
    {
        public int Id { get; set; }

        [StringLength(250, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string About { get; set; }

        [IgnoreMap]
        [Display(Name = "Profile Picture")]
        [ImageValidation]
        public IFormFile Image { get; set; }

        public IEnumerable<string> Genders { get; set; } = new[]
        {
                nameof(Data.Models.Enums.Gender.Male),
                nameof(Data.Models.Enums.Gender.Female),
                nameof(Data.Models.Enums.Gender.Other),
        };
    }
}
