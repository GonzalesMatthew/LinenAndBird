using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {
        static List<Bird> _birds = new List<Bird>
        {
            new Bird
            {
                Id = Guid.NewGuid(),
                Name = "Jimmy",
                Color = "Red",
                Size = "Small",
                Type = BirdType.Dead,
                Accessories = new List<string> { "Beanie", "Gold wing tips"}
            }
        };

        
        internal IEnumerable<Bird> GetAll()
        {
            // use 'Using Microsoft.Data.SqlClient;' up above first
            // must have a connection to the database if you want to use ado.net:
            // use connectionstrings.com/sql-server/ for documentation
            using var connection = new SqlConnection("Server=localhost; Database=LinenAndBird; Trusted_Connection=true;"); //'using' in this context is different than up above. it means when we are done with this code block, this IDisposable will be cleaned up and removed after the block runs. this prevents creating multiple connections over time, accumulating them, and things blowing up in your face in production
            // connections aren't open by default, we've goota do that ourselves
            connection.Open();

            //establish a connection
            var command = connection.CreateCommand();
            //this is what tells sql what we want to do
            command.CommandText = @"Select * 
                                    From Birds";
            //then run command against connection
            //execute reader is for when we care about getting all the results of out query
            //this command only ever holds 1 row of data. when it starts it hold 0 rows of data
            var reader = command.ExecuteReader();
            command.Ex

            //make place to store each bird
            var birds = new List<Bird>();

            //.Read() is a boolean value on each read row.
            //data readers are weird, only get one row from the results at a time
            while (reader.Read()) //"while there is a next record, keep doing this..."
            {

                //MAPPING DATA FROM THE RELATIONAL MODEL TO THE OBJECT MODEL (like ORM does for free)
                var bird = new Bird();
                bird.Id = reader.GetGuid(0); //ordinal call works great but you need to know where the column lives. No need to express data types this way.
                bird.Size = reader["Size"].ToString(); //if specifically naming the column you want, you then need to convert it to the appropriate data type
                //Enum.TryParse<BirdType>(reader["Type"].ToString(),out var birdType);
                bird.Type = (BirdType)reader["Type"]; //explicit cast when using custom type. if however the types are incorrect and not matching then things blow up
                bird.Color = reader["Color"].ToString();
                bird.Name = reader["Name"].ToString();

                //each bird goes in the list to return later
                birds.Add(bird);
            }

            return birds;
            // return _birds;
        }

        internal void Add(Bird newBird)
        {
            newBird.Id = Guid.NewGuid();
            _birds.Add(newBird);
        }

        internal Bird GetById(Guid birdId)
        {

            using var connection = new SqlConnection("Server=localhost; Database=LinenAndBird; Trusted_Connection=true;");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"Select * 
                                    From Birds
                                    where Id = @id"; //@id is 'parameterization', better than string interpolation because it protects against SQL injection attacks

            //parameterization prevents sql injection (little bobby tables)
            command.Parameters.AddWithValue("id", birdId);

            //execute reader is for when we care about getting all the results of our query
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var bird = new Bird();
                bird.Id = reader.GetGuid(0); 
                bird.Size = reader["Size"].ToString();
                bird.Type = (BirdType)reader["Type"]; 
                bird.Color = reader["Color"].ToString();
                bird.Name = reader["Name"].ToString();

                return bird;
            }

            //return default; -- if no match, return null;
            return null; //same as above but easier to understand/read

            //return _birds.FirstOrDefault(bird => bird.Id == birdId);
        }
    }
}
