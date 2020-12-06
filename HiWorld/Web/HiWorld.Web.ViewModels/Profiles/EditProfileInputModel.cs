namespace HiWorld.Web.ViewModels.Profiles
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.Infrastructure.Attributes;
    using Microsoft.AspNetCore.Http;

    public class EditProfileInputModel : BaseInfoInputModel, IMapFrom<HiWorld.Data.Models.Profile>
    {
        public int Id { get; set; }

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
