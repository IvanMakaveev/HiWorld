namespace HiWorld.Web.ViewModels.Pages
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.Infrastructure.Attributes;
    using Microsoft.AspNetCore.Http;

    public class EditPageInputModel : IMapFrom<Page>, IHaveCustomMappings
    {
        public int Id { get; set; }

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

        [IgnoreMap]
        [ImageValidation]
        [Display(Name = "Profile Picture")]
        public IFormFile Image { get; set; }

        public IList<string> Tags { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Page, EditPageInputModel>()
                .ForMember(x => x.Tags, opt =>
                    opt.MapFrom(x => x.PageTags.Select(tag => tag.Tag.Name).ToList()));
        }
    }
}
