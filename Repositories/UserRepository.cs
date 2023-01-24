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
	Left Join Usuario_Perfil On
		Usuario.Id = Usuario_Perfil.Usuario_Id
	Left Join Perfil On
		Usuario_Perfil.Perfil_Id = Perfil.Id
";

        public async Task<User> GetUser(int id)
        {
            User user = GetUserById(id);
            return await Task.FromResult(user);
        }

        private User GetUserById(int id)
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
            }

            return user;
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
                if (int.TryParse(dataRow["Perfil_Id"].ToString(), out _))
                {
                    Profile perfil = new Profile()
                    {
                        Id = int.Parse(dataRow["Perfil_Id"].ToString()),
                        Descricao = dataRow["Descricao"].ToString()
                    };
                    perfis.Add(perfil);
                }
            }
            user.Perfis = perfis;
            users.Add(user);
        }

        public async Task<User> AddUser(User user)
        {
            string cmdInsert = "Insert Into Usuario Output Inserted.Id Values (@nome, @senha)";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            cmd.Parameters.Add("@nome", SqlDbType.VarChar);
            cmd.Parameters["@nome"].Value = user.Username;
            cmd.Parameters.Add("@senha", SqlDbType.VarChar);
            cmd.Parameters["@senha"].Value = user.Password;
            con.Open();
            var newId = await Task.FromResult(cmd.ExecuteScalar());
            con.Close();
            User newUser = GetUserById((Int32)newId);
            return newUser;
        }

        public async Task<string> UpdateUser(User user)
        {
            string cmdInsert = "Update Usuario set nome=@nome, senha=@senha where Id=@id";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = user.Id;
            cmd.Parameters.Add("@nome", SqlDbType.VarChar);
            cmd.Parameters["@nome"].Value = user.Username;
            cmd.Parameters.Add("@senha", SqlDbType.VarChar);
            cmd.Parameters["@senha"].Value = user.Password;

            var result = await Task.FromResult(cmd.ExecuteNonQuery());
            if (result > 0)
                return $"Usuario {user.Username} atualizado com sucesso";
            else
                return "Erro ao atualizar usuario";
        }

        public async Task<string> DeleteUser(int id)
        {
            string cmdInsert = "Delete From Usuario where Id=@id";
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand(cmdInsert, con);
            cmd.Parameters.Add("@Id", SqlDbType.Int);
            cmd.Parameters["@Id"].Value = id;

            var result = await Task.FromResult(cmd.ExecuteNonQuery());
            if (result > 0)
                return $"Usuario deletado com sucesso";
            else
                return "Erro ao deletar usuario";
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
