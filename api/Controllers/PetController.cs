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
        public async Task<IActionResult> Create([FromBody] CreatePetRequestDto petDto){
            var petModel = petDto.ToPetFromCreateDto();
            await _context.Pets.AddAsync(petModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = petModel.Id}, petModel.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePetRequestDto petDto){
            var petModel = await _context.Pets.FirstOrDefaultAsync(u => u.Id == id);
            if (petModel == null){
                return NotFound();
            }
            petModel.Name = petDto.Name;
            petModel.Animal = petDto.Animal;

            _context.SaveChanges();

            return Ok(petModel.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id){
            var petModel = _context.Pets.FirstOrDefault(u => u.Id == id);
            if (petModel == null){
                return NotFound();
            }
            _context.Pets.Remove(petModel);

            _context.SaveChanges();

            return NoContent();
        }


        
        
    }
}