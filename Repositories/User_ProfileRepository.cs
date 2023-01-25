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
    public class User_ProfileRepository : IUser_ProfileRepository
    {
        private IConfiguration _configuration;
        public User_ProfileRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<User_Profile> GetUser_Profile(int id)
        {
            User_Profile user = GetById(id);
            return await Task.FromResult(user);
        }

        public async Task<IEnumerable<User_Profile>> GetUsers_Profiles()
        {
            List<User_Profile> users_profiles = new List<User_Profile>();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("Select * From Usuario_Perfil", con);
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                User_Profile newUser_Profile = new User_Profile();
                newUser_Profile.Id = int.Parse(item["Id"].ToString());
                newUser_Profile.UserId = int.Parse(dt.Rows[0]["Usuario_Id"].ToString());
                newUser_Profile.ProfileId = int.Parse(dt.Rows[0]["Perfil_Id"].ToString());
            }

            return await Task.FromResult(users_profiles);
        }

        public async Task<User_Profile> AddUser_Profile(User_Profile user_profile)
        {
            string cmdInsert = "Insert Into Usuario Output Inserted.Id Values (@UserId, @ProfileId)";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            cmd.Parameters.Add("@UserId", SqlDbType.Int);
            cmd.Parameters["@UserId"].Value = user_profile.UserId;
            cmd.Parameters.Add("@ProfileId", SqlDbType.Int);
            cmd.Parameters["@ProfileId"].Value = user_profile.UserId;
            con.Open();
            var newId = await Task.FromResult(cmd.ExecuteScalar());
            con.Close();
            User_Profile newUser = GetById((Int32)newId);
            return newUser;
        }

        public async Task<string> DeleteUser_Profile(int id)
        {
            string cmdInsert = "Delete From Usuario_Perfil where Id=@id";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            con.Open();
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;

            var result = await Task.FromResult(cmd.ExecuteNonQuery());
            con.Close();
            if (result > 0)
                return $"Relacionamento deletado com sucesso";
            else
                return "Erro ao deletar relacionamento";
        }

        private User_Profile GetById(int id)
        {
            User_Profile user_profile = new User_Profile();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("Select * From Usuario_Perfil Where Id=@Id", con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                user_profile.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                user_profile.UserId = int.Parse(dt.Rows[0]["Usuario_Id"].ToString());
                user_profile.ProfileId = int.Parse(dt.Rows[0]["Perfil_Id"].ToString());
            }

            return user_profile;
        }
    }
}
