using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class IssuesApproveController : ApiController
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
                    SqlCommand command = new SqlCommand("SELECT * FROM nalco_materials.dbo.np_issue WHERE issue_approved_by='NA';", conn);
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
            }
            catch (Exception ex)
            {
                JArray outputa = new JArray();
                JObject output = new JObject();
                output["status"] = "error";
                output["message"] = ex.Message;
                outputa.Add(output);
                return outputa;
            }
        }

        public object GetAll(string id)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();
                SqlCommand cmdCount = new SqlCommand("SELECT * FROM np_issue WHERE issue_voucher_no LIKE '%" + id + "%' AND issue_approved_by='NA';", conn);
                using (SqlDataReader reader = cmdCount.ExecuteReader())
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
        }

        // POST api/<controller>

        public object Post([FromBody] JObject data)
        {
            //try
            //{
            JObject dataval = data["data"].ToObject<JObject>();
            if ((int)dataval["quantity"] <= 0)
            {
                JObject output = new JObject();
                output["status"] = "quantity too low";
                return output;
            }
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();
                SqlCommand cmdCount3 = new SqlCommand("SELECT issue_mat_code from np_issue WHERE issue_voucher_no='" + (string)dataval["voucher"] + "';", conn);
                Int64 issuedmaterial = (Int64)cmdCount3.ExecuteScalar();
                SqlCommand cmdCount2 = new SqlCommand("UPDATE [nalco_materials].[dbo].[np_materials] SET [material_quantity] = [material_quantity] - @1 WHERE [material_code] = @2 AND [material_quantity] - @1 > -1;", conn);
                cmdCount2.Parameters.AddWithValue("@1", (int)dataval["quantity"]);
                cmdCount2.Parameters.AddWithValue("@2", issuedmaterial);
                int count2 = cmdCount2.ExecuteNonQuery();
                if (count2 == 0)
                {
                    JObject output = new JObject();
                    output["status"] = "not enough";
                    return output;
                }
                SqlCommand cmdCount = new SqlCommand("UPDATE np_issue SET issue_quantity=@1, issue_collected_by=@2, issue_approved_by=@3 WHERE issue_voucher_no=@4 AND issue_approved_by='NA';", conn);
                cmdCount.Parameters.AddWithValue("@1", (int)dataval["quantity"]);
                cmdCount.Parameters.AddWithValue("@2", (string)dataval["collected"]);
                cmdCount.Parameters.AddWithValue("@3", (string)dataval["approved"]);
                cmdCount.Parameters.AddWithValue("@4", (string)dataval["voucher"]);
                int count = cmdCount.ExecuteNonQuery();
                if (count > 0)
                {
                    JObject output = new JObject();
                    output["status"] = "approved";
                    return output;
                }
                else
                {
                    JObject output = new JObject();
                    output["status"] = "error";
                    return output;
                }
            }

            //}
            //catch (Exception ex)
            //{
            //    JObject output = new JObject();
            //    output["status"] = "error";
            //    output["message"] = ex.Message;
            //    return output;
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