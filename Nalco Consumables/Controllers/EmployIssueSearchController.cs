using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class EmployIssueSearchController : ApiController
    {
        public string connection = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

        public object Get(string id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT TOP 3 * FROM [nalco_materials].[dbo].[np_issue] WHERE issue_issued_to=" + id + " ORDER BY issue_date DESC", conn);
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
                            JObject output = new JObject();
                            output["status"] = "not exists";
                            return output;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                JObject output = new JObject();
                output["status"] = "error";
                output["message"] = ex.Message;
                return output;
            }
        }

        public object Get()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_employ", conn);
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
                            JObject output = new JObject();
                            output["status"] = "not exists";
                            return output;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                JObject output = new JObject();
                output["status"] = "error";
                output["message"] = ex.Message;
                return output;
            }
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