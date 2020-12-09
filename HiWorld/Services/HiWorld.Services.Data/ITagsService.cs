namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    public interface ITagsService
    {
        Task<int> GetId(string text);
    }
}
