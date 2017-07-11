using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class IssuesController : ApiController
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
                    SqlCommand cmdCount = new SqlCommand("DELETE FROM nalco_materials.dbo.np_issues WHERE issue_voucher_no = @issuenumber", conn);
                    cmdCount.Parameters.AddWithValue("@issuenumber", id);
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
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_issues", conn);
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
                    SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_issue WHERE issue_voucher_no=" + id, conn);
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
            //try
            //{
            JObject dataval = data["data"].ToObject<JObject>();
            DateTime issuedate;
            bool createquery = (bool)dataval["createquery"];
            bool substore = (bool)dataval["substore"];
            bool approveissue = false;
            string sapvoucher = null, materialcode = null, issuequantity = null, issuedepartment = null, issueto = null, issuecollected = null, issuelocation = null, issueapprovedby = null;
            if (!substore)
            {
                approveissue = true;
                sapvoucher = "CS" + (string)dataval["sapvoucher"];
                materialcode = (string)dataval["materialcode"];
                issuedate = (DateTime)dataval["issuedate"];
                issuequantity = (string)dataval["issuequantity"];
                issuedepartment = (string)dataval["issuedepartment"];
                issueto = (string)dataval["issueto"];
                issuecollected = (string)dataval["issuecollectedby"];
                issuelocation = (string)dataval["issuelocation"];
                issueapprovedby = (string)dataval["issueapprovedby"];
            }
            else
            {
                issuedate = DateTime.Now;
            }
            string issuematerialcode = (string)dataval["issuematerialcode"];
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();
                SqlCommand cmdCount = new SqlCommand("SELECT * from np_issue WHERE issue_voucher_no = @vouchernumber", conn);
                cmdCount.Parameters.AddWithValue("@vouchernumber", sapvoucher);
                int count = (int)cmdCount.ExecuteNonQuery();
                if (count > 0)
                {
                    if (createquery == false)
                    {
                        JObject output = new JObject();
                        output["status"] = "cannot be updated";
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
                        SqlCommand updCommand2 = new SqlCommand("UPDATE [nalco_materials].[dbo].[np_materials] SET [material_quantity] = [material_quantity] - @1 WHERE [material_code] = @2 AND [material_quantity] - @1 > -1;", conn);
                        updCommand2.Parameters.AddWithValue("@2", materialcode);
                        updCommand2.Parameters.AddWithValue("@1", Int64.Parse(issuequantity));
                        int rowsUpdated2 = updCommand2.ExecuteNonQuery();
                        if (!(rowsUpdated2 > 0))
                        {
                            JObject output = new JObject();
                            output["status"] = "enough material not available";
                            return output;
                        }
                        else
                        {
                            SqlCommand updCommand = new SqlCommand("INSERT INTO [nalco_materials].[dbo].[np_issue] ([issue_voucher_no] ,[issue_mat_code] ,[issue_date] ,[issue_quantity] ,[issue_dept] ,[issue_location] ,[issue_issued_to] ,[issue_collected_by],[issue_approved_by]) VALUES (@1 ,@2 ,@3 ,@4 ,@5 ,@6 ,@7 ,@8,@9)", conn);
                            updCommand.Parameters.AddWithValue("@1", sapvoucher);
                            updCommand.Parameters.AddWithValue("@2", Int64.Parse(materialcode));
                            updCommand.Parameters.AddWithValue("@3", issuedate);
                            updCommand.Parameters.AddWithValue("@4", issuequantity);
                            updCommand.Parameters.AddWithValue("@5", issuedepartment);
                            updCommand.Parameters.AddWithValue("@6", issuelocation);
                            updCommand.Parameters.AddWithValue("@7", issueto);
                            updCommand.Parameters.AddWithValue("@8", issuecollected);
                            updCommand.Parameters.AddWithValue("@9", issueapprovedby);
                            int rowsUpdated = updCommand.ExecuteNonQuery();
                            JObject output = new JObject();
                            output["status"] = "created";
                            return output;
                        }
                    }
                    else
                    {
                        JObject output = new JObject();
                        output["status"] = "cannot be updated";
                        return output;
                    }
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