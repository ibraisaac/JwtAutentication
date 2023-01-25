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

        public async Task<Profile> AddProfile(Profile profile)
        {
            string cmdInsert = "Insert Into Perfil Output Inserted.Id Values (@descricao)";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            cmd.Parameters.Add("@descricao", SqlDbType.VarChar);
            cmd.Parameters["@descricao"].Value = profile.Descricao;
            con.Open();
            var newId = await Task.FromResult(cmd.ExecuteScalar());
            con.Close();
            Profile newProfile = GetProfileById((Int32)newId);
            return profile;
        }

        public async Task<string> DeleteProfile(int id)
        {
            string cmdInsert = "Delete From Perfil where Id=@id";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            con.Open();
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;

            var result = await Task.FromResult(cmd.ExecuteNonQuery());
            con.Close();
            if (result > 0)
                return $"Perfil deletado com sucesso";
            else
                return "Erro ao deletar perfil";
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

        public async Task<Profile> UpdateProfile(Profile profile)
        {
            string cmdInsert = "Update Perfil set Descricao=@descricao where Id=@id";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = profile.Id;
            cmd.Parameters.Add("@descricao", SqlDbType.VarChar);
            cmd.Parameters["@descricao"].Value = profile.Descricao;
            con.Open();
            var result = await Task.FromResult(cmd.ExecuteNonQuery());
            con.Close();

            return profile;
        }

        private Profile GetProfileById(int id)
        {
            Profile profile = new Profile();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("Select * From Perfil Where Id = @Id", con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                profile.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                profile.Descricao = dt.Rows[0]["Descricao"].ToString();
            }

            return profile;
        }
    }
}
