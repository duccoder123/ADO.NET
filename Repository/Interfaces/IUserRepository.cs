using Domain;
using System.Data;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserInfo>> GetAllUserAsync(string roleName, string methodName);
        Task<UserInfo> GetUserByIdAsync(string id);
        Task<Int32> AddAsync(UserInfo userInfo);
        Task<Int32> UpdateAsync(UserInfo userInfo);
        Task<Int32> DeleteAsync(string id);
        Task<Int32> ActivateUsersAsync(List<UserInfo> users);
        Task<Int32> DeactivateUsersAsync(List<UserInfo> users);
    }
}
