using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Microsoft.IdentityServer.Web.Authentication.External;

namespace RekTec.Crm.AdfsCaptcha.MFAadapter
{
    /// <summary>
    /// 元数据
    /// </summary>
    public class CustomMetadata : IAuthenticationAdapterMetadata
    {
        /// <summary>
        /// Returns an array of strings containing URIs indicating the set of authentication methods implemented by the adapter 
        /// AD FS requires that, if authentication is successful, the method actually employed will be returned by the
        /// final call to TryEndAuthentication(). If no authentication method is returned, or the method returned is not
        /// one of the methods listed in this property, the authentication attempt will fail.
        /// 不能为空
        /// </summary>
        public string[] AuthenticationMethods { get { return new[] { "http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/hardwaretoken" }; } }

        /// <summary>
        /// 适配器验证上下文中包含的身份标识声明（只能有一个）
        /// 例如账户、邮箱、手机号码等
        /// </summary>
        public string[] IdentityClaims { get { return new[] { ClaimTypes.Upn }; } }

        /// <summary>
        /// Returns the name of the provider that will be shown in the AD FS management UI (not visible to end users)
        /// </summary>
        public string AdminName { get { return "RekTec MFA 短信验证码"; } }

        /// <summary>
        /// 可用的语言
        /// </summary>
        public int[] AvailableLcids { get{ return new[] { new CultureInfo("zh-cn").LCID, new CultureInfo("en-us").LCID }; } }

        /// <summary>
        /// 描述
        /// </summary>
        public Dictionary<int, string> Descriptions
        {
            get
            {
                Dictionary<int, string> _descriptions = new Dictionary<int, string>();
                _descriptions.Add(new CultureInfo("en-us").LCID, "用户手机短信验证");
                _descriptions.Add(new CultureInfo("fr").LCID, "用户手机短信验证");
                return _descriptions;
            }
        }

        /// <summary>
        /// 多语言
        /// </summary>
        public Dictionary<int, string> FriendlyNames
        {
            get
            {
                Dictionary<int, string> _friendlyNames = new Dictionary<int, string>();
                _friendlyNames.Add(new CultureInfo("en-us").LCID, "RekTec MFA Message Verify");
                _friendlyNames.Add(new CultureInfo("zh-cn").LCID, "RekTec MFA 短信验证码");
                return _friendlyNames;
            }
        }

        /// <summary>
        /// 默认True
        /// </summary>
        public bool RequiresIdentity { get { return true; } }
    }
}
