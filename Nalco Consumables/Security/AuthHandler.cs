using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using Nalco_Consumables.Security;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;

public class AuthHandler : DelegatingHandler
{
    private string _userName = "";

    //Method to validate credentials from Authorization
    //header value
    private bool ValidateCredentials(AuthenticationHeaderValue authenticationHeaderVal)
    {
        try
        {
            if (authenticationHeaderVal != null
                && !String.IsNullOrEmpty(authenticationHeaderVal.Parameter))
            {
                string[] decodedCredentials
                = Encoding.ASCII.GetString(Convert.FromBase64String(
                authenticationHeaderVal.Parameter))
                .Split(new[] { ':' });

                //now decodedCredentials[0] will contain
                //username and decodedCredentials[1] will
                //contain password.

                var username = decodedCredentials[0];
                var password = decodedCredentials[1];
                if (CheckUserInDatabase(username, password))
                {
                    _userName = username;
                    return true;//request authenticated.
                }
            }
            return false;//request not authenticated.
        }
        catch
        {
            return false;
        }
    }

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

    private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                    SqlDataReader reader)
    {
        var result = new Dictionary<string, object>();
        foreach (var col in cols)
            result.Add(col, reader[col]);
        return result;
    }

    private bool CheckUserInDatabase(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = "Data Source=DESKTOP-97AH258\\SQLEXPRESS;Initial Catalog=nalco_materials;Integrated Security=True";
            conn.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_users WHERE new_pno=" + username + " AND password='" + password + "'", conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //if the credentials are validated,
        //set CurrentPrincipal and Current.User
        if (ValidateCredentials(request.Headers.Authorization))
        {
            Thread.CurrentPrincipal = new APIPrincipal(_userName);
            HttpContext.Current.User = new APIPrincipal(_userName);
        }
        //Execute base.SendAsync to execute default
        //actions and once it is completed,
        //capture the response object and add
        //WWW-Authenticate header if the request
        //was marked as unauthorized.

        //Allow the request to process further down the pipeline
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized
            && !response.Headers.Contains("WwwAuthenticate"))
        {
            response.Headers.Add("WwwAuthenticate", "Basic");
        }

        return response;
    }
}