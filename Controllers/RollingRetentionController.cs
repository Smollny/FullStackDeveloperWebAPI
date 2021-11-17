using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RollingRetentionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RollingRetentionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{days}")]
        public JsonResult Get(int days)
        {
            if (days == 0) days = 20;
            List<(int i, double v)> result = new List<(int i, double value)>();
            var sqlDataSource = _configuration.GetConnectionString("dbwebapi");
            GetData("select * from \"userInfo\" ", new NpgsqlConnection(sqlDataSource), out List<UserInfoModel> list);
            var minData = System.DateTime.Parse(list.Min(item => item.DateRegistration));
            for (int i = 0; i < days+1; i++)
            {
                result.Add((i,
                    list.Where(item => System.DateTime.Parse(item.DateRegistration) <= minData.AddDays(i)).ToList().Count(item => System.DateTime.Parse(item.DateLastActivity) >= minData.AddDays(i)) 
                    / (double)list.Count(item => System.DateTime.Parse(item.DateRegistration) <= minData.AddDays(i)) * 100));
            }
            return new JsonResult(result);
        }


        void GetData(string query, NpgsqlConnection connection, out List<UserInfoModel> list)
        {
            list = new List<UserInfoModel>();
            var com = new NpgsqlCommand(query, connection);
            connection.Open();
            var sdr = com.ExecuteReader(CommandBehavior.CloseConnection);
            while (sdr.Read())
            {
                list.Add(new UserInfoModel
                {
                    UserID = int.Parse(sdr.GetValue(0).ToString()),
                    DateRegistration = sdr.GetValue(1).ToString(),
                    DateLastActivity = sdr.GetValue(2).ToString(),
                    Uid = int.Parse(sdr.GetValue(3).ToString())
                });
            }
            connection.Close();
        }
    }
}
