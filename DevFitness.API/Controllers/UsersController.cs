using DevFitness.API.Core.Entities;
using DevFitness.API.Models.InputModels;
using DevFitness.API.Models.ViewModels;
using DevFitness.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevFitness.API.Controllers
{
    // api/users -> [controller] pega o nome da controller e tira a palavra "controller"
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        //private readonly DevFitnessDBContext dbContexto = new DevFitnessDBContext();

        private readonly DevFitnessDBContext _dbContext;
        public UsersController(DevFitnessDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == id);

            if(user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel(user.Id, user.FullName, user.Height, user.Weight, user.BirthDate);
            return Ok(userViewModel);
        }

        //api/users método HTTP POST
        [HttpPost]
        public IActionResult Post([FromBody] CreateUserInputModel inputUser)
        {
            var user = new User(inputUser.FullName, inputUser.Height, inputUser.Weight, inputUser.BirthDate);
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            //return BadRequest();

            return CreatedAtAction(nameof(Get), new { id = user.Id }, inputUser);

        }
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateUserInputModel updateUser)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Update(updateUser.Height, updateUser.Weight);

            _dbContext.Users.Update(user); // NAO PRECISARIA DE CHAMAR O METODO UPDATE, POIS O CONTEXTO JA ESTA SENDO RASTREADO(TRACKING)
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
