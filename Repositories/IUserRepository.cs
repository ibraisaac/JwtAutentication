using JwtAutentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<User> AddUser(User user);
        Task<string> UpdateUser(User user);
        Task<string> DeleteUser(int id);
        User UserAutentication(string name, string password);
    }
}
