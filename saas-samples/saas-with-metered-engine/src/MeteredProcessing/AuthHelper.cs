using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace SaaS.Metered.Processing
{
    public class AuthHelper
    {
        public static async Task<string> GetTokenAsync()
        {

            


            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
            string tenantId = Environment.GetEnvironmentVariable("TenantId", EnvironmentVariableTarget.Process);
            string scope = Environment.GetEnvironmentVariable("Scope", EnvironmentVariableTarget.Process);
            string adUrl = String.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", tenantId);
            
            var client = new HttpClient();
            var form = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"scope", scope},
                };
            HttpResponseMessage tokenResponse = await client.PostAsync(adUrl, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token result = JsonConvert.DeserializeObject<Token>(jsonContent);
            
            var creds = new ClientSecretCredential(tenantId, clientId, clientSecret);
            return result.AccessToken;
        }


        public static ClientSecretCredential GetCreds()
        {

            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret", EnvironmentVariableTarget.Process);
            string tenantId = Environment.GetEnvironmentVariable("TenantId", EnvironmentVariableTarget.Process);
            
            return new ClientSecretCredential(tenantId, clientId, clientSecret);
        }
        internal class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }
    }
}
