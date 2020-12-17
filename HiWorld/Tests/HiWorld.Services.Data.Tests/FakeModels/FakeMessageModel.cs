namespace HiWorld.Services.Data.Tests.FakeModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class FakeMessageModel : IMapFrom<Message>
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }
}
