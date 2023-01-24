using JwtAutentication.Models;
using JwtAutentication.Repositories;
using JwtAutentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Controllers
{
    [Route("v1/cadastro")]
    public class HomeController : ControllerBase
    {
        #region Construtor

        private IUserRepository userRepository;
        private IProfileRepository profileRepository;
        public HomeController(IUserRepository repoUser, IProfileRepository repoProfile)
        {
            userRepository = repoUser;
            profileRepository = repoProfile;
        }

        #endregion

        #region Autentication

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate(string name, string password)
        {
            var user = userRepository.UserAutentication(name, password);

            if (user == null)
                return NotFound(new { message = "Usuario ou senha invalidos" });

            var token = TokenService.GenerateToken();
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => $"Autenticado";

        #endregion

        #region User

        [HttpGet]
        [Route("users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var user = await userRepository.GetUsers();
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar usuarios no banco de dados.");
            }
        }

        [HttpGet]
        [Route("user{id:int}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await userRepository.GetUser(id);

                if (user is null)
                    return NotFound(new { message = "Usuario nao encontrado" });

                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar usuarios no banco de dados." + ex.Message);
            }
        }

        [HttpPost]
        [Route("user")]
        [Authorize]
        public async Task<ActionResult<User>> CreateUser([FromBody]User user)
        {
            try
            {
                if (user is null)
                    return BadRequest();

                var createdUser = await userRepository.AddUser(user);

                return CreatedAtAction(nameof(GetUser),
                    new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao inserir usuario" + ex.Message);
            }
        }

        #endregion

        #region Profile

        [HttpGet]
        [Route("profiles")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            try
            {
                var profile = await profileRepository.GetProfiles();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar perfis no banco de dados." + ex.Message);
            }
        }

        [HttpGet]
        [Route("profile{id:int}")]
        [Authorize]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            try
            {
                var profile = await profileRepository.GetProfile(id);

                if (profile is null)
                    return NotFound(new { message = "Perfil nao encontrado" });

                return profile;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar perfil no banco de dados." + ex.Message);
            }
        }

        #endregion
    }
}
