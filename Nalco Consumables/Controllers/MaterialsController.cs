using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MaterialsController : ApiController
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
                    SqlCommand cmdCount = new SqlCommand("DELETE FROM nalco_materials.dbo.np_materials WHERE material_code = @materialcode", conn);
                    cmdCount.Parameters.AddWithValue("@materialcode", id);
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
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_materials", conn);
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
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_materials WHERE material_code=" + id, conn);
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
                string materialcode, materialdescription, materialprinter, materialprinterdescription, materialprintercount, materialquantity, materialcriticalflag, materialreorderlevel, materialstorage;
                bool createquery = (bool)dataval["createquery"]; ;
                materialcode = (string)dataval["materialcode"];
                materialdescription = (string)dataval["materialdescription"];
                materialprinterdescription = (string)dataval["materialprinterdescription"];
                materialprinter = (string)dataval["materialprinter"];
                materialprintercount = (string)dataval["materialprintercount"];
                materialquantity = (string)dataval["materialquantity"];
                materialcriticalflag = (string)dataval["materialcriticalflag"];
                materialreorderlevel = (string)dataval["materialreorderlevel"];
                materialstorage = (string)dataval["materialstorage"];
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connection;
                    conn.Open();
                    SqlCommand cmdCount = new SqlCommand("SELECT count(*) from np_materials WHERE material_code = @materialcode", conn);
                    cmdCount.Parameters.AddWithValue("@materialcode", materialcode);
                    int count = (int)cmdCount.ExecuteScalar();

                    if (count > 0)
                    {
                        if (createquery == false)
                        {
                            SqlCommand updCommand = new SqlCommand("UPDATE np_materials SET material_description = @materialdescription, material_printer_count = @materialprintercount,material_quantity = @materialquantity, material_printer = @materialprinter, material_critical_flag = @materialcriticalflag, material_reorder_level = @materialreorderlevel, material_storage = @materialstorage, material_printer_description = @materialprinterdescription WHERE material_code=@materialcode", conn);
                            updCommand.Parameters.AddWithValue("@materialcode", materialcode);
                            updCommand.Parameters.AddWithValue("@materialdescription", materialdescription);
                            updCommand.Parameters.AddWithValue("@materialprinter", materialprinter);
                            updCommand.Parameters.AddWithValue("@materialprinterdescription", materialprinterdescription);
                            updCommand.Parameters.AddWithValue("@materialprintercount", materialprintercount);
                            updCommand.Parameters.AddWithValue("@materialquantity", materialquantity);
                            updCommand.Parameters.AddWithValue("@materialcriticalflag", materialcriticalflag);
                            updCommand.Parameters.AddWithValue("@materialreorderlevel", materialreorderlevel);
                            updCommand.Parameters.AddWithValue("@materialstorage", materialstorage);
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
                            SqlCommand updCommand = new SqlCommand("INSERT INTO[dbo].[np_materials]([material_code], [material_description], [material_printer], [material_printer_description], [material_printer_count], [material_quantity], [material_critical_flag], [material_reorder_level], [material_storage]) VALUES(@materialcode, @materialdescription, @materialprinter, @materialprinterdescription, @materialprintercount, @materialquantity, @materialcriticalflag, @materialreorderlevel, @materialstorage)", conn);
                            updCommand.Parameters.AddWithValue("@materialcode", materialcode);
                            updCommand.Parameters.AddWithValue("@materialdescription", materialdescription);
                            updCommand.Parameters.AddWithValue("@materialprinter", materialprinter);
                            updCommand.Parameters.AddWithValue("@materialprinterdescription", materialprinterdescription);
                            updCommand.Parameters.AddWithValue("@materialprintercount", materialprintercount);
                            updCommand.Parameters.AddWithValue("@materialquantity", materialquantity);
                            updCommand.Parameters.AddWithValue("@materialcriticalflag", materialcriticalflag);
                            updCommand.Parameters.AddWithValue("@materialreorderlevel", materialreorderlevel);
                            updCommand.Parameters.AddWithValue("@materialstorage", materialstorage);
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

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
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