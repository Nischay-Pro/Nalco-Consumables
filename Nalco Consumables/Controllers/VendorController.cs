using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class VendorController : ApiController
    {
        public string connection = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

        // DELETE api/<controller>/5
        public JObject Delete(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand cmdCount = new SqlCommand("DELETE FROM nalco_materials.dbo.np_vendor WHERE vendor_code = @vendorcode", conn);
                    cmdCount.Parameters.AddWithValue("@vendorcode", id);
                    int count = cmdCount.ExecuteNonQuery();
                    if (count > 0)
                    {
                        JObject output = new JObject();
                        output["status"] = "deleted";
                        return output;
                    }
                    else
                    {
                        JObject output = new JObject();
                        output["status"] = "not exists";
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

        // GET api/<controller>
        public object Get()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_vendor", conn);
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

        // GET api/<controller>/5
        public JObject Get(Int64 id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_vendor WHERE vendor_code=" + id, conn);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            var r = Serialize(reader);
                            JObject output = new JObject();
                            string abc = JsonConvert.SerializeObject(r, Formatting.Indented).TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
                            output = JObject.Parse(abc);
                            return output;
                        }
                        else
                        {
                            JObject response = new JObject();
                            response["status"] = "not exists";
                            return response;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                JObject response = new JObject();
                response["status"] = "error";
                response["message"] = ex.Message;
                return response;
            }
        }

        // POST api/<controller>
        public JObject Post([FromBody] JObject data)
        {
            try
            {
                JObject dataval = data["data"].ToObject<JObject>();
                string vendorcode, vendorname, vendorcontact;
                bool createquery = (bool)dataval["createquery"]; ;
                vendorcode = (string)dataval["vendorcode"];
                vendorname = (string)dataval["vendorname"];
                vendorcontact = (string)dataval["vendorcontact"];
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand cmdCount = new SqlCommand("SELECT count(*) from np_vendor WHERE vendor_code = @vendorcode", conn);
                    cmdCount.Parameters.AddWithValue("@vendorcode", vendorcode);
                    int count = (int)cmdCount.ExecuteScalar();

                    if (count > 0)
                    {
                        if (createquery == false)
                        {
                            SqlCommand updCommand = new SqlCommand("UPDATE np_vendor SET vendor_name = @vendorname, vendor_contact = @vendorcontact WHERE vendor_code=@vendorcode", conn);
                            updCommand.Parameters.AddWithValue("@vendorcode", vendorcode);
                            updCommand.Parameters.AddWithValue("@vendorname", vendorname);
                            updCommand.Parameters.AddWithValue("@vendorcontact", vendorcontact);
                            int rowsUpdated = updCommand.ExecuteNonQuery();
                            JObject output = new JObject();
                            output["status"] = "updated";
                            return output;
                        }
                        else
                        {
                            JObject output = new JObject();
                            output["status"] = "exists";
                            return output;
                        }
                    }
                    else
                    {
                        if (createquery == true)
                        {
                            SqlCommand updCommand = new SqlCommand("INSERT INTO[dbo].[np_vendor]([vendor_code], [vendor_name], [vendor_contact]) VALUES(@vendorcode, @vendorname, @vendorcontact)", conn);
                            updCommand.Parameters.AddWithValue("@vendorcode", vendorcode);
                            updCommand.Parameters.AddWithValue("@vendorname", vendorname);
                            updCommand.Parameters.AddWithValue("@vendorcontact", vendorcontact);
                            int rowsUpdated = updCommand.ExecuteNonQuery();
                            JObject output = new JObject();
                            output["status"] = "created";
                            return output;
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