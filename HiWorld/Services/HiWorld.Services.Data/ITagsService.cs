using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public interface ITagsService
    {
        Task<int> GetId(string text);
    }
}
