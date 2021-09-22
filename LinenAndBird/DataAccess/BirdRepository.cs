using LinenAndBird.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper; //to use Dapper. gives access to Query methods, 
using Microsoft.Extensions.Configuration;

namespace LinenAndBird.DataAccess
{
    public class BirdRepository
    {

        readonly string _connectionString;

        // http request => creates IConfiguration => creates BirdRepository => creates BirdController

        public BirdRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("LinenAndBird") ;
        }
        internal IEnumerable<Bird> GetAll()
        {

            //USING DAPPER:
            using var db = new SqlConnection(_connectionString); 

            var birds = db.Query<Bird>(@"Select *
                            From Birds");

            var accessorySql = @"Select * From BirdAccessories";
            var accessories = db.Query<BirdAccessory>(accessorySql);

            foreach (var bird in birds)
            {
                bird.Accessories = accessories.Where(accessory => accessory.BirdId == bird.Id);
            }

            return birds;

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
            //DAPPER APPROACH:
            using var db = new SqlConnection(_connectionString);

            var birdSql = @"Select *
                        From Birds
                        where id = @id";

            //return first match
                                                        //anonymous data type helps us map @id to our function's argument
                                                        //new { type = property}
            var bird = db.QuerySingleOrDefault<Bird>(birdSql, new { id = birdId });

            if (bird == null) return null;

            //lets get the accessories for the bird.

            var accessorySql = @"Select * 
                                 From BirdAccessories
                                 Where BirdId = @birdId";

            var accessories = db.Query<BirdAccessory>(accessorySql, new { birdId });

            bird.Accessories = accessories;

            return bird;

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
