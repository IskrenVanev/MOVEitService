using MOVEit.Platform.Extensions;
using MOVEit.Platform.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MOVEit.Platform.Services
{
    public class AuthService
    {
        public async Task<TokenResponseDTO> Authenticate(GrantType grantType, string userName = null, string password = null,
            string oldToken = null)
        {
            try
            {
                RestClient client = new RestClient("https://testserver.moveitcloud.com/api/v1/token");

                RestRequest req = new RestRequest();

                req.AddParameter("grant_type", grantType.GetDescription());

                switch (grantType)
                {
                    case GrantType.Password:
                        // TODO: add null checks for params
                        req.AddParameter("username", userName);
                        req.AddParameter("password", password);
                        break;
                    case GrantType.RefreshToken:
                        // TODO: add null checks for params
                        req.AddParameter("refresh_token", oldToken);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var response = await client.PostAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var tokenObject = JsonSerializer.Deserialize<TokenResponseDTO>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return tokenObject;
            }
            catch (Exception ex)
            {
                //TODO: Log error to server
                throw ex;
            }


        }

        public async Task<UserResponseDTO> GetUserInfoAsync(string token)
        {
            try
            {
                RestClient client = new RestClient("https://testserver.moveitcloud.com/api/v1/users/self");

                RestRequest req = new RestRequest();

                req.AddHeader("Authorization", $"Bearer {token}");

                var response = await client.GetAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var tokenObject = JsonSerializer.Deserialize<UserResponseDTO>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return tokenObject;
            }
            catch (Exception ex)
            {
                //TODO: Log error to server
                throw;
            }
        }
    }
}
