using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOVEit.Platform.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string SyncDirectory { get; set; }
        public string RefreshToken { get; set; }
    }
}
