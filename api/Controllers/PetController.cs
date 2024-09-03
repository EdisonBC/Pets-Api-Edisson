using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Dtos.Pet;
using api.Models;

namespace api.Controllers
{
    [Route("api/pet")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public PetController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var pets = await _context.Pets.ToListAsync();
            var petsDto = pets.Select(pets => pets.ToDto());
            return Ok(petsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById([FromRoute] int id){
            var pet =  await _context.Pets.FirstOrDefaultAsync(u => u.Id == id);
            if(pet == null){
                return NotFound();
            }
            return Ok(pet.ToDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreatePetRequestDto petDto){
            var petModel = petDto.ToPetFromCreateDto();
            _context.Pets.Add(petModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(getById), new { id = petModel.Id}, petModel.ToDto());
        }


        
        
    }
}