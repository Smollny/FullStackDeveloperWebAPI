using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistogramController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public HistogramController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = "select \"userID\", max(\"dateLastActivity\"-\"dateRegistration\") as razn from \"userInfo\" group by \"userID\" ";

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

    }
}
