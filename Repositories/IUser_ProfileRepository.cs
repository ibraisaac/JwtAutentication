using JwtAutentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Repositories
{
    public interface IUser_ProfileRepository
    {
        Task<IEnumerable<User_Profile>> GetUsers_Profiles();
        Task<User_Profile> GetUser_Profile(int id);
        Task<User_Profile> AddUser_Profile(User_Profile user_profile);
        Task<string> DeleteUser_Profile(int id);
    }
}
