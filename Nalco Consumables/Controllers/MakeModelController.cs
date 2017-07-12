using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class MakeModelController : ApiController
    {
        public string connection = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

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

        public object Post([FromBody] JObject data)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    JObject dataval = data["data"].ToObject<JObject>();
                    bool search = (bool)dataval["search"];
                    if (search == false)
                    {
                        bool makelist = (bool)dataval["makesearch"];
                        SqlCommand command;
                        if (makelist == true)
                        {
                            command = new SqlCommand("SELECT DISTINCT make FROM ast.dbo.ast_master;", conn);
                        }
                        else
                        {
                            string makename = (string)dataval["makename"];
                            command = new SqlCommand("SELECT DISTINCT model FROM ast.dbo.ast_master WHERE make='" + makename + "';", conn);
                        }
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
                        bool makelist = (bool)dataval["makelist"];
                        SqlCommand command;
                        if (makelist == true)
                        {
                            var makename = dataval["makename"];
                            command = new SqlCommand("SELECT DISTINCT ast.dbo.ast_master.make FROM ast.dbo.ast_master WHERE ast.dbo.ast_master.make LIKE '%" + makename.ToString() + "%';", conn);
                        }
                        else
                        {
                            string makename = (string)dataval["makename"];
                            string modelname = (string)dataval["modelname"];
                            command = new SqlCommand("SELECT DISTINCT model FROM ast.dbo.ast_master WHERE make='" + makename + "' AND model LIKE '%" + modelname + "%';", conn);
                        }
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