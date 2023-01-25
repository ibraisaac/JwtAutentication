using JwtAutentication.Models;
using JwtAutentication.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAutentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region User

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var user = await _userRepository.GetUsers();
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar usuarios no banco de dados.");
            }
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUser(id);

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
        [Authorize]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {
                if (user is null)
                    return BadRequest();

                var createdUser = await _userRepository.AddUser(user);

                return CreatedAtAction(nameof(GetUser),
                    new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao inserir usuario" + ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (id != user.Id)
                    return BadRequest("Identificadores não coincidem.");

                var userToUpdate = await _userRepository.GetUser(id);
                if (userToUpdate.Id == 0)
                    return NotFound($"Usuário com id { id } não encontrado.");
                
                user.Id = userToUpdate.Id;
                return await _userRepository.UpdateUser(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Erro ao atualizar usuário" + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<string>> DeleteUser(int id)
        {
            try
            {
                var user = _userRepository.GetUser(id);

                if (user is null)
                    return NotFound($"Usuário com Identificador {id} não encontrado.");

                return await _userRepository.DeleteUser(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Erro ao deletar usuário" + ex.Message);
            }
        }

        #endregion
    }
}
