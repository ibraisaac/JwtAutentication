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
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration config)
        {
            _configuration = config;
        }

        string cmdGetUser = @"
Select Usuario.Id, Usuario.Nome,  Usuario.Senha, 
       Perfil.Descricao, Perfil.Id As Perfil_Id
From Usuario 
	Inner Join Usuario_Perfil On
		Usuario.Id = Usuario_Perfil.Usuario_Id
	Inner Join Perfil On
		Usuario_Perfil.Perfil_Id = Perfil.Id
";

        public async Task<User> GetUser(int id)
        {
            User user = new User();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdGetUser + " Where Usuario.Id=@Id", con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                user.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                user.Username = dt.Rows[0]["Nome"].ToString();
                user.Password = "****";

                FillPerfis(new List<User>(), dt, user);

                return await Task.FromResult(user);
            }
            else
                return await Task.FromResult(user);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            List<User> users = new List<User>();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdGetUser, con);
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                User user = new User();
                user.Id = int.Parse(item["Id"].ToString());
                user.Username = item["Nome"].ToString(); ;
                user.Password = "****";

                if (users.Count(c => c.Id == user.Id) == 0)
                {
                    FillPerfis(users, dt, user);
                }
            }

            return await Task.FromResult(users);
        }

        private static void FillPerfis(List<User> users, DataTable dt, User user)
        {
            List<Profile> perfis = new List<Profile>();
            var perfisFilter = dt.AsEnumerable().Where(w => w.Field<int>("Id") == user.Id).ToList();

            foreach (var dataRow in perfisFilter)
            {
                Profile perfil = new Profile()
                {
                    Id = int.Parse(dataRow["Perfil_Id"].ToString()),
                    Descricao = dataRow["Descricao"].ToString()
                };
                perfis.Add(perfil);
            }
            user.Perfis = perfis;
            users.Add(user);
        }

        public User UserAutentication(string name, string password)
        {
            User user = new User();
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("Select * From Usuario Where nome=@name and senha=@password", con);
            cmd.Parameters.Add("@name", SqlDbType.VarChar);
            cmd.Parameters["@name"].Value = name;
            cmd.Parameters.Add("@password", SqlDbType.VarChar);
            cmd.Parameters["@password"].Value = password;
            using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                user.Id = int.Parse(dt.Rows[0]["Id"].ToString());
                user.Username = dt.Rows[0]["Nome"].ToString();
                user.Password = dt.Rows[0]["Senha"].ToString();
                return user;
            }
            else
                return new User();
        }
    }
}
