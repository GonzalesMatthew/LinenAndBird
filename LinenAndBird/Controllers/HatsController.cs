using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinenAndBird.Models;
using LinenAndBird.DataAccess;

namespace LinenAndBird.Controllers
{
    [Route("api/hats")]  // an attribute -- adds metadata about whatever they sit on top of  // exposed at this endpoint
    [ApiController]  // an attribute - adds metadata about whatever they sit on top of  // an api controller, so it returns json or xml
    public class HatsController : ControllerBase
    {
        HatRepository _repo;

        public HatsController(HatRepository hatRepo)
        {
            _repo = hatRepo;
        }

        [HttpGet]
        public List<Hat> GetAllHats()
        {
            return _repo.GetAll();
        }

        // GET /api/hats/styles/1  -> all open backed hats
        [HttpGet("styles/{style}")] // name in curly braces need to match name we gave parameter below
        public IEnumerable<Hat> GetHatsByStyle(HatStyle style)
        {
            var matches = _repo.GetByStyle(style);
            return matches;
        }

        [HttpPost]
        public void AddHat(Hat newHat)
        {
            _repo.Add(newHat);
        }
    }
}
