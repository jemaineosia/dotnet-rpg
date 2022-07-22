using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CharacterController : ControllerBase
  {
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
      _characterService = characterService;
    }


    [HttpGet("GetAll")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
    {
      return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int Id)
    {
      return Ok(await _characterService.GetCharacterById(Id));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Delete(int id)
    {
      var response = await _characterService.DeleteCharacter(id);

      if (response.Data == null)
      {
        return NotFound(response);
      }

      return Ok(response);
    }

    [HttpPost("AddCharacter")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
    {
      return Ok(await _characterService.AddCharacter(character));
    }

    [HttpPut("UpdateCharacter")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updateCharacterDto)
    {
      var response = await _characterService.UpdateCharacter(updateCharacterDto);

      if (response.Data == null)
      {
        return NotFound(response);
      }

      return Ok(response);
    }
  }
}
