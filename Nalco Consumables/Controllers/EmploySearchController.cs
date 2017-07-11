using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class EmploySearchController : ApiController
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
                    SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[np_employ] WHERE employ_dept_cd='" + id + "'", conn);
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

        public object Post([FromBody] JObject data)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    JObject dataval = data["data"].ToObject<JObject>();
                    if ((bool)dataval["dept"] == true)
                    {
                        SqlCommand command = new SqlCommand("SELECT employ_dept_cd, employ_dept_name FROM nalco_materials.dbo.np_employ GROUP BY employ_dept_cd,employ_dept_name;", conn);
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
                    else if ((bool)dataval["dept"] == false)
                    {
                        string deptcode = (string)dataval["deptcode"];
                        SqlCommand command = new SqlCommand("SELECT employ_loc_cd , employ_loc_name,employ_dept_name FROM nalco_materials.dbo.np_employ WHERE employ_dept_cd=@1 GROUP BY employ_loc_cd,  employ_loc_name,employ_dept_name;", conn);
                        command.Parameters.AddWithValue("@1", deptcode);
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
                    else
                    {
                        JObject output = new JObject();
                        output["status"] = "invalid command";
                        return output;
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