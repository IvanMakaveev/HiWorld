namespace HiWorld.Web.ViewModels.Groups
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.Infrastructure.Attributes;
    using Microsoft.AspNetCore.Http;

    public class EditGroupInputModel : IMapFrom<Group>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Description { get; set; }

        [IgnoreMap]
        [ImageValidation]
        [Display(Name = "Group Picture")]
        public IFormFile Image { get; set; }
    }
}
