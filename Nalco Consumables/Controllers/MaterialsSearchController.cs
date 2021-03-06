﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class MaterialsSearchController : ApiController
    {
        public string connection = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

        [HttpGet]
        public object Get(string id)
        {
            //try
            //{
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM [nalco_materials].[dbo].[np_materials] WHERE material_code LIKE '%" + id + "%' OR material_description LIKE '%" + id + "%';", conn);
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