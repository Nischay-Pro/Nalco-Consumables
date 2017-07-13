using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class UsersSearchController : ApiController
    {
        public string connection = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        public string connection2 = System.Configuration.ConfigurationManager.ConnectionStrings["otherConnectionString"].ConnectionString;

        public object Post([FromBody] JObject data)
        {
            //try
            //{
            JObject dataval = data["data"].ToObject<JObject>();
            var model = dataval["description"];
            var dept = dataval["department"];
            var location = dataval["location"];
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection2;
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT custodian FROM ast.dbo.ast_master WHERE (concat(make,' ', model)=@1) AND dept=@2 AND location=@3;", conn);
                command.Parameters.AddWithValue("@1", model.ToString());
                command.Parameters.AddWithValue("@2", dept.ToString());
                command.Parameters.AddWithValue("@3", location.ToString());
                string abdfdsf = model.ToString();
                string abcd = "SELECT custodian FROM ast.dbo.ast_master WHERE concat(make,' ',model) = '" + model + "' AND dept='" + dept + "' AND location='" + location + "';";

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows == true)
                    {
                        var r = Serialize(reader);
                        JArray output = new JArray();
                        string abc = JsonConvert.SerializeObject(r, Formatting.Indented);
                        output = JArray.Parse(abc);
                        JObject outputa = new JObject();
                        outputa["data"] = output;
                        return outputa;
                    }
                    else
                    {
                        JArray outputa = new JArray();
                        JObject response = new JObject();
                        response["status"] = "not exists";
                        outputa.Add(response);
                        return outputa;
                    }
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    JArray outputa = new JArray();
            //    JObject output = new JObject();
            //    output["status"] = "error";
            //    output["message"] = ex.Message;
            //    outputa.Add(output);
            //    return outputa;
            //}
        }

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
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }
    }
}