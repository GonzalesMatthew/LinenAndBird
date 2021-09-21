using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace LinenAndBird.DataAccess
{
    public class HatRepository
    {
        const string _connectionString = "Server=localhost; Database=LinenAndBird; Trusted_Connection=true;";

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
            //create connection
            using var db = new SqlConnection(_connectionString);

                                          //the data type we want back (so we need those columns)
            var hat = db.QueryFirstOrDefault<Hat>("Select * from Hats where Id = @id", new { id = hatId });

            return hat;

            //return _hats.FirstOrDefault(hat => hat.Id == hatId);
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
