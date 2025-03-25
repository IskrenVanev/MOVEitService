using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOVEit.Platform.Models
{
    //  password, refresh_token, otp, code, external_token
    public enum GrantType
    {
        [Description("password")]
        Password,
        [Description("refresh_token")]
        RefreshToken,
        [Description("otp")]
        Otp,
        [Description("code")]
        Code,
        [Description("external_token")]
        ExternalToken
    }
}
