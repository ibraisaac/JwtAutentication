using JwtAutentication.Models;
using JwtAutentication.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_ProfileController : Controller
    {
        private IUser_ProfileRepository _userProfileRepository;
        public User_ProfileController(IUser_ProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User_Profile>>> GetUser_Profiles()
        {
            try
            {
                var User_Profile = await _userProfileRepository.GetUsers_Profiles();
                return Ok(User_Profile);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar relacionamentos usuario e perfil no banco de dados.");
            }
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<User_Profile>> GetUser_Profile(int id)
        {
            try
            {
                var User_Profile = await _userProfileRepository.GetUser_Profile(id);

                if (User_Profile is null)
                    return NotFound(new { message = "Relacionamento nao encontrado." });

                return User_Profile;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar relacionamento usuario e perfil no banco de dados. " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<User_Profile>> CreateUser_Profile([FromBody] User_Profile User_Profile)
        {
            try
            {
                if (User_Profile is null)
                    return BadRequest();

                var createdUser_Profile = await _userProfileRepository.AddUser_Profile(User_Profile);

                return CreatedAtAction(nameof(GetUser_Profile),
                    new { id = createdUser_Profile.Id }, createdUser_Profile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao inserir relacionamento. " + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<string>> DeleteUser_Profile(int id)
        {
            try
            {
                var User_Profile = _userProfileRepository.GetUser_Profile(id);

                if (User_Profile is null)
                    return NotFound($"Relaciomento com Identificador {id} não encontrado.");

                return await _userProfileRepository.DeleteUser_Profile(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Erro ao deletar relaciomento. " + ex.Message);
            }
        }
    }
}
