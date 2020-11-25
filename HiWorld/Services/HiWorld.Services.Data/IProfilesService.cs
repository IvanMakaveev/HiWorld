namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Profiles;

    public interface IProfilesService
    {
        Task<int> Create(BaseInfoInputModel input);
    }
}
