using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public interface IUserService
    {
        Task<List<UserInfo>> GetAllUsersAsync(string roleName, string methodName);
        Task<UserInfo> GetUserByIdAsync(string id);
        Task<Int32> AddAsync(UserInfo userInfo);
        Task<Int32> UpdateAsync(UserInfo userInfo);
        Task<Int32> DeleteAsync(string id);
        Task<Int32> ActivateUsersAsync(List<UserInfo> users);
        Task<Int32> DeActivateUsersAsync(List<UserInfo> users);
    }
}
