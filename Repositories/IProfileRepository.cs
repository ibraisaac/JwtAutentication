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
        Task<Profile> AddProfile(Profile profile);
        Task<Profile> UpdateProfile(Profile profile);
        Task<string> DeleteProfile(int id);

    }
}
