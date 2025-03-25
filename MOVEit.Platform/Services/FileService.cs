using MOVEit.Platform.Entities;
using MOVEit.Platform.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MOVEit.Platform.Services
{
    public class FileService
    {
        private readonly AuthService _authService;

        public FileService()
        {
            _authService = new AuthService();            
        }

        public async Task<int> GetUserDefaultFolderIdAsync(string token)
        {
            var userInfoResult = await _authService.GetUserInfoAsync(token);
            return userInfoResult.DefaultFolderID;
        }

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
                    return null;
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
