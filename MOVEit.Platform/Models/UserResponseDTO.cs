using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MOVEit.Platform.Models
{
    public class DisplaySettings
    {
        [JsonPropertyName("userListPageSize")]
        public int UserListPageSize { get; set; }

        [JsonPropertyName("fileListPageSize")]
        public int FileListPageSize { get; set; }

        [JsonPropertyName("liveViewPageSize")]
        public int LiveViewPageSize { get; set; }
    }
    
    public class UserResponseDTO
    {
        [JsonPropertyName("emailFormat")]
        public string EmailFormat { get; set; }

        [JsonPropertyName("notes")]
        public object Notes { get; set; }

        [JsonPropertyName("statusNote")]
        public object StatusNote { get; set; }

        [JsonPropertyName("passwordChangeStamp")]
        public DateTime PasswordChangeStamp { get; set; }

        [JsonPropertyName("receivesNotification")]
        public object ReceivesNotification { get; set; }

        [JsonPropertyName("forceChangePassword")]
        public object ForceChangePassword { get; set; }

        [JsonPropertyName("folderQuota")]
        public int FolderQuota { get; set; }

        [JsonPropertyName("totalFileSize")]
        public int TotalFileSize { get; set; }

        [JsonPropertyName("authMethod")]
        public string AuthMethod { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("homeFolderID")]
        public int HomeFolderID { get; set; }

        [JsonPropertyName("defaultFolderID")]
        public int DefaultFolderID { get; set; }

        [JsonPropertyName("expirationPolicyID")]
        public object ExpirationPolicyID { get; set; }

        [JsonPropertyName("displaySettings")]
        public DisplaySettings DisplaySettings { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("orgID")]
        public int OrgID { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("permission")]
        public string Permission { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("status")]
        public object Status { get; set; }

        [JsonPropertyName("lastLoginStamp")]
        public DateTime LastLoginStamp { get; set; }
    }


}
