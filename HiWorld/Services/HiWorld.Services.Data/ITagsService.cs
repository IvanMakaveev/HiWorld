namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    public interface ITagsService
    {
        Task<int> GetIdAsync(string text);
    }
}
