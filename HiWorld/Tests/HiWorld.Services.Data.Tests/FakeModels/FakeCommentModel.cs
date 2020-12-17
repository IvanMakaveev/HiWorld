using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Services.Data.Tests.FakeModels
{
    public class FakeCommentModel : IMapFrom<Comment>
    {
        public int Id { get; set; }
    }
}
