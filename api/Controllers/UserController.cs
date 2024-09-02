using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Mappers;
using Microsoft.EntityFrameworkCore;
using api.Dtos.User;

namespace api.Controllers
{

    [Route("api/user")]
    [ApiController]

    public class UserController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var users = await _context.Users.Include(user => user.Pets).ToListAsync();
            var usersDto = users.Select(users => users.ToDto());
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById([FromRoute] int id){
            var user =  await _context.Users.Include(user 
            => user.Pets).FirstOrDefaultAsync(u => u.Id == id);
            if(user == null){
                return NotFound();
            }
            return Ok(user.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto){
            var userModel = userDto.ToUserFromCreateDto();
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = userModel.Id}, userModel.ToDto());
        }

        [HttpPut]
        [Route("{id}")]
         public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            userModel.Age = userDto.Age;
            userModel.FirstName = userDto.FirstName;
            userModel.LastName = userDto.LastName;

            await _context.SaveChangesAsync();

            return Ok(userModel.ToDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (userModel == null){
                return NotFound();
            }
            _context.Users.Remove(userModel);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        
    }
}