using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper; //to use Dapper. gives access to Query methods, 

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {

        const string _connectionString = "Server=localhost; Database=LinenAndBird; Trusted_Connection=true;";
        internal IEnumerable<Bird> GetAll()
        {
            // use 'Using Microsoft.Data.SqlClient;' up above first
            // must have a connection to the database if you want to use ado.net:
            // use connectionstrings.com/sql-server/ for documentation

            //USING DAPPER:
            using var db = new SqlConnection(_connectionString); //'using' in this context is different than up above. it means when we are done with this code block, this IDisposable will be cleaned up and removed after the block runs. this prevents creating multiple connections over time, accumulating them, and things blowing up in your face in production
            // connections aren't open by default, we've goota do that ourselves

            //Query<T> is for getting results from the database and putting them into a C# type
            var birds = db.Query<Bird>(@"Select *
                            From Birds"); //Dapper does all of the below with this one line of code
                                          //Notice we no longer have to open the connection anymore.
                                          //When we are done, we no longer have to close it either.
                                          //It handles the 'command' stuff such as the database Reader and looping over it to get the things you want
                                          //The mapping portion looks at the results that came back from sql, and matches the sql fields to matching properties according to the <Type> you signified and maps them automatically
                                          //however the same words across the sql table and the <Type> have to match
                                          //if there is not a clean match then you can use Aliasing in your sql to force them to match
                                          //so the power is in being consistent: naming your sql table fields and your Type properties the same thing from the get go

            return birds;

            //OLD ADO METHODOLOGY BELOW ---
            //connection.Open();

            //establish a connection
            //var command = connection.CreateCommand();
            //this is what tells sql what we want to do
            //command.CommandText = @"Select * 
              //                      From Birds";
            //then run command against connection
            //execute reader is for when we care about getting all the results of out query
            //this command only ever holds 1 row of data. when it starts it hold 0 rows of data
            //var reader = command.ExecuteReader();

            //make place to store each bird
            //var birds = new List<Bird>();

            //.Read() is a boolean value on each read row.
            //data readers are weird, only get one row from the results at a time
            //while (reader.Read()) //"while there is a next record, keep doing this..."
            {

                //MAPPING DATA FROM THE RELATIONAL MODEL TO THE OBJECT MODEL (like ORM does for free)
                //after this explicit example we will use MapFromReader to save effort
                //var bird = new Bird();
                //bird.Id = reader.GetGuid(0); //ordinal call works great but you need to know where the column lives. No need to express data types this way.
                //bird.Size = reader["Size"].ToString(); //if specifically naming the column you want, you then need to convert it to the appropriate data type
                //Enum.TryParse<BirdType>(reader["Type"].ToString(),out var birdType);
                //bird.Type = (BirdType)reader["Type"]; //explicit cast when using custom type. if however the types are incorrect and not matching then things blow up
                //bird.Color = reader["Color"].ToString();
                //bird.Name = reader["Name"].ToString();

                //var bird = MapFromReader(reader);

                //each bird goes in the list to return later
            //    birds.Add(bird);
            }

            //return birds;
     
        }

        internal object Update(Guid id, Bird bird)
        {
            //prep work for the sql stuff
            using var db = new SqlConnection(_connectionString);
            //using var connection = new SqlConnection(_connectionString);

            var sql = @"update Birds
                                Set Color = @color,
	                                Name = @name,
	                                Type = @type,
	                                Size = @size
                                output inserted.*
                                Where Id = @id";

            bird.Id = id;

                                                           //if we have an object already that maps what we need for us already, we can just plug it in.
                                                           //so here an anonymous data type is not necessary and we can use the bird calss
            var updatedBird = db.QuerySingleOrDefault(sql, bird);

            return updatedBird;
            

            //connection.Open();

            //var cmd = connection.CreateCommand();
            //cmd.CommandText = @"update Birds
            //Set Color = @color,
            //Name = @name,
            //Type = @type,
            //Size = @size
            //output inserted.*
            //Where Id = @id";

            //bird comes from the http request in the controller
            //note, capitalization dn have to match, just the word needs to match
            //cmd.Parameters.AddWithValue("Type", bird.Type);
            //cmd.Parameters.AddWithValue("Color", bird.Color);
            //cmd.Parameters.AddWithValue("Size", bird.Size);
            //cmd.Parameters.AddWithValue("Name", bird.Name);
            //cmd.Parameters.AddWithValue("Id", bird.Id);

            //execution of the sql against the database
            //var reader = cmd.ExecuteReader();

            //working with the results
            //if (reader.Read())
            //{
            //var updatedBird =  MapFromReader(reader);
            //return updatedBird;
            //}
            //return null;
        }

        internal void Remove(Guid id)
        {
            using var db = new SqlConnection(_connectionString);
            
            var sql = @"Delete
                        From Birds 
                        Where Id = @id";
            //db.Execute(sql, new { id = id });
            db.Execute(sql, new { id }); // funcationally equivalent to above line and can do so because the type and property are the same name

            //connection.Open();

            //var cmd = connection.CreateCommand();
            //cmd.CommandText = @"Delete
              //                  From Birds 
              //                  Where Id = @id";

            //cmd.Parameters.AddWithValue("id", id);

            //cmd.ExecuteNonQuery(); //run the cmd then forget about it
        }

        internal void Add(Bird newBird)
        {
            using var db = new SqlConnection(_connectionString); //sets up connection

            var sql = @"insert into Birds(Type,Color,Size,Name)
                                values(@Type, @Color, @Size, @Name)";

            //execute scalar, grab the guid data type from the sql, where the newBird properties match the variables in your sql code (the @ sql parameters)
            var id = db.ExecuteScalar<Guid>(sql, newBird);
            newBird.Id = id;

            // connection.Open(); //opens the connection

            // var cmd = connection.CreateCommand(); //set up a command against the connection
            // cmd.CommandText = @"insert into Birds(Type,Color,Size,Name)
               //                 values(@Type, @Color, @Size, @Name)"; //express what command says
                                
            // cmd.Parameters.AddWithValue("Type", newBird.Type); //map values to above @s (avoid sql injection)
            // cmd.Parameters.AddWithValue("Color", newBird.Color);
            // cmd.Parameters.AddWithValue("Size", newBird.Size);
            // cmd.Parameters.AddWithValue("Name", newBird.Name);

            //execute the query, but don't are about the results, just number of rows
            //var numberOfRowsAffected = cmd.ExecuteNonQuery(); //this cmd.ExecuteNonQuery() returns number of rows affected. bit of a misnomer. essentially it's not returning query results
            
            //execute the query and only get the id of the new row
            // var newId = (Guid) cmd.ExecuteScalar();
            
            // newBird.Id = newId;
            //_birds.Add(newBird);
        }

        internal Bird GetById(Guid birdId)
        {
            //connections are like the tunnel between our app and the database
            //DAPPER APPROACH:
            using var db = new SqlConnection(_connectionString);

            var sql = @"Select *
                        From Birds
                        where id = @id";

            //return first match
                                                        //anonymous data type helps us map @id to our function's argument
                                                        //new { type = property}
            var bird = db.QuerySingleOrDefault<Bird>(sql, new { id = birdId });

            return bird;

            //OLD ADO WAY:
            //using var connection = new SqlConnection(_connectionString);
            //connection.Open();

            //var command = connection.CreateCommand();
            //command.CommandText = @"Select * 
            //                        From Birds
            //                        where Id = @id"; //@id is 'parameterization', better than string interpolation because it protects against SQL injection attacks

            //parameterization prevents sql injection (little bobby tables)
            //command.Parameters.AddWithValue("id", birdId);

            //execute reader is for when we care about getting all the results of our query
            //var reader = command.ExecuteReader();

            //if (reader.Read())
            //{
            //    return MapFromReader(reader);
            //}

            //return default; -- if no match, return null;
            //return null; //same as above but easier to understand/read

            //return _birds.FirstOrDefault(bird => bird.Id == birdId);
        }

        Bird MapFromReader(SqlDataReader reader)
        {
            var bird = new Bird();
            bird.Id = reader.GetGuid(0);
            bird.Size = reader["Size"].ToString();
            bird.Type = (BirdType)reader["Type"];
            bird.Color = reader["Color"].ToString();
            bird.Name = reader["Name"].ToString();

            return bird;
        }
    }
}
