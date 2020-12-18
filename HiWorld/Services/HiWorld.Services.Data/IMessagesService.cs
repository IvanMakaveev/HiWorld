namespace HiWorld.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Groups;

    public interface IMessagesService
    {
        Task DeleteMessageAsync(int messageId);

        bool IsOwner(int messageId, int profileId);

        Task<int> AddMessageAsync(int groupId, int profileId, string text);

        T GetById<T>(int id);
    }
}
