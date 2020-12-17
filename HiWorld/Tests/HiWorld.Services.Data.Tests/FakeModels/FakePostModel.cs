using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Services.Data.Tests.FakeModels
{
    public class FakePostModel : IMapFrom<Post>
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }
}
