namespace HiWorld.Services.Data.Tests.FakeModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class FakeCommentModel : IMapFrom<Comment>
    {
        public int Id { get; set; }
    }
}
