using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task<ActionResult<ServiceResponse<List<Character>>>> Get()
    {
      return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<ServiceResponse<Character>>> GetSingle(int Id)
    {
      return Ok(await _characterService.GetCharacterById(Id));
    }

    [HttpPost("AddCharacter")]
    public async Task<ActionResult<ServiceResponse<Character>>> AddCharacter(Character character)
    {
      return Ok(await _characterService.AddCharacter(character));
    }
  }
}
