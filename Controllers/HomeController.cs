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
    [Route("v1/home")]
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

    }
}
