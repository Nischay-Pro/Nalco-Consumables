﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using Nalco_Consumables.Models;

namespace Nalco_Consumables.Controllers
{
    [Authorize]
    public class MaterialsController : ApiController
    {
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
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        // GET api/<controller>
        public string Get()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=DESKTOP-97AH258\\SQLEXPRESS;Initial Catalog=nalco_materials;Integrated Security=True";
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_materials", conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var r = Serialize(reader);
                    string json = JsonConvert.SerializeObject(r, Formatting.Indented);
                    return json;
                }
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=DESKTOP-97AH258\\SQLEXPRESS;Initial Catalog=nalco_materials;Integrated Security=True";
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_materials WHERE material_code=" + id, conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var r = Serialize(reader);
                    string json = JsonConvert.SerializeObject(r, Formatting.Indented);
                    return json;
                }
            }
        }

        // POST api/<controller>
        public string Post([FromBody] JObject data)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=DESKTOP-97AH258\\SQLEXPRESS;Initial Catalog=nalco_materials;Integrated Security=True";
                conn.Open();
                SqlCommand cmdCount = new SqlCommand("SELECT count(*) from np_materials WHERE material_code = @materialcode", conn);
                cmdCount.Parameters.AddWithValue("@order", materialcode);
                int count = (int)cmdCount.ExecuteScalar();

                if (count > 0)
                {
                    SqlCommand updCommand = new SqlCommand("UPDATE np_materials SET material_description = @materialdescription, material_printer_count = @materialprintercount,material_quantity = @materialquantity, material_printer = @materialprinter, material_critical_flag = @materialcriticalflag, material_reorder_level = @materialreorderlevel, material_storage = @materialstorage, material_printer_description = @materialprinterdescription WHERE materialcode=@material_code", conn);
                    updCommand.Parameters.AddWithValue("@materialdescription", materialdescription);
                    updCommand.Parameters.AddWithValue("@materialprinter", materialprinter);
                    updCommand.Parameters.AddWithValue("@materialprinterdescription", materialprinterdescription);
                    updCommand.Parameters.AddWithValue("@materialprintercount", materialprintercount);
                    updCommand.Parameters.AddWithValue("@materialquantity", materialquantity);
                    updCommand.Parameters.AddWithValue("@materialcriticalflag", materialcriticalflag);
                    updCommand.Parameters.AddWithValue("@materialreorderlevel", materialredorderlevel);
                    updCommand.Parameters.AddWithValue("@materialstorage", materialstorage);
                    int rowsUpdated = updCommand.ExecuteNonQuery();
                    return "{status:updated}";
                }
                else
                {
                    return null;
                    // INSERT STATEMENT
                    //SqlCommand insCommand = new SqlCommand("INSERT into Shipment (TrackingNumber, OrderId, ShippedDateUtc, CreatedOnUtc, TotalWeight) VALUES (@tracking, @order, @date, @date, @weight)", conn);
                    //insCommand.Parameters.AddWithValue("@order", _orderID);
                    //insCommand.Parameters.AddWithValue("@tracking", _trackingID);
                    //insCommand.Parameters.AddWithValue("@date", _date);
                    //insCommand.Parameters.AddWithValue("@weight", _weightID);
                    //int rowsUpdated = insCommand.ExecuteNonQuery();
                    //return "{status:inserted}";
                }
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}