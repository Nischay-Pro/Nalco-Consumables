using Nalco_Consumables.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Helpers;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class MaterialsController : ApiController
    {
        [NonAction]
        public IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }

        [NonAction]
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        // GET api/<controller>
        public string Get()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=DESKTOP-97AH258\\SQLEXPRESS;Initial Catalog=nalco_materials;Integrated Security=True";
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_materials", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var r = Serialize(reader);
                    string json = JsonConvert.SerializeObject(r, Formatting.Indented);
                    return json;
                }
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=DESKTOP-97AH258\\SQLEXPRESS;Initial Catalog=nalco_materials;Integrated Security=True";
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_materials WHERE material_code=" + id, conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var r = Serialize(reader);
                    string json = JsonConvert.SerializeObject(r, Formatting.Indented);
                    return json;
                }
            }
        }

        // POST api/<controller>
        public string Post([FromBody]string value)
        {
            return value;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}