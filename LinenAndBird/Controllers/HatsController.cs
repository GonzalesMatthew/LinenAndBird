using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinenAndBird.Models;

namespace LinenAndBird.Controllers
{
    [Route("api/hats")]  // an attribute -- adds metadata about whatever they sit on top of  // exposed at this endpoint
    [ApiController]  // an attribute - adds metadata about whatever they sit on top of  // an api controller, so it returns json or xml
    public class HatsController : ControllerBase
    {
        // this is a _field
        static List<Hat> _hats = new List<Hat>
            {
                new Hat
                {
                    Color = "Red",
                    Designer = "Charlie",
                    Style = HatStyle.OpenBack
                },
                new Hat
                {
                    Color = "Black",
                    Designer = "Nathan",
                    Style = HatStyle.Fascinator
                },
                new Hat
                {
                    Color = "Blue",
                    Designer = "Matthew",
                    Style = HatStyle.WideBrim
                }
            };

        [HttpGet]
        public List<Hat> GetAllHats()
        {
            return _hats;
        }

        // GET /api/hats/styles/1  -> all open backed hats
        [HttpGet("styles/{style}")] // name in curly braces need to match name we gave parameter below
        public IEnumerable<Hat> GetHatsByStyle(HatStyle style)
        {
            var matches = _hats.Where(hat => hat.Style == style); // uses "deferred execution" -> gets IEnumerated in next line when returned
            return matches;
        }

        [HttpPost]
        public void AddHat(Hat newHat)
        {
            _hats.Add(newHat);
        }
    }
}
