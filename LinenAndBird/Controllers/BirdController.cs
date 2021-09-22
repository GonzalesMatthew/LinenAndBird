using LinenAndBird.DataAccess;
using LinenAndBird.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LinenAndBird.Controllers
{
    [Route("api/birds")]
    [ApiController]
    public class BirdController : ControllerBase
    {
        BirdRepository _repo;

        //this is asking asp.net for the application configuration
        //this is known as Dependency Injection
        public BirdController(BirdRepository birdRepo)
        {
            _repo = birdRepo; //with Startup.cs refactor we no longer need below method
            //var connectionString = config.GetConnectionString("LinenAndBird");
            //_repo = new BirdRepository(connectionString);
        }
        [HttpGet]
        public IActionResult GetAllBirds()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetBirdById(Guid id)
        {
            var bird = _repo.GetById(id);

            if (bird == null)
            {
                return NotFound($"No bird with the id {id} was found.");
            }

            return Ok(bird);
        }

        [HttpPost]
        public IActionResult AddBird(Bird newBird) //IActionResult is an interface -- allows us to have granular controller over what we are returning at any given point in time. Otherwise we only return 200's or 500's
        {
            if (string.IsNullOrEmpty(newBird.Name) || string.IsNullOrEmpty(newBird.Color))
            {
                return BadRequest("Name and Color are required fields"); //method comes from base class
            }

            _repo.Add(newBird);

            //return Ok(); //Ok() method comes from the base class, returns ok result - an empty 200()
            return Created($"/api/bird/{newBird.Id})", newBird); //201 status code, includes varying amounts of specificity on how we are good
        }

        //api/birds/{id}
        [HttpDelete]
        public IActionResult DeleteBird(Guid id)
        {
            _repo.Remove(id);

            return Ok();
        }

        //api/birds/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateBird(Guid id, Bird bird)
        {
            var birdToUpdate = _repo.GetById(id);

            if (birdToUpdate == null)
            {
                return NotFound($"Could not find bird with the id {id} for updating");
            }

            var updatedBird = _repo.Update(id, bird);

            return Ok(updatedBird);
        }
    }
}
