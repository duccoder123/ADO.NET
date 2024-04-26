using Domain;
using Repositories.Interfaces;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implement
{
    public class UserRepository : RepositoryBase<UserInfo>, IUserRepository
    {
       public async Task<Int32> ActivateUsersAsync(List<UserInfo> users)
        {
            var res = await base.ActivateUsersAsync(users);
            return res;
        }
        public async Task<int> DeactivateUsersAsync(List<UserInfo> users)
        {
            var res = await base.DeActivateUsersAsync(users);
            return res;
        }

        public async Task<Int32> DeleteAsync(string id)
        {
            var res = await base.DeleteAysnc(id);
            return res;
        }

        public async Task<List<UserInfo>> GetAllUserAsync(string roleName, string methodName)
        {
            var res = await base.GetAllUsersAsync(roleName,methodName);
            return res;
        }

        public async Task<Int32> AddAsync(UserInfo userInfo)
        {
            
           var res = await base.AddAsync(userInfo);
            
            return res;
        }

        public async Task<Int32> UpdateAsync(UserInfo userInfo)
        {
           var res = await base.UpdateAsync(userInfo);
            return res;
        }
    }
}
