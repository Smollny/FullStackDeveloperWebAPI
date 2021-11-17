using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserInfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = "select \"userID\", \"Uid\", to_char(\"dateRegistration\", 'DD-MM-YYYY') as  dateRegistration, to_char(\"dateLastActivity\", 'DD-MM-YYYY') as dateLastActivity from \"userInfo\" ";

            var table = new System.Data.DataTable();
            var sqlDataSource = _configuration.GetConnectionString("dbwebapi");
            using (var connection = new NpgsqlConnection(sqlDataSource))
            {
                connection.Open();
                var command = new NpgsqlCommand(query, connection);
                var reader = command.ExecuteReader();
                table.Load(reader);
                reader.Close();
                connection.Close();
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(UserInfoModel userInfo)
        {
            string query = "INSERT INTO \"userInfo\"(\"userID\", \"dateRegistration\", \"dateLastActivity\") VALUES(@id, @dateReg, @dateLast); ";

            var table = new System.Data.DataTable();
            var sqlDataSource = _configuration.GetConnectionString("dbwebapi");
            using (var connection = new NpgsqlConnection(sqlDataSource))
            {
                connection.Open();
                var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userInfo.UserID);
                command.Parameters.AddWithValue("@dateReg", DateTime.Parse(userInfo.DateRegistration));
                command.Parameters.AddWithValue("@dateLast", DateTime.Parse(userInfo.DateLastActivity));
                var reader = command.ExecuteReader();
                table.Load(reader);
                reader.Close();
                connection.Close();
            }

            return new JsonResult("Added Successfully");
        }
    }
}
