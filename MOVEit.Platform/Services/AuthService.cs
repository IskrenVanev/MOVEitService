using MOVEit.Platform.Extensions;
using MOVEit.Platform.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MOVEit.Platform.Services
{
    /// <summary>
    /// Service responsible for authentication and user information retrieval.
    /// </summary>
    public class AuthService
    {
        /// <summary>
        /// Authenticates a user using either password-based authentication or token refresh.
        /// </summary>
        /// <param name="grantType">The type of authentication grant (Password or RefreshToken).</param>
        /// <param name="userName">The username for authentication (required for Password grant type).</param>
        /// <param name="password">The password for authentication (required for Password grant type).</param>
        /// <param name="oldToken">The refresh token for authentication (required for RefreshToken grant type).</param>
        /// <returns>
        /// A <see cref="TokenResponseDTO"/> containing access token details if authentication succeeds.
        /// Throws an <see cref="HttpRequestException"/> if authentication fails.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if an unsupported grant type is provided.</exception>
        /// <exception cref="HttpRequestException">Thrown if the authentication request fails.</exception>
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
                        req.AddParameter("username", userName);
                        req.AddParameter("password", password);
                        break;
                    case GrantType.RefreshToken:
                        req.AddParameter("refresh_token", oldToken);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported grant type: {grantType}", nameof(grantType));
                }

                var response = await client.PostAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Authentication failed with status code {response.StatusCode}: {response.Content}");
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

        /// <summary>
        /// Retrieves user information based on the provided authentication token.
        /// </summary>
        /// <param name="token">The access token used for authorization.</param>
        /// <returns>
        /// A <see cref="UserResponseDTO"/> containing user details if the request succeeds.
        /// Throws an <see cref="HttpRequestException"/> if the request fails.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown if the request to fetch user information fails.</exception>

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
                    throw new HttpRequestException($"Getting user info failed with status code {response.StatusCode}: {response.Content}");
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
