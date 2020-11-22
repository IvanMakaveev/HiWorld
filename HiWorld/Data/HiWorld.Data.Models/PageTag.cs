namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class PageTag : BaseModel<int>
    {
        public int PageId { get; set; }

        public virtual Page Page { get; set; }

        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
