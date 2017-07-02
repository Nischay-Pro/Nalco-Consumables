using ClosedXML.Excel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Nalco_Consumables.Controllers
{
    public class ReportController : ApiController
    {
        public string connection = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

        [NonAction]
        public object GenerateReport(DataTable data, string name, string sheetname)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(data, sheetname);
                var stream = new MemoryStream();
                wb.SaveAs(stream);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name
                };
                return result;
            }
        }

        public object Get([FromBody] JObject data)
        {
            //try
            //{
            return Querify("SELECT * FROM dbo.np_vendor", "Vendor.xlsx", "Vendors");

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
        public object Querify(string sqlquery, string filename, string sheetname)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlquery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            return GenerateReport(dt, filename, sheetname);
                        }
                    }
                }
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