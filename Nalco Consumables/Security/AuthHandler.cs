using Nalco_Consumables.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

public class AuthHandler : DelegatingHandler
{
    private string _userName = "";
    private bool AdminRole;

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

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //if the credentials are validated,
        //set CurrentPrincipal and Current.User
        if (ValidateCredentials(request.Headers.Authorization))
        {
            if (AdminRole)
            {
                Thread.CurrentPrincipal = new APIPrincipalAdmin(_userName);
                HttpContext.Current.User = new APIPrincipalAdmin(_userName);
            }
            else
            {
                Thread.CurrentPrincipal = new APIPrincipalUser(_userName);
                HttpContext.Current.User = new APIPrincipalUser(_userName);
            }
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

    private bool CheckUserInDatabase(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            conn.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_users WHERE pers_no=" + username + " AND pers_passwd='" + password + "'", conn);
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

    private bool IsAdmin(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            conn.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM dbo.np_users WHERE pers_no=" + username + " AND pers_passwd='" + password + "' AND access_flg='1'", conn);
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

    private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        SqlDataReader reader)
    {
        var result = new Dictionary<string, object>();
        foreach (var col in cols)
            result.Add(col, reader[col]);
        return result;
    }

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
                    AdminRole = IsAdmin(username, password);
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
}