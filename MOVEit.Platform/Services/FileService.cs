using MOVEit.Platform.Entities;
using MOVEit.Platform.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MOVEit.Platform.Services
{
    /// <summary>
    /// Service responsible for handling file and folder-related operations in MOVEit Transfer.
    /// </summary>
    public class FileService
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        public FileService()
        {
            _authService = new AuthService();            
        }

        /// <summary>
        /// Retrieves the default folder ID for the authenticated user.
        /// </summary>
        /// <param name="token">The access token used for authentication.</param>
        /// <returns>The default folder ID associated with the user.</returns>
        /// <exception cref="HttpRequestException">Thrown if retrieving user information fails.</exception>
        public async Task<int> GetUserDefaultFolderIdAsync(string token)
        {
            var userInfoResult = await _authService.GetUserInfoAsync(token);
            return userInfoResult.DefaultFolderID;
        }

        /// <summary>
        /// Retrieves the contents of a specified folder in MOVEit Transfer.
        /// </summary>
        /// <param name="token">The access token used for authentication.</param>
        /// <param name="folderId">The ID of the folder to retrieve files from.</param>
        /// <returns>
        /// A <see cref="FolderFilesResponseDTO"/> object containing details about the files in the folder.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown if the request to fetch folder contents fails.</exception>
        public async Task<FolderFilesResponseDTO> GetFolderContents(string token, int folderId)
        {
            try
            {
                RestClient client = new RestClient($"https://testserver.moveitcloud.com/api/v1/folders/{folderId}/files");

                RestRequest req = new RestRequest();

                req.AddHeader("Authorization", $"Bearer {token}");

                var response = await client.GetAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Getting folder contents failed with status code {response.StatusCode}: {response.Content}");
                }

                var tokenObject = JsonSerializer.Deserialize<FolderFilesResponseDTO>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
