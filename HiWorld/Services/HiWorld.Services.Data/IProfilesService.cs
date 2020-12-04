namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Profiles;

    public interface IProfilesService
    {
        DisplayProfileViewModel GetById(int id);

        Task<int> Create(BaseInfoInputModel input);
    }
}
