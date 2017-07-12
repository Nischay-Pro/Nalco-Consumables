using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
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
            string model = (string)dataval["description"];
            string dept = (string)dataval["department"];
            string location = (string)dataval["location"];
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();

                //SqlCommand command = new SqlCommand("SELECT * FROM ast.dbo.ast_master;", conn);
                SqlCommand command = new SqlCommand("SELECT custodian FROM nalco_materials.dbo.np_test WHERE CONVERT(VARCHAR, make)= '@1' AND CONVERT(VARCHAR, dept)='@2' AND CONVERT(VARCHAR, location)='@3';", conn);
                command.Parameters.AddWithValue("@1", Regex.Unescape(model));
                command.Parameters.AddWithValue("@2", Regex.Unescape(dept));
                command.Parameters.AddWithValue("@3", Regex.Unescape(location));
                string abdfdsf = model.ToString();
                string abcd = "SELECT custodian FROM ast.dbo.ast_master WHERE concat(make,' ',model) = '" + model + "' AND dept='" + dept + "' AND location='" + location + "';";
                int adf = model.Length;

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