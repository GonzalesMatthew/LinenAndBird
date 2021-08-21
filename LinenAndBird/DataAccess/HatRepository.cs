using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinenAndBird.DataAccess
{
    public class HatRepository
    {
        // this is a _field
        static List<Hat> _hats = new List<Hat>
            {
                new Hat
                {
                    Id = Guid.NewGuid(),
                    Color = "Red",
                    Designer = "Charlie",
                    Style = HatStyle.OpenBack
                },
                new Hat
                {
                    Id = Guid.NewGuid(),
                    Color = "Black",
                    Designer = "Nathan",
                    Style = HatStyle.Fascinator
                },
                new Hat
                {
                    Id = Guid.NewGuid(),
                    Color = "Blue",
                    Designer = "Matthew",
                    Style = HatStyle.WideBrim
                }
            };

        internal Hat GetById(Guid hatId)
        {
            return _hats.FirstOrDefault(hat => hat.Id == hatId);
        }
        internal List<Hat> GetAll() // 'internal' means anyone can use this inside the project. Fine in this case
        {
            return _hats;
        }

        internal IEnumerable<Hat> GetByStyle(HatStyle style)
        {
            return _hats.Where(hat => hat.Style == style); // uses "deferred execution" -> gets IEnumerated in next line when returned
        }

        internal void Add(Hat newHat)
        {
            newHat.Id = Guid.NewGuid();
            _hats.Add(newHat);
        }
    }
}
