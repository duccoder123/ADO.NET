using Domain;
using Repositories.Interfaces;
using Services.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository; 
        }
        public async Task<int> ActivateUsersAsync(List<UserInfo> users)
        {
            var result = await _userRepository.ActivateUsersAsync(users);
            return result;
        }

        public async Task<int> AddAsync(UserInfo userInfo)
        {
           var result = await _userRepository.AddAsync(userInfo);
            return result;
        }

        public async Task<int> DeActivateUsersAsync(List<UserInfo> users)
        {
            var result = await _userRepository.DeactivateUsersAsync(users);
            return result;
        }

        public async Task<int> DeleteAsync(string id)
        {
           var result = await _userRepository.DeleteAsync(id);
            return result;
        }

        public async Task<List<UserInfo>> GetAllUsersAsync(string roleName, string methodName)
        {
            var result = await _userRepository.GetAllUserAsync(roleName,methodName);
            return result;

        }

        public async Task<UserInfo> GetUserByIdAsync(string id)
        {
            var result = await _userRepository.GetUserByIdAsync(id);
            return result;
        }

        public async Task<int> UpdateAsync(UserInfo userInfo)
        {
            var result = await _userRepository.UpdateAsync(userInfo);
            return result;
        }
    }
}
