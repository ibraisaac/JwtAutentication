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
    public class ProfileController : ControllerBase
    {
        private IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        #region Profile

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            try
            {
                var profile = await _profileRepository.GetProfiles();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao buscar perfis no banco de dados." + ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            try
            {
                var profile = await _profileRepository.GetProfile(id);

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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Profile>> CreateProfile([FromBody] Profile profile)
        {
            try
            {
                if (profile is null)
                    return BadRequest();

                var createdProfile = await _profileRepository.AddProfile(profile);

                return CreatedAtAction(nameof(GetProfile),
                    new { id = createdProfile.Id }, createdProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao inserir usuario" + ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Profile>> UpdateProfile(int id, [FromBody] Profile profile)
        {
            try
            {
                if (id != profile.Id)
                    return BadRequest("Identificadores não coincidem.");

                var ProfileToUpdate = await _profileRepository.GetProfile(id);
                if (ProfileToUpdate.Id == 0)
                    return NotFound($"Usuário com id { id } não encontrado.");

                profile.Id = ProfileToUpdate.Id;
                return await _profileRepository.UpdateProfile(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Erro ao atualizar usuário" + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<string>> DeleteProfile(int id)
        {
            try
            {
                var profile = _profileRepository.GetProfile(id);

                if (profile is null)
                    return NotFound($"Usuário com Identificador {id} não encontrado.");

                return await _profileRepository.DeleteProfile(id);
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
