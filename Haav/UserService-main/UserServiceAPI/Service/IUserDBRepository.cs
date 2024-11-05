using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model;

namespace Service;
public interface IUserDBRepository
{
    Task<User> CreateUserAsync(User user);
    Task<User> GetUserByIdAsync(string id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> UpdateUserAsync(string id, User updatedUser);
    Task<bool> DeleteUserAsync(string id);
    Task<User?> GetUserByEmailAsync(string Email);

}
