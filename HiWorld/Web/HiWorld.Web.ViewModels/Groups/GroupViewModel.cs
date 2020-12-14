using AutoMapper;
using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Groups
{
    public class GroupViewModel : IMapFrom<Group>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [IgnoreMap]
        public int ProfileId { get; set; }

        [IgnoreMap]
        public bool IsAdmin { get; set; }

        public IEnumerable<MessageViewModel> Messages { get; set; }
    }
}
