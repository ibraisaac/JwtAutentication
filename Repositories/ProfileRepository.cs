using JwtAutentication.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IConfiguration _configuration;

        public ProfileRepository(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<Profile> GetProfile(int id)
        {
            Profile profile = new Profile();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(@"Select * From Perfil Where Id=@Id", con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                profile.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                profile.Descricao = dt.Rows[0]["Descricao"].ToString();
                return await Task.FromResult(profile);
            }
            else
                return await Task.FromResult(profile);
        }

        public async Task<IEnumerable<Profile>> GetProfiles()
        {
            List<Profile> profiles = new List<Profile>();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("Select * From Perfil", con);
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                Profile profile = new Profile();
                profile.Id = int.Parse(item["Id"].ToString());
                profile.Descricao = item["Descricao"].ToString();
                profiles.Add(profile);
            }

            return await Task.FromResult(profiles);
        }
    }
}
