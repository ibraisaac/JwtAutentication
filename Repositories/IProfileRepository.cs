using JwtAutentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Repositories
{
    public interface IProfileRepository
    {
        Task<IEnumerable<Profile>> GetProfiles();
        Task<Profile> GetProfile(int id);
        Task<User> AddUser(User user);
        Task<string> UpdateUser(User user);
        Task<string> DeleteUser(int id);

    }
}
